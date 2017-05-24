using UnityEngine;
using System.Collections.Generic;
using MovementEffects;
using System.Collections;
using DDK.Base.Animations;


namespace DDK.Base.Extensions
{
    public static class CanvasGroupExt 
    {
        /// <summary>
        /// Sets the specified canvas group's alpha after the specified delay.
        /// </summary>
        public static IEnumerator<float> SetAlphaAfter( this CanvasGroup group, float target, float delay )
        {
            if( !group )
                yield break;
            yield return Timing.WaitForSeconds( delay );
            group.alpha = target;
        }
        /// <summary>
        /// Fades the specified canvas group, until it reaches the specified target value.
        /// </summary>
        public static IEnumerator AlphaTo( this CanvasGroup group, float target, float duration )
        {
            if( !group )
                yield break;

            ValidationFlag validationFlags = group.gameObject.AddGetComponent<ValidationFlag>();
            if( validationFlags.IsFlagged( ValidationFlag.Flags.Alpha ) )//allow animation to be overridden.
            {
                validationFlags.SetFlagged( ValidationFlag.Flags.Alpha, false );
                yield return null;//wait a frame so current animation can finish
            }
            validationFlags.SetFlagged( ValidationFlag.Flags.Alpha, true );

            float iniAlpha = group.alpha;
            float time = Time.unscaledDeltaTime;
            while( time <= duration ) //while( group.alpha != target )
            {
                group.alpha = Mathf.Lerp( iniAlpha, target, time / duration );
                yield return null;
                if( !group || !validationFlags.IsFlagged( ValidationFlag.Flags.Alpha ) )
                    yield break;
                time += Time.unscaledDeltaTime;
            }
            group.alpha = target;

            validationFlags.SetFlagged( ValidationFlag.Flags.Alpha, false );
        }       
        /// <summary>
        /// Fades the specified canvas groups, until they reaches the specified target value.
        /// </summary>
        public static void AlphaTo( this IList<CanvasGroup> groups, float target, float duration )
        {
            if( groups == null )
                return;
            for( int i=0; i<groups.Count; i++ )
            {
                groups[i].AlphaTo( target, duration ).Start();
            }
        }
        /// <summary>
        /// Animates the alpha of the specified object's CanvasGroup's until the target is reached or this object is destroyed.
        /// </summary>
        /// <param name="obj">The canvas group.</param>
        /// <param name="targetValue">Target alpha.</param>
        /// <param name="duration">Animation duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this CanvasGroup obj, float targetValue, float duration, 
            AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( !obj )
                yield break;
            if( toggleAnimator )
            {
                obj.gameObject.ToggleAnimator();
            }

            ValidationFlag validationFlags = obj.gameObject.AddGetComponent<ValidationFlag>();
            if( validationFlags.IsFlagged( ValidationFlag.Flags.Alpha ) )
                yield break;
            validationFlags.SetFlagged( ValidationFlag.Flags.Alpha, true );

            if( multiplier == null || multiplier.keys.Length <= 1 )
                multiplier = AnimationCurve.Linear( 0f, 1f, duration, 1f );
            targetValue *= multiplier.keys[ multiplier.keys.Length - 1 ].value;

            float initialValue = obj.alpha;
            float time = 0f;
            while( time < duration )
            {
                time += Time.deltaTime;
                float multiply = multiplier.Evaluate( Mathf.Lerp( 0f, 
                    multiplier.keys[ multiplier.keys.Length - 1 ].time, time / duration ) );
                obj.alpha = Mathf.Lerp( initialValue, targetValue, time / duration ) * multiply;
                yield return null;
                if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.Alpha ) )
                    yield break;
                if( !obj )//IT MIGHT GET DESTROYED WHILE ANIMATING
                    break;
            }
            validationFlags.SetFlagged( ValidationFlag.Flags.Alpha, false );
        }
        /// <summary>
        /// Blinks the specified canvas group, until it reaches the specified target value the specified amount of /repeat/.
        /// </summary>
        /// <param name="blinkDuration"> The duration of each blink </param>
        public static void Blink( this CanvasGroup group, float target, float blinkDuration, int repeat = 1 )
        {
            if( !group )
                return;
            group.BlinkCo( target, blinkDuration, repeat ).Start();
        }
        /// <summary>
        /// Blinks the specified canvas group, until it reaches the specified target value the specified amount of /repeat/.
        /// </summary>
        /// <param name="blinkDuration"> The duration of each blink </param>
        public static IEnumerator BlinkCo( this CanvasGroup group, float target, float blinkDuration, int repeat = 1 )
        {
            if( !group )
                yield break;
            float iniAlpha = group.alpha;
            for( int i=0; i<repeat + 1; i++ )
            {
                yield return group.AlphaTo( target, blinkDuration * 0.5f ).Start();
                yield return group.AlphaTo( iniAlpha, blinkDuration * 0.5f ).Start();
            }
        }   
        /// <summary>
        /// Fades the specified canvas groups, until they reach the specified target value.
        /// </summary>
        public static void Blink( this IList<CanvasGroup> groups, float target, float blinkDuration, int repeat = 1 )
        {
            if( groups == null )
                return;
            groups.BlinkCo( target, blinkDuration, repeat ).Run();
        }
        /// <summary>
        /// Fades the specified canvas groups, until they reach the specified target value.
        /// </summary>
        public static IEnumerator<float> BlinkCo( this IList<CanvasGroup> groups, float target, float blinkDuration, int repeat = 1 )
        {
            if( groups == null )
                yield break;
            for( int i=0; i<groups.Count; i++ )
            {
                groups[i].BlinkCo( target, blinkDuration, repeat ).Start();
            }
            yield return Timing.WaitForSeconds( blinkDuration * ( repeat + 1 ) );
        }
        public static void SetInteractableFor( this CanvasGroup group, bool interactable, float duration )
        {
            group.SetInteractableForCo( interactable, duration ).Start();
        }
        public static IEnumerator SetInteractableForCo( this CanvasGroup group, bool interactable, float duration )
        {
            if( !group )
                yield break;
            duration = duration.Clamp( 0f, float.MaxValue );
            group.interactable = interactable;
            yield return new WaitForSeconds( duration );
            group.interactable = !interactable;
        }
        public static void SetInteractable( this CanvasGroup group, bool interactable, float delay )
        {
            group.SetInteractableCo( interactable, delay ).Start();
        }
        public static IEnumerator SetInteractableCo( this CanvasGroup group, bool interactable, float delay )
        {
            if( !group )
                yield break;
            delay = delay.Clamp( 0f, float.MaxValue );
            yield return new WaitForSeconds( delay );
            group.interactable = interactable;
        }
        public static void SetInteractable( this CanvasGroup group, bool interactable, AudioClip delay )
        {
            group.SetInteractableCo( interactable, delay ).Start();
        }
        public static IEnumerator SetInteractableCo( this CanvasGroup group, bool interactable, AudioClip delay )
        {
            yield return group.SetInteractableCo( interactable, delay.length ).Start();
        }
    }

}