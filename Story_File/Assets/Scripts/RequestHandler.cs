using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using DDK.Base.Extensions;

namespace StoryFile
{
	public class RequestHandler : MonoBehaviour {

		public string url = "https://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";
		public AudioSource audioSource;


		/// <summary>
		/// The Transcription's web socket uri
		/// </summary>
		private const string WSS_TRANSCRIPTION_URI = "wss://storyfilestage.com:7070";


		// Use this for initialization
		void Start () {
			using (var ws = new WebSocket ( WSS_TRANSCRIPTION_URI /*"ws://dragonsnest.far/Laputa"*/ )) {
				ws.OnMessage += (sender, e) => Debug.Log ("Laputa says: " + e.Data);
				ws.OnOpen += (sender, e) => { 
					Debug.Log ("Connected");
					Debug.Log (ws.Ping ());
				};
				ws.OnError += (sender, e) => Debug.LogError ("Laputa says: " + e.Message);
				ws.OnClose += (sender, e) => Debug.Log ("Connection closed");

				ws.EmitOnPing = true;

				ws.ConnectAsync ();
				ws.Send ( audioSource.clip.EncodeToWav () );
			}

			//SendRequest ();
		}
		public void SendRequest()
		{
			StartCoroutine ( SendRequestAndWait () );
		}


		IEnumerator SendRequestAndWait()
		{
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer ();
			UnityWebRequest request = new UnityWebRequest (url, "GET");
			request.downloadHandler = downloadHandler;
			Debug.Log ("Starting Download..");
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
	}
}
