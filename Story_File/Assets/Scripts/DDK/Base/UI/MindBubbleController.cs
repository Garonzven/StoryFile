//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.Base.UI 
{
	public class MindBubbleController : MonoBehaviourExt 
    {
		[SerializeField]
		[Tooltip("This should reference a prefab..")]
		private MindBubble mindBubble;
		[Tooltip("The mind bubble's parent. The mind bubble will be positioned at (0,0,0). The name of the object should be unique")]
        [IndentAttribute( 1 )]
		public SearchableGameObject parent;
		public bool lookAtMainCamera = true;
		[Indent(1)]
		[Tooltip("If true, the mindBubble will always look at the camera even if it moves.")]
		public bool keepLooking;
		public bool flipBubbleX;
		public bool flipContentX;
		[Tooltip("The image/sprite to show inside the bubble")]
		public Sprite content;
		public string textContent;
		[Indent(1)]
		[Tooltip("If true, the text will be set to null/empty after its proceeding mind bubble is shown")]
		public bool autoResetAfterShown = true;
		public Vector3 scale = Vector3.one;
		public Vector3 offsetPos;
		/// <summary>
		/// The duration of the mind bubble after fading in, and before fading out
		/// </summary>
		[Tooltip("The duration of the mind bubble after fading in, and before fading out")]
		public float activeDuration = 1f;
		[Header("Events")]
		public ComposedEvent beforeShow = new ComposedEvent();
		public ComposedEvent afterDestroy = new ComposedEvent();


		// Use this for initialization
		void Start () {} //Allows to enable/disable the component


		protected bool _IsMindBubbleValid 
		{
			get
			{
				if( !mindBubble )
					return false;
				return mindBubble.m_IsValid;
			}
		}
		/// <summary>
		/// The instantiated mind bubble, if any.
		/// </summary>
        public GameObject m_MindBubbleObj { get; private set; }
		/// <summary>
		/// The instantiated mind bubble, if any.
		/// </summary>
        public MindBubble m_MindBubble { get; private set; }
		protected Transform _MainCam 
		{
			get
			{
				if( !Camera.main )
					return FindObjectOfType<Camera>().transform;
				return Camera.main.transform;
			}
		}


		public void SetMindBubble( MindBubble pMindBubble )
		{
			if( !pMindBubble )
				return;
			mindBubble = pMindBubble;
		}
		public void SetMindBubbleParent( GameObject newParent )
		{
			if( !newParent )
				return;
			parent.m_gameObject = newParent;
		}
		public void SetMindBubbleParent( string newParent )
		{
			if( string.IsNullOrEmpty( newParent ) )
				return;
			var newP = newParent.Find();
			if( !newP )
				return;
			parent.m_gameObject = newP;
		}
		public void SetContent( Sprite content )
		{
			if( !content )
				return;
			this.content = content;
		}
		public void SetTextContent( string content )
		{
			textContent = content;
		}
		public void SetFlipBubbleSpriteX( bool flip )
		{
			flipBubbleX = flip;
		}
		public void SetFlipContentSpriteX( bool flip )
		{
			flipContentX = flip;
		}
		public void SetMindBubbleScale( Vector3 _scale )
		{
			if( _scale == default( Vector3 ) )
				return;
			scale = _scale;
		}
		public void SetDuration( float duration )
		{
			if( !m_MindBubble || duration <= 0f )
				return;
			activeDuration = duration;
		}
		public void Show()
		{
			_ShowMindBubble();
		}
		public void Show( float duration )
		{
			_ShowMindBubble( duration );
		}
		public void Show( Sprite content )
		{
			SetContent( content );
			_ShowMindBubble();
		}
		public void Show( Sprite content, float duration )
		{
			SetContent( content );
			Show ( duration );
		}
		public void Show( Sprite content, float duration, float scale )
		{
			SetScale( scale.GetVector3() );
			Show ( content, duration );
		}
		public void Show( Sprite content, float duration, Vector3 scale )
		{
			SetScale( scale );
			Show ( content, duration );
		}
		public void ShowText( string content )
		{
			SetTextContent( content );
			_ShowMindBubble();
		}
		/// <summary>
		/// Shows the Mind Bubble with the specified text, and no sprite.
		/// </summary>
		/// <param name="textContent">Text content.</param>
		public void ShowTextOnly( string textContent )
		{
			SetTextContent( textContent );
			content = null;
			_ShowMindBubble();
		}
		public void ShowTextOnly( string textContent, float duration )
		{
			SetTextContent( textContent );
			content = null;
			_ShowMindBubble( duration );
		}
		public void MultiplyScale( float multiplier )
		{
            if( multiplier.CloseTo( 0f ) )
				return;
			scale *= multiplier;
		}
		public void SetScale( Vector3 _scale )
		{
			if( _scale == default( Vector3 ) )
				return;
			scale = _scale;
		}
		public void SetOffsetPos( Vector3 offset )
		{
			offsetPos = offset;
		}


		protected void _ShowMindBubble( float duration = -1f )
		{
			if( !mindBubble )
				return;

			beforeShow.Invoke();
			//SETUP
			if( !m_MindBubbleObj )
			{
				m_MindBubbleObj = mindBubble.gameObject.SetActiveInHierarchy();
				m_MindBubbleObj.SetParent( parent.m_transform );
				m_MindBubbleObj.transform.localPosition = Vector3.zero;
				m_MindBubble = m_MindBubbleObj.GetComponent<MindBubble>();
				if( duration > 0 )
					activeDuration = duration;
				m_MindBubble.SetScale( scale );
				m_MindBubble.transform.localPosition += offsetPos;
                if( flipBubbleX )
                    m_MindBubble.FlipBubbleX();
                if( flipContentX )
                    m_MindBubble.FlipContentX();
				if( lookAtMainCamera )
				{
					if( keepLooking )
					{
						StartCoroutine( LookAt( _MainCam ) );
                    }
                    else m_MindBubbleObj.transform.LookAt( _MainCam );
				}
				_DestroyMindBubble().Start();
			}
			m_MindBubble.SetTextContent( textContent );
			if( autoResetAfterShown )
				textContent = "";
			m_MindBubble.SetContent( content );
		}


		/// <summary>
		/// Looks at a target until this object or the target is destroyed.
		/// </summary>
		public IEnumerator LookAt( Transform target )
		{
			while( m_MindBubbleObj && target )
			{
				m_MindBubbleObj.transform.LookAt( target );
				yield return null;
			}
		}

		private IEnumerator _DestroyMindBubble()
		{
            float delay = activeDuration;
            if( m_MindBubble.fader )
            {
                delay += m_MindBubble.fader.onEnableDelay + m_MindBubble.fader.duration;
            }
            yield return new WaitForSeconds( delay );
			if( !m_MindBubble || !m_MindBubble.fader )
				yield break;
			m_MindBubble.fader.enabled = false;//FADE OUT
			yield return new WaitForSeconds( m_MindBubble.fader.onDisableDelay + m_MindBubble.fader.duration );
			Destroy ( m_MindBubbleObj );
			while( m_MindBubbleObj )
				yield return null;
			afterDestroy.Invoke();
		}
	}
}
