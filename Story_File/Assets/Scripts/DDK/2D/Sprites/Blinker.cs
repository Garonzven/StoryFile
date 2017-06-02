//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.UI;
using DDK.Base.Managers;


namespace DDK._2D.Sprites 
{
	/// <summary>
	/// Attach to a SpriteRenderer, Text or Image GameObject to make it flash/flicker/blink.
	/// </summary>
	public class Blinker : MonoBehaviour 
    {		
		public float initialDelay;
		[Tooltip("If true, the /initialDelay will be set to 0 after the last flash")]
		[IndentAttribute(1)]
		public bool resetInitialDelayAfterLastFlash;
		[Tooltip("Name of the SfxManager's Audio Source that will be played after the initial delay")]
		public string playAfterInitialDelay = "Effects";
		public float fadeOutValue = 0.5f;
		public float fadeOutTime = 0.5f;
		public bool doNotFadeInFirstTime = true;
		public float fadeInTime = 0.5f;
		[Tooltip("After fading out and in")]
		public float pauseDuration;
		/// <summary>
		/// If 0, it won't stop.
		/// </summary>
		[Tooltip("If 0, it won't stop.")]
		public int numberOfFlashes = 3;
		public bool affectChildren;
		/// <summary>
		/// If false, the object will be deactivated or destroyed in case it is a prefab.
		/// </summary>
		[Tooltip("If false, the object will be deactivated or destroyed in case it is a prefab.")]
		[Header("Events")]
		public bool doNotFadeOutOnLastFlash = true;
		public bool deactivateThisCompAfterLastFlash = true;
		


		protected Color color;
		private bool _fading;
		internal bool m_Fading
        {
			get{ return _fading; }
			private set{ _fading = value; }
		}
		private bool _fadingOut;
		internal bool m_FadingOut
        {
			get{ return _fadingOut; }
			private set{ _fadingOut = value; }
		}
		private bool _fadingIn;
		internal bool m_FadingIn
        {
			get{ return _fadingIn; }
			private set{ _fadingIn = value; }
		}
		protected float t;
		protected AudioSource _playAfterInitialDelay;
		internal AudioSource m_playAfterInitialDelay
        {
			get
            {
				_playAfterInitialDelay = SfxManager.GetSource( playAfterInitialDelay );
				return _playAfterInitialDelay;
			}
			set
            {
				if( value )
				{
					_playAfterInitialDelay = value;
				}
				else Debug.LogWarning("The value you want to set is null");
			}
		}			
		
		
		void OnEnable()
		{
			Init( gameObject );

			if( affectChildren )
			{
				var children = gameObject.GetChildren( true );
				for( int i=0; i<children.Length; i++ )
				{
					Init ( children[i] );
				}
			}
		}				


		#region FUNCTIONS
		protected void Init( GameObject obj )
		{
			StartCoroutine( DoFade( numberOfFlashes, obj ) );
		}		
        /// <summary>
        /// Executes a fade in and fade out as specified with the public variables.
        /// </summary>
        /// <param name="repeat">Amount of times that the fade should be repeated.</param>
        /// <param name="obj">gameObject that must have a SpriteRenderer in order to be faded.</param>
		protected IEnumerator DoFade( int repeat, GameObject obj )
		{
			yield return new WaitForSeconds( initialDelay );
			
            SpriteRenderer sRenderer = obj.GetComponent<SpriteRenderer>();
			Graphic graphic = null;
			if( !sRenderer )
			{
				graphic = obj.GetComponent<Graphic>();
				color = graphic.color;
			}
			else color = sRenderer.color;
			var fadeInValue = color.a;
		    
            if(!string.IsNullOrEmpty(playAfterInitialDelay))	
			    m_playAfterInitialDelay.PlaySound( true );
			
			if( repeat == 0 )
			{
				repeat = int.MaxValue;
			}
			for( int i=0; i < repeat; i++ )
			{
				m_Fading = true;
				if( i == 0 )
				{
					if( !doNotFadeInFirstTime)
					{
						yield return StartCoroutine( DoFadeIn(graphic, sRenderer, fadeInValue) );
					}
				}
				else yield return StartCoroutine( DoFadeIn(graphic, sRenderer, fadeInValue) );
				
				yield return new WaitForSeconds( pauseDuration );
				
				if( i == repeat - 1 )
				{
					if( !doNotFadeOutOnLastFlash )
					{
						yield return StartCoroutine( DoFadeOut(graphic, sRenderer, fadeInValue) );
						m_Fading = false;
						ResetInitialDelay();
						gameObject.SetActiveInHierarchy( false );
					}
					else if( deactivateThisCompAfterLastFlash )
					{
						m_Fading = false;
						ResetInitialDelay();
						enabled = false;
					}
				}
				else
				{
					yield return StartCoroutine( DoFadeOut(graphic, sRenderer, fadeInValue) );
					m_Fading = false;
				}
			}
		}		
        /// <summary>
        /// Fades in the specified /graphic/ or /sRenderer/ if the graphic is null.
        /// </summary>
        /// <param name="graphic">The Graphic to animate/fade in.</param>
        /// <param name="sRenderer">The SpriteRenderer to fade in instead if the Graphic is null.</param>
        /// <param name="fadeInValue">The target fade in alpha value.</param>
		public IEnumerator DoFadeIn( Graphic graphic, SpriteRenderer sRenderer, float fadeInValue )
		{
			m_FadingIn = true;
			t = fadeOutValue;
			while ( t < fadeInValue )
			{
				yield return null;
				t = Mathf.Clamp01(t + Time.deltaTime / fadeInTime);
				if( graphic )
				{
					graphic.color = new Color( color.r, color.g, color.b, t );
				}
				else sRenderer.color = new Color( color.r, color.g, color.b, t );
			}
			m_FadingIn = false;
		}		
        /// <summary>
        /// Fades out the specified /graphic/ or /sRenderer/ if the graphic is null.
        /// </summary>
        /// <param name="graphic">The Graphic to animate/fade out.</param>
        /// <param name="sRenderer">The SpriteRenderer to fade out instead if the Graphic is null.</param>
        /// <param name="fadeOutValue">The target fade out alpha value.</param>
		public IEnumerator DoFadeOut( Graphic graphic, SpriteRenderer sRenderer, float fadeOutValue )
		{
			m_FadingOut = true;
			t = fadeOutValue;
			while ( t > fadeOutValue )
			{
				yield return null;
				t = Mathf.Clamp01(t - Time.deltaTime / fadeOutTime);
				if( graphic )
				{
					graphic.color = new Color( color.r, color.g, color.b, t );
				}
				else sRenderer.color = new Color( color.r, color.g, color.b, t );
			}
			m_FadingOut = false;
		}	
        /// <summary>
        /// Resets the /initialDelay/ by setting it to cero (0) only if /resetInitialDelayAfterLastFlash/ is true.
        /// </summary>
		public void ResetInitialDelay()
		{
			if( resetInitialDelayAfterLastFlash )
			{
				initialDelay = 0f;
			}
		}
		#endregion		
	}	
}