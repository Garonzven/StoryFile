using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AmandaScript : MonoBehaviour
{
    public bool isTalking = false;
    public Text txtTalk;
    void Start()
    {
        txtTalk.text = "HOLD TO TALK";
    }

    void Update()
    {
        if (isTalking)
        {
            txtTalk.text = "RELEASE TO LISTEN";
        }

        else if (!isTalking)
        {

            txtTalk.text = "HOLD TO TALK";
        }
    }
    public void onPointerDownButton()
    {
        isTalking = true;
    }
    public void onPointerUpButton()
    {
        isTalking = false;
    }
}
