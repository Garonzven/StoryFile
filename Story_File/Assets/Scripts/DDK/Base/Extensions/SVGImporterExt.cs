#if USE_SVG_IMPORTER
//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using SVGImporter;
using DDK.Base.Animations;
using System.Collections.Generic;


namespace DDK.Base.Extensions
{
    /// <summary>
    /// SVGImporter plugin extension class.
    /// </summary>
    public static class SVGImporterExt 
    {
        #region SVG FRAME ANIMATORS
        /// <summary>
        /// Animate the specified animator's frames.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static void Animate( this SVGFrameAnimator animator, float duration, bool reverse = false )
        {
            animator.AnimateCo( duration, reverse ).Start();
        }
        /// <summary>
        /// Animate the specified animator's frames.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static IEnumerator AnimateCo( this SVGFrameAnimator animator, float duration, bool reverse = false )
        {
            if( animator == null )
                yield break;

            ValidationFlag validationFlags = animator.gameObject.AddGetComponent<ValidationFlag>();
            if( validationFlags.IsFlagged( ValidationFlag.Flags.Frame ) )
                yield break;
            validationFlags.SetFlagged( ValidationFlag.Flags.Frame, true );

            int totalFrames = animator.frames.Length - 1;
            float wait = (duration / totalFrames).SetNumDecimals( 2 );
            var _wait = new WaitForSeconds( wait );

            if( reverse && animator.frameIndex < totalFrames )
            {
                animator.frameIndex = totalFrames;
                /*Debug.LogWarning("The specified SVGFrameAnimator's current frame index is below the last frame, make sure" +
                    " it is in the last frame to run the animation backwards", animator.gameObject );
                yield break;*/
            }
            else if( !reverse && animator.frameIndex > 0f )
            {
                animator.frameIndex = 0f;
                /*Debug.LogWarning("The specified SVGFrameAnimator's current frame index is higher than the first frame, " +
                    "make sure it is in the first frame to run the animation", animator.gameObject );
                yield break;*/
            }
                
            for( int i=0; i<totalFrames; i++ )
            {
                yield return _wait;
                if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.Frame ) )
                {
                    if( reverse )
                    {
                        animator.frameIndex = totalFrames;
                    }
                    else if( !reverse )
                    {
                        animator.frameIndex = 0f;
                    }
                    yield break;
                }
                if( reverse )
                    animator.frameIndex--;
                else animator.frameIndex++;
            }

            validationFlags.SetFlagged( ValidationFlag.Flags.Frame, false );
            yield return null;
        }
        /// <summary>
        /// Animate the specified animator's frames. If it is running an animation that one is stopped and this one played.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static void AnimateForced( this SVGFrameAnimator animator, float duration, bool reverse = false )
        {
            animator.gameObject.SetFlagged( ValidationFlag.Flags.Frame, false );
            animator.AnimateCo( duration, reverse ).Start();
        }
        /// <summary>
        /// Animate the specified animator's frames. If it is running an animation that one is stopped and this one played.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static IEnumerable AnimateForcedCo( this SVGFrameAnimator animator, float duration, bool reverse = false )
        {
            animator.gameObject.SetFlagged( ValidationFlag.Flags.Frame, false );
            yield return animator.AnimateCo( duration, reverse ).Start();
        }
        /// <summary>
        /// If /loopCount/ is cero (0) the animation will loop indefinitely, if this is desired and you want to stop the 
        /// animation later, just destroy the ValidationFlag component attached to the animator's gameObject, or set its 
        /// /Frame/ flag to false.
        /// </summary>
        /// <param name="loopCount">The amount of times the animation will go forth and back.</param>
        public static void AnimateLoop( this SVGFrameAnimator animator, float duration, int loopCount = 0 )
        {
            animator.AnimateLoopCo( duration, loopCount ).Start();
        }
        /// <summary>
        /// If /loopCount/ is cero (0) the animation will loop indefinitely, if this is desired and you want to stop the 
        /// animation later, just destroy the ValidationFlag component attached to the animator's gameObject, or set its 
        /// /Frame/ flag to false. NOTE: This will yield until the animation is over.
        /// </summary>
        /// <param name="loopCount">The amount of times the animation will go forth and back.</param>
        public static IEnumerator AnimateLoopCo( this SVGFrameAnimator animator, float duration, int loopCount = 0 )
        {
            if( animator == null )
                yield break;

            ValidationFlag validationFlags = animator.gameObject.AddGetComponent<ValidationFlag>();
            if( validationFlags.IsFlagged( ValidationFlag.Flags.Frame ) )
                yield break;
            validationFlags.SetFlagged( ValidationFlag.Flags.Frame, true );

            int totalFrames = animator.frames.Length - 1;
            float wait = (duration / totalFrames).SetNumDecimals( 2 );
            var _wait = new WaitForSeconds( wait );
            int loops = 0;
            while( true )
            {
                for( int i=0; i<totalFrames; i++ )
                {
                    yield return _wait;
                    if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.Frame ) )
                    {
                        animator.frameIndex = 0f;
                        yield break;
                    }
                    animator.frameIndex++;
                }
                loops++;
                if( loops == loopCount )
                    break;
                yield return new WaitForSeconds( wait );
                if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.Frame ) )
                    yield break;
                animator.frameIndex = 0f;
            }

            validationFlags.SetFlagged( ValidationFlag.Flags.Frame, false );
            yield return null;
        }
        public static void StopAnimating( this SVGFrameAnimator animator )
        {
            ValidationFlag validationFlags = animator.gameObject.GetComponent<ValidationFlag>();
            validationFlags.SetFlagged( ValidationFlag.Flags.Frame, false );
        }
        #endregion

        #region SVG FRAME ANIMATORS ARRAYS
        /// <summary>
        /// Animate the specified animators frames.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static void Animate( this SVGFrameAnimator[] animators, float duration, bool reverse = false )
        {
            animators.AnimateCo( duration, reverse ).Start();
        }
        /// <summary>
        /// Animate the specified animators frames.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static IEnumerator AnimateCo( this SVGFrameAnimator[] animators, float duration, bool reverse = false )
        {
            if( animators == null )
                yield break;

            for( int i=0; i<animators.Length; i++ )
            {
                animators[i].AnimateCo( duration, reverse ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        /// <summary>
        /// Animate the specified animator's frames. If it is running an animation that one is stopped and this one played.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static void AnimateForced( this SVGFrameAnimator[] animator, float duration, bool reverse = false )
        {
            animator.AnimateForcedCo( duration, reverse ).Start();
        }
        /// <summary>
        /// Animate the specified animator's frames. If it is running an animation that one is stopped and this one played.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static IEnumerable AnimateForcedCo( this SVGFrameAnimator[] animators, float duration, bool reverse = false )
        {
            if( animators == null )
                yield break;

            for( int i=0; i<animators.Length; i++ )
            {
                animators[i].gameObject.SetFlagged( ValidationFlag.Flags.Frame, false );
                animators[i].AnimateCo( duration, reverse ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        /// <summary>
        /// If /loopCount/ is cero (0) the animation will loop indefinitely, if this is desired and you want to stop the 
        /// animation later, just destroy the ValidationFlag component attached to the animator's gameObject, or set its 
        /// /Frame/ flag to false.
        /// </summary>
        /// <param name="loopCount">The amount of times the animation will go forth and back.</param>
        public static void AnimateLoop( this SVGFrameAnimator[] animators, float duration, int loopCount = 0 )
        {
            animators.AnimateLoopCo( duration, loopCount ).Start();
        }
        /// <summary>
        /// If /loopCount/ is cero (0) the animation will loop indefinitely, if this is desired and you want to stop the 
        /// animation later, just destroy the ValidationFlag component attached to the animator's gameObject, or set its 
        /// /Frame/ flag to false.
        /// </summary>
        /// <param name="loopCount">The amount of times the animation will go forth and back.</param>
        public static IEnumerator AnimateLoopCo( this SVGFrameAnimator[] animators, float duration, int loopCount = 0 )
        {
            if( animators == null )
                yield break;

            for( int i=0; i<animators.Length; i++ )
            {
                animators[i].AnimateLoopCo( duration, loopCount ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        public static void StopAnimating( this SVGFrameAnimator[] animators )
        {
            for( int i=0; i<animators.Length; i++ )
            {
                animators[i].StopAnimating();
            }
        }


        /// <summary>
        /// Animate the specified animators frames.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static void Animate( this List<SVGFrameAnimator> animators, float duration, bool reverse = false )
        {
            animators.AnimateCo( duration, reverse ).Start();
        }
        /// <summary>
        /// Animate the specified animators frames.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static IEnumerator AnimateCo( this List<SVGFrameAnimator> animators, float duration, bool reverse = false )
        {
            if( animators == null )
                yield break;

            for( int i=0; i<animators.Count; i++ )
            {
                animators[i].AnimateCo( duration, reverse ).Start();
            }
            yield return new WaitForSeconds( duration );
            Debug.LogWarning(Time.frameCount);
        }
        /// <summary>
        /// Animate the specified animator's frames. If it is running an animation that one is stopped and this one played.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static void AnimateForced( this List<SVGFrameAnimator> animator, float duration, bool reverse = false )
        {
            animator.AnimateForcedCo( duration, reverse ).Start();
        }
        /// <summary>
        /// Animate the specified animator's frames. If it is running an animation that one is stopped and this one played.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public static IEnumerable AnimateForcedCo( this List<SVGFrameAnimator> animators, float duration, bool reverse = false )
        {
            if( animators == null )
                yield break;

            for( int i=0; i<animators.Count; i++ )
            {
                animators[i].gameObject.SetFlagged( ValidationFlag.Flags.Frame, false );
                animators[i].AnimateCo( duration, reverse ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        /// <summary>
        /// If /loopCount/ is cero (0) the animation will loop indefinitely, if this is desired and you want to stop the 
        /// animation later, just destroy the ValidationFlag component attached to the animator's gameObject, or set its 
        /// /Frame/ flag to false.
        /// </summary>
        /// <param name="loopCount">The amount of times the animation will go forth and back.</param>
        public static void AnimateLoop( this List<SVGFrameAnimator> animators, float duration, int loopCount = 0 )
        {
            animators.AnimateLoopCo( duration, loopCount ).Start();
        }
        /// <summary>
        /// If /loopCount/ is cero (0) the animation will loop indefinitely, if this is desired and you want to stop the 
        /// animation later, just destroy the ValidationFlag component attached to the animator's gameObject, or set its 
        /// /Frame/ flag to false.
        /// </summary>
        /// <param name="loopCount">The amount of times the animation will go forth and back.</param>
        public static IEnumerator AnimateLoopCo( this List<SVGFrameAnimator> animators, float duration, int loopCount = 0 )
        {
            if( animators == null )
                yield break;

            for( int i=0; i<animators.Count; i++ )
            {
                animators[i].AnimateLoopCo( duration, loopCount ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        public static void StopAnimating( this List<SVGFrameAnimator> animators )
        {
            for( int i=0; i<animators.Count; i++ )
            {
                animators[i].StopAnimating();
            }
        }
        #endregion
    }
}
#endif
