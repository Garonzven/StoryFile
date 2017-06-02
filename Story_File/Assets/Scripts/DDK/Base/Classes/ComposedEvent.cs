//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine.Events;
using DDK.Base.Classes;
using UnityEngine;
using DDK.Base.Extensions;
using System.Collections;
using DDK.Base.Statics;

namespace DDK.Base.Classes 
{
    /// <summary>
    /// This is similar to Unity's UnityEvent but ads extra functionality by allowing to execute some events once, 
    /// adding a delay and also an easy and fast way of enabling/disabling or activating/deactivating components or 
    /// gameObjects without having to reference them 1 by 1 in the UnityEvent. 
    /// <para>
    /// </para>
    /// <para>
    /// </para>
    /// In the other hand, you can also print to 
    /// the console all events that are invoked using this class, just by clicking in the menu bar: 
    /// Custom/Preferences/Toggle Composed Events Console Logs.
    /// </summary>
	[System.Serializable]
	public class ComposedEvent 
	{
		protected Object _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="DDK.Base.Classes.ComposedEvent"/> class.
		/// </summary>
		/// <param name="context">The context object for debugging purposes.</param>
		public ComposedEvent( Object context = null )
		{
			_context = context;
		}


		[Tooltip("If true, when the events are executed the first time, they will be removed.")]
		public bool once;
		[NotLessThan( 0f )]
		public float delay;
		[ShowIfAttribute( "_Delay", 1 )]
		[Tooltip("If true, the delay will not take into account the Time.timeScale")]
		public bool realtime;
		public EnableDisableState onActionStates = new EnableDisableState();
		public UnityEvent onAction = new UnityEvent();


		protected bool _Delay()
		{
			return delay > 0f;
		}
		

		/// <summary>
		/// Invoke the specified events.
		/// </summary>
		/// <param name="context">Context object for debugging purposes.</param>
		public void Invoke( Object context = null )
		{
			Invoke( delay, context );
		}
		/// <summary>
		/// Invoke the specified events.
		/// </summary>
		/// <param name="context">Context object for debugging purposes.</param>
		public void Invoke( float delay, Object context = null )
		{
			this._context = context;
			_Invoke( delay );
		}


		protected void _Invoke( float delay )
		{
			_InvokeCo( delay ).Start();
		}
		protected IEnumerator _InvokeCo( float delay )
		{
			if( delay > 0f )
			{
				if( realtime )
					yield return Utilities.WaitForRealSeconds( delay ).Start();
				else yield return new WaitForSeconds( delay );
			}
			_DebugEvents();
			onActionStates.SetStates();
			try{
				onAction.Invoke();
			} catch( System.ArgumentException e )//Failed to convert parameters..
			{
				Debug.LogWarning ( e.Message );
			}
			
			_Reset ().Start();
		}
		protected IEnumerator _Reset()
		{
			yield return null;
			while( !onActionStates._areSet )
				yield return null;
			if( once )
			{
				onActionStates = new EnableDisableState();
				onAction.RemoveAllListeners();
				onAction = new UnityEvent();
			}
		}
		protected void _DebugEvents()
		{
#if UNITY_EDITOR
			string msgExtra = ": ";
			try {
				int uEvtsCount = onAction.GetPersistentEventCount();
				int enableBehavioursCount = onActionStates.enable.behaviours.Length;
				int enableObjsCount = onActionStates.enable.objs.Length;
				int enableCollidersCount = onActionStates.enable.colliders.Length;
				int enableRenderersCount = onActionStates.enable.renderers.Length;
				int disableBehavioursCount = onActionStates.disable.behaviours.Length;
				int disableObjsCount = onActionStates.enable.objs.Length;
				int disableCollidersCount = onActionStates.enable.colliders.Length;
				int disableRenderersCount = onActionStates.enable.renderers.Length;
				if( uEvtsCount > 0 || enableBehavioursCount > 0 || enableObjsCount > 0 || enableCollidersCount > 0 ||  enableRenderersCount > 0 ||
				   disableBehavioursCount > 0 || disableObjsCount > 0 || disableCollidersCount > 0 || disableRenderersCount > 0 )
				{
					if( uEvtsCount == 1 )
					{
						_context = onAction.GetPersistentTarget(0);
						msgExtra = "\n\t Method: \"" + onAction.GetPersistentMethodName(0) + "\"";
					}
					else for( int j=0; j<uEvtsCount; j++ )
					{
						msgExtra += "\n\t "+ (j+1) +"- Persistent Target: \""+ onAction.GetPersistentTarget(j).ToString().
							GetFileExtension().RemoveLastEndPoint( true, false, ")" ) + "\" Invoking Method: \"" + onAction.GetPersistentMethodName(j) + "\"";
					}
					for( int j=0; j<enableBehavioursCount; j++ )
					{
						msgExtra += "\n\t "+ (j+1) +"- Enabling Behaviour: \""+ onActionStates.enable.behaviours[j].name + "\"";
					}
					for( int j=0; j<enableObjsCount; j++ )
					{
						msgExtra += "\n\t "+ (j+1) +"- Activating/Instantiating GameObject: \""+ onActionStates.enable.objs[j].name + "\"";
					}
					for( int j=0; j<enableCollidersCount; j++ )
					{
						msgExtra += "\n\t "+ (j+1) +"- Enabling Collider: \""+ onActionStates.enable.colliders[j].name + "\"";
					}
					for( int j=0; j<enableRenderersCount; j++ )
					{
						msgExtra += "\n\t "+ (j+1) +"- Enabling Renderer: \""+ onActionStates.enable.renderers[j].name + "\"";
					}
					for( int j=0; j<disableBehavioursCount; j++ )
					{
						msgExtra += "\n\t "+ (j+1) +"- Disabling Behaviour: \""+ onActionStates.disable.behaviours[j].name + "\"";
					}
					for( int j=0; j<disableObjsCount; j++ )
					{
						msgExtra += "\n\t "+ (j+1) +"- Deactivating/Destroying GameObject: \""+ onActionStates.disable.objs[j].name + "\"";
					}
					for( int j=0; j<disableCollidersCount; j++ )
					{
						msgExtra += "\n\t "+ (j+1) +"- Disabling Collider: \""+ onActionStates.disable.colliders[j].name + "\"";
					}
					for( int j=0; j<disableRenderersCount; j++ )
					{
						msgExtra += "\n\t "+ (j+1) +"- Disabling Renderer: \""+ onActionStates.disable.renderers[j].name + "\"";
					}
                    if( UnityEditor.EditorPrefs.GetBool( "ComposedEventsLogs" ) )
                    {
                        string contextName = ( _context != null ) ? _context.name : "_NoContext_";
                        string msg = "ComposedEvent in context object \"" + contextName + "\" is invoking some actions" + msgExtra;
                        onAction.AddListener( () => Debug.Log ( msg, _context ) );   
                    }
				}
			} catch( System.NullReferenceException e )
			{
					Debug.LogWarning( e.Message );
			}
#endif
		}
	}
}
