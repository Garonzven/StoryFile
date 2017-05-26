using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using DDK.Base.Extensions;
using System;
using UnityEngine.Video;

namespace StoryFile
{
	public class RequestHandler : MonoBehaviour {

		public string url = "https://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";
		public AudioSource audioSource;
		public VideoPlayer videoPlayer;


		WebSocket _ws;
		byte[] _clip;
		//string _clip;


		public static bool m_Connected { get; private set; }


		/// <summary>
		/// The Transcription's web socket uri
		/// </summary>
		private const string WSS_TRANSCRIPTION_URI = "wss://storyfilestage.com:7070"; //ws://echo.websocket.org/


		// Use this for initialization
		void Start () {
			//_clip = audioSource.clip.EncodeToWav ();
			//_clip = Convert.ToBase64String( audioSource.clip.EncodeToWav () );
			_ws = new WebSocket (WSS_TRANSCRIPTION_URI);
			//_ws = new WebSocket ("ws://echo.websocket.org/");
			_ws.EmitOnPing = true;

			_ws.OnMessage += OnMessage;
			_ws.OnOpen += OnOpen;
			_ws.OnError += OnError;
			_ws.OnClose += OnClose;

			_ws.Compression = CompressionMethod.Deflate;
			_ws.WaitTime = new TimeSpan (0, 1, 0);

			//_ws.ConnectAsync ();
		}


		public void ConnectAsyncAndSendAudio()
		{
			_clip = audioSource.clip.EncodeToWav ();
			_ws.ConnectAsync ();
		}
		public IEnumerator ConnectAsyncSendAudioAndWait()
		{
			_clip = audioSource.clip.EncodeToWav ();
			_ws.ConnectAsync ();
			while (!m_Connected)
				yield return null;
			while (!QuestionsHandler.m_resquestInProgress)
				yield return null;
			while (QuestionsHandler.m_resquestInProgress)
				yield return null;
			if( !videoPlayer )
			{
				Debug.LogError ("There is no video player reference, can't play the received video url", gameObject);
				yield break;
			}
			videoPlayer.source = VideoSource.Url;
			videoPlayer.url = QuestionsHandler.m_lastVideoUrl;
			videoPlayer.Prepare ();
			while (!videoPlayer.isPrepared)
				yield return null;
			videoPlayer.Play ();
			_ws.CloseAsync ();
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
