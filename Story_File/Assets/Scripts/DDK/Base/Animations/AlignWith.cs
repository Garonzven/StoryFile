//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using DDK.Base.Events;
using DDK.Base.Statics;


#if USE_THREE_D
using DDK._3D;
#endif


namespace DDK.Base.Animations 
{
	/// <summary>
	/// Attach to an object to align it with another object.
	/// </summary>
	[ExecuteInEditMode]
	public class AlignWith : MonoBehaviourExt 
    {
        public SearchableGameObject objToAlign = new SearchableGameObject();
		[Tooltip("If true, Align() will be called on OnEnable() after the specified delay")]
		public bool onEnable;
        [ShowIfAttribute( "onEnable", 1 )]
		public float delay;
		[Space(5f)]
		[ShowIfAttribute( "_ShowTargetField" )]
		public Transform target;
		[Tooltip("The name of the target Transform to search if /target/ is null")]
		[ShowIfAttribute( "_IsTargetInvalid" )]
		public string targetName;
		[Tooltip("If true, the /targetName/ will be used to search by tag")]
		[ShowIfAttribute( "_IsTargetInvalid", 1 )]
		public bool isTag;
		[NotLessThan( 0f )]
		[Tooltip("If cero (0), and LookAt is called, the object will look at the target indefinitely")]
		public float animDuration = 1f;
		[Header("Events")]
        [Tooltip("This is used in some functions (with a string param) to split the arguments")]
        public char argsSplitter = ':';
		[Tooltip("If AlignPingPong() is called, this will be invoked after the object comes back")]
		public ComposedEvent onAligned = new ComposedEvent();
		[Tooltip("Invoked when the target is reached. NOTE: Try not to use this since it seems to fail on mobile devices")]
		public ComposedEvent onMoved = new ComposedEvent();


		#region VALIDATION FUNCTIONS
		protected virtual bool _IsTargetInvalid()
		{
			return target == null;
		}
		protected virtual bool _ShowTargetField()
		{
			return true;
		}
		#endregion


		public bool _IsAligning 
        {
			get{ return Flags.IsFlagged( ValidationFlag.Flags.Align ); }
			set{ Flags.SetFlagged( ValidationFlag.Flags.Align, value ); }
		}
		public bool _IsMoving 
        {
			get{ return Flags.IsFlagged( ValidationFlag.Flags.Move ); }
			set{ Flags.SetFlagged( ValidationFlag.Flags.Move, value ); }
		}
		public bool _IsLookingAt 
        {
			get{ return Flags.IsFlagged( ValidationFlag.Flags.Rotate ); }
			set{ Flags.SetFlagged( ValidationFlag.Flags.Rotate, value ); }
		}
		private ValidationFlag _flags;
		public ValidationFlag Flags
        {
			get
            {
				if( !_flags )
                    _flags = objToAlign.m_gameObject.AddGetComponent<ValidationFlag>();
				return _flags;
			}
		}
#if USE_THREE_D
		protected LookAt _lookAt;
#endif
		protected virtual Transform _Target 
        {
			get
            {
				if( !target && !string.IsNullOrEmpty( targetName ) )
					SetTarget( targetName );
				return target;
			}
			set
            {
				if( value )
					target = value;
			}
		}
		protected bool _started;
		protected const string ALIGNING = "ALIGNING";
		protected const string ALIGNING_PP = "ALIGNING_PINGPONG";
		protected const string MOVING = "MOVING";
		protected const string LOOKING_AT = "LOOKING_AT";



		/// <summary>
        /// Fixes the main objects references if necessary, also marks this instance as started and invokes OnEnable() 
        /// if some conditions are fulfilled.
        /// </summary>
		protected void Start () 
        {
			if( !Application.isPlaying )
				return;
            
            if( string.IsNullOrEmpty( objToAlign.objName ) )
                objToAlign.m_gameObject = gameObject;
            
			if( !target && !string.IsNullOrEmpty( targetName ) )
				SetTarget( targetName );
			if( !_started && onEnable )
			{
				_started = true;
				OnEnable();
			}

			_started = true;
		}
		#if UNITY_EDITOR
        /// <summary>
        /// Checks each frame if any of the main objects references were lost, if so, it tries to fix them.
        /// </summary>
		protected void Update() 
        {
			if( Application.isPlaying )
				return;

            if( !objToAlign.m_gameObject )
                objToAlign.m_gameObject = gameObject;
			
			if( !target )
			{
				SetTarget( targetName );
			}
			else targetName = target.name;
		}
		#endif
        /// <summary>
        /// If specified ( /onEnable/ = true ), invokes AlignAfter( /delay/ ).
        /// </summary>
        protected virtual void OnEnable()
		{
			if( !Application.isPlaying )
				return;
			if( _started && onEnable && _Target )
				AlignAfter ( delay );
		}


        /// <summary>
        /// Sets the align /_target/.
        /// </summary>
        /// <param name="target">The align target, it won't be assigned if null.</param>
		public void SetTarget( Transform alignTarget )
		{
			if( !alignTarget )
				return;
			_Target = alignTarget;
		}
        /// <summary>
        /// Sets the align /_target/ by searching for it with the specified name.
        /// </summary>
        /// <param name="targetName">The align target's name, it won't be assigned if name is empty or gameObject not found.</param>
		public void SetTarget( string alignTargetName )
		{
			if( string.IsNullOrEmpty( alignTargetName ) )
				return;
            _Target = isTag ? alignTargetName.FindWithTag<Transform>() : alignTargetName.Find<Transform>();
		}
        /// <summary>
        /// Sets the align animation's duration.
        /// </summary>
		public void SetAnimDuration( float duration )
		{
			animDuration = duration.Clamp( 0f, float.MaxValue );
		}


		public void AlignAfter( float alignDelay )
		{
			_Align( _Target, animDuration ).Start( alignDelay, ALIGNING );
		}
		public void Align()
		{
			_Align( _Target, animDuration ).Start( ALIGNING );
		}
		public void Align( Transform alignTarget )
		{
			_Align( alignTarget, animDuration ).Start( ALIGNING );
        }
		public void Align( float duration )
		{
			_Align( _Target, duration ).Start( ALIGNING );
        }
        /// <summary>
        /// Aligns the specified /objToAlign/ gameObject with the specified target and then back to its original alignment.
        /// </summary>
        /// <param name="alignDelay">Delay before starting the align animation.</param>
		public void AlignPingPongAfter( float alignDelay )
		{
			_AlignPingPong( _Target, animDuration ).Start( alignDelay, ALIGNING_PP );
		}
        /// <summary>
        /// Aligns the specified /objToAlign/ gameObject with the specified target and then back to its original alignment.
        /// </summary>
		public void AlignPingPong()
		{
			_AlignPingPong( _Target, animDuration ).Start( ALIGNING_PP );
		}
        /// <summary>
        /// Aligns the specified /objToAlign/ gameObject with the specified /alignTarget/ and then back to its original alignment.
        /// </summary>
		public void AlignPingPong( Transform alignTarget )
		{
			_AlignPingPong( alignTarget, animDuration ).Start( ALIGNING_PP );
		}
        /// <summary>
        /// Aligns the specified /objToAlign/ gameObject with the specified target and then back to its original alignment.
        /// </summary>
        /// <param name="duration">The duration of the animation since it starts until it gets back to its original alignment.</param>
		public void AlignPingPong( float duration )
		{
			_AlignPingPong( _Target, duration ).Start( ALIGNING_PP );
		}
		/// <summary>
		/// If the gameObject is being aligned, this will interrupt the alignment (stop it)
		/// </summary>
		public void StopAlignment()
		{
			if( !_flags )
				return;
			_flags.DestroyImmediate();
		}
		public void Move()
		{
			_Move( _Target, animDuration ).Start( MOVING );
		}
		public void Move( Transform moveTarget )
		{
			_Move( moveTarget, animDuration ).Start( MOVING );
		}
		public void Move( string moveTarget )
		{
			Transform _target = moveTarget.Find<Transform>();
			if( !_target )
			{
                Utilities.LogWarning("Move() -> No target was found with the specified name: " + moveTarget, gameObject );
				return;
			}
			_Move( _target, animDuration ).Start( MOVING );
		}
		public void Move( float duration )
		{
			_Move( _Target, duration ).Start( MOVING );
		}
		public void MoveAfter( float delay )
		{
			_Move( _Target, animDuration ).Start( delay, MOVING );
		}
		/// <summary>
		/// If the gameObject is being moved, this will interrupt the movement (stop it)
		/// </summary>
		public void StopMovement()
		{
            objToAlign.m_gameObject.SetFlagged( ValidationFlag.Flags.Move, false );
		}
		public void LookAt()
		{
			LookAt( _Target, animDuration );
		}
		/// <summary>
		/// Rotates this gameObject until it is looking at the specified /pTarget/.
		/// </summary>
		public void LookAt( Transform pTarget )
		{
			LookAt( pTarget, animDuration );
		}
		/// <summary>
		/// Rotates this gameObject until it is looking at the specified target.
		/// </summary>
		/// <param name="targetName">The target's name.</param>
		[System.Obsolete( "This is not good for performance use the LookAt( Transform ) version of this method." )]
		public void LookAt( string targetName )
		{
			Transform _target = targetName.Find<Transform>();
			if( !_target )
			{
                Utilities.LogWarning("LookAt() -> No target was found with the specified name: " + targetName, gameObject );
				return;
			}
			LookAt( _target, animDuration );
		}
		/// <summary>
		/// Rotates this gameObject until it is looking at the specified target.
		/// </summary>
		/// <param name="duration">Animation's duration.</param>
		public void LookAt( float duration )
		{
			LookAt( _Target, duration );
		}
		/// <summary>
		/// Rotates this gameObject until it is looking at the specified /pTarget/ until StopLookAt() is called.
		/// </summary>
		public void LookAtIndefinitely( Transform pTarget )
		{
			LookAt( pTarget, 0f );
		}
		/// <summary>
		/// Rotates this gameObject until it is looking at the specified target until StopLookAt() is called.
		/// </summary>
		/// <param name="targetName">The target's name.</param>
		[System.Obsolete( "This is not good for performance use the LookAtIndefinitely( Transform ) version of this method." )]
		public void LookAtIndefinitely( string targetName )
		{
			LookAt( targetName, 0f );
		}
		/// <summary>
		/// Rotates this gameObject until it is looking at the specified /pTarget/.
		/// </summary>
		/// <param name="pTarget">The target to look at.</param>
		/// <param name="duration">Animation's duration.</param>
		public void LookAt( Transform pTarget, float duration )
		{
			if( pTarget.IsFlagged( ValidationFlag.Flags.Rotate ) )
			{
                Utilities.LogWarning ("This object's Rotate flag is true, it might already be looking at a specified target," +
					"call StopLookAt() before calling LookAt() again, or if the last animation's duration was not set to" +
					"cero (0) just wait for it to end");
				return;
			}
#if USE_THREE_D
			_lookAt = transform.GetOrAddComponent<LookAt>();
			_lookAt._target = target;
			if( animDuration > 0 )
				_lookAt.Destroy( animDuration );
#else
			_LookAtCo( pTarget, duration ).Start( LOOKING_AT );
#endif
		}
		/// <summary>
		/// Rotates this gameObject until it is looking at the specified /pTarget/.
		/// </summary>
		/// <param name="targetName">The target's name.</param>
		/// <param name="duration">Animation's duration.</param>
		[System.Obsolete( "This is not good for performance use the LookAt( Transform, float ) version of this method." )]
		public void LookAt( string targetName, float duration )
		{
			Transform _target = targetName.Find<Transform>();
			if( !_target )
			{
                Utilities.LogWarning("LookAt() -> No target was found with the specified name: " + targetName, gameObject );
				return;
			}
			LookAt( _target, duration );
		}
		/// <summary>
		/// If the gameObject is rotating to look at a target, this will interrupt the rotation's animation (stop it)
		/// </summary>
		public void StopLookAt()
		{
			_IsLookingAt = false;
#if USE_THREE_D
			if( _lookAt )
				DestroyImmediate( _lookAt );
#endif
		}



		/// <summary>
		/// Sets/Overrides the /onAligned/ ComposedEvent with the specified SingleComposedEvent's composed events.
		/// </summary>
		public void SetOnAligned( SingleComposedEvent scEvent )
		{
			if( scEvent.m_composedEvent != null )
			{
				onAligned = scEvent.m_composedEvent;
			}
		}
		/// <summary>
		/// Sets/Overrides the /onMoved/ ComposedEvent with the specified SingleComposedEvent's composed events.
		/// </summary>
		public void SetOnMoved( SingleComposedEvent scEvent )
		{
			if( scEvent.m_composedEvent != null )
			{
				onMoved = scEvent.m_composedEvent;
			}
		}


		/// <summary>
		/// Moves the specified gameObject and destroys it after it reaches .
		/// </summary>
		/// <param name="targetAndDelay">Target's name followed by the specified /argsSplitter/ and then the animation's delay.</param>
        public void MoveAndDestroyObjAfter( string targetAndDelay = "targetObjectName:0.5" )
        {
            string[] values = targetAndDelay.Split( argsSplitter );
            if( values.Length != 2 )
            {
                Utilities.LogWarning("The specified parameters format is wrong. It must be --> targetObjectName"+ argsSplitter +"0.5");
                return;
            }
			float movementDelay = values[1].ToFloat( 0 );
            Move( values[0] );
			objToAlign.m_gameObject.Destroy( movementDelay + animDuration );
        }


		/// <summary>
		/// Aligns the /objToAlign/ with the specified /pTarget/ in the specified /duration/.
		/// </summary>
		protected virtual IEnumerator _Align( Transform pTarget, float duration )
		{
            objToAlign.m_transform.AlignCo( pTarget, duration ).Start( ALIGNING );
            while( objToAlign.m_transform.position != pTarget.position || objToAlign.m_transform.rotation != pTarget.rotation )
			{
				if( !_IsAligning )
					yield break;
				yield return null;
				if( !pTarget || this == null )
				{
					yield break;
				}
			}
			yield return null;//WAIT FOR ALIGN FLAG TO BE RESET
			onAligned.Invoke();
		}
		/// <summary>
		/// Aligns the /objToAlign/ with the specified /pTarget/ and then back to its original alignment, in the specified /duration/.
		/// </summary>
		protected virtual IEnumerator _AlignPingPong( Transform pTarget, float duration )
		{
			if( !_flags )
                _flags = objToAlign.m_gameObject.AddGetComponent<ValidationFlag>();
			Transform original = new GameObject( name + "PingPongSource" ).transform;
            original.CopyTransformFrom( objToAlign.m_transform );
            yield return objToAlign.m_transform.AlignCo( pTarget, duration * 0.5f ).Start( ALIGNING_PP );
			if( !_flags )
				yield break;
            yield return objToAlign.m_transform.AlignCo( original, duration * 0.5f ).Start( ALIGNING_PP );
			Destroy ( original.gameObject );
			if( !pTarget || this == null || !_flags )
			{
				yield break;
			}
			onAligned.Invoke();
		}
		/// <summary>
		/// Yields until /objToAlign/ is aligned with the specified /pTarget/. 
		/// </summary>
		protected IEnumerator _WaitForAlignment( Transform pTarget )
		{
            while( objToAlign.m_transform.position != pTarget.position || objToAlign.m_transform.rotation != pTarget.rotation )
			{
				if( !_IsAligning )
					yield break;
				yield return null;
				if( !pTarget || this == null )
					yield break;
			}
		}
		/// <summary>
		/// Move /objToAlign/ until it reaches the specified /pTarget/ in the specified /duration/.
		/// </summary>
		protected virtual IEnumerator _Move( Transform pTarget, float duration )
		{
            objToAlign.m_transform.MoveTowardsCo( duration, pTarget ).Start( MOVING );

			while( objToAlign != null && objToAlign.m_transform != null && pTarget != null && objToAlign.m_transform.position != pTarget.position )
			{
				if( !_IsMoving )
					yield break;
				yield return null;
				if( !pTarget || this == null )
				{
					yield break;
				}
			}
			yield return null;//WAIT FOR MOVE FLAG TO BE RESET
			onMoved.Invoke();
		}
		/// <summary>
		/// Rotates /objToAlign/ until it looks at the specified /pTarget/ in the specified /duration/.
		/// </summary>
		protected virtual IEnumerator _LookAtCo( Transform pTarget, float duration )
		{
			if( duration > 0 )
			{
                yield return StartCoroutine( objToAlign.m_transform.LookAtCo( pTarget, duration ) );
			}
			else
			{
				_IsLookingAt = true;
                while( objToAlign.m_transform && pTarget )
				{
					yield return null;
					if( !_IsLookingAt )
						break;
                    if( !objToAlign.m_transform || this == null )
					{
						yield break;
					}
                    objToAlign.m_transform.LookAt( pTarget );
				}
			}
		}
	}
}