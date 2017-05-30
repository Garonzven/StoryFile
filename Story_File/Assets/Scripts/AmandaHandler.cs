using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

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

        private const string HOLD_TEXT = "HOLD TO TALK";
        private const string RELEASE_TEXT = "RELEASE TO LISTEN";

        public void OnPointerDownButton()
        {
			if( !btRecord.interactable )
			{
				return;
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
				watsonStreaming.Listen(false);
			}
			else {
				mic.Record(false);
				requestHandler.ConnectAsyncAndSendAudio();
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
