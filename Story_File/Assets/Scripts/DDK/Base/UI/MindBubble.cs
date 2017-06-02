//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using UnityEngine.UI;


namespace DDK.Base.UI 
{
	/// <summary>
	/// This must go on the top parent of the mind bubble.
	/// </summary>
	public class MindBubble : MonoBehaviourExt 
    {
		[Tooltip("If null, it will be searched")]
		public CanvasGroupFader fader;
		[Tooltip("If null, it will be searched")]
		public Image bubble;
		[Tooltip("If null, it will be searched")]
		public Image content;
		[Tooltip("If null, it will be searched")]
		public Text textContent;


		/// <summary>
		/// True if this mind bubble is valid
		/// </summary>
		internal bool m_IsValid { get; private set; }


		void Awake () 
        {
			if( !fader )
			{
				fader = GetComponentInChildren<CanvasGroupFader>();
				if( !fader )
				{
					Debug.LogWarning ("There is no CanvasGroupFader..");
					enabled = false;
					return;
				}
			}
			if( !bubble )
			{
				bubble = GetComponentInChildren<Image>();
				if( !bubble )
				{
					Debug.LogWarning ("There is no Image..");
					enabled = false;
					return;
				}
			}
			if( !content )
			{
				content = bubble.gameObject.GetCompInChildren<Image>();
				if( !content )
				{
					Debug.Log ("There is no content Image..");
				}
			}
			if( !textContent )
			{
				textContent = bubble.gameObject.GetCompInChildren<Text>();
			}
			m_IsValid = true;
		}


		/// <summary>
		/// Sets the content Image's sprite. If there is no content, the bubble's Image's sprite is set instead.
		/// </summary>
		/// <param name="sprite">Sprite.</param>
		public void SetContent( Sprite sprite )
		{
			if( !m_IsValid )
				return;
			if( content )
			{
				content.sprite = sprite;
				if( !sprite )
					content.SetAlpha( 0f );
				else content.SetAlpha( 1f );
			}
			else bubble.sprite = sprite;
		}
		/// <summary>
		/// Sets the mind bubble's Text content if any.
		/// </summary>
		public void SetTextContent( string text )
		{
			if( !textContent || !m_IsValid )
				return;
			textContent.text = text;
		}
		public void SetScale( Vector3 scale )
		{
			if( scale == default( Vector3 ) )
				return;
			transform.localScale = scale;
		}
		/// <summary>
		/// Flips the MindBubble's Image.
		/// </summary>
		public void FlipBubbleX()
		{
			if( !m_IsValid )
				return;
			transform.localScale = transform.localScale.InvertX ();
		}
		/// <summary>
		/// Flips the MindBubble's Image.
		/// </summary>
		public void FlipContentX()
		{
			if( !m_IsValid )
				return;
			if( content )
				content.transform.localScale = content.transform.localScale.InvertX();
			if( textContent )
				textContent.transform.localScale = textContent.transform.localScale.InvertX();
		}
	}
}
