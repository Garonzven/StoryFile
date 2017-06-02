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
	public class TranscriptionHandler : MonoBehaviour {

		public string url = "https://storyfilestage.com:7070/transcribe";
		public AudioSource audioSource;


		byte[] _clip;


		// Use this for initialization
		void Start () {
			_clip = audioSource.clip.EncodeToWav ();
			SendRequest ();
		}


		public void SendRequest()
		{
			StartCoroutine ( Upload () );
		}

		IEnumerator Upload() {
			List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
			formData.Add( new MultipartFormFileSection("audio", _clip) );

			UnityWebRequest www = UnityWebRequest.Post(url, formData);
			yield return www.Send();

			if(www.isError) {
				Debug.Log(www.error);
			}
			else {
				Debug.Log("Form upload complete!");
				Debug.Log (string.Format ("Text received: {0}", www.downloadHandler.text));
				QuestionsHandler.m_question = www.downloadHandler.text;
				QuestionsHandler.Instance.SendRequest ();
			}
		}
		/*IEnumerator SendRequestAndWait()
		{
			Dictionary<string, object> bodyData = new Dictionary<string, object> () {
				{ "audio", audioSource.clip },
			};
			UploadHandler uploadHandler = new UploadHandlerRaw ( Convert.FromBase64String ( bodyData.Serialize () ) ) {
				contentType = "multipart/form-data"
			};
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer ();
			UnityWebRequest request = new UnityWebRequest ( authUrl, "POST", downloadHandler, uploadHandler );
			request.SetRequestHeader ("APP-TOKEN", "secureapptoken");

			AsyncOperation asyncOperation = request.Send ();
			yield return asyncOperation;
			Debug.Log ( "Auth Request done: " + asyncOperation.isDone);
			Debug.Log ( "Progress: " + asyncOperation.progress);
			Debug.Log ( "Response Code: " + request.responseCode);
			if( !string.IsNullOrEmpty (request.error )  )
			{
				Debug.LogError (request.error);
			}
			else
			{
				Debug.Log ( downloadHandler.text ); //response body (session id)
				var session = JSON.Parse (downloadHandler.text);
				if( session == null )
				{
					Debug.LogError (string.Format( "Can't Parse response json: {0}", session ), gameObject);
					yield break;
				}
				_SessionId = session["session"].Value;
				Debug.Log (_SessionId);
			}
		}*/
	}
}
