//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using UnityEngine.UI;


namespace DDK.Base.Animations
{
    /// <summary>
    /// Allows animating a Renderer's material(s), RendererGroup, CanvasGroup or Graphic's alpha.
    /// </summary>
    public class AnimateAlpha : AnimateBase<float>
    {
        #region ANIMATIONS
        /// <summary>
        /// Animate the /targetObj/'s alpha until it reaches the specified /targetAlpha/ in the specified /animDuration/. 
        /// The gameObject must have a Renderer, RendererGroup, Graphic or CanvasGroup component.
        /// </summary>
        public override void Animate( GameObject targetObj, float targetAlpha, float animDuration )
        {
            if( targetObj == null )
                return;
            targetObj.AnimAlphaWithMultiplier( targetAlpha, animDuration, multiplier, toggleAnimator ).Start( delay );
        }
        /// <summary>
        /// Animate the /targetObjs/ alpha until they reach the specified /targetAlpha/ in the specified /animDuration/
        /// after the specified /animDelay/. The gameObjects must have a Renderer, RendererGroup, Graphic or 
        /// CanvasGroup component.
        /// </summary>
        public override void Animate( float animDelay, float targetAlpha, float animDuration )
        {
            targetObjs.AnimAlphaWithMultiplier( targetAlpha, animDuration, multiplier, toggleAnimator ).Start( animDelay );
        }
        /// <summary>
        /// Animate the /targetObjs/ alpha until they reach the target alpha, which is the current alpha multiplied by 
        /// the specified /valueMultiplier/, in the specified duration and after the specified /animDelay/. The 
        /// gameObjects must have a Renderer, RendererGroup, Graphic or CanvasGroup component.
        /// </summary>
        public override void AnimateMultiplyValue( float valueMultiplier )
        {
            float targetAlpha = 1f;
            Graphic g;//caching
            Renderer ren;//caching
            for( int i=0; i<targetObjs.Length; i++ )
            {
                g = targetObjs[ i ].GetComponent<Graphic>();
                if( !g )
                {
                    ren = targetObjs[ i ].GetComponent<Renderer>();
                    targetAlpha = ren.GetTrueMaterial().color.a;
                }
                else targetAlpha = g.color.a;
                targetAlpha *= valueMultiplier;
                Animate( delay, targetAlpha, duration );
            }
        }
        /// <summary>
        /// Stops animating the alpha value.
        /// </summary>
        public override void StopAnimating()
        {
            _ValidationFlags.SetFlagged( ValidationFlag.Flags.Alpha, false );
        }
        /// <summary>
        /// Stops animating the alpha value of the specified /target/ gameObject.
        /// </summary>
        public override void StopAnimating( GameObject targetObj )
        {
            targetObj.SetFlagged( ValidationFlag.Flags.Alpha, false );
        }
        #endregion
    }
}
