//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.UI;
using System.Collections.Generic;
using DDK.Base.Fx.Transitions;
using DDK.Base.Classes;
using DDK.Base.Statics;


namespace DDK.Base.UI {

	/// <summary>
	/// Use CanvasGroup and Fader.cs instead. Add to a UI gameobject to make it fadeable.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent( typeof( RectTransform ) )]
	[System.Obsolete("Use CanvasGroup, IgnoreParentGroup, and Fader.cs instead")]
	public class FadeImages : MonoBehaviour {

		[Tooltip("This Describes this component's current usage to easily difference it from other FadeImages")]
		public string description;
		public float delay;
		public string[] excludeFromFade = new string[0];
		public bool fadeAllImages;
		public bool fadeThisImage = true;
		public bool fadeChildrenImages;
		public bool includeSubChildren = false;
		public float fadeInTime = 0.5f;
		public float fadeInTarget = 1f;
		public GameObject[] activateAfterFadeIn;
		public MonoBehaviour[] enableAfterFadeIn;
		public float pauseDuration;
		public bool fadeInOnly;
		public bool fadeOutOnly;
		public float fadeOutTime = 0.5f;
		public float fadeOutTarget = 0f;
		public GameObject[] activateAfterFadeOut;
		public MonoBehaviour[] enableAfterFadeOut;
		public bool greyScaleAlso;
		[Space(10f)]
		public bool reloadSceneAfterFadeOut;
		public FadeSettings fade;
		[Space(10f)]
		public bool destroyWhenFadedOut;
		public bool deactivateWhenFadedOut;
		public bool disableCompWhenFadedOut;
		public bool disableAnimatorBeforeFade = true;
		public bool enableAnimatorAfterFade = true;		
		

		
		protected List<Image> imgs = new List<Image>();
		protected List<Text> txts = new List<Text>();
		/// <summary>
		/// They prevent image animation from executing correctly, so they must be disabled until fade is over.
		/// </summary>
		protected List<Animator> animators = new List<Animator>();
		protected Color[] colors, txtColors;
		private bool _fading;
		internal bool fading{
			get{
				return _fading;
			}
			private set{
				_fading = value;
			}
		}
		private bool _fadingOut;
		internal bool fadingOut{
			get{
				return _fadingOut;
			}
			private set{
				_fadingOut = value;
			}
		}
		private bool _fadingIn;
		internal bool fadingIn{
			get{
				return _fadingIn;
			}
			private set{
				_fadingIn = value;
			}
		}
			
		
		
		#if UNITY_EDITOR
		void OnGUI()
		{
			if( !Application.isPlaying )
			{
				fadeAllImages = !(!fadeAllImages | fadeChildrenImages | fadeThisImage);
				fadeChildrenImages = !(fadeAllImages | !fadeChildrenImages);
				fadeThisImage = !(fadeAllImages | !fadeThisImage);

				fadeInOnly = !( !fadeInOnly | fadeOutOnly );
				fadeOutOnly = !( fadeInOnly | !fadeOutOnly );

				destroyWhenFadedOut = !( !destroyWhenFadedOut | deactivateWhenFadedOut );
				deactivateWhenFadedOut = !( destroyWhenFadedOut | !deactivateWhenFadedOut );
			}
		}
		#endif
		
		void OnEnable()
		{
			if( Application.isPlaying )
			{
				if( fadeThisImage )
				{
					imgs.Add( GetComponent<Image>() );
				}
				else if( fadeAllImages )
				{
					imgs.AddRange( GameObject.FindObjectsOfType<Image>() );
				}
				if( fadeChildrenImages )
				{
					imgs.AddRange( gameObject.GetCompsInChildren<Image>( includeSubChildren ) );
				}
				if( imgs != null )
				{
					colors = new Color[imgs.Count];
					for( int i=0; i<imgs.Count; i++ )
					{
						/*if( imgs[i] == null )
						{
							Debug.LogError ( "If you are trying to fade a Text, make sure it is a child of a game object with an Image component." );
						}*/
						if( excludeFromFade.Contains<string>( imgs[i].name ) )
						{
							imgs.RemoveAt( i );
							i--;
							continue;
						}
						if( imgs[i].GetComponentInChildren<Text>() )
						{
							txts.AddRange( imgs[i].GetComponentsInChildren<Text>() );
						}
						colors[i] = imgs[i].color;
						//DISABLE ANIMATORS
						animators.Add( imgs[i].GetComponent<Animator>() );
						if( animators[i] != null )
						{
							if( disableAnimatorBeforeFade )
							{
								animators[i].enabled = false;
							}
						}
					}
					txtColors = new Color[txts.Count];
					for( int i=0; i<txts.Count; i++ )
					{
						txtColors[i] = txts[i].color;
					}
					for( int i=0; i<imgs.Count; i++ )
					{
						StartCoroutine( DoFade( imgs[i], null, colors[i], Color.black ) );
					}
					for( int i=0; i<txts.Count; i++ )
					{
						StartCoroutine( DoFade( null, txts[i], Color.black, txtColors[i] ) );
					}
				}
			}
		}
		
				
		
		private bool _singleIn = false;
		private bool _singleOut = false;
		private bool _willBeDestroyed = false;
		
		protected IEnumerator DoFade( Image img, Text txt, Color color, Color txtColor )
		{
			yield return new WaitForSeconds( delay );
			if( !( img == null && txt == null ) )
			{
				float t = 0f;
				
				if( !fadeOutOnly )//FADE IN
				{
					if( img )
					{
						t = img.color.a;
					}
					else t = txt.color.a;
					fading = true;
					fadingIn = true;
					while ( t < fadeInTarget )
					{
						yield return null;
						t = Mathf.Clamp01(t + Time.deltaTime / fadeInTime);
						if( img ) {
							if( greyScaleAlso ) {
								img.color = new Color( t, t, t, t );
							}
							else img.color = new Color( color.r, color.g, color.b, t );
						}
						else if( greyScaleAlso ) {
							txt.color = new Color( t, t, t, t );
						}
						else txt.color = new Color( txtColor.r, txtColor.g, txtColor.b, t );
					}
					
					if( !_singleIn )
					{
						_singleIn = true;
						activateAfterFadeIn.SetActiveInHierarchy();
						enableAfterFadeIn.SetEnabled();
					}
					
					fadingIn = false;
					fading = false;
				}
				
				bool didFadeOut = true;
				if( !fadeInOnly )// FADE OUT
				{
					if( img )
					{
						t = img.color.a;
					}
					else t = txt.color.a;
					if( t == 0 )//PREVENT IMAGES WITH 0 ALPHA TO TRY THE FADE OUT, THAT CAUSES THE FADE OUT WITH MULTIPLE OBJECTS TO WORK INCORRECTLY
					{
						didFadeOut = false;
						enableAnimatorAfterFade = false;
					}
					yield return new WaitForSeconds( pauseDuration );
					
					fading = true;
					fadingOut = true;
					float equalizer = t;//this makes the fade to end at the same time if multiple images/texts have different initial alpha values
					while ( t > fadeOutTarget )
					{
						yield return null;
						t = Mathf.Clamp01(t - Time.deltaTime / ( fadeOutTime / (equalizer + Time.deltaTime) ) );
						if( img ) {
							if( greyScaleAlso ) {
								img.color = new Color( t, t, t, t );
							}
							else img.color = new Color( color.r, color.g, color.b, t );
						}
						else if( greyScaleAlso ) {
							txt.color = new Color( t, t, t, t );
						}
						else txt.color = new Color( txtColor.r, txtColor.g, txtColor.b, t );
					}
					
					fadingOut = false;
					fading = false;
					
					if( !_singleOut )
					{
						_singleOut = true;
						activateAfterFadeOut.SetActiveInHierarchy();
						enableAfterFadeOut.SetEnabled();
						if( reloadSceneAfterFadeOut )
						{
							AutoFade.LoadLevel( "", fade.fadeOutTime, fade.fadeInTime, fade.fadeColor );
						}
					}
					if( destroyWhenFadedOut && didFadeOut )
					{
						if( !_willBeDestroyed )
						{
							_willBeDestroyed = true;
							gameObject.Destroy();
						}
					}
					else if ( deactivateWhenFadedOut && didFadeOut ) 
					{
						gameObject.SetActiveInHierarchy(false);
					}
				}
				
				//ENABLE ANIMATORS
				if( enableAnimatorAfterFade )
				{
					animators.Enable();
				}
				
				if( disableCompWhenFadedOut && didFadeOut )
				{
					enabled = false;
				}
			}
		}
		
		
	}
	
	
}
