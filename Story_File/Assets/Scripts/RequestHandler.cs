using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using DDK.Base.Extensions;
using System;
using UnityEngine.Video;
using SimpleJSON;
using DDK.Networking;

namespace StoryFile
{
	public class RequestHandler : MonoBehaviour {

		public string url = "https://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";
		public AudioSource micSource;
		public CanvasGroup btRecord;
        public bool useWss;


		WebSocket _ws;
		byte[] _clip;


		public static bool m_Connected { get; private set; }


		/// <summary>
		/// The Transcription's web socket uri
		/// </summary>
		private const string WSS_TRANSCRIPTION_URI = "wss://storyfilestage.com:7070"; //ws://echo.websocket.org/
        private const string HTTPS_TRANSCRIPTION_URI = "https://storyfilestage.com:7070/ai/audio_query";


		// Use this for initialization
		void Start () {

            if( useWss )
            {
                _ws = new WebSocket (WSS_TRANSCRIPTION_URI);
                //_ws = new WebSocket ("ws://echo.websocket.org/");
                _ws.EmitOnPing = true;

                _ws.OnMessage += OnMessage;
                _ws.OnOpen += OnOpen;
                _ws.OnError += OnError;
                _ws.OnClose += OnClose;
            }
		}


        /// <summary>
        /// Sends the audio and waits for the transcription, which is assigned to the QuestionHandler.m_question.
        /// </summary>
        IEnumerator _SendRequestAndWait()
        {
            WWWForm wwwForm = new WWWForm();
            wwwForm.AddBinaryData( "audio", _clip );

            DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer ();
            UnityWebRequest request = UnityWebRequest.Post( HTTPS_TRANSCRIPTION_URI, wwwForm ); //new UnityWebRequest ( HTTPS_TRANSCRIPTION_URI, "POST" );
            request.downloadHandler = downloadHandler;

			QuestionsHandler.Instance.ValidateUI (false);

			Debug.Log ("Sending Transcription Request");
            AsyncOperation www = request.Send ();
            yield return www;

            Debug.Log ( "Transcription Request done: " + www.isDone);
            Debug.Log ( "Response Code: " + request.responseCode);
            if( !string.IsNullOrEmpty (request.error )  )
            {
                Debug.LogError (request.error);
            }
            else
            {
				Debug.Log ( downloadHandler.text ); //response body: url, videoId, question, transcription (Amanda's answer)
                var transcription = JSON.Parse (downloadHandler.text);
                if( transcription == null )
                {
                    Debug.LogError (string.Format( "Can't Parse response json: {0}", transcription ), gameObject);
                    yield break;
                }
				QuestionsHandler.m_LastVideoUrl = transcription["url"].Value;
				Debug.Log (string.Format( "Question: {0}", transcription["question"].Value ));
				Debug.Log (string.Format( "Answer: {0}", transcription["transcription"].Value ));
            }
        }


        /// <summary>
        /// Sends the audio, waits for the transcription, sends the question, and waits for the video answer to be played.
        /// </summary>
		public void ConnectAsyncAndSendAudio()
		{
            StartCoroutine( ConnectAsyncSendAudioAndWait() );
		}
        /// <summary>
        /// Sends the audio, waits for the transcription, sends the question, and waits for the video answer to be played.
        /// </summary>
		public IEnumerator ConnectAsyncSendAudioAndWait()
		{
            yield return CheckInternet.CheckAndWait().Run();
            if( !CheckInternet.m_IsConnectionAvailable )
            {
                yield break;
            }
			_clip = micSource.clip.EncodeToWav ();
            if( useWss ) {
                _ws.ConnectAsync ();
                while (!m_Connected)
                    yield return null;
            }
            else {
                yield return StartCoroutine( _SendRequestAndWait() );//send audio and wait for transcription
            }
			yield return StartCoroutine( QuestionsHandler.Instance.ShowVideoInUrl () );
            if( useWss ) {
                _ws.CloseAsync ();
            }
		}
			
		#region Callbacks
		void OnMessage( object sender, MessageEventArgs e )
		{
			Debug.Log ("website says: " + e.Data);
			//QuestionsHandler.m_question = e.Data;
			//QuestionsHandler.Instance.SendRequest ();
		}
		void OnOpen( object sender, EventArgs e )
		{ 
			m_Connected = true;
			Debug.Log ("Connected");
			Debug.Log (_ws.Ping ());
			//_ws.SendAsync ("Test Message", OnSent);

			_ws.SendAsync ( _clip, OnSent );
		}
		void OnClose( object sender, CloseEventArgs e )
		{
			Debug.Log ("Connection closed");
		}
		void OnError( object sender, ErrorEventArgs e )
		{
			Debug.LogError ("website says: " + e.Message);
		}
		void OnSent( bool completed )
		{
			Debug.Log ( "Sent: " + completed );
		}
		#endregion
	}
}
