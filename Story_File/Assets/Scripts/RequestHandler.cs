using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using DDK.Base.Extensions;
using System;

namespace StoryFile
{
	public class RequestHandler : MonoBehaviour {

		public string url = "https://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";
		public AudioSource audioSource;


		WebSocket _ws;
		byte[] _clip;
		//string _clip;


		/// <summary>
		/// The Transcription's web socket uri
		/// </summary>
		private const string WSS_TRANSCRIPTION_URI = "wss://storyfilestage.com:7070"; //ws://echo.websocket.org/


		// Use this for initialization
		void Start () {
			_clip = audioSource.clip.EncodeToWav ();
			//_clip = Convert.ToBase64String( audioSource.clip.EncodeToWav () );
			//_ws = new WebSocket (WSS_TRANSCRIPTION_URI);
			_ws = new WebSocket ("ws://echo.websocket.org/");
			//_ws.SetCredentials ("foo@bar.com", "opensource", true);
			_ws.EmitOnPing = true;

			_ws.OnMessage += OnMessage;
			_ws.OnOpen += OnOpen;
			_ws.OnError += OnError;
			_ws.OnClose += OnClose;

			_ws.ConnectAsync ();

			//SendRequest ();
		}

		public void SendRequest()
		{
			StartCoroutine ( SendRequestAndWait () );
		}


		IEnumerator SendRequestAndWait()
		{
			//DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer ();
			Dictionary<string, object> bodyData = new Dictionary<string, object> () {
				{ "email", "foo@bar.com" },
				{ "password", "opensource" }
			};
			UploadHandler uploadHandler = new UploadHandlerRaw ( Convert.FromBase64String ( bodyData.Serialize () ) ) {
				contentType = "application-json",
			};
			UnityWebRequest request = new UnityWebRequest (url, "POST");
			request.uploadHandler = uploadHandler;
			request.SetRequestHeader ("APP-TOKEN", "secureapptoken");
			request.SetRequestHeader ("Content-Length", "60");	

			AsyncOperation asyncOperation = request.Send ();
			yield return asyncOperation;
			Debug.Log (asyncOperation.isDone);
			Debug.Log (asyncOperation.progress);
			if( !string.IsNullOrEmpty (request.error )  )
			{
				Debug.LogError (request.error);
			}
			else
			{
				Debug.Log (request.downloadHandler.data);
			}
		}
		#region Callbacks
		void OnMessage( object sender, MessageEventArgs e )
		{
			Debug.Log ("website says: " + e.Data);
		}
		void OnOpen( object sender, EventArgs e )
		{ 
			Debug.Log ("Connected");
			Debug.Log (_ws.Ping ());
			_ws.SendAsync ("Balus", OnSent);
			//_ws.SendAsync ( _clip, OnSent );
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
