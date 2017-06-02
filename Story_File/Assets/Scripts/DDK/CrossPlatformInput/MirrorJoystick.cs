//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
#if CROSS_PLATFORM_INPUT
using UnityStandardAssets.CrossPlatformInput;
#endif


namespace DDK.CrossPlatformInput
{
    /// <summary>
    /// This can be used in various ways. Eg: If this is attached to an Aim target in a Top Down Shooter game, the aim 
    /// object is then parented to a Player, the player's aimer object (ie: a gun)  has a LookAt.cs component, so by 
    /// moving the Joystick, this will mirror it causing the aim to move around the player making it look in the 
    /// direction the user is pointing.
    /// </summary>
    /// <seealso cref="LookAt.cs"/>
    public class MirrorJoystick : MonoBehaviour 
    {
        #if MOBILE_INPUT
        public Joystick joystick;
        public int magnitude = 1;


        private Vector3 _startPos;


        // Use this for initialization
        void Start () {

            if( !joystick )
                enabled = false;

            _startPos = transform.position;
        }

        // Update is called once per frame
        void Update () {

            if( !Input.GetMouseButton( 0 ) )
                return;

            transform.position = _startPos + ( joystick.transform.position - joystick.m_StartPos ).normalized * magnitude;
        }
        #else
        [HelpBoxAttribute]
        public string msg = "Mobile Input is disabled. MOBILE_INPUT scripting symbol is not defined.";
        #endif
    }
}
