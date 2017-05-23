//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using DDK.Base.Statics;


namespace DDK.Base.Animations
{
    /// <summary>
    /// Base class to inherit from when creating a script to do some simple animation. NOTE: AnimateScale.cs was done 
    /// before this script and can't inherit from this because it would affect instances of that script that are already in use.
    /// </summary>
    public abstract class AnimateBase<T> : MonoBehaviourExt
    {
        public string targetsTag;
        [Tooltip("If no target, this gameObject's transform will be used")]
        [DisableIfAttribute( "_TargetsTag" )]
        public SearchableGameObject[] targetObjs = new SearchableGameObject[0];
        public bool onEnable;
        public float delay;
        public T target;
        [NotLessThan( 0.1f )]
        public float duration = 1f;
        [ShowIfAttribute( "_CurveUpdate" )]
        public AnimationCurve multiplier;
        [Tooltip( "If this is a Graphic and it has a Button with its transition set to Animation, its Animator will be toggled (enabled/disabled)" )]
        public bool toggleAnimator;


        #if UNITY_EDITOR
        /// <summary>
        /// Updates the /multiplier/ AnimationCurve in the Editor so it can be valid (from 0 to 1).
        /// </summary>
        protected bool _CurveUpdate()//EDITOR
        {
            if( duration.CloseTo( 0f ) )
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
                else if( i == multiplier.keys.Length - 1 && !multiplier.keys[ i ].time.CloseTo( duration ) )
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
        #endif


        protected bool _TargetsTag()//EDITOR
        {
            return !string.IsNullOrEmpty( targetsTag );
        }
        protected ValidationFlag[] _ValidationFlags { get; private set; }


        /// <summary>
        /// Validate/Initialize objects.
        /// </summary>
        void Awake() 
        {
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
            _ValidationFlags = targetObjs.GetGameObjects().AddGetComponents<ValidationFlag>();
        }
        /// <summary>
        /// If /onEnable/ equals true, Animate() is invoked.
        /// </summary>
        void OnEnable() 
        {
            if( !onEnable )
                return;
            Animate();
        }


        #region ANIMATIONS
        public abstract void Animate( GameObject targetObj, T target, float duration );
        public abstract void Animate( float delay, T target, float duration );
        public abstract void AnimateMultiplyValue( float valueMultiplier );
        public abstract void StopAnimating();
        public abstract void StopAnimating( GameObject target );

        /// <summary>
        /// Animate this instance.
        /// </summary>
        public void Animate()
        {
            Animate( delay, target, duration );
        }
        /// <summary>
        /// This will stop the current animation and animate the object again.
        /// </summary>
        public void AnimateForced()
        {
            StopAnimating();
            Animate();
        }
        /// <summary>
        /// Animate the specified /targetObj/.
        /// </summary>
        public void Animate( GameObject targetObj )
        {
            if( targetObj == null )
                return;
            Animate( targetObj, target, duration );
        }
        /// <summary>
        /// This will stop the current /targetObj/ animation and animate again.
        /// </summary>
        public void AnimateForced( GameObject targetObj )
        {
            StopAnimating( targetObj );
            Animate( targetObj );
        }
        /// <summary>
        /// Animate the specified /targetObj/'s gameObject.
        /// </summary>
        /// <param name="targetObj">The component holding the target gameObject.</param>
        public void Animate( Component targetObj )
        {
            if( targetObj == null )
                return;
            Animate( targetObj.gameObject );
        }
        /// <summary>
        /// Animate the /targetObj/'s gameObject, until it reaches the /target/ in the specified /duration/ seconds. 
        /// Context depends on the child class implementation.
        /// </summary>
        public void Animate( Component targetObj, T target, float animDuration )
        {
            if( targetObj == null )
                return;
            Animate( targetObj.gameObject, target, animDuration );
        }
        /// <summary>
        /// This will stop the current /targetObj/ scale animation and animate again.
        /// </summary>
        /// <param name="targetObj"> The component holding the target gameObject </param>
        public void AnimateForced( Component targetObj )
        {
            StopAnimating( targetObj );
            Animate( targetObj );
        }
        /// <summary>
        /// Animate the specified /targetObjs/.
        /// </summary>
        public void Animate( GameObject[] targetObjs )
        {
            if( targetObjs == null )
                return;
            for( int i=0; i<targetObjs.Length; i++ )
            {
                Animate( targetObjs[ i ] );
            }
        }
        /// <summary>
        /// Animate the specified /targetObjs/.
        /// </summary>
        /// <param name="targetObjs"> The components holding the target gameObjects.</param>
        public void Animate( Component[] targetObjs )
        {
            if( targetObjs == null )
                return;
            for( int i=0; i<targetObjs.Length; i++ )
            {
                Animate( targetObjs[ i ] );
            }
        }
        /// <summary>
        /// Animate the /targetObjs/, until they reach the /target/ in the specified /duration/ seconds.
        /// </summary>
        public void Animate( GameObject[] targetObjs, T target, float animDuration )
        {
            if( targetObjs == null )
                return;
            for( int i=0; i<targetObjs.Length; i++ )
            {
                Animate( targetObjs[ i ], target, animDuration );
            }
        }
        /// <summary>
        /// Animate the specified /targetObjs/'s gameObjects, until they reach the /target/ in the specified 
        /// /duration/ seconds. Context depends on the child class implementation.
        /// </summary>
        public void Animate( Component[] targetObjs, T target, float animDuration )
        {
            if( targetObjs == null )
                return;
            for( int i=0; i<targetObjs.Length; i++ )
            {
                Animate( targetObjs[ i ], target, animDuration );
            }
        }
        /// <summary>
        /// This will stop the current /targetObjs/ scale animation and animate again.
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
        /// <summary>
        /// Animates until it reaches the specified /target/. Context depends on the child class implementation.
        /// </summary>
        public void AnimateToTarget( T target )
        {
            Animate( delay, target, duration );
        }
        /// <summary>
        /// Animates with the specified /duration/.
        /// </summary>
        public void AnimateWithDuration( float animDuration )
        {
            Animate( delay, target, animDuration );
        }
        /// <summary>
        /// Animate the /target/ in the specified /duration/ seconds. Context depends on the child class implementation.
        /// </summary>
		public void Animate( T target, float animDuration )
        {
            Animate( delay, target, animDuration );
        }
        /// <summary>
        /// Stops animating the specified /target/'s gameObject.
        /// </summary>
        /// <param name="target">Target.</param>
        public void StopAnimating( Component target )
        {
            if( !target )
            {
                Utilities.LogWarning( "Target is null", gameObject );
                return;
            }
            StopAnimating( target.gameObject );
        }
        /// <summary>
        /// Stops animating the specified /targets/.
        /// </summary>
        public void StopAnimating( GameObject[] targets )
        {
            if( targets == null )
            {
                Utilities.LogWarning( "Targets are null", gameObject );
                return;
            }
            for( int i=0; i<targets.Length; i++ )
            {
                StopAnimating( targets[i] );
            }
        }
        /// <summary>
        /// Stops animating the specified /targets/'s gameObjects.
        /// </summary>
        public void StopAnimating( Component[] targets )
        {
            if( targets == null )
            {
                Utilities.LogWarning( "Targets are null", gameObject );
                return;
            }
            for( int i=0; i<targets.Length; i++ )
            {
                StopAnimating( targets[i] );
            }
        }
        #endregion


        #region SET
        /// <summary>
        /// Sets the /target/ as the specified /newTarget/. Context depends on the child class implementation.
        /// </summary>
        public void SetTarget( T newTarget )
        {
            target = newTarget;
        }
        /// <summary>
        /// Sets the /duration/ as the specified /newDuration/.
        /// </summary>
        public void SetDuration( float newDuration )
        {
            duration = newDuration;
        }
        /// <summary>
        /// Sets the target gameObject.
        /// </summary>
        /// <param name="targetObj">Target gameObject.</param>
        public void SetTargetObj( GameObject targetObj )
        {
            if( targetObj == null )
                return;
            targetObjs = new []{ new SearchableGameObject { m_gameObject = targetObj } };
        }
        /// <summary>
        /// Sets the target gameObjects.
        /// </summary>
        /// <param name="newTargetObjs">New target gameObjects.</param>
        public void SetTargetObjs( GameObject[] newTargetObjs )
        {
            if( newTargetObjs == null )
                return;
            targetObjs = new SearchableGameObject[ newTargetObjs.Length ];
            for( int i=0; i<targetObjs.Length; i++ )
            {
                targetObjs[ i ].m_gameObject = newTargetObjs[ i ];
            }
        }
        #endregion


        /// <summary>
        /// Adds another /targetObj/ to the /targetObjs/ array.
        /// </summary>
        public void AddTargetObj( GameObject targetObj )
        {
            if( targetObj == null )
                return;
            targetObjs.Add( new SearchableGameObject { m_gameObject = targetObj }, targetObjs.Length + 1 );
        }
    }
}
