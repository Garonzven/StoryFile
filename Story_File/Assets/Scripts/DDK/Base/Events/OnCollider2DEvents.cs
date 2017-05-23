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
	/// the associated <see cref="Collider2D"/> these events can be fired.
	/// </summary>
	/// <seealso cref="MonoBehaviour" />
	public class OnCollider2DEvents : MonoBehaviour
	{
		[Serializable]
		public struct Collider2DEvents
		{
			public DelayedAction[] onEnter;
			public DelayedAction[] onExit;
			public DelayedAction[] onStay;
		}
		
		/// <summary>
		/// A list of <see cref="Collider2D"/> that can be accepted by the <see cref="_collider"/>
		/// </summary>
		[Tooltip( "A list of /Collider2D/ that can be accepted by the /_collider/" )]
		public List<Collider2D> acceptedObjects;
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
		         " can be already setup through the Unity Editor, having this field can be useful for accepting many objects." )]
		public LayerMask acceptedLayers;
		
		/// <summary>
		/// Enter / Stay / Exit events for non-trigger colliders
		/// </summary>
		[Header ( "Events" )]
		[ShowIf ( "_SourceIsCollider2D" )]
		public Collider2DEvents collisionEvents;
		/// <summary>
		/// Enter / Stay / Exit events for trigger colliders
		/// </summary>
		[ShowIf ( "_SourceIsTrigger" )]
		public Collider2DEvents triggerEvents;
		[Tooltip( "With this enabled, all events with a /description/ that contains an acceptedObject's " +
		         "name, will be invoked and the rest ignored." )]
		public bool descriptionAsFilter;
		

		private Collider2D _collider;		
		public Collider2D SourceCollider2D
		{
			get
			{
				if ( _collider != null )
				{
					return _collider;
				}
				
				_collider = GetComponent<Collider2D>();
				
                if ( !_collider || !_collider.enabled )
				{
                    _collider = GetComponentInChildren<Collider2D>();
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
			get { return SourceCollider2D.isTrigger; }
		}
		
		
		private void Awake()
		{
			if ( acceptedObjects != null )
			{
				var childs = new List<Collider2D>();
				
				for ( int i = 0; i < acceptedObjects.Count; i++ )
				{
					childs.AddRange( acceptedObjects[i].GetComponentsInChildren<Collider2D>() );
				}
				
				acceptedObjects.AddRange ( childs );
			}
		}
		
		// Use this for initialization
		private void Start ()
		{
		}
		
		protected bool _SourceIsCollider2D()
		{
			return !IsTrigger;
		}
		
		protected bool _SourceIsTrigger()
		{
			return IsTrigger;
		}
		
		void OnCollisionEnter2D ( Collision2D collision )
		{
			if ( !_IsAccepted ( collision.collider ) )
			{
				return;
			}
			
			_CallAction ( collisionEvents.onEnter, collision.collider );
		}
		
		void OnCollisionStay2D ( Collision2D collision )
		{
			if ( !_IsAccepted ( collision.collider ) )
			{
				return;
			}
			
			_CallAction ( collisionEvents.onStay, collision.collider );
		}
		
		void OnCollisionExit2D ( Collision2D collision )
		{
			if ( !_IsAccepted ( collision.collider ) )
			{
				return;
			}
			
			_CallAction ( collisionEvents.onExit, collision.collider );
		}
		
		void OnTriggerEnter2D ( Collider2D other )
		{
			if ( !_IsAccepted ( other ) )
			{
				return;
			}
			
			_CallAction ( triggerEvents.onEnter, other ); ;
		}
		
		void OnTriggerStay2D ( Collider2D other )
		{
			if ( !_IsAccepted ( other ) )
			{
				return;
			}
			
			_CallAction ( triggerEvents.onStay, other ); ; ;
		}
		
		void OnTriggerExit2D ( Collider2D other )
		{
			if ( !_IsAccepted ( other ) )
			{
				return;
			}
			
			_CallAction ( triggerEvents.onExit, other ); ;
		}
		
		public void Remove ( Collider2D col )
		{
			acceptedObjects.Remove ( col );
		}
		
		public void RemoveWithChildren ( GameObject col )
		{
			var childs = col.GetComponentsInChildren<Collider2D>();
			acceptedObjects.RemoveAll( t => t.gameObject == col || childs.Contains ( t ) );
		}
		
		private void _CallAction ( DelayedAction[] list, Collider2D go )
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
		
		private bool _IsAccepted ( Collider2D go )
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

