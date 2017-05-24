using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class WelcomeScript : MonoBehaviour
    {
        public GameObject welcomePanel;
        public GameObject amandaPanel;

        // Use this for initialization
        void Start()
        {

        }

        public void GetStarted()
        {
            welcomePanel.gameObject.SetActive(false);
            amandaPanel.gameObject.SetActive(true);
        }

        // Update is called once per frame
        /*void Update () {
		
        }*/
    }
}

