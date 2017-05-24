using DDK.Base.Extensions;

using UnityEngine;

namespace DDK.Base.Events.States
{
    /// <summary>
    /// Allows extending another class to execute some events after something happens.
    /// </summary>
    public class FinalState : MonoBehaviourExt
    {
        public bool disableThisAfter;
        public bool destroyThisAfter;
        public bool destroyObjectAfter;

        public bool IsBeingDestroyed { get; private set; }

        protected virtual void _FinalStateAction()
        {
            if ( disableThisAfter && ! ( destroyThisAfter || destroyObjectAfter ) &&
                    enabled )
            {
                enabled = false;
            }
            else if ( destroyThisAfter && !destroyObjectAfter )
            {
                DestroyImmediate ( this );
            }
            else if ( destroyObjectAfter )
            {
                Destroy ( gameObject );
            }
        }

        void OnApplicationQuit()
        {
            IsBeingDestroyed = true;
        }
    }
}