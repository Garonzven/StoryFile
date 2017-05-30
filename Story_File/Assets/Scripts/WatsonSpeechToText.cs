using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Logging;

public class WatsonSpeechToText : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_AudioClip = new AudioClip();
    private SpeechToText m_SpeechToText = new SpeechToText();

    void Start()
    {
        m_SpeechToText.Recognize(m_AudioClip, HandleOnRecognize);
    }

    void HandleOnRecognize(SpeechRecognitionEvent result)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    string text = alt.transcript;
                    Debug.Log(string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
                }
            }
        }
    }

}

