#if UNITY_EDITOR
//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;


namespace DDK.Base.Misc 
{
    /// <summary>
    /// This allows creating line in editor scripts to separate inspector parameters. Just call the static Draw() function.
    /// </summary>
    public static class GUILineStyle 
    {       
        private static GUIStyle _lineStyle = null;


        public static GUIStyle EditorLine
        {
            get{
                if( _lineStyle == null )
                    _Create ();
                return _lineStyle;
            }
        }


        public static void Draw( float width = -1, float height = 1f )
        {
            if( width < 0 )
            {
                GUILayout.Box( GUIContent.none, GUILineStyle.EditorLine, GUILayout.ExpandWidth(true), 
                    GUILayout.Height( Mathf.Clamp( height, 1f, float.MaxValue ) ) );
            }
            else GUILayout.Box( GUIContent.none, GUILineStyle.EditorLine, GUILayout.Width( width ), 
                GUILayout.Height( Mathf.Clamp( height, 1f, float.MaxValue ) ) );
        }


        private static void _Create()
        {
            _lineStyle = new GUIStyle("box");
            _lineStyle.border.top = _lineStyle.border.bottom = 1;
            _lineStyle.margin.top = _lineStyle.margin.bottom = 1;
            _lineStyle.padding.top = _lineStyle.padding.bottom = 1;
        }
    }
}
#endif
