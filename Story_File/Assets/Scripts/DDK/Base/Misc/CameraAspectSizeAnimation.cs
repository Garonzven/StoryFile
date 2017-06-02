using UnityEngine;
using DDK.Base.Extensions;


namespace DDK.Base.Misc
{
    /// <summary>
    /// This allows animating a Camera's aspect size.
    /// </summary>
	[ExecuteInEditMode]
	public class CameraAspectSizeAnimation : MonoBehaviourExt
	{		
		public float duration = 1f;
        [Tooltip("The camera that will be animated")]
		public CameraAspectSize from;
		public CameraAspectSize to;
		public bool onEnable;
		[ShowIfAttribute( "_OnEnable", 1 )]
		public bool reverseOnDisable;


		protected bool _OnEnable()
		{
			return onEnable;
		}


		protected Camera _camFrom;
		protected CameraAspectSize _from;


		void Awake()
		{
			_from = GetComponent<CameraAspectSize>();
			if( !_from )
				_from = from;
			_camFrom = _from.GetComponent<Camera>();
		}
#if UNITY_EDITOR
		void Update()
		{
			if( !from )
			{
				from = GetComponent<CameraAspectSize>();
			}
		}
#endif
		void OnEnable()
		{
			if( !onEnable || !from || !_camFrom )
				return;
			if( _camFrom.orthographic )
				AnimateOrthographicSize();
			else AnimateFieldOfViewTo();
		}
		void OnDisable()
		{
			if( !onEnable || !from || !reverseOnDisable || !_camFrom )
				return;
			if( _camFrom.orthographic )
				AnimateOrthographicSize( to, from );
			else AnimateFieldOfViewTo( to, from );
		}



		public void AnimateOrthographicSize()
		{
			AnimateOrthographicSize( to );
		}
		public void AnimateFieldOfViewTo()
		{
			AnimateFieldOfViewTo( to );
		}
		public void AnimateOrthographicSize( CameraAspectSize toTarget )
		{
			AnimateOrthographicSize( from, toTarget );
		}
		public void AnimateFieldOfViewTo( CameraAspectSize toTarget )
		{
			AnimateFieldOfViewTo( from, toTarget );
		}

		public void AnimateOrthographicSize( CameraAspectSize fromSize, CameraAspectSize toSize )
		{
			if( !fromSize )
			{
				Debug.LogWarning ("No /from/ CameraAspectSize component has been specified");
				return;
			}
			_from.AnimateOrthographicSizeToCo( duration, toSize ).Start();
		}
		public void AnimateFieldOfViewTo( CameraAspectSize fromValue, CameraAspectSize toTarget )
		{
			if( !fromValue )
			{
				Debug.LogWarning ("No /from/ CameraAspectSize component has been specified");
				return;
			}
			_from.AnimateFieldOfViewToCo( duration, toTarget ).Start();
		}
	}
}
