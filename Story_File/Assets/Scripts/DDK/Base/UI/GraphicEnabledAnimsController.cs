//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using DDK.Base.Fx.Transitions;


namespace DDK.Base.UI 
{
	/// <summary>
	/// Animates this Graphic. This has a editor class.
	/// </summary>
	[RequireComponent( typeof( Graphic ) )]
    public class GraphicEnabledAnimsController : MonoBehaviour //This has a editor class.
    {		
		/// <summary>
		/// The graphic's color when this component is enabled
		/// </summary>
        [Tooltip("The graphic's color when this component is enabled")]
		public Color enabledColor = Color.yellow;
		public bool avoidAlpha = false;
		[Space(5f)]
		public bool disabledColorIsSameAsOriginal = false;
			//FALSE
			public Color disabledColor = Color.black;
		[Space(5f)]
		[Tooltip("If true, when this Graphic is being enabled (its animation is playing) its siblings will have the specified " +
			"applied")]
		public bool applyToSiblingsOnEnableAnim;
			//TRUE
			public Color siblingsColor;
			public bool includeSiblingsSubChildren = true;
			public bool includeInactive = false;
        /// <summary>
        /// The graphic's scale when this component is enabled
        /// </summary>
		[Space(5f)]
		public Vector3 enabledScl = new Vector3( 1f, 1f, 1f );
		public float animDuration = 0.4f;
		[Header("Events")]
		public bool destroyAfterAnimDone;
		public bool disableAfterAnimDone;
		[Tooltip("If true and next sibling has this component, it will be enabled")]
		public bool enableOnNextSiblingAfterDone;
		[SerializeField]
		protected ComposedEvent _onAnimationEnd = new ComposedEvent();
								
		
		protected Graphic _graphic;
		protected Vector3 _iniScl;
		protected bool _quitting;

		/// <summary>
		/// This delegate can be used by other classes. UnityEvents give some issues, so it's better to use this instead
		/// of ComposedEvent
		/// </summary>
		public delegate void OnAnimationEnd();
		/// <summary>
		/// This can be used by other classes. UnityEvents give some issues, so it's better to use this instead
		/// of ComposedEvent
		/// </summary>
		public OnAnimationEnd onAnimationEnd;			
		


		// Use this for initialization
		void Awake () 
        {			
			_graphic = GetComponent<Graphic>();
			_iniScl = transform.localScale;
		}
        void Start(){} //Allows enabling/disabling this component
		
		void OnEnable()
		{
			if( disabledColorIsSameAsOriginal )
			{
				disabledColor = _graphic.color;
			}
			StartOnEnableCoroutines();
		}		
		void OnDisable()
		{
			if( !_quitting && !AutoFade._Fading )
			{
				if( applyToSiblingsOnEnableAnim )
				{
					gameObject.SetSiblingGraphicsColor( enabledColor, includeSiblingsSubChildren, includeInactive);
				}
                _graphic.AnimColor( disabledColor, animDuration, avoidAlpha ).Run();
                _graphic.AnimScale( _iniScl, animDuration ).Run();
			}
		}		
		void OnApplicationQuit()
		{
			_quitting = true;
		}


		protected void StartOnEnableCoroutines()
		{ 
			_StartOnEnableCoroutines().Start();
		}
		private IEnumerator _StartOnEnableCoroutines()
		{
			if( applyToSiblingsOnEnableAnim )
			{
				gameObject.SetSiblingGraphicsColor( siblingsColor, includeSiblingsSubChildren, includeInactive);
			}
            _graphic.AnimColor( enabledColor, animDuration, avoidAlpha ).Run();
            _graphic.AnimScale( enabledScl, animDuration ).Run();

			var nextSibling = gameObject.GetNextSibling();
			yield return new WaitForSeconds( animDuration );

			if( enableOnNextSiblingAfterDone && nextSibling )
			{
			    var geac = nextSibling.GetComponent<GraphicEnabledAnimsController>();

			    if ( geac != null )
			    {
			        geac.enabled = true;
			    }
			}

			if( _onAnimationEnd != null )
			{
				_onAnimationEnd.Invoke();
			}
			if( onAnimationEnd != null )
			{
				onAnimationEnd.Invoke();
			}
			if( destroyAfterAnimDone )
			{
				if( this )
				{
					Destroy ( this );
				}
			}
			if( disableAfterAnimDone )
			{
				if( this )
				{
					enabled = false;
				}
			}
		}
	}	
}
