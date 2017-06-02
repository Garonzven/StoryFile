//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.ObjManagement {

	[ExecuteInEditMode]
	public class CopyCompsFrom : MonoBehaviour {

		[Tooltip("If false, Awake() will be used instead")]
		public bool onStart;
        [ShowIfAttribute( "_OnStart", true )]
        public bool onEnable;
		[Tooltip("THIS WILL BE REMOVED. The source object")]
		[ReadOnlyAttribute]
		public string from;//THIS WILL BE REMOVED /source/ WILL BE USED INSTEAD
        public bool sourceMustBeActiveInHierarchy;
		public SearchableGameObject source = new SearchableGameObject();
        [Header("Events")]
		[Tooltip("If true, this component is destroyed after the copy is done")]
		public bool destroyAfter = true;
		[Tooltip("If true, the source object /source/ is destroyed after the copy is done")]
		public bool destroySourceAfter = true;


        #if UNITY_EDITOR
        protected bool _OnStart()
        {
            return onStart;
        }
        #endif



		void Awake() {

#if UNITY_EDITOR
			Update ();
			if( !Application.isPlaying )
				return;
#endif
			if ( !source.m_gameObject )
			{
				enabled = false;
			}
			if( !onStart )
			{
				Copy();
			}
		}

		// Use this for initialization
		void Start () {

#if UNITY_EDITOR
			if( !Application.isPlaying )
				return;
#endif
            if ( !source.m_gameObject )
            {
                enabled = false;
            }
			if( onStart )
			{
				Copy();
			}
		}
        void OnEnable()
        {
            Awake();
        }

#if UNITY_EDITOR
		void Update()
		{
			if( string.IsNullOrEmpty( source.objName ) && !string.IsNullOrEmpty( from ) )
			{
				source = new SearchableGameObject( from );
			}
		}
#endif



		public void Copy()
		{
            if( source.m_gameObject && !source.m_gameObject.activeInHierarchy )
                return;
			transform.CopyTransformFrom( source.m_gameObject );
			gameObject.CopyAllCompsFrom( source.m_gameObject );
			if( destroySourceAfter )
			{
				Destroy( source.m_gameObject );
			}
			if( destroyAfter )
			{
				Destroy( this );
			}
		}
	}

}
