//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.UI;
using DDK.Base.Extensions;


namespace DDK.Base.UI
{
    /// <summary>
    /// Sets the specified Text's text value to the specified sibling's index
    /// </summary>
    [RequireComponent( typeof( Text ) )]
    [ExecuteInEditMode]
    public class TextValueAsSiblingIndex : MonoBehaviour 
    {
        [Tooltip("Each index represents an ancestor, cero (0) is this gameObject, one (1) is the parent, two (2) is the grand parent, and so on...")]
        public byte siblingIsAncestor = 0;
        public int addToIndex = 1;
        public string prefix;
        public string suffix;
        public bool destroyOnPlay;


        [HideInInspector]
        public Text m_text;
        [HideInInspector]
        public Transform m_sibling;


        // Use this for initialization
        void Start () 
        {
            _Init();

            #if UNITY_EDITOR
            if( !Application.isPlaying )
                return;
            #endif
            if( destroyOnPlay )
            {                
                Destroy( this );
            }
        }

        // Update is called once per frame
        void Update () 
        {
            #if UNITY_EDITOR
            if( !Application.isPlaying )
            {
                _Init();
            }
            #endif
            m_text.text = prefix + ( m_sibling.GetSiblingIndex() + addToIndex ) + suffix;
        }


        protected void _Init()
        {
            m_sibling = gameObject.GetAncestor( siblingIsAncestor ).transform;
            if( m_text )
                return;
            m_text = GetComponentInChildren<Text>();
            if( !m_text )
                m_text = GetComponentInParent<Text>();
            if( !m_text )
                enabled = false;
        }
    }
}
