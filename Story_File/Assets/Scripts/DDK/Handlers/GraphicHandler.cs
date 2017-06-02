//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;
using DDK.Base.Classes;
#if USE_SVG_IMPORTER
using SVGImporter;
#endif


namespace DDK.Handlers {

	/// <summary>
	/// This allows handling a Graphic object/component. You can use this class with Events Calls.
	/// </summary>
	public class GraphicHandler : MonoBehaviourExt {

		[HelpBoxAttribute]
		public string msg = "If disabled, no actions will be invoked";
		public SearchableGraphic graphic = new SearchableGraphic();
		[Tooltip("This is used to split the arguments of some functions")]
		public char argsSeparator = ':';
		public float animsDuration = 1f;


		private Graphic _graphic;
		public Graphic m_Graphic {
			get{
				if( !_graphic )
				{
					_graphic = graphic.m_object;
					if( !_graphic )
						_graphic = GetComponentInChildren<Graphic>();
				}
				return _graphic;
			}
		}


		void Start(){}//If this is removed the component can't be enabled/disabled


		public void SetAnimsDuration( float duration )
		{
			if( !enabled )
				return;
			if( duration <= 0f )
				return;
			animsDuration = duration;
		}
		public void AnimAlpha( float target )
		{
			AnimAlpha( target, animsDuration );
		}
		public void AnimAlpha( string target_duration = "1f:1f" )
		{
			if( !enabled )
				return;
			if( string.IsNullOrEmpty( target_duration ) )
				return;
			string[] args = target_duration.Split( argsSeparator );
			if( args.Length != 2 )
			{
				Debug.LogWarning ("Wrong arguments..");
				return;
			}
			m_Graphic.AnimAlpha( args[0].ToFloat(), args[1].ToFloat() );
		}
		public void AnimAlpha( float target, float duration = 1f )
		{
			if( !enabled )
				return;
			m_Graphic.AnimAlpha( target, duration );
		}
		public void AnimAlpha( Graphic graphic, float target, float duration = 1f )
		{
			if( !enabled || !graphic )
				return;
			graphic.AnimAlpha( target, duration );
		}
		public void SetColor( Color target )
		{
			if( !enabled )
				return;
			m_Graphic.color = target;
		}
		public void AnimColor( Color target )
		{
			if( !enabled )
				return;
            m_Graphic.AnimColor( target, animsDuration ).Run();
		}
		public void SetColor( string rgba = "255:255:255:255" )
		{
			if( !enabled )
				return;
			Color color = Color.white;
			if( !GetRGBA( rgba, out color ) )
				return;
			m_Graphic.color = color;
		}
		public void SetColorHex( string hex = "#FFFFFFFF" )
		{
			if( !enabled )
				return;
            if( !hex.StartsWith( "#" ) )
            {
                hex = hex.Insert( 0, "#" );
            }
			Color color = Color.white;
			if( !ColorUtility.TryParseHtmlString( hex, out color ) )
			{
				Debug.LogWarning ("Couldn't parse html string parameter, ensure it's in the correct format: #001122FF");
                return;
            }
            m_Graphic.color = color;
        }
		public void SetAlpha( float alpha )
		{
			if( !enabled )
				return;
			m_Graphic.SetAlpha( alpha );
		}
		public void AnimColor( string rgba = "255:255:255:255" )
		{
			if( !enabled )
				return;
			Color color = Color.white;
			if( !GetRGBA( rgba, out color ) )
				return;
            m_Graphic.AnimColor( color, animsDuration ).Run();
		}
		public void AnimColorToHex( string hex = "#FFFFFFFF" )
		{
			if( !enabled )
				return;
            if( !hex.StartsWith( "#" ) )
            {
                hex = hex.Insert( 0, "#" );
            }
			Color color = Color.white;
			if( !ColorUtility.TryParseHtmlString( hex, out color ) )
			{
				Debug.LogWarning ("Couldn't parse html string parameter, ensure it's in the correct format: #001122FF");
				return;
			}
            m_Graphic.AnimColor( color, animsDuration ).Run();
		}
		public void CopyColorFrom( Graphic target )
		{
			if( !enabled || !target )
				return;
			SetColor( target.color );
		}
		public void AnimColorTowards( Graphic target )
		{
			if( !enabled || !target )
				return;
			AnimColor( target.color );
        }


        public IEnumerator EnableGraphic( bool enable, float delay = 0f )
        {
            if( !enabled || !m_Graphic )
                yield break;
            if( delay > 0f )
            {
                yield return new WaitForSeconds( delay );
            }
            m_Graphic.enabled = enable;
        }


        #if USE_SVG_IMPORTER
        /// <summary>
        /// Animate the specified animator's frames.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public void Animate( bool reverse )
        {
            graphic.svgFrameAnimator.Animate( animsDuration, reverse );
        }
        /// <summary>
        /// Animate the specified animator's frames. If it is running an animation that one is stopped and this one played.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public void AnimateForced( bool reverse )
        {
            graphic.svgFrameAnimator.AnimateForced( animsDuration, reverse );
        }
        /// <summary>
        /// If /loopCount/ is cero (0) the animation will loop indefinitely, if this is desired and you want to stop the 
        /// animation later, just destroy the ValidationFlag component attached to the animator's gameObject, or set its 
        /// /Frame/ flag to false.
        /// </summary>
        /// <param name="loopCount">The amount of times the animation will go forth and back.</param>
        public void AnimateLoop( int loopCount )
        {
            graphic.svgFrameAnimator.AnimateLoop( animsDuration, loopCount );
        }
        #region MULTI PARAM FUNCTIONS
        /// <summary>
        /// Animate the specified animator's frames.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public void Animate( float duration, bool reverse = false )
        {
            graphic.svgFrameAnimator.Animate( duration, reverse );
        }
        /// <summary>
        /// Animate the specified animator's frames. If it is running an animation that one is stopped and this one played.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public void AnimateForced( float duration, bool reverse = false )
        {
            graphic.svgFrameAnimator.AnimateForced( duration, reverse );
        }
        /// <summary>
        /// Animate the specified animator's frames.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public IEnumerator AnimateCo( float duration, bool reverse = false )
        {
            yield return graphic.svgFrameAnimator.AnimateCo( duration, reverse ).Start();
        }
        /// <summary>
        /// Animate the specified animator's frames. If it is running an animation that one is stopped and this one played.
        /// </summary>
        /// <param name="reverse">If set to <c>true</c> the animation will play backwards.</param>
        public IEnumerable AnimateForcedCo( float duration, bool reverse = false )
        {
            yield return graphic.svgFrameAnimator.AnimateForcedCo( duration, reverse ).Start();
        }
        /// <summary>
        /// If /loopCount/ is cero (0) the animation will loop indefinitely, if this is desired and you want to stop the 
        /// animation later, just destroy the ValidationFlag component attached to the animator's gameObject, or set its 
        /// /Frame/ flag to false.
        /// </summary>
        /// <param name="loopCount">The amount of times the animation will go forth and back.</param>
        public void AnimateLoop( float duration, int loopCount = 0 )
        {
            graphic.svgFrameAnimator.AnimateLoop( duration, loopCount );
        }
        /// <summary>
        /// If /loopCount/ is cero (0) the animation will loop indefinitely, if this is desired and you want to stop the 
        /// animation later, just destroy the ValidationFlag component attached to the animator's gameObject, or set its 
        /// /Frame/ flag to false. NOTE: This will yield until the animation is over.
        /// </summary>
        /// <param name="loopCount">The amount of times the animation will go forth and back.</param>
        public IEnumerator AnimateLoopCo( float duration, int loopCount = 0 )
        {
            yield return graphic.svgFrameAnimator.AnimateLoopCo( duration, loopCount ).Start();
        }
        #endregion
        #endif


		protected bool GetRGBA( string rgba, out Color color )
		{
			color = rgba.GetFromRGBA255( argsSeparator, gameObject );
			return true;
		}
	}

}