using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using DDK.Base.Statics;

namespace StoryFile
{
    public class AmandaHandler : MonoBehaviour
    {
		public bool useWatson;
		public CanvasGroup btRecord;
        public Text txtTalk;
        public Image imgMic;
        public Image imgButton;
        public Color32 colorHold;
        public Color32 colorRelease;
        public WatsonStreamingSpeechToText watsonStreaming;
        public RequestHandler requestHandler;
        public MicrophoneHandler mic;
		public AudioClip micDown;
		public AudioClip micUp;

        private const string HOLD_TEXT = "HOLD TO TALK";
        private const string RELEASE_TEXT = "RELEASE TO LISTEN";

		public void Start()
		{
			if( useWatson )
			{
				Utilities.Log ( Color.green, "Using Watson..", gameObject );
				watsonStreaming.Init ();
			}
			else Utilities.Log ( Color.green, "Using REST API..", gameObject );
		}
        public void OnPointerDownButton()
        {
			if( !btRecord.interactable )
			{
				return;
			}

			if( micDown )
			{
				AudioSource.PlayClipAtPoint ( micDown, Vector3.zero );
			}
			SetupBt ( colorRelease, RELEASE_TEXT );
			if( useWatson )
			{
				watsonStreaming.m_mustListen = true;
			}
			else {
				mic.Record(true);
			}
        }
        public void OnPointerUpButton()
        {
			if( !btRecord.interactable )
			{
				return;
			}
                
			SetupBt ( colorHold, HOLD_TEXT );
			if( useWatson )
			{
				watsonStreaming.m_mustListen = false;
                StartCoroutine( QuestionsHandler.Instance.SendRequestAndWaitForAnswerURL() );
				//watsonStreaming.Listen(false);
			}
			else {
				mic.Record(false);
				requestHandler.ConnectAsyncAndSendAudio();
			}
            if( micUp )
            {
                AudioSource.PlayClipAtPoint ( micUp, Vector3.zero );
            }
        }


		void SetupBt( Color color, string text )
		{
			txtTalk.text = text;
			imgMic.color = color;
			imgButton.color = color;
		}
    }
}
