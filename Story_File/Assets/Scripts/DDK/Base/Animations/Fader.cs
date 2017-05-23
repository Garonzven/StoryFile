//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using UnityEngine.Events;
using DDK.Base.Components;
using DDK.Base.Statics;


namespace DDK.Base.Animations
{
	/// <summary>
	/// Attach to an object to allow fading a CanvasGroup or RendererGroup.
	/// </summary>
	public class Fader : MonoBehaviourExt
	{
		[System.Serializable]
		public class FaderEvents 
        {
            [Tooltip("If true, the CanvasGroup properties: interactable and blockRaycasts. NOTE: Ensure you are enabling " +
                "the Fader to show the group, and disabling it to hide it, otherwise, this might not work as expected")]
            public bool handleGroupProperties;
			[Tooltip("If true, this component will be destroyed after this component is enabled, or disabled")]
			public bool destroyThisAfterDone;
			[Tooltip("If true, this gameobject will be destroyed after this component is disabled")]
			public bool destroyObjAfterDisabled;
			[Tooltip("If true, this gameobject will be deactivated after this component is disabled")]
			public bool deactivateObjAfterDisabled;
			public ComposedEvent onEnable = new ComposedEvent();
			public ComposedEvent onEnablePostDelay = new ComposedEvent();
			public ComposedEvent onDisable = new ComposedEvent();
			public ComposedEvent onDisablePostDelay = new ComposedEvent();
            [Tooltip("When the target alpha is higher than the /onDisableTarget/")]
			public ComposedEvent onFadedIn = new ComposedEvent();
            [Tooltip("When the target alpha is lower than the /onDisableTarget/")]
			public ComposedEvent onFadedOut = new ComposedEvent();
		}



		[HelpBoxAttribute]
		public string msg = "YOU CAN use direct references in the Groups arrays, references won't get lost";
		[ShowIfAttribute( "_IncludeInactive" )]
		[Tooltip("If true, and any Group is empty and with no Group Component on its parent, then its inactive children" +
			" will be included in the group component search")]
		public bool includeInactive;
		[Tooltip("If empty, this or the parent's CanvasGroup will be used. If also empty, the children will be searched")]
		public CanvasGroup[] canvasGroups = new CanvasGroup[0];
		[Tooltip("If empty, this or the parents's RendererGroup will be used. If also empty, the children will be searched")]
		public RendererGroup[] rendererGroup = new RendererGroup[0];
		[HideInInspector]
		public string[] _canvasGroupsNames;
		[HideInInspector]
		public string[] _rendererGroupsNames;

        /// <summary>
        /// The unscaled delay. If true, the delays won't take into account the Time.timeScale. By default the fade animations are unscaled
        /// </summary>
		[Space(10f)]
		[DisplayNameAttribute( "Unscaled Delays" )]
		[Tooltip("If true, the delays won't take into account the Time.timeScale. By default the fade animations are unscaled")]
		public bool unscaledTime;
		[NotLessThan( 0f )]
		public float onEnableDelay;
		[Tooltip("The target alpha when this component is enabled")]
		[Range(0f, 1f)]
		public float onEnableTarget = 1f;
		[NotLessThan( 0f )]
		public float onDisableDelay;
		[Tooltip("The target alpha when this component is disabled")]
		[Range(0f, 1f)]
		public float onDisableTarget = 0f;
		[NotLessThan( 0f )]
		public float duration = 0.5f;
		public FaderEvents events = new FaderEvents();


		protected bool _IncludeInactive()
		{
			_UpdateGroups(); // <-- THIS IS JUST TO UPDATE THE GROUPS NAMES IN THE EDITOR, without making this class ExecuteInEditMode
			return canvasGroups.Length == 0 || rendererGroup.Length == 0;
		}		
		protected bool _UpdateGroups()
		{
			if( canvasGroups == null || _canvasGroupsNames == null )
				return false;
			if( canvasGroups.Length == _canvasGroupsNames.Length )
			{
				for( int i=0; i<canvasGroups.Length; i++ )
				{
					if( canvasGroups[i] == null )
					{
						canvasGroups[i] = _canvasGroupsNames[i].Find<CanvasGroup>();
					}
				}
			}
			if( rendererGroup.Length == _rendererGroupsNames.Length )
			{
				for( int i=0; i<rendererGroup.Length; i++ )
				{
					if( rendererGroup[i] == null )
					{
						rendererGroup[i] = _rendererGroupsNames[i].Find<RendererGroup>();
					}
				}
			}
			if( !canvasGroups.HasNull() )
			{
				_canvasGroupsNames = canvasGroups.GetNames();
			}
			if( !rendererGroup.HasNull() )
			{
				_rendererGroupsNames = rendererGroup.GetNames();
			}
			return false;
		}
		
		
		/// <summary>
		/// True if this component or this object is being destroyed.
		/// </summary>
		private bool _isBeingDestroyed;
		/// <summary>
		/// This prevents OnEnable from being called if the object hasn't being initialized.
		/// </summary>
		private bool _started;
        private bool _isFading;


		
		// Use this for initialization
		void Awake()
		{
			_UpdateGroups();
			if( canvasGroups.Length == 0 )
			{
				canvasGroups = new CanvasGroup[]{ GetComponentInParent<CanvasGroup>() };
				if( canvasGroups.Length == 0 )
				{
					canvasGroups = GetComponentsInChildren<CanvasGroup>( includeInactive );
				}
			}
			if( rendererGroup.Length == 0 )
			{
				rendererGroup = new RendererGroup[]{ GetComponentInParent<RendererGroup>() };
				if( rendererGroup.Length == 0 )
				{
					rendererGroup = GetComponentsInChildren<RendererGroup>( includeInactive );
				}
			}
		}		
		void Start() 
        {
			#if UNITY_EDITOR
			if( !Application.isPlaying )
				Awake();
			#endif
			_started = true;
			if( enabled )
				OnEnable();
		}			
		void OnEnable()
		{
			if( !_started )
				return;
            if( events.handleGroupProperties )
            {
                canvasGroups.SetInteractable( true );
            }
			Fade( onEnableDelay, onEnableTarget ).Start();
			//EVENTS
			events.onEnable.Invoke();
			events.onEnablePostDelay.Invoke( onEnableDelay );
		}		
		void OnDisable()
		{
			if( _isBeingDestroyed )
				return;
            if( events.handleGroupProperties )
            {
                canvasGroups.SetInteractable( false );
            }
			Fade( onDisableDelay, onDisableTarget ).Start();
			//EVENTS
			events.onDisable.Invoke();
			events.onDisablePostDelay.Invoke( onDisableDelay );
			Disabled().Start();
		}		
		void OnApplicationQuit()
		{
			_isBeingDestroyed = true;
		}
		
		

		public void SetDuration( float duration )
		{
			this.duration = duration.Clamp( 0f, float.MaxValue );
		}
		public IEnumerator Fade( float delay, float alphaTarget )
		{
			if( delay > 0f )
			{
				if( unscaledTime )
					yield return Utilities.WaitForRealSeconds( delay ).Start();
				else yield return new WaitForSeconds( delay );
			}

			if( _isBeingDestroyed )
			{
				yield break;
			}
            while( _isFading )
                yield return null;
            _isFading = true;
			if( canvasGroups.Length > 0 )
				canvasGroups.AlphaTo( alphaTarget, duration );
			if( rendererGroup.Length > 0 )
				rendererGroup.AlphaTo( alphaTarget, duration );
			Done( alphaTarget ).Start();
		}
		public void Fade( float alphaTarget )
		{
			Fade ( 0f, alphaTarget ).Start();
		}




		/// <summary>
		/// This waits for the onDisable Fade to end, and executes some events.
		/// </summary>
		IEnumerator Disabled()
		{
            if( onDisableDelay > 0f )
            {
                if( unscaledTime )
                    yield return Utilities.WaitForRealSeconds( onDisableDelay ).Start();
                else yield return new WaitForSeconds( onDisableDelay );
            }
            yield return Utilities.WaitForRealSeconds( duration ).Start();
			yield return null;
			if( this == null )
				yield break;
			//EVENTS
			if( gameObject != null && events.destroyObjAfterDisabled )
			{
				_isBeingDestroyed = true;
				Destroy( gameObject );
			}
			else if( gameObject != null && events.deactivateObjAfterDisabled )
			{
				gameObject.SetActive( false );
			}
		}
		/// <summary>
		/// This executes some events after a group is faded in/out.
		/// </summary>
		/// <param name="alphaTarget">Alpha target.</param>
		IEnumerator Done( float alphaTarget )
		{
			yield return Utilities.WaitForRealSeconds( duration ).Start();
			yield return null;
            _isFading = false;
			//EVENTS
            if( events.handleGroupProperties )
            {
                canvasGroups.SetBlocksRaycasts( alphaTarget > onDisableTarget );
            }
			if( alphaTarget > onDisableTarget )
			{
				events.onFadedIn.Invoke();
			}
			else 
            {
                events.onFadedOut.Invoke();
            }
			if( events.destroyThisAfterDone )
			{
				_isBeingDestroyed = true;
				Destroy ( this );
			}
		}
        void SetGroupProperties( bool state )
        {
            if( events.handleGroupProperties )
            {
                canvasGroups.SetInteractable( state );
                canvasGroups.SetBlocksRaycasts( state );
            }
        }
		
	}
	
}
