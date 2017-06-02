//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Statics;
using System.Collections.Generic;


namespace DDK.Base.Misc 
{
	/// <summary>
	/// Attach to a camera object to change its ortho size or field of view deppending on the aspect ratio. It works in
	/// edit mode.
	/// </summary>
	[RequireComponent( typeof( Camera ) )]
	[ExecuteInEditMode]
	public class CameraAspectSize : MonoBehaviour 
    {		
		[System.Serializable]
		public class Overrider
		{
			[Tooltip("The Aspect Ratio to which the orthoSize/fieldOfView should be assigned")]
			public Vector2 aspect;
			public float orthoSize = 5f;
			public float fieldOfView = 60f;


			public Overrider() {}
			public Overrider( Vector2 aspect )
			{
				this.aspect = aspect;
			}
			public Overrider( Vector2 aspect, float ortho, float fView )
			{
				this.aspect = aspect;
				orthoSize = ortho;
				fieldOfView = fView;
			}
		}
	


        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "This executes on Start() and updates in Edit Mode only";
        #endif
		public Overrider squarestScreen = new Overrider( new Vector2( 5f, 4f ), 5f, 74f );
		public Overrider largestScreen = new Overrider( new Vector2( 17f, 9f ), 5f, 60f );
		[Space(10f)]
		[Tooltip("If this is not empty, any aspect that matches current will override the automatically calculated aspect " +
			"set by /Squarest Screen/ and /Largest Screen/")]
		public Overrider[] overriders = new Overrider[0];



		/// <summary>
		/// True if the camera is orthographic
		/// </summary>
		protected bool _isOrtho 
        {
			get { return m_Cam.orthographic; }
		}
        private Camera _cam;
        /// <summary>
        /// A reference to this gameObject's Camera.
        /// </summary>
        /// <value>The cam.</value>
        public Camera m_Cam 
        {
            get
            {
                if( !_cam )
                {
                    _cam = GetComponent<Camera>();
                    if( !_cam )
                        _cam = FindObjectOfType<Camera>();
                }
                return _cam;
            }
            set
            {
                if( value )
                    _cam = value;
            }
        }

        public static List<CameraAspectSize> Instances { get; private set; }
	

				
        void Awake()
        {
            if( enabled )
            {
                if( Instances == null )
                {
                    Instances = new List<CameraAspectSize>( 10 );
                }
                Instances.Add( this );
            }
            m_Cam = GetComponent<Camera>();
        }
		// Use this for initialization
		void Start () 
        {
			AutoCalculateSizeOrFOV();
		}
#if UNITY_EDITOR
		// Update is called once per frame
		void Update () 
        {
			if( !Application.isPlaying )
			{
				AutoCalculateSizeOrFOV();
				CheckOverrides();
			}
		}
#endif
        void OnDestroy()
        {
            #if UNITY_EDITOR
            if( !Application.isPlaying )
                return;
            #endif
            if( Instances != null )
            {
                Instances.Remove( this );
            }
        }



		/// <summary>
		/// Returns the automatically calculated orthographic size.
		/// </summary>
		public float GetCalculatedSize()
		{
			float largestAspect = largestScreen.aspect.GetAspectRatio();
			float squarestAspect = squarestScreen.aspect.GetAspectRatio();
			
			if( _isOrtho )
			{
				float largestOrtho = largestScreen.orthoSize;
				float squarestOrtho = squarestScreen.orthoSize;
				
				float maxAR = largestAspect - squarestAspect;//100%
				float currentAR = largestAspect - m_Cam.aspect;//actual aspect %
				float AR = currentAR / maxAR;//actual aspect ( 0 - 1 )%
				
				float maxDif = (squarestOrtho - largestOrtho).Abs();//100%
				
				float ortho = largestOrtho + ( AR * maxDif );
				return ortho.SetNumDecimals(3);
			}
			return m_Cam.orthographicSize;
		}
		/// <summary>
		/// Returns the automatically calculated field of view.
		/// </summary>
		public float GetCalculatedFOV()
		{
			float largestAspect = largestScreen.aspect.GetAspectRatio();
			float squarestAspect = squarestScreen.aspect.GetAspectRatio();
			
			if( _isOrtho )
			{
				return m_Cam.fieldOfView;
			}
			else 
			{
				float largestView = largestScreen.fieldOfView;
				float squarestView = squarestScreen.fieldOfView;
				
				float maxAR = largestAspect - squarestAspect;//100%
				float actAR = largestAspect - m_Cam.aspect;//actual aspect %
				float AR = actAR / maxAR;//actual aspect ( 0 - 1 )%
				
				float maxDif = (squarestView - largestView).Abs();//100%
				float fView = largestView + ( AR * maxDif );
				return fView.SetNumDecimals(3);
			}
		}

        /// <summary>
        /// Calculates the Camera's Orthographic Size or Field Of View.
        /// </summary>
		public void AutoCalculateSizeOrFOV()
		{
			if( _isOrtho )
			{
				SetOrthographicSize( GetCalculatedSize() );
			}
			else 
			{
				SetFieldOfView( GetCalculatedFOV() );
			}
		}
        /// <summary>
        /// Checks the overrides array to see if any Aspect ratio or Field Of View needs to be overridden.
        /// </summary>
		void CheckOverrides()
		{
			for( byte i=0; i<overriders.Length; i++ )
			{
				if( Device.aspectRatio.CloseTo( overriders[i].aspect.GetAspectRatio(), 0.05f ) )
				{
					if( _isOrtho )
					{
						SetOrthographicSize( overriders[i].orthoSize );
					}
					else 
					{
						SetFieldOfView( overriders[i].fieldOfView );
					}
				}
			}
		}


        /// <summary>
        /// Sets the Camera's orthographic size.
        /// </summary>
		public void SetOrthographicSize( float size )
		{
			m_Cam.orthographicSize = size;
		}
        /// <summary>
        /// Sets the Camera's field of view.
        /// </summary>
		public void SetFieldOfView( float value )
		{
			m_Cam.fieldOfView = value;
		}
        /// <summary>
        /// Sets the Camera's orthographic size by copying the value /from/ the specified CameraAspectSize.
        /// </summary>
		public void CopyOrthographicSize( CameraAspectSize from )
		{
			m_Cam.orthographicSize = from.GetCalculatedSize();
		}
        /// <summary>
        /// Sets the Camera's field of view by copying the value /from/ the specified CameraAspectSize.
        /// </summary>
		public void CopyFieldOfView( CameraAspectSize from )
		{
			m_Cam.fieldOfView = from.GetCalculatedFOV();
		}
        /// <summary>
        /// Animates the Camera's orthographic size to the specified /target/'s orthographic size in /duration/ seconds.
        /// </summary>
		public void AnimateOrthographicSizeTo( float duration, CameraAspectSize target )
		{
			AnimateOrthographicSizeToCo( duration, target ).Start();
		}
        /// <summary>
        /// Animates the Camera's field of view to the specified /target/'s field of view in /duration/ seconds.
        /// </summary>
		public void AnimateFieldOfViewTo( float duration, CameraAspectSize target )
		{
			AnimateFieldOfViewToCo( duration, target ).Start();
		}
        /// <summary>
        /// Animates the Camera's orthographic size to the specified /target/'s orthographic size in /duration/ seconds. 
        /// This yields until animation is done.
        /// </summary>
		public IEnumerator AnimateOrthographicSizeToCo( float duration, CameraAspectSize target )
		{
			yield return AnimateOrthographicSizeToCo( duration, target.GetCalculatedSize() ).Start();
		}
        /// <summary>
        /// Animates the Camera's field of view to the specified /target/'s field of view in /duration/ seconds. This yields until the animation is done.
        /// </summary>
		public IEnumerator AnimateFieldOfViewToCo( float duration, CameraAspectSize target )
		{
			yield return AnimateFieldOfViewToCo( duration, target.GetCalculatedFOV() ).Start();
		}
        /// <summary>
        /// Animates the Camera's orthographic size to the specified /target/ in /duration/ seconds.
        /// </summary>
		public void AnimateOrthographicSizeTo( float duration, float target )
		{
			AnimateOrthographicSizeToCo( duration, target ).Start();
		}
        /// <summary>
        /// Animates the Camera's field of view to the specified /target/ in /duration/ seconds.
        /// </summary>
		public void AnimateFieldOfViewTo( float duration, float target )
		{
			AnimateFieldOfViewToCo( duration, target ).Start();
		}
        /// <summary>
        /// Animates the Camera's orthographic size to the specified /target/ in /duration/ seconds. This yields until animation is done.
        /// </summary>
		public IEnumerator AnimateOrthographicSizeToCo( float duration, float target )
		{
			float oTarget = target;
			float time = 0f;
            while( !m_Cam.orthographicSize.CloseTo( oTarget ) )
			{
				time += Time.deltaTime;
				m_Cam.orthographicSize = m_Cam.orthographicSize.Lerp( oTarget, time / duration );
				yield return null;
				if( this == null )
					yield break;
			}
            m_Cam.orthographicSize = target;//Fix lerp floating point error
		}
        /// <summary>
        /// Animates the Camera's field of view to the specified /target/ in /duration/ seconds. This yields until the animation is done.
        /// </summary>
		public IEnumerator AnimateFieldOfViewToCo( float duration, float target )
		{
			float fovTarget = target;
			float time = 0f;
            while( !m_Cam.fieldOfView.CloseTo( fovTarget ) )
			{
				time += Time.deltaTime;
				m_Cam.fieldOfView = m_Cam.fieldOfView.Lerp( fovTarget, time / duration );
				yield return null;
			}
            m_Cam.fieldOfView = target;//Fix lerp floating point error
		}		
	}
}