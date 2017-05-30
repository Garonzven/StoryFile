using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace StoryFile
{
<<<<<<< HEAD
    public Text txtTalk;
    public Image imgMic;
    public Image imgButton;
    public Color32 colorHold;
    public Color32 colorRelease;
    public WatsonStreamingSpeechToText watsonStream;

    private const string HOLD_TEXT = "HOLD TO TALK";
    private const string RELEASE_TEXT = "RELEASE TO LISTEN";


    public void OnPointerDownButton()
    {
        txtTalk.text = RELEASE_TEXT;
        imgMic.color = colorRelease;
        imgButton.color = colorRelease;
        //watsonStream.Active = true;
        watsonStream.m_mustListen = true;

    }
    public void OnPointerUpButton()
    {
        txtTalk.text = HOLD_TEXT;
        imgMic.color = colorHold;
        imgButton.color = colorHold;
        watsonStream.m_mustListen = false;
      //  Microphone.End(watsonStream.m_MicrophoneID);
=======
    public class AmandaHandler : MonoBehaviour
    {
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
            txtTalk.text = RELEASE_TEXT;
            imgMic.color = colorRelease;
            imgButton.color = colorRelease;
            mic.Record(true);
            //watsonStreaming.m_mustListen = true;
        }
        public void OnPointerUpButton()
        {
            txtTalk.text = HOLD_TEXT;
            imgMic.color = colorHold;
            imgButton.color = colorHold;
            mic.Record(false);
            requestHandler.ConnectAsyncAndSendAudio();
            /*watsonStreaming.m_mustListen = false;
            watsonStreaming.Listen(false);*/
        }
>>>>>>> de14a192f2d1f6d256b9d4f9e7e7172ef1c3caa8
    }
}
