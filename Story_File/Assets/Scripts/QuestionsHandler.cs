using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using DDK.Base.Extensions;
using SimpleJSON;

/// <summary>
/// Handles the questions requests that must be sent to the server to receive the video url with the 
/// answer that must be shown to the user
/// </summary>
public class QuestionsHandler : MonoBehaviour {

	public string questionsUrl = "https://private-anon-907e96fa3f-storyfile.apiary-mock.com/ai/answer";


	public static QuestionsHandler Instance;
	/// <summary>
	/// The question that will be send to the server in the next request.
	/// </summary>
	public static string m_question = "hello";
	private static string _lastVideoUrl = null;
	/// <summary>
	/// The last video url (answer) received by the server in the last question request.
	/// </summary>
	public static string m_lastVideoUrl {
		get{
			string videoUrl = _lastVideoUrl;
			_lastVideoUrl = null;
			return videoUrl;
		}
		private set{
			_lastVideoUrl = value;
		}
	}
	/// <summary>
	/// true if a request is in progress.
	/// </summary>
	public static bool m_resquestInProgress = false;


	void Awake()
	{
		Instance = this;
	}
	// Use this for initialization
	void Start () {
		//SendRequest ();
	}

	public void SendRequest()
	{
		StartCoroutine ( SendRequestAndWait () );
	}


	public IEnumerator SendRequestAndWait()
	{
		m_resquestInProgress = true;
		Dictionary<string, object> bodyData = new Dictionary<string, object> () {
			{ "question", m_question },
			{ "userid", "1" }
		};
		UploadHandler uploadHandler = new UploadHandlerRaw ( Convert.FromBase64String ( bodyData.Serialize () ) ) {
			contentType = "application-json"
		};
		DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer ();
		UnityWebRequest request = new UnityWebRequest ( questionsUrl, "POST", downloadHandler, uploadHandler );
		request.SetRequestHeader ("APP-TOKEN", "secureapptoken");
		//request.SetRequestHeader ("SESSION_ID", SessionHandler._SessionId);
		request.SetRequestHeader ("SESSION_ID", "yourSessionId");

		Debug.Log ("Sending Question/Answer Request");
		AsyncOperation asyncOperation = request.Send ();
		yield return asyncOperation;
		Debug.Log ( "Question/Answer Request done: " + asyncOperation.isDone);
		Debug.Log ( "Response Code: " + request.responseCode);
		if( !string.IsNullOrEmpty (request.error )  )
		{
			Debug.LogError (request.error);
		}
		else
		{
			Debug.Log ( downloadHandler.text ); //response body (video url)
			var videoUrl = JSON.Parse (downloadHandler.text);
			if( videoUrl == null )
			{
				Debug.LogError (string.Format( "Can't Parse response json: {0}", videoUrl ), gameObject);
				//Show error video

				yield break;
			}
			m_lastVideoUrl = videoUrl["video_url"].Value;
			Debug.Log ( "Last Video URL: " + _lastVideoUrl);
			m_resquestInProgress = false;
		}
	}
}
