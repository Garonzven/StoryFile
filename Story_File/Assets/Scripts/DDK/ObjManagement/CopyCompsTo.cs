//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.ObjManagement {

    public class CopyCompsTo : MonoBehaviour {

        [System.Serializable]
        public struct DestroyEvents
        {
            [Tooltip("If true, this component is destroyed after the copy is done")]
            public bool destroyAfter;
            [Tooltip("If true, the target object /target/ is destroyed after the copy is done")]
            public bool destroyTargetAfter;
            [Tooltip("If true, this gameObject is destroyed after the copy is done")]
            public bool destroySourceAfter;
        }


        [ShowIfAttribute( "_OnStartOrOnEnable", true )]
        public bool onAwake;
        [ShowIfAttribute( "_OnAwakeOrOnEnable", true )]
        public bool onStart;
        [ShowIfAttribute( "_OnAwakeOrStart", true )]
        public bool onEnable;
        public SearchableGameObject target = new SearchableGameObject();
        [Header("Events")]
        public DestroyEvents destroyEvents = new DestroyEvents();


        #if UNITY_EDITOR
        protected bool _OnStartOrOnEnable()
        {
            return onStart || onEnable;
        }
        protected bool _OnAwakeOrOnEnable()
        {
            return onAwake || onEnable;
        }
        protected bool _OnAwakeOrStart()
        {
            return onAwake || onStart;
        }
        #endif



        void Awake() {
            
            if( !onAwake )
                return;

            CopyToTarget();
        }
        // Use this for initialization
        void Start () {

            if( !onStart )
                return;
            CopyToTarget();
        }
        void OnEnable()
        {
            if( !onEnable )
                return;
            CopyToTarget();
        }



        public void CopyToTarget()
        {
            if ( !target.m_gameObject )
            {
                enabled = false;
                Debug.LogWarning( "There is no target", gameObject );
                return;
            }

            transform.CopyTransformTo( target.m_gameObject );
            gameObject.CopyAllCompsTo( target.m_gameObject );
            if( destroyEvents.destroyTargetAfter )
            {
                Destroy( target.m_gameObject );
            }
            if( destroyEvents.destroySourceAfter )
            {
                Destroy( gameObject );
            }
            else if( destroyEvents.destroyAfter )
            {
                Destroy( this );
            }
        }
    }

}

