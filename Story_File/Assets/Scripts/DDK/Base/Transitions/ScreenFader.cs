using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.Base.Fx.Transitions
{
	/// <summary>
	/// Attach to an object to handle automatic Screen Fading.
	/// </summary>
	public class ScreenFader : MonoBehaviourExt
	{		
		public bool onEnable;
		[ShowIfAttribute( "_OnEnable", 1 )]
		public float delay;
		public FadeSettings settings = new FadeSettings();
		[Header("Events")]
		public ComposedEvent onFadedOut = new ComposedEvent();
		public ComposedEvent onFadedIn = new ComposedEvent();


		protected bool _OnEnable()
		{
			return onEnable;
		}


		void OnEnable () {

			if( onEnable )
			{
				FadeOutInCo().Start( delay );
			}
		}


		public void FadeOut()
		{
			FadeOutCo().Start();
		}
		public void FadeIn()
		{
			FadeInCo().Start();
		}
		public void FadeOutIn()
		{
			FadeOutInCo().Start();
        }


		public IEnumerator FadeOutCo()
		{
			AutoFade.FadeOut( settings );
			yield return new WaitForSeconds( settings.fadeOutTime );
			onFadedOut.Invoke();
		}
		public IEnumerator FadeInCo()
		{
			AutoFade.FadeIn( settings );
			yield return new WaitForSeconds( settings.fadeInTime );
			onFadedIn.Invoke();
        }
		public IEnumerator FadeOutInCo()
		{
			AutoFade.FadeOutIn( settings );
			yield return new WaitForSeconds( settings.fadeOutTime );
			onFadedOut.Invoke();
			yield return new WaitForSeconds( settings.fadeInTime );
			onFadedIn.Invoke();
        }
    }
}
