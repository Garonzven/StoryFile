//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using UnityEngine.Events;
using System.Collections.Generic;


namespace DDK.Base.Events 
{
	/// <summary>
	/// Adds multiple Animator functions that can be called through events. You can use this to, for example, reference
	/// an Animator component and reference this inside a UnityEvent.
	/// </summary>
	/// <seealso cref="UnityEvent"/>
	public class AnimatorActions : MonoBehaviourExt 
    {
		[System.Serializable]
		public class EventsParams
		{
			[DisplayNameAttribute( "Current State" )]
			public string state = "default";
			[DisplayNameAttribute( "Current Layer" )]
			public int layer;
			[Tooltip("The param that will be used when calling any function related to a param that doesn't ask for" +
				" its name, only for its value")]
			public string currentParamName;
		}


		[HelpBoxAttribute]
		public string msg = "If disabled, events won't be invoked";
		[SerializeField]
		protected SearchableAnimator _controller = new SearchableAnimator();
		[SerializeField]
		protected SearchableAnimator[] _controllers = new SearchableAnimator[0];
		[Tooltip("This will be used on some string params to split the arguments")]
		public char argsSplitter = ':';
		public EventsParams eventsParams = new EventsParams();


        /// <summary>
        /// This ensures that coroutines that animate animator parameters don't get called before that parameter's 
        /// previous animation ends. That would cause multiple coroutines to run and never end, hence causing a lot of 
        /// garbage that can never be collected
        /// </summary>
        protected Dictionary<string, float> _animatedParamsHandler;

        /// <summary>
        /// Tells which AnimateFloatToCo() coroutine is running (per parameter)
        /// </summary>
        private Dictionary<string, bool> _animatingFloatTo;//paraName, animating
        private Dictionary<string, float> _target; 
        private Dictionary<string, float> _duration;
        /// <summary>
        /// Tells if the running coroutine must restart the animation.
        /// </summary>
        private Dictionary<string, bool> _reanimate;


        void Start() //IF REMOVED, THE COMPONENT CAN'T BE DISABLED
        {

		}


		#region PROPERTIES
		public string State 
        {
			get{ return eventsParams.state; }
			set{ eventsParams.state = value; }
		}
		public int Layer 
        {
			get{ return eventsParams.layer; }
			set{ eventsParams.layer = value; }
		}
		public string CurrentParamName 
        {
			get{ return eventsParams.currentParamName; }
			set{ eventsParams.currentParamName = value; }
		}
		public Animator Controller 
        {
			get
			{
                if( this != null && !_controller.m_object )
				{
					_controller.m_object = GetComponentInChildren<Animator>();
					if( !_controller.m_object )
					{
						Debug.LogWarning ("No controller has been specified..");
						return null;
					}
				}
				return _controller.m_object;
			}
			set
			{
				if( !value )
				{
					Debug.LogWarning ("The animator that you want to set is null");
				}
				else _controller.m_object = value;
			}
		}
		public Animator[] Controllers 
        {
			get
			{
				if( _controllers.Length == 0 )
				{
					Debug.LogWarning ("No controllers have been specified..");
					return null;
				}
				return _controllers.GetComponents();
			}
		}
		/// <summary>
		///The current controller's tuntime animator controller.
		/// </summary>
		public RuntimeAnimatorController RuntimeController
        {
			get
            {
				if( !_overrider )
					_overrider = new AnimatorOverrideController();
				return Controller.runtimeAnimatorController;
			}
			set{ Controller.runtimeAnimatorController = value; }
		}
		protected AnimatorOverrideController _overrider;
		/// <summary>
		///The current animator overrider controller. This can be used to replace animation clips and then assign it as
		/// the RuntimeController. Ex: RuntimeController = ControllerOverrider
		/// </summary>
		public AnimatorOverrideController ControllerOverrider 
        {
			get
            {
				if( !_overrider )
					_overrider = new AnimatorOverrideController();
				_overrider.runtimeAnimatorController = RuntimeController;
				return _overrider;
			}
		}
		#endregion


		#region ANIMATE
        /// <summary>
        /// Animates the Animator's specified float param's value.
        /// </summary>
        /// <param name="paramName">Animator's parameter name.</param>
        private IEnumerator<float> _AnimateFloatToCo( string paramName )
        {
            if( !Controller )
            {
                _animatingFloatTo[ paramName ] = false;
                yield break;
            }         
            float time = Time.deltaTime;
            float from = Controller.GetFloat( paramName );
            float target = _target[ paramName ];
            float duration = _duration[ paramName ];
            while( true )
            {
                if( time > duration )
                {
                    yield return 0f;
                    if( _reanimate[ paramName ] )//animate to new target?
                    {
                        time = Time.deltaTime;//reset to start new animation
                        from = Controller.GetFloat( paramName );
                        target = _target[ paramName ];
                        duration = _duration[ paramName ];
                        _reanimate[ paramName ] = false;
                    }
                    continue;
                }
                Controller.SetFloatValue( paramName, Mathf.Lerp( from, target, time / duration ) );
                yield return 0f;
                time += Time.deltaTime;
                if( !Controller )
                {
                    _animatingFloatTo[ paramName ] = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Animates the Animator's specified float param's value to the specified /target/ in the specified /duration/.
        /// </summary>
		public void AnimateFloatTo( string paramName, float target, float duration )
		{
			if( !enabled )
				return;
            if( _animatedParamsHandler == null )
            {
                _animatedParamsHandler = new Dictionary<string, float>();
            }
            if( _animatedParamsHandler.ContainsKey( paramName ) )
            {
                if( _animatedParamsHandler[ paramName ] > Time.time )
                    return;
            }
            _animatedParamsHandler[ paramName ] = Time.time + duration;
            if( _animatingFloatTo == null )
            {
                _animatingFloatTo = new Dictionary<string, bool>();
                _target = new Dictionary<string, float>();
                _duration = new Dictionary<string, float>();
                _reanimate = new Dictionary<string, bool>();
            }
            _target[ paramName ] = target;
            _duration[ paramName ] = duration;
            _reanimate[ paramName ] = true;//reset so coroutine can start the new animation
            if( !_animatingFloatTo.ContainsKey( paramName ) )//start coroutine (for specific parameter) if it hasn't been started
            {
                _animatingFloatTo.Add( paramName, true );//mark coroutine as started
                _AnimateFloatToCo( paramName ).Run();
            }
		}
        /// <summary>
        /// Animates the Animator's specified float param's value to the specified target in the specified duration.
        /// </summary>
        /// <param name="args">Arguments split by /argsSplitter/. In order: parameter name, target float value, duration</param>
		public void AnimateFloatTo( string args = "paramName:1f:1f" )
		{
			if( !enabled )
				return;
			Controller.AnimateFloatTo( args, argsSplitter );
		}
        /// <summary>
        /// Animates the Animator's specified /layerIndex/'s weight to the specified /target/ in the specified /duration/.
        /// </summary>
		public void AnimateLayerWeightTo( int layerIndex, float target, float duration )
		{
			if( !enabled )
				return;
			Controller.AnimateLayerWeightTo( layerIndex, target, duration );
		}
        /// <summary>
        /// Animates the Animator's specified layer's weight to the specified target in the specified duration.
        /// </summary>
        /// <param name="args">Arguments split by /argsSplitter/. In order: layer index, target weight value, duration</param>
		public void AnimateLayerWeightTo( string args = "layerIndex:1f:1f" )
		{
			if( !enabled )
				return;
			Controller.AnimateLayerWeightTo( args, argsSplitter );
		}
		#endregion


		#region IS
		/// <summary>
		/// Returns true if the current animator state matches the specified.
		/// </summary>
		public bool IsStateActive()
		{
			return Controller.GetCurrentAnimatorStateInfo( Layer ).IsName( State );
		}
		/// <summary>
		/// Returns true if the current animator state matches the specified.
		/// </summary>
		public bool IsStateActive( string stateName )
		{
			return Controller.GetCurrentAnimatorStateInfo( Layer ).IsName( stateName );
		}
		/// <summary>
		/// Returns true if the current animator state matches the specified.
		/// </summary>
		public bool IsStateActive( string stateName, int layerIndex )
		{
			return Controller.GetCurrentAnimatorStateInfo( layerIndex ).IsName( stateName );
		}
		/// <summary>
		/// Returns true if the current animator state matches the specified.
		/// </summary>
		public bool IsStateActive( Animator animator, string stateName, int layerIndex = 0 )
		{
			return animator.GetCurrentAnimatorStateInfo( layerIndex ).IsName( stateName );
		}
		#endregion


		#region GET
        /// <summary>
        /// Gets the Animator's specified bool parameter's value.
        /// </summary>
		public bool GetBool( string paramName )
		{
			return Controller.GetBool( paramName );
		}
        /// <summary>
        /// Gets the Animator's specified int parameter's value.
        /// </summary>
		public int GetInt( string paramName )
		{
			return Controller.GetInteger( paramName );
		}
        /// <summary>
        /// Gets the Animator's specified float parameter's value.
        /// </summary>
		public float GetFloat( string paramName )
		{
			return Controller.GetFloat( paramName );
		}
        /// <summary>
        /// Gets the specified clip's length, which is cero (0) if clip is null.
        /// </summary>
		public float GetClipLength( AnimationClip clip )
		{
			if( !clip )
			{
				Debug.LogWarning ( "The specified clip is null, returning 0", gameObject );
				return 0;
			}
			return clip.length;
		}
        /// <summary>
        /// Gets the specified clips total length, which is cero (0) if clips array is null or empty.
        /// </summary>
		public float GetClipsLength( AudioClip[] clips )
		{
			if( clips == null )
			{
				Debug.LogWarning ( "The specified clips array is null, returning 0", gameObject );
				return 0;
			}
			if( clips.Length == 0 )
			{
				Debug.LogWarning ( "The specified clips array is empty, returning 0", gameObject );
				return 0;
			}
			return clips.GetClipsTotalLength();
		}
		#endregion


		#region SET
        /// <summary>
        /// Sets the Animator's layer weight.
        /// </summary>
        /// <param name="layerIndex">Layer index.</param>
        /// <param name="value">Weight.</param>
		public void SetLayerWeight( int layerIndex, float value )
		{
			if( !enabled )
				return;
			value = Mathf.Clamp01( value );
			Controller.SetLayerWeight( layerIndex, value );
		}
        /// <summary>
        /// Triggers the Animator's specified parameter
        /// </summary>
        /// <param name="paramName">Parameter name.</param>
		public void SetTrigger( string paramName )
		{
			if( !enabled )
				return;
			Controller.SetTriggerValue( paramName );
		}
		/// <summary>
		/// Yields until the triggered animation ends. MUST BE TESTED.
		/// </summary>
		public IEnumerator SetTriggerCo( string paramName, string layerName )
		{
			yield return StartCoroutine( SetTriggerCo( paramName, Controller.GetLayerIndex( layerName ) ) );
		}
		/// <summary>
		/// Yields until the triggered animation ends. MUST BE TESTED.
		/// </summary>
		public IEnumerator SetTriggerCo( string paramName, int layerIndex = 0 )
		{
			if( !enabled )
                yield break;
			Controller.SetTriggerValue( paramName );
			while( Controller.GetAnimatorTransitionInfo( layerIndex ).normalizedTime < 1f )
				yield return null;
			yield return new WaitForSeconds( Controller.GetCurrentAnimatorStateInfo( layerIndex ).length );
			while( Controller.GetAnimatorTransitionInfo( layerIndex ).normalizedTime < 1f )
				yield return null;
		}
        /// <summary>
        /// Sets the Animator's bool parameter's value.
        /// </summary>
        /// <param name="args">Arguments split by /argsSplitter/ as follow: parameter name, T (true) or F (false).</param>
		public void SetBool( string args = "paramName:T" )
		{
			if( !enabled )
				return;
			Controller.SetBool( args, argsSplitter );
		}
        /// <summary>
        /// Sets the Animator's int parameter's value.
        /// </summary>
        /// <param name="args">Arguments split by /argsSplitter/ as follow: parameter name, int value.</param>
		public void SetInt( string args = "paramName:1" )
		{
			if( !enabled )
				return;
			Controller.SetInt( args, argsSplitter );
		}
        /// <summary>
        /// Sets the Animator's float parameter's value.
        /// </summary>
        /// <param name="args">Arguments split by /argsSplitter/ as follow: parameter name, float value.</param>
		public void SetFloat( string args = "paramName:1f" )
		{
			if( !enabled )
				return;
			Controller.SetFloat( args, argsSplitter );
		}
        /// <summary>
        /// Sets the Animator's specified bool parameter's value.
        /// </summary>
		public void SetBool( bool value )
		{
			SetBool( CurrentParamName, value );
        }
        /// <summary>
        /// Sets the Animator's specified int parameter's value.
        /// </summary>
		public void SetInt( int value )
		{
			SetInt( CurrentParamName, value );
		}
        /// <summary>
        /// Sets the Animator's specified float parameter's value.
        /// </summary>
		public void SetFloat( float value )
		{
			SetFloat( CurrentParamName, value );
		}
        /// <summary>
        /// Sets the Animator's specified bool parameter's value.
        /// </summary>
		public void SetBool( string paramName, bool value )
		{
			SetBool( paramName, value, 0f );
		}
        /// <summary>
        /// Sets the specified bool parameter's value for the specified /duration/.
        /// </summary>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="value">The value.</param>
        /// <param name="duration">If equal or below cero (0) it won't be taken into account. Hence, the animator's
        /// param will stay /value/.</param>
		public void SetBool( string paramName, bool value, float duration )
		{
			if( duration <= 0f )
				Controller.SetBoolValue( paramName, value );
			else SetBoolCo( paramName, value, duration ).Start();
		}
        /// <summary>
        /// Sets the specified bool parameter's value for the specified /duration/.
        /// </summary>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="value">The value.</param>
        /// <param name="duration">If equal or below cero (0) it won't be taken into account. Hence, the animator's
        /// param will stay /value/.</param>
		public IEnumerator SetBoolCo( string paramName, bool value, float duration )
		{
			if( !enabled )
				yield break;
            if( duration <= 0f )
            {
                Controller.SetBoolValue( paramName, value );
                yield break;
            }
			Controller.SetBoolValue( paramName, value );
			yield return new WaitForSeconds( duration );
			Controller.SetBoolValue( paramName, !value );
		}
        /// <summary>
        /// Sets the specified int parameter's value.
        /// </summary>
		public void SetInt( string paramName, int value )
		{
			if( !enabled )
				return;
			Controller.SetIntValue( paramName, value );
		}
        /// <summary>
        /// Sets the specified float parameter's value.
        /// </summary>
		public void SetFloat( string paramName, float value )
		{
            if( !enabled )
                return;
			Controller.SetFloatValue( paramName, value );
        }
        /// <summary>
        /// Sets the Animator's speed.
        /// </summary>
        public void SetSpeed( float speed )
		{
			if( !Controller || !enabled )
				return;
			Controller.speed = speed;
		}
        /// <summary>
        /// Sets the event parameters state value.
        /// </summary>
        /// <param name="state">Name of the current state.</param>
		public void SetEventsParamState( string state )
		{
			if( !enabled )
				return;
			State = state;
		}
        /// <summary>
        /// Sets the event parameters layer index.
        /// </summary>
        /// <param name="layer">Layer index.</param>
		public void SetEventsParamLayer( int layer )
		{
			if( !enabled )
				return;
			Layer = layer;
		}
        /// <summary>
        /// Sets the event parameters current parameter name.
        /// </summary>
        /// <param name="paramName">Name of the current parameter.</param>
		public void SetEventsParamCurrentParamName( string paramName )
		{
			if( !enabled )
				return;
			CurrentParamName = paramName;
		}
		/// <summary>
		/// This will override the current referenced animator Controller's runtime controller with the specified /overrider/
		/// </summary>
		public void SetAnimatorControllerOverrider( RuntimeAnimatorController overrider )
		{
			if( !enabled )
				return;
			if( !Controller )
			{
				Debug.LogWarning( "No Controller is being referenced", gameObject );
				return;
			}
			if( !overrider )
			{
				Debug.LogWarning( "No overrider has been specified", gameObject );
				return;
			}
			RuntimeController = overrider;
		}
		/// <summary>
		/// Replaces the specified clip in the curent animator's animator override controller, by the specified /newClip/.
		/// </summary>
		protected void _ReplaceClip( string clipName, AnimationClip newClip )
		{
			if( !enabled )
				return;
			if( !Controller )
			{
				Debug.LogWarning( "No Controller is being referenced", gameObject );
				return;
			}
			if( !newClip )
			{
				Debug.LogWarning( "No newClip has been specified", gameObject );
				return;
			}
			if( !ControllerOverrider )
			{
				Debug.LogWarning( "No ControllerOverrider has been specified in the referenced animator controller", gameObject );
				return;
			}
			if( !ControllerOverrider.animationClips.GetNamed( clipName ) )
			{
				Debug.LogWarning( "The specified /clipName/ doesn't match any clip inside the referenced animator's controller overrider clips", gameObject );
				return;
			}
			ControllerOverrider[ clipName ] = newClip;
			RuntimeController = ControllerOverrider;
		}
		/// <summary>
		/// Replaces the specified /clip/ in the curent animator's animator override controller, by the specified /newClip/.
		/// </summary>
		public void ReplaceClip( AnimationClip clip, AnimationClip newClip )
		{
			if( !clip || !newClip )
			{
				Debug.LogWarning( "One of the clips hasn't been specified", gameObject );
                return;
            }
			if( Controller.IsStateActive( clip.name ) )
			{
				Debug.LogWarning ("You can't replace the clip of the currently active animator's state");
				return;
			}
			_ReplaceClip( clip.name, newClip );
		}
		/// <summary>
		/// Replaces the specified /clip/ in the curent animator's animator override controller, by the specified /newClip/.
		/// </summary>
		public void ReplaceClipAndPlay( AnimationClip clip, AnimationClip newClip, string stateName )
		{
			if( !clip || !newClip )
			{
				Debug.LogWarning( "One of the clips hasn't been specified", gameObject );
				return;
			}
			if( Controller.IsStateActive( stateName ) )
			{
				Debug.LogWarning ("You can't replace the clip of the currently active animator's state");
				return;
			}
			_ReplaceClip( clip.name, newClip );
			Controller.Play( stateName );
		}
		/// <summary>
		/// Replaces the specified /clip/ in the curent animator's animator override controller, by the specified /newClip/. 
		/// And sets the specified animator's bool parameter value.
		/// </summary>
		public void ReplaceClipAndSetBool( Pair<AnimationClip, AnimationClip> newAndOldClips, string animatorParam, bool value )
		{
			if( !newAndOldClips.first || !newAndOldClips.second )
			{
				Debug.LogWarning( "One of the clips hasn't been specified", gameObject );
                return;
            }
			if( Controller.IsStateActive( newAndOldClips.first.name ) )
			{
				Debug.LogWarning ("You can't replace the clip of the currently active animator's state");
				return;
			}
            _ReplaceClip( newAndOldClips.first.name, newAndOldClips.second );
			Controller.SetBoolValue( animatorParam, value );
        }
		#endregion


		#region SETS
        /// <summary>
        /// Sets the specified layers weight, in all the specified Animator Controllers.
        /// </summary>
        /// <param name="layerIndex">Layer index.</param>
        /// <param name="value">Weight.</param>
		public void SetLayersWeight( int layerIndex, float value )
		{
			if( !enabled )
				return;
			Controllers.SetLayersWeight( layerIndex, value );
		}
        /// <summary>
        /// Trigger the specified parameter, in all the specified Animator Controllers.
        /// </summary>
        /// <param name="paramName">Parameter name.</param>
		public void SetTriggers( string paramName )
		{
			if( !enabled )
				return;
			Controllers.SetTrigger( paramName );
		}
        /// <summary>
        /// Sets the specified parameter's bool value, in all the specified Animator Controllers.
        /// </summary>
        /// <param name="args">Arguments split by /argsSplitter/ as follow: parameter name, bool value.</param>
		public void SetBools( string args = "paramName:T" )
		{
			if( !enabled )
				return;
			Controllers.SetBool( args, argsSplitter );
		}
        /// <summary>
        /// Sets the specified parameter's bool value, in all the specified Animator Controllers.
        /// </summary>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="value">The value to set.</param>
		public void SetBools( string paramName, bool value )
		{
			if( !enabled )
				return;
			Controllers.SetBool( paramName, value );
		}
        /// <summary>
        /// Sets the specified parameter's int value, in all the specified Animator Controllers.
        /// </summary>
        /// <param name="args">Arguments split by /argsSplitter/ as follow: parameter name, int value.</param>
		public void SetInts( string args = "paramName:1" )
		{
			if( !enabled )
				return;
			Controllers.SetInt( args, argsSplitter );
		}
        /// <summary>
        /// Sets the specified parameter's int value, in all the specified Animator Controllers.
        /// </summary>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="value">The value to set.</param>
		public void SetInts( string paramName, int value )
		{
			if( !enabled )
				return;
			Controllers.SetInt( paramName, value );
		}
        /// <summary>
        /// Sets the specified parameter's float value, in all the specified Animator Controllers.
        /// </summary>
        /// <param name="args">Arguments split by /argsSplitter/ as follow: parameter name, float value.</param>
		public void SetFloats( string args = "paramName:1f" )
		{
			if( !enabled )
				return;
			Controllers.SetFloat( args, argsSplitter );
		}
        /// <summary>
        /// Sets the specified parameter's float value, in all the specified Animator Controllers.
        /// </summary>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="value">The value to set.</param>
		public void SetFloats( string paramName, float value )
		{
			if( !enabled )
				return;
			Controllers.SetFloat( paramName, value );
		}
        /// <summary>
        /// Sets the current parameter's int value, in all the specified Animator Controllers.
        /// </summary>
		public void SetInts( int value )
		{
			if( !enabled )
				return;
			Controllers.SetInt( CurrentParamName, value );
		}
        /// <summary>
        /// Sets the current parameter's float value, in all the specified Animator Controllers.
        /// </summary>
		public void SetFloats( float value )
		{
			if( !enabled )
				return;
			Controllers.SetFloat( CurrentParamName, value );
		}
        /// <summary>
        /// Sets the speed for all the specified Animator Controllers.
        /// </summary>
		public void SetSpeeds( float speed )
		{
			if( !enabled )
				return;
			Controllers.SetSpeed( speed );
		}
		#endregion


		#region MISC
        /// <summary>
        /// Cross fades the Animator Controller to the /State/ in the specified /Layer/.
        /// </summary>
        /// <param name="transitionDuration">Transition duration.</param>
		public void CrossFade( float transitionDuration )
		{
			if( !Controller || !enabled )
				return;
			Controller.CrossFade( State, transitionDuration, Layer );
		}
        /// <summary>
        /// Enables/Disables the Animator Controller.
        /// </summary>
		public void EnableAnimator( bool enable )
		{
			if( !enabled )
				return;
			Controller.enabled = enable;
		}
        /// <summary>
        /// Enables/Disables the Animator Controller after the specified /delay/.
        /// </summary>
		public void EnableAnimatorAfter( bool enable, float delay )
		{
			if( !enabled )
				return;
			Controller.SetEnabledAfter( enable, delay );
		}
        /// <summary>
        /// Enables the Animator Controller after the specified /delay/.
        /// </summary>
		public void EnableAnimatorAfter( float delay )
		{
			EnableAnimatorAfter( true, delay );
		}
        /// <summary>
        /// Disables the Animator Controller after the specified /delay/.
        /// </summary>
		public void DisableAnimatorAfter( float delay )
		{
			EnableAnimatorAfter( false, delay );
		}
		#endregion


		#region MISCS
        /// <summary>
        /// Cross fades the Animator Controllers to the /State/ in the specified /Layer/.
        /// </summary>
        /// <param name="transitionDuration">Transition duration.</param>
		public void CrossFadeAll( float transitionDuration )
		{
			if( !this || !enabled )
				return;
			Controllers.CrossFade( State, transitionDuration, Layer );
		}
        /// <summary>
        /// Enables/Disables the Animator Controllers.
        /// </summary>
		public void EnableAnimators( bool enable )
		{
			if( !this || !enabled )
				return;
			Controllers.Enable( enable );
		}
        /// <summary>
        /// Enables/Disables the Animator Controllers after the specified /delay/.
        /// </summary>
		public void EnableAnimatorsAfter( bool enable, float delay )
		{
			if( !this || !enabled )
				return;
			Controllers.SetEnabledAfter( enable, delay );
		}
        /// <summary>
        /// Enables the Animator Controllers after the specified /delay/.
        /// </summary>
		public void EnableAnimatorsAfter( float delay )
		{
			EnableAnimatorsAfter( true, delay );
		}
        /// <summary>
        /// Disables the Animator Controllers after the specified /delay/.
        /// </summary>
		public void DisableAnimatorsAfter( float delay )
		{
			EnableAnimatorsAfter( false, delay );
		}
        /// <summary>
        /// Enables/Disables the Animator Controllers, if the current state matches /State/.
        /// </summary>
		public void EnableAnimatorsWhenCurrentStateMatches( bool enable )
		{
			if( !this || !enabled )
				return;
			InvokeWhenCurrentStateMatchesCo( () => Controllers.Enable( enable ) ).Start();
		}
        /// <summary>
        /// Enables/Disables the Animator Controllers after the specified /delay/, if the current state matches /State/.
        /// </summary>
		public void EnableAnimatorsWhenCurrentStateMatchesAfter( bool enable, float delay )
		{
			if( !this || !enabled )
				return;
			InvokeWhenCurrentStateMatchesCo( () => Controllers.SetEnabledAfter( enable, delay ) ).Start();
		}
        /// <summary>
        /// Enables the Animator Controllers after the specified /delay/, if the current state matches /State/.
        /// </summary>
		public void EnableAnimatorsWhenCurrentStateMatchesAfter( float delay )
		{
			EnableAnimatorsWhenCurrentStateMatchesAfter( true, delay );
		}
        /// <summary>
        /// Disables the Animator Controllers after the specified /delay/, if the current state matches /State/.
        /// </summary>
		public void DisableAnimatorsWhenCurrentStateMatchesAfter( float delay )
		{
			EnableAnimatorsWhenCurrentStateMatchesAfter( false, delay );
		}
		#endregion


		#region EVENTS
        /// <summary>
        /// Invokes the specified /actions/ if current state matches /State/.
        /// </summary>
		public void InvokeIfCurrentStateMatches( SingleComposedEvent actions )
		{
			if( !Controller || !enabled )
				return;
			AnimatorStateInfo stateInfo = Controller.GetCurrentAnimatorStateInfo( Layer );
			if( stateInfo.IsName( State ) && actions )
			{
				actions.Invoke();
			}
		}
        /// <summary>
        /// Invokes the specified /actions/ if current state doesn't match /State/.
        /// </summary>
		public void InvokeIfCurrentStateDoesntMatch( SingleComposedEvent actions )
		{
			if( !Controller || !enabled )
				return;
			AnimatorStateInfo stateInfo = Controller.GetCurrentAnimatorStateInfo( Layer );
			if( !stateInfo.IsName( State ) && actions )
			{
				actions.Invoke();
			}
		}
		/// <summary>
		///Waits for the current /state/ to match, and invokes the specified actions.
		/// </summary>
		public void InvokeWhenCurrentStateMatches( SingleComposedEvent actions )
		{
			InvokeWhenCurrentStateMatchesCo( () => actions.Invoke() ).Start();
		}
		/// <summary>
		///Waits for the current /state/ to match, and invokes the specified actions.
		/// </summary>
		public IEnumerator InvokeWhenCurrentStateMatchesCo( UnityAction action )
		{
			if( !Controller || !enabled )
				yield break;
			if( action == null )
				yield break;
			while( true )
			{
				if( this == null )
					yield break;
				AnimatorStateInfo stateInfo = Controller.GetCurrentAnimatorStateInfo( Layer );
				if( stateInfo.IsName( State ) )
				{
					action.Invoke();
					break;
				}
				yield return null;
			}
		}
		/// <summary>
		///Waits for the current state to end, and invokes the specified actions.
		/// </summary>
		public void InvokeOnCurrentStateEnd( SingleComposedEvent actions )
		{
			InvokeOnCurrentStateEndCo( () => actions.Invoke() ).Start();
		}
		/// <summary>
		///Waits for the current state to end, and invokes the specified actions.
		/// </summary>
		public IEnumerator InvokeOnCurrentStateEndCo( UnityAction action )
		{
			if( !Controller || !enabled )
				yield break;
			if( action == null )
				yield break;
			AnimatorStateInfo stateInfo = Controller.GetCurrentAnimatorStateInfo( Layer );
			if( stateInfo.loop )
			{
				float previoustime = stateInfo.normalizedTime;
				while( previoustime - ( (int) stateInfo.normalizedTime ) < 0f )
				{
					previoustime = stateInfo.normalizedTime;
					yield return null;
					stateInfo = Controller.GetCurrentAnimatorStateInfo( Layer );
				}
			}
			else while( stateInfo.normalizedTime < 1 )
			{
				yield return null;
				if( this == null )
					yield break;
				stateInfo = Controller.GetCurrentAnimatorStateInfo( Layer );
			}
			action.Invoke();
		}
        /// <summary>
        /// Invokes the specified /actions/ if /CurrentParamName/ bool parameter is true.
        /// </summary>
		public void InvokeIfBoolIsTrue( SingleComposedEvent actions )
		{
			if( !Controller || !enabled )
				return;
			if( Controller.GetBool( CurrentParamName ) && actions )
			{
				actions.Invoke();
			}
		}
        /// <summary>
        /// Invokes the specified /actions/ if /CurrentParamName/ bool parameter is false.
        /// </summary>
		public void InvokeIfBoolIsFalse( SingleComposedEvent actions )
		{
			if( !Controller || !enabled )
				return;
			if( !Controller.GetBool( CurrentParamName ) && actions )
			{
				actions.Invoke();
			}
		}
		#endregion
    }
}
