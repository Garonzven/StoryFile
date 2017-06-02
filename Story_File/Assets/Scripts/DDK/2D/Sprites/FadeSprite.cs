//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.UI;


namespace DDK._2D.Sprites 
{
    /// <summary>
    /// You can also use Fader.cs which has more functionality. Add to a gameobject to make it fadeable, this script 
    /// uses SpriteRenderer.
    /// </summary>
    /// <see cref="Fader.cs"/>
	[ExecuteInEditMode]
	public class FadeSprite : MonoBehaviour 
    {		
		public float delay;
		public float fadeInTime = 0.5f;
		public float pauseDuration;
		public bool fadeInOnly;
		public float fadeOutTime = 0.5f;
		public bool destroyWhenFadedOut;
		public bool disableWhenFadedOut;		

		
		protected SpriteRenderer _renderer;
		protected Text _txt;
		protected Color _color;
        protected WaitForSeconds _delay;
        protected WaitForSeconds _pause;
		private bool _fading;
		private bool _fadingOut;
		private bool _fadingIn;

		internal bool _isFading
        {
			get{ return _fading; }
			private set { _fading = value; }
		}
		internal bool _isFadingOut
        {
			get{ return _fadingOut; }
			private set{ _fadingOut = value; }
		}
		internal bool _isFadingIn
        {
			get{ return _fadingIn; }
			private set{ _fadingIn = value; }
		}		
		
		
		#if UNITY_EDITOR
		void OnGUI()
		{
			_renderer = GetComponent<SpriteRenderer>();
			_txt = GetComponent<Text>();
			if( !_renderer && !_txt )
				gameObject.AddComponent<SpriteRenderer>();
		}
		#endif
		
        /// <summary>
        /// If App is running, this will assign/fix some references and start fading.
        /// </summary>
		void OnEnable()
		{
			if( !Application.isPlaying )
			    return;

            _delay = new WaitForSeconds( delay );
            _pause = new WaitForSeconds( pauseDuration );

            _renderer = GetComponent<SpriteRenderer>();
            _txt = GetComponent<Text>();
            _color = _renderer ? _renderer.color : _txt.color;
            StartCoroutine( _DoFade() );
		}		
		
		
        /// <summary>
        /// Fade the sprite in and/or out.
        /// </summary>
		protected IEnumerator _DoFade()
		{
            yield return _delay;
			float t = 0f;
			_isFading = true;
			_isFadingIn = true;
			while ( t<1f )
			{
				yield return null;
				t = Mathf.Clamp01( t + Time.deltaTime / fadeInTime );
				if( _renderer )
					_renderer.color = new Color( _color.r, _color.g, _color.b, t );
				else _txt.color = new Color( _color.r, _color.g, _color.b, t );
			}
			_isFadingIn = false;
			
			if( !fadeInOnly )
			{
                yield return _pause;
				
				_isFadingOut = true;
				while( t>0f )
				{
					yield return null;
					t = Mathf.Clamp01( t - Time.deltaTime / fadeOutTime );
					if( _renderer )
						_renderer.color = new Color( _color.r, _color.g, _color.b, t );
					else _txt.color = new Color( _color.r, _color.g, _color.b, t );
				}
				_isFadingOut = false;
				_isFading = false;
				if( disableWhenFadedOut )
					enabled = false;
				else if( destroyWhenFadedOut )
					gameObject.Destroy();
				else gameObject.SetActiveInHierarchy(false);
			}
		}		
	}
}