//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using System.Collections;


namespace DDK.Base.Components 
{
    /// <summary>
    /// This class was made similar to Unity's CanvasGroup.cs for the same purpose, to group multiple Renderers and 
    /// handle them all together.
    /// </summary>
	[ExecuteInEditMode]
	public class RendererGroup : MonoBehaviourExt 
    {		
        [InspectorButtonAttribute( "_ExtractRenderers", true )]
        public bool updateRenderers;
		[Range( 0f, 1f )]
		[SerializeField]
		protected float _alpha;
		[IndentAttribute(1)]
		public bool includeInactive;
		public bool useSharedMaterials;
		

		public float m_Alpha 
        {
			get{ return _alpha; }
			set
            {
				_alpha = value.Clamp01();
				_Set ();
			}
		}


		protected Renderer[] _renderers;
		

		// Use this for initialization
		void Start () 
        {
            _ExtractRenderers();
		}
		
#if UNITY_EDITOR
		// Update is called once per frame
		void Update () 
        {
			if( !Application.isPlaying )
			{
				if( _renderers == null )
                    _ExtractRenderers();					
				_Set ();
			}
		}
#endif

        /// <summary>
        /// Sets the alpha for this RendererGroup.
        /// </summary>
		protected virtual void _Set()
		{
			if( Application.isPlaying )
			{
				_renderers.SetAlpha( _alpha, useSharedMaterials );
			}
            else _renderers.SetAlpha( _alpha, true );
		}
        /// <summary>
        /// Extracts the Renderer components from the children and sets the /_renderers/ variable.
        /// </summary>
	    protected virtual void _ExtractRenderers()
	    {
            _renderers = GetComponentsInChildren<Renderer, IgnoreParentGroup>(includeInactive);
        }


        /// <summary>
        /// Animates the alpha value of the specified Renderer's material.
        /// </summary>
        /// <param name="targetAlpha">Target alpha.</param>
        /// <param name="duration">Animation's duration.</param>
        public void AnimateAlpha( float targetAlpha, float duration )
        {
            AnimateAlphaCo( targetAlpha, duration ).Start();
        }
        /// <summary>
        /// Animates the alpha value of the specified Renderer's material. this yields until the animation is done.
        /// </summary>
        /// <param name="targetAlpha">Target alpha.</param>
        /// <param name="duration">Animation's duration.</param>
        public IEnumerator AnimateAlphaCo( float targetAlpha, float duration )
        {
            if( m_Alpha.CloseTo( targetAlpha ) )
                yield break;
            float time = 0f;
            float initialAlpha = m_Alpha;
            while( time <= duration )
            {
                time += Time.deltaTime;
                m_Alpha = Mathf.Lerp( initialAlpha, targetAlpha, time / duration );
                yield return null;
            }
        }
        /// <summary>
        /// Fades the specified renderer group, until it reaches the specified target value.
        /// </summary>
        public IEnumerator AlphaTo( float target, float duration )
        {
            float iniAlpha = m_Alpha;
            float time = 0f;
            while( m_Alpha != target )
            {
                time += Time.unscaledDeltaTime;
                m_Alpha = Mathf.Lerp( iniAlpha, target, time / duration );
                yield return null;
            }
        }       
        /// <summary>
        /// Blinks the specified renderer group, until it reaches the specified target value the specified amount of /repeat/.
        /// </summary>
        /// <param name="blinkDuration"> The duration of each blink </param>
        public void Blink( float target, float blinkDuration, int repeat = 1 )
        {
            BlinkCo( target, blinkDuration, repeat ).Start();
        }
        /// <summary>
        /// Blinks the specified renderer group, until it reaches the specified target value the specified amount of /repeat/.
        /// </summary>
        /// <param name="blinkDuration"> The duration of each blink </param>
        public IEnumerator BlinkCo( float target, float blinkDuration, int repeat = 1 )
        {
            float iniAlpha = m_Alpha;
            for( int i=0; i<repeat + 1; i++ )
            {
                yield return AlphaTo( target, blinkDuration * 0.5f ).Start();
                yield return AlphaTo( iniAlpha, blinkDuration * 0.5f ).Start();
            }
        }   
	}
}
