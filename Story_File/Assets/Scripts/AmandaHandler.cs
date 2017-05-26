using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AmandaHandler : MonoBehaviour
{
    public Text txtTalk;
    public Image imgMic;
    public Image imgButton;
    public Color32 colorHold;
    public Color32 colorRelease;
	public WatsonStreamingSpeechToText watsonStreaming;

    private const string HOLD_TEXT = "HOLD TO TALK";
    private const string RELEASE_TEXT = "RELEASE TO LISTEN";

    public void OnPointerDownButton()
    {
        txtTalk.text = RELEASE_TEXT;
        imgMic.color = colorRelease;
        imgButton.color = colorRelease;
		watsonStreaming.m_mustListen = true;
    }
    public void OnPointerUpButton()
    {
        txtTalk.text = HOLD_TEXT;
        imgMic.color = colorHold;
        imgButton.color = colorHold;
		watsonStreaming.m_mustListen = false;
		watsonStreaming.Listen(false);
		watsonStreaming.Listen (true);
    }
}
