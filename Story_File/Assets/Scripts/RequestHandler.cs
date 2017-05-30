using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using DDK.Base.Extensions;
using System;
using UnityEngine.Video;
using SimpleJSON;

namespace StoryFile
{
	public class RequestHandler : MonoBehaviour {

		public string url = "https://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";
		public AudioSource audioSource;
		public VideoPlayer videoPlayer;
        public bool useWss;


		WebSocket _ws;
		byte[] _clip;


		public static bool m_Connected { get; private set; }


		/// <summary>
		/// The Transcription's web socket uri
		/// </summary>
		private const string WSS_TRANSCRIPTION_URI = "wss://storyfilestage.com:7070"; //ws://echo.websocket.org/
        private const string HTTPS_TRANSCRIPTION_URI = "https://storyfilestage.com:7070/transcribe";


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

            AsyncOperation www = request.Send ();
            yield return www;

            Debug.Log ( "Transcription Request done: " + www.isDone);
            Debug.Log ( "Progress: " + www.progress);
            Debug.Log ( "Response Code: " + request.responseCode);
            if( !string.IsNullOrEmpty (request.error )  )
            {
                Debug.LogError (request.error);
            }
            else
            {
                Debug.Log ( downloadHandler.text ); //response body (transcription)
                var transcription = JSON.Parse (downloadHandler.text);
                if( transcription == null )
                {
                    Debug.LogError (string.Format( "Can't Parse response json: {0}", transcription ), gameObject);
                    yield break;
                }
                QuestionsHandler.m_question = transcription["transcription"].Value;
                Debug.Log (QuestionsHandler.m_question);
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
			_clip = audioSource.clip.EncodeToWav ();
            if( useWss ) {
                _ws.ConnectAsync ();
                while (!m_Connected)
                    yield return null;
            }
            else {
                yield return StartCoroutine( _SendRequestAndWait() );//send audio and wait for transcription
                QuestionsHandler.Instance.SendRequest();//send question
            }
            yield return StartCoroutine( WaitForQuestionAndAnswer() );//wait for answer video url
            if( useWss ) {
                _ws.CloseAsync ();
            }
		}
        /// <summary>
        /// Waits for the question to be sent, and the video url (answer) to be returned and played (video).
        /// </summary>
        public IEnumerator WaitForQuestionAndAnswer()
        {
            while (!QuestionsHandler.m_resquestInProgress)
                yield return null;
            while (QuestionsHandler.m_resquestInProgress)
                yield return null;
            if( !videoPlayer )
            {
                Debug.LogError ("There is no video player reference, can't play the received video url", gameObject);
                yield break;
            }
            if( videoPlayer.isPlaying )
                videoPlayer.Stop();
            videoPlayer.isLooping = false;
            //Assign the Audio from Video to AudioSource to be played
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = QuestionsHandler.m_lastVideoUrl;
            videoPlayer.Prepare ();
            while (!videoPlayer.isPrepared)
                yield return null;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.controlledAudioTrackCount = 1;
            videoPlayer.SetTargetAudioSource(0, videoPlayer.GetComponent<AudioSource>());
            videoPlayer.Play ();
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
