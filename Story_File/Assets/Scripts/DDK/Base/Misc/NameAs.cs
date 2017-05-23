//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;


namespace DDK.Base.UI
{
	/// <summary
	/// Renames the object it is attached to, as a child class specifies.
	/// </summary>
	[ExecuteInEditMode]
	public abstract class NameAs : MonoBehaviour 
    {
#if UNITY_EDITOR
        [System.Serializable]
        public class SiblingIndex
        {
            public bool enable;
            [Tooltip("If cero (0), this object's sibling index will be used, if one (1) the direct parent's sibling index will be used, and so on...")]
            [IndentAttribute( 1 )]
            public byte ancestorOffset;
            [IndentAttribute( 1 )]
            public int offset;
            [Tooltip( "If true, the value will be added after the /prefix/ or /suffix/" )]
            [IndentAttribute( 1 )]
            public bool after;


            public string Get( GameObject obj )
            {
                if( !enable )
                    return "";
                return ( obj.GetAncestor( ancestorOffset ).transform.GetSiblingIndex() + offset ).ToString();
            }
        }


		[HelpBoxAttribute]
		public string message = "Editor Only, this won't be compiled on build";
		public bool stopUpdate;
        [SerializeField]
        private string prefix;
        public SiblingIndex siblingIndexAsPrefix = new SiblingIndex();
        [SerializeField]
        private string suffix;
        public SiblingIndex siblingIndexAsSuffix = new SiblingIndex();
		[Tooltip("The name's characters limit. If cero (0), the name won't be clamped")]
		[NotLessThan( 0 )]
		public int clamp = 15;


        protected string Prefix
        {
            get
            {
                string _prefix = prefix;
                if( siblingIndexAsPrefix.enable )
                {
                    if( siblingIndexAsPrefix.after )
                    {
                        _prefix += siblingIndexAsPrefix.Get( gameObject );
                    }
                    else _prefix = siblingIndexAsPrefix.Get( gameObject ) + _prefix;
                }
                return _prefix;
            }
        }
        protected string Suffix
        {
            get
            {
                string _suffix = suffix;
                if( siblingIndexAsSuffix.enable )
                {
                    if( siblingIndexAsSuffix.after )
                    {
                        _suffix += siblingIndexAsSuffix.Get( gameObject );
                    }
                    else _suffix = siblingIndexAsSuffix.Get( gameObject ) + _suffix;
                }
                return _suffix;
            }
        }
		
		
		protected virtual void OnEnable() 
        {			
			if( Application.isPlaying )
				Destroy ( this );
		}
#endif
	}
}
