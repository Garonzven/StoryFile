//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.ObjManagement {

	/// <summary>
	/// Attach to an object to assign its sibling index.
	/// </summary>
	[ExecuteInEditMode]
	public class SetSiblingIndex : MonoBehaviour {

		[HelpBoxAttribute]
		public string msg = "It is set OnEnable()";
		public float delay;
		[Tooltip("If higher than 0, it is set on enable or in editor mode if specified")]
        [ShowIfAttribute( "_AlwaysLast", true )]
		public int index = -1;
        [Tooltip("If true, the index will be set backwards, from the last child to the first (0 equals the last child)")]
        [ShowIfAttribute( "_AlwaysLast", true, 1 )]
        public bool backwards;
        [Tooltip("If true, this object will always be the last sibling")]
        public bool alwaysLast;
		public bool executeInEditMode = false;
        public bool update;
		[Tooltip("Valid just on play mode. This has priority over /destroyThisAfterSet/")]
		[ShowIfAttribute( "_ExecuteInEditMode" )]
		public bool disableAfterSet;
		[Tooltip("Valid just on play mode. If true, this script/component is destroyed after sibling index is set")]
		[ShowIfAttribute( "_ExecuteInEditMode" )]
		public bool destroyThisAfterSet;


        #if UNITY_EDITOR
        protected bool _ExecuteInEditMode()
        {
            return executeInEditMode;
        }
        protected bool _AlwaysLast()
        {
            index = Mathf.Clamp( index, -1, int.MaxValue );
            return alwaysLast;
        }
        #endif



		void OnEnable()
		{
			Invoke( "Set", delay );
		}

		// Use this for initialization
		void Start () { }//Allows enabling/disabling this component

		// Update is called once per frame
		void Update () 
        {
#if UNITY_EDITOR
			if( !Application.isPlaying && enabled && executeInEditMode )
			{
				Set ();
			}
			else
#endif
            if( update && Application.isPlaying )
			{
				Set ();
			}
		}



		public void Set()
		{
            if( alwaysLast )
            {
                index = transform.parent.childCount - 1;
            }
			if( index >= 0 )
            {
                int sIndex = index;
                if( backwards )
                {
                    int lastIndex = transform.parent.childCount - 1;
                    sIndex = Mathf.Clamp( lastIndex - index, 0, lastIndex );
                }
                transform.SetSiblingIndex( sIndex );
            }
			if( Application.isPlaying )
			{
				if( disableAfterSet )
				{
					enabled = false;
				}
				else if( destroyThisAfterSet )
				{
					Destroy ( this );
				}
			}
		}

	}

}
