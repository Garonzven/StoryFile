using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using DDK.Base.Classes;
using DDK.Base.Extensions;

namespace DDK.Base.Events
{
    /// <summary>
    /// This class binds collider events to <see cref="DelayedAction"/>s
    /// so they can be called when a collider enters, stays, or exits
    /// the associated <see cref="Collider"/> these events can be fired.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class OnColliderEvents : MonoBehaviour
    {
        [Serializable]
        public struct ColliderEvents
        {
            public DelayedAction[] onEnter;
            public DelayedAction[] onExit;
            public DelayedAction[] onStay;
        }

        /// <summary>
        /// A list of <see cref="Collider"/> that can be accepted by the <see cref="_collider"/>
        /// </summary>
		[Tooltip( "A list of /Collider/ that can be accepted by the /_collider/" )]
        public List<Collider> acceptedObjects;
        /// <summary>
        /// A list of objects names that can be accepted by the <see cref="_collider"/>
        /// </summary>
		[Tooltip( "A list of objects names that can be accepted by the /_collider/" )]
        public string[] acceptedNames;
        /// <summary>
        /// A list of objects tags that can be accepted by the <see cref="_collider"/>
        /// </summary>
		[Tooltip( "A list of objects tags that can be accepted by the /_collider/" )]
        public string[] acceptedTags;
        /// <summary>
        /// A list of objects layers that can be accepted by
        /// the <see cref="_collider"/>. Though layer-to-layer collisions
        /// can be already setup through the Unity Editor, having this
        /// field can be useful for accepting many objects.
        /// </summary>
		[Tooltip( "A list of objects layers that can be accepted by  the /_collider/. Though layer-to-layer collisions" +
			"can be already setup through the Unity Editor, having this field can be useful for accepting many objects." )]
        public LayerMask acceptedLayers;

        /// <summary>
        /// Enter / Stay / Exit events for non-trigger colliders
        /// </summary>
        [Header ( "Events" )]
        [ShowIf ( "_SourceIsCollider" )]
        public ColliderEvents collisionEvents;
        /// <summary>
        /// Enter / Stay / Exit events for trigger colliders
        /// </summary>
        [ShowIf ( "_SourceIsTrigger" )]
        public ColliderEvents triggerEvents;
		[Tooltip( "With this enabled, all events with a /description/ that contains an acceptedObject's " +
			"name, will be invoked and the rest ignored." )]
        public bool descriptionAsFilter;

        private Collider _collider;

        public Collider SourceCollider
        {
            get
            {
                if ( _collider != null )
                {
                    return _collider;
                }

                _collider = GetComponent<Collider>();

                if ( !_collider || !_collider.enabled )
                {
                    _collider = GetComponentInChildren<Collider>();
                    if( !_collider )
                    {
                        enabled = false;
                    }
                }

                return _collider;
            }
        }

        public bool IsTrigger
        {
            get { return SourceCollider.isTrigger; }
        }


        private void Awake()
        {
            if ( acceptedObjects != null )
            {
                var childs = new List<Collider>();

                for ( int i = 0; i < acceptedObjects.Count; i++ )
                {
                    childs.AddRange
                    (
                        acceptedObjects[i].GetComponentsInChildren<Collider>()
                    );
                }

                acceptedObjects.AddRange ( childs );
            }
        }

        // Use this for initialization
        private void Start ()
        {
        }

        protected bool _SourceIsCollider()
        {
            return !IsTrigger;
        }

        protected bool _SourceIsTrigger()
        {
            return IsTrigger;
        }

        void OnCollisionEnter ( Collision collision )
        {
            if ( !_IsAccepted ( collision.collider ) )
            {
                return;
            }

            _CallAction ( collisionEvents.onEnter, collision.collider );
        }

        void OnCollisionStay ( Collision collision )
        {
            if ( !_IsAccepted ( collision.collider ) )
            {
                return;
            }

            _CallAction ( collisionEvents.onStay, collision.collider );
        }

        void OnCollisionExit ( Collision collision )
        {
            if ( !_IsAccepted ( collision.collider ) )
            {
                return;
            }

            _CallAction ( collisionEvents.onExit, collision.collider );
        }

        void OnTriggerEnter ( Collider other )
        {
            if ( !_IsAccepted ( other ) )
            {
                return;
            }

            _CallAction ( triggerEvents.onEnter, other ); ;
        }

        void OnTriggerStay ( Collider other )
        {
            if ( !_IsAccepted ( other ) )
            {
                return;
            }

            _CallAction ( triggerEvents.onStay, other ); ; ;
        }

        void OnTriggerExit ( Collider other )
        {
            if ( !_IsAccepted ( other ) )
            {
                return;
            }

            _CallAction ( triggerEvents.onExit, other ); ;
        }

        public void Remove ( Collider col )
        {
            acceptedObjects.Remove ( col );
        }

        public void RemoveWithChildren ( GameObject col )
        {
            var childs = col.GetComponentsInChildren<Collider>();
            acceptedObjects.RemoveAll
            (
                t => t.gameObject == col || childs.Contains ( t )
            );
        }

        private void _CallAction ( DelayedAction[] list, Collider go )
        {
            if ( descriptionAsFilter )
            {
                list.Where( t => {
                    if( !string.IsNullOrEmpty( t.description ) )
                    {
                        return t.description.Contains ( go.name );
                    }
                    return false;
                } ).ToArray().InvokeAll();
            }
            else
            {
                list.InvokeAll();
            }
        }

        private bool _IsAccepted ( Collider go )
        {
            if ( acceptedObjects.Contains ( go ) )
            {
                return true;
            }

            if ( acceptedNames.Contains ( go.name ) )
            {
                return true;
            }

            if ( acceptedTags.Contains ( go.tag ) )
            {
                return true;
            }

            if ( go.gameObject.IsInLayerMask ( acceptedLayers ) )
            {
                return true;
            }

            return false;
        }
    }
}
