//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using System.Collections.Generic;


namespace DDK.Base.Fx.Transitions 
{	
	/// <summary>
	/// Inherit to allow fading renderers.
	/// </summary>
	[ExecuteInEditMode]
	public abstract class FadeRenderers<T> : MonoBehaviour where T : Renderer 
    {
		[Tooltip("If true, the desired fade effect will be reversed on disable.")]
		public bool reverseOnDisable;
		public float delay;
		public string[] excludeFromFade = new string[0];
		public bool fadeAllRenderers;
		public bool fadeChildrenRenderers;
		public bool fadeThisRenderer = true;
        [Space(10f)]
		public float fadeInTime = 0.5f;
        [IndentAttribute(1)]
		public float fadeInTarget = 1f;
		public float pauseDuration;
		public bool fadeInOnly;
		public bool fadeOutOnly;
		public float fadeOutTime = 0.5f;
        [IndentAttribute(1)]
		public float fadeOutTarget = 0f;
        [Header( "Events" )]
		public bool destroyWhenFadedOut;
		public bool deactivateWhenFadedOut;
		public bool disableCompWhenFadedOut;		

		

		protected List<T> _renderers = new List<T>();
		protected Color[] _colors;
        internal bool _Fading { get; private set; }
        internal bool _FadingOut { get; private set; }
        internal bool _FadingIn { get; private set; }
		protected bool _reversed;
		
		
		
		#if UNITY_EDITOR
		void OnGUI()
		{
			if( !Application.isPlaying )
			{
				fadeAllRenderers = !(!fadeAllRenderers | fadeChildrenRenderers | fadeThisRenderer);
				fadeChildrenRenderers = !(fadeAllRenderers | !fadeChildrenRenderers | fadeThisRenderer);
				fadeThisRenderer = !(fadeAllRenderers | fadeChildrenRenderers | !fadeThisRenderer);
				
				fadeInOnly = !( !fadeInOnly | fadeOutOnly );
				fadeOutOnly = !( fadeInOnly | !fadeOutOnly );
				
				destroyWhenFadedOut = !( !destroyWhenFadedOut | deactivateWhenFadedOut );
				deactivateWhenFadedOut = !( destroyWhenFadedOut | !deactivateWhenFadedOut );
			}
		}
		#endif
		
		void OnEnable()
		{
			if( !Application.isPlaying )
				return;
			if( _reversed && reverseOnDisable )
			{
				fadeInOnly = !fadeInOnly;
				fadeOutOnly = !fadeOutOnly;
				_reversed = false;
			}

			if( fadeThisRenderer )
			{
				_renderers.Add( GetComponent<T>() );
			}
			else if( fadeAllRenderers )
			{
				_renderers.AddRange( FindObjectsOfType<T>() );
			}
			else if( fadeChildrenRenderers )
			{
				_renderers.AddRange( GetComponentsInChildren<T>() );
				if( GetComponent<T>() )
				{
					_renderers.Remove( GetComponent<T>() );
				}
			}
			if( _renderers != null )
			{
				_colors = new Color[_renderers.Count];
				for( int i=0; i<_renderers.Count; i++ )
				{
					if( excludeFromFade.Contains( _renderers[i].name ) )
					{
						_renderers.RemoveAt( i );
						i--;
						continue;
					}
                    _colors[i] = _renderers[i].material.color;
				}
				for( int i=0; i<_renderers.Count; i++ )
				{
                    _DoFade( _renderers[i].material, _colors[i] ).Start();
				}
			}
		}		
		void OnDisable()
		{
			if( !reverseOnDisable )
				return;
			fadeInOnly = !fadeInOnly;
			fadeOutOnly = !fadeOutOnly;
			OnEnable ();
			_reversed = true;
		}

		
		
        /// <summary>
        /// Fade the /pRenderer/ into the specified color and then back out.
        /// </summary>
        /// <param name="pRenderer">Renderer to fade.</param>
        /// <param name="color">Color to fade into.</param>
		protected IEnumerator _DoFade( Material pMaterial, Color color )
		{
            if( delay > 0f )
            {
                yield return new WaitForSeconds( delay );
            }
			float t = 0f;
			
            //FADE IN TO COLOR
			if( !fadeOutOnly )
			{
				if( pMaterial )
				{
                    t = pMaterial.color.a;
				}
				_Fading = true;
				_FadingIn = true;
				while ( t < fadeInTarget )
                {
                    yield return null;
                    t = Mathf.Clamp01(t + Time.deltaTime / fadeInTime);
                    if( pMaterial ) 
                    {
                        pMaterial.color = new Color( color.r, color.g, color.b, t );
                    }
                }
                _FadingIn = false;
                _Fading = false;
            }
            //FADE OUT FROM COLOR
            if( !fadeInOnly )
            {
                if( pMaterial )
                {
                    t = pMaterial.color.a;
                }
                yield return new WaitForSeconds( pauseDuration );
                
                _Fading = true;
                _FadingOut = true;
                while ( t > fadeOutTarget )
                {
                    yield return null;
                    t = Mathf.Clamp01(t - Time.deltaTime / fadeOutTime );
                    if( pMaterial ) 
                    {
                        pMaterial.color = new Color( color.r, color.g, color.b, t );
                    }
                }
                _FadingOut = false;
                _Fading = false;
                if( destroyWhenFadedOut )
                {
                    gameObject.Destroy();
                }
                else if ( deactivateWhenFadedOut ) 
                {
                    gameObject.SetActiveInHierarchy(false);
                }
            }
            
            if( this && disableCompWhenFadedOut )
            {
                enabled = false;
            }
        }		
    }	
}
