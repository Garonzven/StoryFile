//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.Base.Animations
{
    /// <summary>
    /// Animates the scale of a gameObject's transform.
    /// </summary>
    public class AnimateScale : MonoBehaviourExt
    {
        public string targetsTag;
        [Tooltip("If no target, this gameObject's transform will be used")]
        [DisableIfAttribute( "_TargetsTag" )]
        public SearchableGameObject[] targetObjs = new SearchableGameObject[0];
        public bool onEnable;
        public float delay;
        public bool fromCurrentScale = true;
        [ShowIfAttribute( "_FromCurrentScale", true, 1 )]
        public Vector3 from = Vector3.one;
        [ShowIfAttribute( "_FromCurrentScale", true )]
        public bool toCurrentScale;
        [ShowIfAttribute( "_ToCurrentScale", true, 1 )]
        public Vector3 target = Vector3.one;
        [NotLessThan( 0.1f )]
        public float duration = 1f;
        [ShowIfAttribute( "_CurveUpdate" )]
        public AnimationCurve multiplier;
        [Tooltip( "If this is a Graphic and it has a Button with its transition set to Animation, its Animator will be toggled (enabled/disabled)" )]
        public bool toggleAnimator;


        protected bool _TargetsTag()//EDITOR
        {
            return !string.IsNullOrEmpty( targetsTag );
        }
        protected ValidationFlag[] scaleFlags { get; private set; }
        protected bool _CurveUpdate()//EDITOR
        {
            if( duration == 0f )
                return false;
            bool durationExceeded = false;
            float time;
            if( multiplier == null || multiplier.keys.Length <= 1 )
            {
                multiplier = AnimationCurve.Linear( 0f, 1f, duration, 1f );
            }
            else for( int i = 0; i<multiplier.keys.Length; i++ )
            {
                if( multiplier.keys[ i ].time > duration && i != multiplier.keys.Length - 1 )//CAN'T EXCEED DURATION
                {
                    durationExceeded = true;
                    multiplier.RemoveKey( i-- );
                }
                //LAST MUST REACH DURATION
                else if( i == multiplier.keys.Length - 1 && multiplier.keys[ i ].time != duration )
                {
                    for( int j = 1; j<multiplier.keys.Length; j++ )//FIX ALL KEYFRAMES
                    {
                        time = duration * multiplier.keys[ j ].time / multiplier.keys[ i ].time;
                        multiplier.MoveKey( j, new Keyframe( time, multiplier.keys[ j ].value ) );
                    }
                }
            }
            if( durationExceeded )
            {
                if( multiplier.keys.Length > 1 )
                    multiplier.AddKey( duration, 1f );
                else multiplier = AnimationCurve.Linear( 0f, 1f, duration, 1f );
            }
            return true;
        }
        protected bool _FromCurrentScale()//EDITOR
        {
            return fromCurrentScale;
        }
        protected bool _ToCurrentScale()//EDITOR
        {
            return toCurrentScale;
        }


        protected Vector3[] _froms;
        protected Vector3[] _targets;
        protected bool _started;



        void Awake() {

            if( _TargetsTag() )
            {
                targetObjs = targetObjs.Fill( GameObject.FindGameObjectsWithTag( targetsTag ) );
            }
            if( targetObjs.Length == 0 )
            {
                SetTargetObj( gameObject );
            }
            else if( !targetObjs[ 0 ].m_gameObject )
            {
                targetObjs[ 0 ].m_gameObject = gameObject;
            }
            scaleFlags = targetObjs.GetGameObjects().AddGetComponents<ValidationFlag>();
        }
        void Start()
        {
            int objsCount = targetObjs.Length;
            if( toCurrentScale )
            {
                _targets = new Vector3[ objsCount ];
                for( int i=0; i<objsCount; i++ )
                {
                    if( !targetObjs[i].m_transform )
                        continue;
                    _targets[i] = targetObjs[i].m_transform.localScale;
                }   
            }
            else 
            {
                _targets = new Vector3[ objsCount ];
                for( int i=0; i<objsCount; i++ )
                {
                    if( !targetObjs[i].m_transform )
                        continue;
                    _targets[i] = target;
                } 
            }
            if( fromCurrentScale )
            {
                _froms = new Vector3[ objsCount ];
                for( int i=0; i<objsCount; i++ )
                {
                    if( !targetObjs[i].m_transform )
                        continue;
                    _froms[i] = targetObjs[i].m_transform.localScale;
                }                    
            }
            else
            {
                _froms = new Vector3[ objsCount ];
                for( int i=0; i<objsCount; i++ )
                {
                    if( !targetObjs[i].m_transform )
                        continue;
                    targetObjs[i].m_transform.localScale = from;
                    _froms[i] = from;
                } 
            }

            _started = true;
            OnEnable();
        }
        void OnEnable() 
        {
            if( !onEnable || !_started )
                return;

            Animate();
        }


        #region ANIMATIONS
        public void Animate()
        {
            Animate( delay, _targets, duration );
        }
        /// <summary>
        /// This will stop the current scale animation and animate the object.
        /// </summary>
        public void AnimateForced()
        {
            StopAnimating();
            Animate();
        }
        public void Animate( GameObject targetObj )
        {
            if( targetObj == null )
                return;
            targetObj.AnimScaleWithMultiplier( target, duration, multiplier, toggleAnimator ).Start( delay );
        }
        /// <summary>
        /// This will stop the current scale animation and animate the object.
        /// </summary>
        public void AnimateForced( GameObject targetObj )
        {
            StopAnimating( targetObj );
            Animate( targetObj );
        }
        public void Animate( Component targetObj )
        {
            if( targetObj == null )
                return;
            targetObj.gameObject.AnimScaleWithMultiplier( target, duration, multiplier, toggleAnimator ).Start( delay );
        }
        public void Animate( GameObject targetObj, Vector3 target, float duration )
        {
            if( targetObj == null )
                return;
            targetObj.AnimScaleWithMultiplier( target, duration, multiplier, toggleAnimator ).Start( delay );
        }
        public void Animate( Component targetObj, Vector3 target, float duration )
        {
            if( targetObj == null )
                return;
            targetObj.gameObject.AnimScaleWithMultiplier( target, duration, multiplier, toggleAnimator ).Start( delay );
        }
        /// <summary>
        /// This will stop the current scale animation and animate the object.
        /// </summary>
        public void AnimateForced( Component targetObj )
        {
            StopAnimating( targetObj );
            Animate( targetObj );
        }
        public void Animate( GameObject[] targetObjs )
        {
            if( targetObjs == null )
                return;
            targetObjs.AnimScaleWithMultiplier( _targets, duration, multiplier, toggleAnimator ).Start( delay );
        }
        public void Animate( Component[] targetObjs )
        {
            if( targetObjs == null )
                return;
            targetObjs.GetGameObjects().AnimScaleWithMultiplier( _targets, duration, multiplier, toggleAnimator ).Start( delay );
        }
        public void Animate( GameObject[] targetObjs, Vector3 target, float duration )
        {
            if( targetObjs == null )
                return;
            for( int i=0; i<targetObjs.Length; i++ )
            {
                targetObjs[ i ].AnimScaleWithMultiplier( target, duration, multiplier, toggleAnimator ).Start( delay );
            }
        }
        public void Animate( Component[] targetObjs, Vector3 target, float duration )
        {
            if( targetObjs == null )
                return;
            for( int i=0; i<targetObjs.Length; i++ )
            {
                targetObjs[ i ].gameObject.AnimScaleWithMultiplier( target, duration, multiplier, toggleAnimator ).Start( delay );
            }
        }
        /// <summary>
        /// This will stop the current scale animation and animate the object.
        /// </summary>
        public void AnimateForced( GameObject[] targetObjs )
        {
            StopAnimating( targetObjs );
            Animate( targetObjs );
        }
        /// <summary>
        /// This will stop the current scale animation and animate the object.
        /// </summary>
        public void AnimateForced( Component[] targetObjs )
        {
            StopAnimating( targetObjs );
            Animate( targetObjs );
        }
        public void AnimateToTarget( Vector3 target )
        {
            Animate( delay, target, duration );
        }
        public void AnimateToTarget( float target )
        {
            Animate( delay, target.GetVector3( 1f ), duration );
        }
        public void AnimateMultiplyScale( float scaleMultiplier )
        {
            for( int i=0; i<targetObjs.Length; i++ )
            {
                Vector3 target = targetObjs[ i ].m_transform.localScale * scaleMultiplier;
                Animate( delay, target, duration );
            }
        }
        public void AnimateWithDuration( float duration )
        {
            Animate( delay, _targets, duration );
        }
        public void Animate( Vector3 target, float duration )
        {
            Animate( delay, target, duration );
        }
        public void Animate( float delay, Vector3[] targets, float duration )
        {            
            targetObjs.AnimScaleWithMultiplier( targets, duration, multiplier, toggleAnimator ).Start( delay );
        }
        public void Animate( float delay, Vector3 target, float duration )
        {            
            targetObjs.AnimScaleWithMultiplier( target, duration, multiplier, toggleAnimator ).Start( delay );
        }
        public void StopAnimating()
        {
            scaleFlags.SetFlagged( ValidationFlag.Flags.Scale, false );
        }
        public void StopAnimating( GameObject target )
        {
            target.SetFlagged( ValidationFlag.Flags.Scale, false );
        }
        public void StopAnimating( Component target )
        {
            if( !target )
            {
                Debug.LogWarning( "Target is null", gameObject );
                return;
            }
            target.gameObject.SetFlagged( ValidationFlag.Flags.Scale, false );
        }
        public void StopAnimating( GameObject[] targets )
        {
            if( targets == null )
            {
                Debug.LogWarning( "Targets are null", gameObject );
                return;
            }
            targets.SetFlagged( ValidationFlag.Flags.Scale, false );
        }
        public void StopAnimating( Component[] targets )
        {
            if( targets == null )
            {
                Debug.LogWarning( "Targets are null", gameObject );
                return;
            }
            targets.GetGameObjects().SetFlagged( ValidationFlag.Flags.Scale, false );
        }
        /// <summary>
        /// Animates the scale from the current scale to /from/
        /// </summary>
        public void AnimateReversed()
        {
            Animate( delay, _froms, duration );
        }
        /// <summary>
        /// Animates the scale from the current scale to /from/
        /// </summary>
        public void AnimateReversedAfter( float delay )
        {
            Animate( delay, _froms, duration );
        }
        #endregion


        #region SET
        public void SetTarget( Vector3 newTarget )
        {
            _targets = new Vector3[]{ newTarget };
        }
        public void SetTarget( float newTarget )
        {
            _targets = new Vector3[]{ newTarget.GetVector3( 1f ) };
        }
        public void SetDelay( float delay )
        {
            this.delay = delay;
        }
        public void SetDuration( float duration )
        {
            float time;
            for( int i = 1; i<multiplier.keys.Length; i++ )
            {
                time = duration * multiplier.keys[i].time / this.duration;
                multiplier.MoveKey( i, new Keyframe( time, multiplier.keys[ i ].value ) );
            }
            this.duration = duration;
        }
        public void SetTargetObj( GameObject targetObj )
        {
            if( targetObj == null )
                return;
            SearchableGameObject obj = new SearchableGameObject();
            obj.m_gameObject = targetObj;
            targetObjs = new SearchableGameObject[]{ obj };
        }
        public void SetTargetObjs( GameObject[] _targetObjs )
        {
            if( _targetObjs == null )
                return;
            targetObjs = new SearchableGameObject[ _targetObjs.Length ];
            for( int i=0; i<targetObjs.Length; i++ )
            {
                targetObjs[ i ].m_gameObject = _targetObjs[ i ];
            }
        }
        public void ScaleMultiplierTimeAndKeepDuration( float scale )
        {
            multiplier.ScaleTime( scale );
        }
        public void ScaleMultiplierTime( float scale )
        {
            SetDuration( duration * scale );
            multiplier.ScaleTime( scale, true );
        }
        #endregion


        /// <summary>
        /// Adds another objet to the /targetObjs/ array.
        /// </summary>
        public void AddTargetObj( GameObject targetObj )
        {
            if( targetObj == null )
                return;
            SearchableGameObject obj = new SearchableGameObject();
            obj.m_gameObject = targetObj;
            targetObjs.Add( obj, targetObjs.Length + 1 );
        }
    }
}
