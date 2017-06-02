//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using System.Collections.Generic;
using DDK.Base.Managers;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DDK.Base.Statics;
using DDK.Base.Classes;
using DDK.Base.SoundFX;


namespace DDK.Base.Extensions 
{
	/// <summary>
	/// MonoBehaviour class extension. This can be used by any script to inherit from and add more functionality to 
    /// Unity's MonoBehaviour
	/// </summary>
	public class MonoBehaviourExt : MonoBehaviour 
    {
        #if UNITY_EDITOR
        [HideInInspector]
        public bool _useCustomIcon;
        [ShowIfAttribute( "_useCustomIcon" )]
        public Texture2D hierarchyIcon;

        [ContextMenu("Toggle Custom Hierarchy Icon")]
        public void _ToggleCustomIcon()
        {
            _useCustomIcon = !_useCustomIcon;
        }
        #endif


        private static EventSystem _evtSystem;
		/// <summary>
		/// This prevents the evtSystem from being searched on application quit
		/// </summary>
		private static bool _evtSystemWasSet;
		public static EventSystem m_EvtSystem 
        {
			get
            {
				if( !_evtSystem && !_evtSystemWasSet )
				{
					_evtSystem = EventSystem.current;
                    if( !_evtSystem )
                    {
                        _evtSystem = FindObjectOfType<EventSystem>();
                        if( !_evtSystem )
                        {
                            _evtSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
                        }
                    }
                    if( _evtSystem )
                    {
                        _evtSystemWasSet = true;
                    }
				}
				if( !_evtSystem )
                    Utilities.LogWarning ("There is no event system..");
				return _evtSystem;
			}
		}



		#region STATES
		public void EnableFor( float duration )
		{
			_EnableFor( true, duration ).Start();
		}
		public void DisableFor( float duration )
		{
			_EnableFor( false, duration ).Start();
		}
		public void EnableFor( AudioClip duration )
		{
			if( !duration )
				return;
			_EnableFor( true, duration.length ).Start();
		}
		public void DisableFor( AudioClip duration )
		{
			if( !duration )
				return;
			_EnableFor( false, duration.length ).Start();
		}
		public void Enable( float delay )
		{
			_Enable( delay ).Start();
		}
		public void Disable( float delay )
		{
			_Enable( delay, false ).Start();
		}
		public void Activate( float delay )
		{
            gameObject.SetActiveAfter( delay, true ).Run();
		}
		public void Deactivate( float delay )
		{
            gameObject.SetActiveAfter( delay, false ).Run();
		}
		/// <summary>
		/// If this object has a CanvasGroup or Button component, it will be set as /interactable/.
		/// </summary>
		public void SetInteractable( bool interactable, float delay )
		{
			CanvasGroup group = GetComponent<CanvasGroup>();
			if( group )
			{
				group.SetInteractable( true, delay );
				return;
			}
			Button graphic = GetComponent<Button>();
			if( graphic )
			{
				graphic.SetInteractable( true, delay );
				return;
			}
		}
		/// <summary>
		/// If this object has a CanvasGroup or Button component, it will be set as interactable.
		/// </summary>
		public void SetInteractable( float delay )
		{
			SetInteractable( true, delay );
		}
		/// <summary>
		/// If this object has a CanvasGroup or Button component, it will be set as not interactable.
		/// </summary>
		public void SetNotInteractable( float delay )
		{
			SetInteractable( false, delay );
		}
		/// <summary>
		/// If this object has a CanvasGroup or Button component, it will be set as interactable.
		/// </summary>
		public void SetInteractable( AudioClip delay )
		{
			SetInteractable( true, delay.length );
		}
		/// <summary>
		/// If this object has a CanvasGroup or Button component, it will be set as not interactable.
		/// </summary>
		public void SetNotInteractable( AudioClip delay )
		{
			SetInteractable( false, delay.length );
		}
		public void EnableEventSystem( bool enabled )
		{
			if( m_EvtSystem )
            {
                m_EvtSystem.enabled = enabled;
            }
			else Utilities.LogWarning( "No eventSystem to enable/disable!", gameObject );
		}


        public IEnumerator Enable( Behaviour comp, bool enable, float delay = 0f )
        {
            if( !comp )
                yield break;
            if( delay > 0f )
            {
                yield return new WaitForSeconds( delay );
            }
            comp.enabled = enable;
        }
		//PROTECTED / PRIVATE
		protected IEnumerator _EnableFor( bool enable, float duration )
		{
			enabled = enable;
			yield return new WaitForSeconds( duration );
			enabled = !enable;
		}
		protected IEnumerator _Enable( float delay, bool enable = true )
		{
            if( delay > 0f )
            {
                yield return new WaitForSeconds( delay );
            }
			if( this )
				enabled = enable;
		}

		public void Activate( GameObject obj, float delay, bool active = true )
		{
            obj.SetActiveAfter( delay, active ).Run();
		}
		#endregion

		#region INSTANTIATE
		public void Instantiate()
		{
			Instantiate( gameObject );
		}
		public void InstantiateOnTarget( Transform target, Transform parent )
		{
            InstantiateOnTargetAndGet( target, parent );
		}
        public GameObject InstantiateOnTargetAndGet( Transform target, Transform parent )
        {
            if( !target )
            {
                Utilities.LogWarning ( "The specified target Transform is null", gameObject );
                return null;
            }
            GameObject instance = GameObject.Instantiate( gameObject );
            instance.SetParent( parent );
            instance.transform.position = target.position;
            return instance;
        }
        internal GameObject InstantiateOnTarget( Transform target )
		{
            return InstantiateOnTargetAndGet( target, null );
		}
        internal GameObject InstantiateOnTargetAndItsParent( Transform target )
		{
			if( !target )
			{
                Utilities.LogWarning ( "The specified target Transform is null", gameObject );
                return null;
			}
            return InstantiateOnTargetAndGet( target, target.parent );
		}
        internal GameObject InstantiateOnTargetAndItsFirstAncestor( Transform target )
		{
			if( !target )
			{
                Utilities.LogWarning ( "The specified target Transform is null", gameObject );
                return null;
			}
			GameObject firstAncestor = target.gameObject.GetFirstParent();
			Transform _firstAncestor = null;
			if( firstAncestor )
				_firstAncestor = firstAncestor.transform;
            return InstantiateOnTargetAndGet( target, _firstAncestor );
		}
		public void InstantiateOnTarget( string target )
		{
			Transform _target = target.Find<Transform>();
			InstantiateOnTarget( _target );
		}
		public void InstantiateOnTargetAndItsParent( string target )
		{
			Transform _target = target.Find<Transform>();
			InstantiateOnTargetAndItsParent( _target );
		}
		public void InstantiateOnTargetAndItsFirstAncestor( string target )
		{
			Transform _target = target.Find<Transform>();
			if( !_target )
			{
                Utilities.LogWarning ( "The specified target Transform is null", gameObject );
				return;
			}
			GameObject firstAncestor = _target.gameObject.GetFirstParent();
			Transform _firstAncestor = null;
			if( firstAncestor )
				_firstAncestor = firstAncestor.transform;
			InstantiateOnTarget( _target, _firstAncestor );
		}
		public void Instantiate( GameObject obj )
		{
			obj.SetActiveInHierarchy();
		}
		public GameObject InstantiateObj( GameObject obj )
		{
			return obj.SetActiveInHierarchy();
		}
		#endregion

		#region TRANSFORM
		/// <summary>
		/// If no /obj/ this object will be used. Animates the /obj/ until it matches the specified's position and rotation
		/// </summary>
		public void AlignWith( Transform obj, Transform target, float duration )
		{
			if( !obj )
				obj = transform;
			obj.Align( target, duration );
		}
		/// <summary>
		/// If no /obj/ this object will be used. Animates the /obj/ until it matches the specified's position and rotation. 
		/// This will yield until the alignment is completed.
		/// </summary>
		public IEnumerator AlignWithAndWait( Transform obj, Transform target, float duration )
		{
            if( !obj )
                obj = transform;
            yield return obj.AlignCo( target, duration ).Start();
        }
		public void SetTransform( SearchableGameObject obj, Transform target )
		{
			if( !obj.m_gameObject )
			{
                Utilities.LogWarning ("No /obj/ has been referenced", gameObject );
				return;
			}
			if( !target )
			{
                Utilities.LogWarning ("No /target/ has been referenced", gameObject );
                return;
            }
			obj.m_transform.CopyTransformFrom( target );
		}
		#endregion

		#region MISC
		public Vector3 WorldToCanvasPosition(Vector3 pos, Camera cam, RectTransform container)
		{
			Vector2 viewPortPos = cam.WorldToViewportPoint (pos);
			Vector2 canvasPos = new Vector2(
				((viewPortPos.x*container.sizeDelta.x)-(container.sizeDelta.x*0.5f)),
				((viewPortPos.y*container.sizeDelta.y)-(container.sizeDelta.y*0.5f)));
			return (Vector3)canvasPos;
		}

		public void QuitApp()
		{
			Application.Quit();
		}		
		/// <summary>
		/// Gets the components in children.
		/// </summary>
		/// <param name="includeInactive">If set to <c>true</c> include inactive children.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <typeparam name="TExclude">If the child has a component of this type, it will be excluded.</typeparam>
		public T[] GetComponentsInChildren<T, TExclude>( bool includeInactive ) where T : Component where TExclude : Component
		{
			return gameObject.GetComponentsInChildren<T, TExclude>( includeInactive );
		}
		public void RemoveAllListeners( Button button )
		{
			button.RemoveAllListeners();
		}
		public void RemoveAllListenersInChildren( GameObject btsParent )
		{
			btsParent.RemoveAllListenersInChildren();
		}
		public void DebugLog( string msg )
		{
            Utilities.Log ( msg );
		}
		public void DebugLogW( string msg )
		{
            Utilities.LogWarning ( msg );
		}
		public void DebugLogE( string msg )
		{
            Utilities.LogError ( msg );
		}
        public void HandheldVibrate()
        {
            #if !UNITY_STANDALONE
            Handheld.Vibrate();
            #endif
        }
		public void OpenURL( string url )
		{
			Application.OpenURL( url );
		}
		public void OpenAppURL()
		{
			#if UNITY_ANDROID
			Application.OpenURL( "market://details?id="+ Application.identifier );
			#elif UNITY_IOS
			Application.OpenURL("itms-apps://itunes.apple.com/app/"+ Application.identifier );
			#endif
		}
		public void InvokeButtonOnClick( Button bt )
		{
			if( !bt )
				return;
			bt.onClick.Invoke();
		}
		/// <summary>
		/// This yields until the timeout has passed without detecting the specified input button.
		/// </summary>
		/// <param name="buttonName"> The button name as specified in the ProjectSettings/Input </param>
		public IEnumerator IfNoButtonThenContinue( float timeout = 10f, string buttonName = "Fire1" )
		{
			float counter = 0f;
			while( counter < timeout )
			{
				if( Input.GetButton( buttonName ) )//detected?
				{
					counter = 0f;//reset
				}
				else counter += Time.deltaTime;
				yield return null;
			}
		}
		/// <summary>
		/// Fade the specified canvasGroup. If no CanvasGroup is referenced the one in this object or any of its parents or children will be used, if this 
		/// object doesn't have one, then one will be added.
		/// </summary>
		public void Fade( CanvasGroup canvasGroup, float targetAlpha, float duration )
		{			
			FadeAndWait( canvasGroup, targetAlpha, duration ).Start();
		}
		/// <summary>
		/// Fade the specified canvasGroup. If no CanvasGroup is referenced the one in this object or any of its parents or children will be used, if this 
		/// object doesn't have one, then one will be added.
		/// </summary>
		public IEnumerator FadeAndWait( CanvasGroup canvasGroup, float targetAlpha, float duration )
		{
			if( !canvasGroup )
			{
				canvasGroup = gameObject.GetCompInParentOrChildren<CanvasGroup>();
				if( !canvasGroup )
				{
					canvasGroup = gameObject.AddComponent<CanvasGroup>();
				}
			}
			yield return canvasGroup.AlphaTo( targetAlpha, duration ).Start();
		}
        /// <summary>
        /// Fade the specified canvasGroups.
        /// </summary>
        public IEnumerator FadeAndWait( CanvasGroup[] canvasGroups, float targetAlpha, float duration )
        {
            if( canvasGroups == null )
                yield break;
            for( int i=0; i<canvasGroups.Length; i++ )
            {
                Fade( canvasGroups[ i ], targetAlpha, duration );
            }
            yield return new WaitForSeconds( duration );
        }
        public IEnumerator WaitForClick( Collider collider )
        {
            yield return StartCoroutine( WaitForClickOnLayers( collider, ~0 ) );
        }
        public IEnumerator WaitForClickOnLayers( Collider collider, LayerMask validLayers )
        {
            RaycastHit hit;
            Ray ray = default( Ray );
            if( !Camera.main )
            {
                Debug.LogWarning( "There is no main camera in the scene..", gameObject );
                yield break;
            }
            while( true )
            {
                yield return null;
                #if UNITY_EDITOR
                ray = Camera.main.ScreenPointToRay( Input.mousePosition );
                Debug.DrawRay( ray.origin, ray.direction, Color.red );
                #endif
                if( !Input.GetMouseButtonDown( 0 ) )
                {
                    continue;
                }

                ray = Camera.main.ScreenPointToRay( Input.mousePosition );
                if( Physics.Raycast( ray, out hit, float.MaxValue, validLayers.value ) )
                {
                    #if UNITY_EDITOR
                    Debug.Log( hit.collider.name );
                    #endif
                    if( hit.collider == collider )
                    {
                        break;
                    }
                }
            }
        }
        public void SetFrameRate( int rate )
        {
            Application.targetFrameRate = rate;
        }

		public Transform FindChildRecursive(Transform parent, string childName)
		{
			return parent.FindChildRecursive (childName);
		}
	#endregion

		#region DESTROY
		public void Destroy( float delay )
		{
			Destroy( this, delay );
		}
		/// <summary>
		/// If delay is below cero (0) it will be destroyed immediately.
		/// </summary>
		/// <param name="delay">Delay.</param>
		public void DestroyObj( float delay )
		{
			if( delay < 0f )
				DestroyImmediate( gameObject );
			else Destroy( gameObject, delay );
		}
		public void DestroyParent( float delay )
		{
			if( !transform.parent )
				return;
			Destroy ( gameObject.GetParent(), delay );
		}
		public void DestroyAllChildren( float delay )
		{
			gameObject.DestroyChildren( delay, null, false );
		}
		public void Destroy( Behaviour comp )
		{
			if( !comp )
				return;
			comp.Destroy();
		}
		public void Destroy( GameObject obj )
		{
			if( !obj )
				return;
			obj.Destroy();
		}
		#endregion

		#region AUDIO
		protected Queue<AudioClip> clipsQueue;
		private bool clipsQueueIsPlaying;
		/// <summary>
		/// If the specified clip is not null, it plays it at this object's position.
		/// </summary>
		public void PlayClip( AudioClip clip )
		{
			if( !clip )
				return;
			AudioSource.PlayClipAtPoint( clip, transform.position );
		}
		public void PlaySfxManagerSource( string source )
		{
			SfxManager.Play( source );
		}
		public IEnumerator PlaySfxManagerSourceCo( string source )
		{
			SfxManager.Play( source );
			yield return new WaitForSeconds (SfxManager.GetSource (source).clip.length);
		}
		public void PlayOnSfxManagerCurrentSource( AudioClip clip )
		{
			SfxManager.Instance.PlayCurrentSource( clip );
		}
		public void SetSfxManagerCurrentSourcePitch( float pitch )
		{
			SfxManager.GetCurrentSource().pitch = pitch.Clamp( -3, 3 );
		}
		/// <summary>
		/// If the specified clip is not null, it is added to the playing clips queue. Each clip is played one after the
		/// other at this object's position.
		/// </summary>
		public void PlayClipQueded( AudioClip clip )
		{
			if( !clip )
				return;
			if( clipsQueue == null )
				clipsQueue = new Queue<AudioClip>();
			clipsQueue.Enqueue( clip );
			PlayQueue().Start();
		}
		/// <summary>
		/// Plays the /clipsQueue/ if it is not playing already.
		/// </summary>
		/// <returns>The queue.</returns>
		public IEnumerator PlayQueue()
		{
			if( clipsQueueIsPlaying || clipsQueue == null )
				yield break;
			clipsQueueIsPlaying = true;
			AudioSource source = new GameObject( name + "_ClipsQueue" ).AddGetComponent<AudioSource>();
			source.transform.position = transform.position;

			while( clipsQueue != null )
			{
				AudioClip clip = clipsQueue.Dequeue();
				if( !clip )
					continue;
				source.Play( clip );
				yield return new WaitForSeconds( clip.length / source.pitch );
				if( clipsQueue == null || clipsQueue.Count == 0 )
					break;
			}
			Destroy ( source.gameObject );
			clipsQueue = null;
			clipsQueueIsPlaying = false;
		}
		public void SetNotInteractableForClipsQueueLength( CanvasGroup group )
		{
			if( clipsQueue == null || clipsQueue.Count == 0 )
				return;
			group.SetInteractableFor( false, clipsQueue.ToArray().GetClipsTotalLength() );
		}
		public void PauseAudioListener( bool pause )
		{
			AudioListener.pause = pause;
		}
		/// <summary>
		/// Plays the specified clips and wait for them to end playing.
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		public IEnumerator PlayDelayedClips( DelayedAudioClip[] delayedClips )
		{
			yield return delayedClips.PlayAll( gameObject ).Start();
		}
		/// <summary>
		/// Plays the specified clips and wait for them to end playing.
		/// </summary>
		/// <param name="delayedClips">Delayed clips.</param>
		public void PlayDelayedAudioClips( DelayedAudioClip[] delayedClips )
		{
			delayedClips.PlayAll( gameObject ).Start();
		}
		#endregion


		/// <summary>
		/// Call the specified function/method and send the specified params/args all in a single string that will be split. NOTE: 
		/// args can only have bool, float, int or string values.
		/// </summary>
		/// <param name="comp">The component in which the method/function resides.</param>
		/// <param name="args">Arguments. Supported types are bool, float, int, and string.</param>
		/// <param name="methodSplitter">Method splitter.</param>
		/// <param name="argsSplitter">Arguments splitter.</param>
		public object Call( Object comp, string args = "methodName#arg0:arg1", char methodSplitter = '#', char argsSplitter = ':' )
		{
			return comp.Call( args, methodSplitter, argsSplitter );
		}

		public IEnumerator DoDelayedAction( float delay, System.Action action, bool realtimeDelay = true )
		{
            if( realtimeDelay )
				Utilities.WaitForRealSeconds( delay );
			else if( delay > 0 )
				yield return new WaitForSeconds( delay );
			action();
        }
		/// <summary>
        /// Stopping particle systems from node canvas trees doesnt work on iOS, this is a workaround
        /// </summary>
        /// <param name="pSystem">P system.</param>
		public void StopParticleSystem( ParticleSystem pSystem )
        {
			pSystem.Stop ();
		}
			
		public void SetCanvasRenderMode( Canvas canvas , RenderMode mode){
			canvas.renderMode = mode;
		}

	}
}
