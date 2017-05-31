using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using DDK.Base.Extensions;
using SimpleJSON;
using UnityEngine.Video;
using DDK;

/// <summary>
/// Handles the questions requests that must be sent to the server to receive the video url with the 
/// answer that must be shown to the user
/// </summary>
public class QuestionsHandler : MonoBehaviour {

	public string questionsUrl = "https://private-anon-907e96fa3f-storyfile.apiary-mock.com/ai/answer";
	public VideoPlayer videoPlayer;
	[Indent(1)]
	public float transitionsDuration = 0.3f;
	public CanvasGroup btRecord;

	CanvasGroup _vPlayer;
	AudioSource _videoAudioSource;
	WaitForSeconds _wait;

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
		_videoAudioSource = videoPlayer.GetComponent<AudioSource> ();
		_vPlayer = videoPlayer.GetComponent<CanvasGroup> ();
		_wait = new WaitForSeconds (transitionsDuration);

		videoPlayer.errorReceived += ErrorReceived;
		videoPlayer.frameDropped += FrameDropped;
		//SendRequest ();
	}

	public void SendRequest()
	{
		StartCoroutine ( SendRequestAndWait () );
	}


	public IEnumerator SendRequestAndWaitForAnswerURL()
	{
		StartCoroutine (SendRequestAndWait ());
		yield return StartCoroutine (WaitForAnswerURL ());
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
				yield break;
			}
			m_lastVideoUrl = videoUrl["video_url"].Value;
			Debug.Log ( "Last Video URL: " + _lastVideoUrl);
			m_resquestInProgress = false;
		}
	}
	/// <summary>
	/// Waits for the question to be sent, and the video url (answer) to be returned and played (video).
	/// </summary>
	public IEnumerator WaitForAnswerURL()
	{
		//Wait for answer (video url)
		while (!QuestionsHandler.m_resquestInProgress)
			yield return null;
		while (QuestionsHandler.m_resquestInProgress)
			yield return null;
		if( !videoPlayer )
		{
			Debug.LogError ("There is no video player reference, can't play the received video url", gameObject);
			yield break;
		}
		//Video Interruption
		/*if( videoPlayer.isPlaying )
			{
				StartCoroutine ( _vPlayer.AlphaTo ( 0f, transitionsDuration ) );
				yield return _wait;
				videoPlayer.Stop();
				_videoAudioSource.Stop ();
			}*/
		//Get video url, show video, and wait for it to end.
		videoPlayer.url = QuestionsHandler.m_lastVideoUrl;
		videoPlayer.Prepare ();
		while (!videoPlayer.isPrepared)
			yield return null;
		StartCoroutine ( _vPlayer.AlphaTo ( 1f, transitionsDuration ) );
		videoPlayer.Play ();
		while( !videoPlayer.isPlaying )
			yield return null;
		Debug.Log ("Video is playing");
		while( videoPlayer.isPlaying )
			yield return null;
		Debug.Log ("Video ended playing");
		StartCoroutine( btRecord.AlphaTo ( 1f, transitionsDuration ) );
		btRecord.interactable = true;
		StartCoroutine ( _vPlayer.AlphaTo ( 0f, transitionsDuration ) );
	}

	#region Callbacks
	void ErrorReceived( VideoPlayer source, string msg )
	{
		Debug.LogError (msg);
	}
	void FrameDropped( VideoPlayer source )
	{
		Debug.LogWarning ("Frame Dropped");
	}
	#endregion
}
