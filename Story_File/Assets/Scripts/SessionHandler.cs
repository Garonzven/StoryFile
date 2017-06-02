using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using DDK.Base.Extensions;
using SimpleJSON;

public class SessionHandler : MonoBehaviour {

	public string authUrl = "https://private-anon-907e96fa3f-storyfile.apiary-mock.com/authenticate";


	public static string _SessionId { get; private set; }


	// Use this for initialization
	void Start () {
		SendRequest ();
	}
	
	public void SendRequest()
	{
		StartCoroutine ( SendRequestAndWait () );
	}


	IEnumerator SendRequestAndWait()
	{
		Dictionary<string, object> bodyData = new Dictionary<string, object> () {
			{ "email", "foo@bar.com" },
			{ "password", "opensource" }
		};
		UploadHandler uploadHandler = new UploadHandlerRaw ( Convert.FromBase64String ( bodyData.Serialize () ) ) {
			contentType = "application-json"
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
			//HEADERS
			/*int i = 0;
			foreach( string value in request.GetResponseHeaders ().Values )
			{
				Debug.Log ( string.Format ( "Response Header {0}: {1}", i++, value ) );
			}*/
		}
	}
}
