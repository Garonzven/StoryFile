using UnityEngine;
using System.Collections;
using DDK.Base.Animations;
using System.Collections.Generic;
using DDK.Base.Classes;
using DDK.Base.Extensions;
using MovementEffects;


namespace DDK.Base.Animations
{
    /// <summary>
    /// Attach to an object to align it with other objects.
    /// </summary>
	public class AlignWithMultiple : AlignWith 
    {
		public enum TargetSelectionMode
		{
			Random,
			Closest,
            Farthest
		}

		[System.Serializable]
		public class TargetGameObject 
        {
			[ShowIfAttribute( "_UpdateName" )]
			public string name;
			public SearchableGameObject target = new SearchableGameObject();
			[Tooltip("This will pause the animation after this target is reached, for the specified amount of time")]
			public float animPause;
            [Tooltip("If higher than 0, it will override the calculated duration")]
            public float animDurationOverride = 0f;
			[Header("Events")]
			public ComposedEvent onAligned = new ComposedEvent();
			public ComposedEvent onMoved = new ComposedEvent();
			public ComposedEvent onAlignedPostPause = new ComposedEvent();
			public ComposedEvent onMovedPostPause = new ComposedEvent();

			protected bool _UpdateName()
			{
				if( !string.IsNullOrEmpty( target.objName ) )
				{
					name = target.objName;
				}
				return false;
			}
		}



		[Space( 10f )]
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "If calling any of the ...AllInOrder() functions take into account that the /animDuration/" +
            " is until the last target is reached. If adding /animPause/ to a target, that will be added to the duration." +
            " Same applies to the events, meaning that they will be invoked when the last target is reached";
        #endif
		public TargetSelectionMode targetSelectionMode = TargetSelectionMode.Random;
		public List<TargetGameObject> targets = new List<TargetGameObject>();


		protected override Transform _Target 
        {
			get
            {
				Transform[] transforms = new Transform[ targets.Count ];
				for( int i=0; i<targets.Count; i++ )
				{
					transforms[i] = targets[i].target.m_transform;
				}
				switch( targetSelectionMode )
				{
                case TargetSelectionMode.Closest: return target = objToAlign.m_transform.GetClosest( transforms );
                case TargetSelectionMode.Farthest: return target = objToAlign.m_transform.GetFarthest( transforms );
				case TargetSelectionMode.Random: return target = transforms.GetRandom<Transform>();
				}
				return target;
			}
			set
            {
				if( value )
					target = value;
			}
		}


#region VALIDATION FUNCTIONS
		protected override bool _IsTargetInvalid()
		{
			return false;
		}
		protected override bool _ShowTargetField()
		{
			return false;
		}
#endregion


        protected override void OnEnable()
        {
            if( !Application.isPlaying )
                return;
            if( _started && onEnable && targets.Count > 0 )
                AlignWithAllInOrderAfter ( delay );
        }


        public void AlignWithAllInOrderAfter( float delay )
        {
            _AlignWithAllInOrderAfter( delay ).Run();
        }
		public void AlignWithAllInOrder( float animDuration )
		{
            _AlignWithAllInOrder( animDuration ).Run();
		}
		public void AlignWithAllInOrderPingPong( float animDuration )
		{
			_AlignWithAllInOrderPingPong( animDuration ).Start ( ALIGNING_PP );
		}
		public void MoveTowardsAllInOrder( float animDuration )
		{
			_MoveTowardsAllInOrder( animDuration ).Start ( MOVING );
		}
		public void LookAtAllInOrder( float animDuration )
		{
			_LookAtAllInOrderCo( animDuration ).Start ( LOOKING_AT );
		}
            


        protected IEnumerator<float> _AlignWithAllInOrderAfter( float delay )
        {
            yield return Timing.WaitForSeconds( delay );
            yield return _AlignWithAllInOrder( animDuration ).RunWaitUntilDone();
        }
        protected IEnumerator<float> _AlignWithAllInOrder( float animDuration )
		{
			if( animDuration > 0 )
				animDuration = animDuration / targets.Count;
            Transform target = null;//caching
			for( int i=0; i<targets.Count; i++ )
			{
				target = targets[i].target.m_transform;
                if( targets[i].animDurationOverride > 0f )
                    animDuration = targets[i].animDurationOverride;
                objToAlign.m_transform.AlignCo( target, animDuration ).Start( ALIGNING );
                //CHECK, THE ALIGNING MIGHT BE CANCELED
                while( objToAlign.m_transform.position != target.position || objToAlign.m_transform.rotation != target.rotation )
				{
					if( !_IsAligning )
                    {
                        yield break;
                    }
					yield return 0f;
					if( !target || this == null )
					{
						yield break;
					}
                }
				yield return 0f;//WAIT FOR ALIGN FLAG TO BE RESET
				targets[i].onAligned.Invoke();
				yield return Timing.WaitForSeconds( targets[i].animPause );
				targets[i].onAlignedPostPause.Invoke();
			}
			onAligned.Invoke();
		}
		protected IEnumerator _AlignWithAllInOrderPingPong( float animDuration )
		{
			if( animDuration > 0 )
				animDuration = animDuration / targets.Count;
			Transform _original = new GameObject( name + "PingPongSource" ).transform;
            _original.CopyTransformFrom( objToAlign.m_transform );
            float _originalDuration = animDuration;
			for( int i=0; i<targets.Count; i++ )
			{
				Transform target = targets[i].target.m_transform;
                if( targets[i].animDurationOverride > 0f )
                {
                    objToAlign.m_transform.AlignCo( target, targets[i].animDurationOverride ).Start( ALIGNING_PP );
                    if( i == 0 )
                        _originalDuration = targets[i].animDurationOverride;
                }
                else objToAlign.m_transform.AlignCo( target, animDuration ).Start( ALIGNING_PP );
				yield return _WaitForAlignment( target ).Start( ALIGNING_PP );
				if( !target || this == null || !_IsAligning )
				{
					Destroy ( _original.gameObject );
					yield break;
				}
				yield return null;//WAIT FOR ALIGN FLAG TO BE RESET
				targets[i].onAligned.Invoke();
				yield return new WaitForSeconds( targets[i].animPause );
				targets[i].onAlignedPostPause.Invoke();
			}
			for( int i=targets.Count-2; i>0; i-- )
			{
				Transform target = targets[i].target.m_transform;
                if( targets[i].animDurationOverride > 0f )
                    objToAlign.m_transform.AlignCo( target, targets[i].animDurationOverride ).Start( ALIGNING_PP );
                else objToAlign.m_transform.AlignCo( target, animDuration ).Start( ALIGNING_PP );
				yield return _WaitForAlignment( target ).Start( ALIGNING_PP );
				if( !target || this == null || !_IsAligning )
				{
					Destroy ( _original.gameObject );
					yield break;
				}
				yield return null;//WAIT FOR ALIGN FLAG TO BE RESET
				targets[i].onAligned.Invoke();
				yield return new WaitForSeconds( targets[i].animPause );
				targets[i].onAlignedPostPause.Invoke();
			}
            objToAlign.m_transform.AlignCo( _original, _originalDuration ).Start( ALIGNING_PP );
			yield return _WaitForAlignment( _original ).Start( ALIGNING_PP );
			if( !_original || this == null || !_IsAligning )
			{
				Destroy ( _original.gameObject );
				yield break;
			}
			_original.gameObject.Destroy();
			onAligned.Invoke();
		}
		protected IEnumerator _MoveTowardsAllInOrder( float animDuration )
		{
			if( animDuration > 0 )
				animDuration = animDuration / targets.Count;
			for( int i=0; i<targets.Count; i++ )
			{
				Transform target = targets[i].target.m_transform;
                if( targets[i].animDurationOverride > 0f )
                    objToAlign.m_transform.MoveTowardsCo( targets[i].animDurationOverride, target ).Start( MOVING );
                else objToAlign.m_transform.MoveTowardsCo( animDuration, target ).Start( MOVING );
                objToAlign.m_transform.MoveTowardsCo( animDuration, target ).Start( MOVING );
                while( objToAlign.m_transform.position != target.position )
				{
					if( !_IsMoving )
						yield break;
					yield return null;
					if( !target || this == null )
					{
						yield break;
					}
				}
				yield return null;//WAIT FOR ALIGN FLAG TO BE RESET
				targets[i].onMoved.Invoke();
				yield return new WaitForSeconds( targets[i].animPause );
				targets[i].onMovedPostPause.Invoke();
			}
			onMoved.Invoke();
		}
		protected IEnumerator _LookAtAllInOrderCo( float animDuration )
		{
			if( animDuration > 0 )
				animDuration = animDuration / targets.Count;
			for( int i=0; i<targets.Count; i++ )
			{
				Transform target = targets[i].target.m_transform;
				if( animDuration > 0 )
				{
                    if( targets[i].animDurationOverride > 0f )
                        yield return StartCoroutine( objToAlign.m_transform.LookAtCo( target, targets[i].animDurationOverride ) );
                    else yield return StartCoroutine( objToAlign.m_transform.LookAtCo( target, animDuration ) );
				}
                else while( _IsLookingAt && objToAlign.m_transform && target )
				{
					if( !_IsLookingAt )
						yield break;
					yield return null;
                    if( !objToAlign.m_transform || this == null )
					{
						_IsLookingAt = false;
						yield break;
					}
                    objToAlign.m_transform.LookAt( target );
				}
			}
		}
	}
}
