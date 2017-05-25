using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AmandaHandler : MonoBehaviour
{
    public Text txtTalk;
    private const string HOLD_TEXT = "HOLD TO TALK";
    private const string RELEASE_TEXT = "RELEASE TO LISTEN";


    public void onPointerDownButton()
    {
        txtTalk.text = "RELEASE TO LISTEN";
    }
    public void onPointerUpButton()
    {
        txtTalk.text = "HOLD TO TALK";
    }
}
