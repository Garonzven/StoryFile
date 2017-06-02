//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;


namespace DDK._2D.Sprites {

	/// <summary>
	/// Attach to an object to allow changing its sprite depending on some action.
	/// </summary>
	public class SpriteChangeController : MonoBehaviour 
	{	
		[System.Serializable]
		public class SpriteChangeEvents
		{
			public IndexEvents[] indexEvents = new IndexEvents[0];
        }
		[System.Serializable]
		public class IndexEvents
		{
			public int index;
			[Tooltip("This will get invoked every time the specified index is set")]
			public UnityEvent onIndexSet = new UnityEvent();
		}


		public Sprite[] sprites = new Sprite[0];
		[ShowIfAttribute( "_HasCollider" )]
		public bool changeToNextOnTouch;
		public bool setInitialSprite = true;
		[ShowIfAttribute( "_SetInitialSprite", 1 )]
		public bool randomInitialSprite;
		[Tooltip("The index of the sprite to assing, inside the /sprites/ array")]
		[ShowIfAttribute( "_SetInitialSprite", 1 )]
		public int initialSprite;
		public SpriteChangeEvents events = new SpriteChangeEvents();


		protected bool _HasCollider()
		{
			return GetComponent<Collider>() || GetComponent<Collider2D>();
        }
		
		
		private int _current = 0;
		/// <summary>
		/// Current sprite index.
		/// </summary>
		/// <value>The current.</value>
		public int m_Current{
			get{
				return _current;
			}
			set{
				if( _current == value )
					return;
				_current = ( value < 0 ) ? sprites.Length-1 : ( value > sprites.Length-1 ) ? 0 : value;
				for( int i=0; i<events.indexEvents.Length; i++ )//CHECK INDEX EVENTS
				{
					if( events.indexEvents[i].index == _current )
					{
						events.indexEvents[i].onIndexSet.Invoke();
						break;
					}
				}
			}
		}
		protected SpriteRenderer _spriteRenderer;
		protected Image _image;


		protected bool _SetInitialSprite()
		{
			return setInitialSprite;
		}

		
		
		protected void Awake()
		{
			_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			_image = GetComponentInChildren<Image>();
		}		
		// Use this for initialization
		protected void Start () {
			
			if( setInitialSprite )
			{
				SetSprite( initialSprite );
				if( randomInitialSprite )
				{
					SetRandomSprite();
				}
			}
			else //UPDATE CURRENT INDEX
			{
				for( int i=0; i<sprites.Length; i++ )
				{
					if( _spriteRenderer && _spriteRenderer.sprite == sprites[i] )
					{
						_current = i;
						break;
					}
					else if( _image && _image.sprite == sprites[i] )
                    {
						_current = i;
                        break;
                    }
                }
            }
        }				
        protected void OnMouseUpAsButton()//IF THIS HAS A COLLIDER
		{
			if( !enabled )
				return;
			if( changeToNextOnTouch )
			{
				NextSprite();
			}
		}

				
		
		public void SetSprite( int i )
		{
			if( !enabled )
				return;
			m_Current = i;
			if( _spriteRenderer )
				_spriteRenderer.sprite = sprites[ m_Current ];
			else if ( _image ) _image.sprite = sprites[ m_Current ];
		}		
		public void SetRandomSprite()
		{
			if( !enabled )
				return;
			int ran = Random.Range( 0, sprites.Length );
			SetSprite( ran );
		}	
		/// <summary>
		/// Changes the current sprite index, to the next one.
		/// </summary>
		public void NextSprite()
		{
			SetSprite( m_Current + 1 );
		}
		/// <summary>
		/// Animates the sprite until it reaches the target sprite index.
		/// </summary>
		public IEnumerator AnimateToSpriteCo( int targetIndex, float animDuration = 1f )
		{
			if( !enabled || sprites.Length == 0 || animDuration == 0f )
				yield break;
			float time = 0f;
			while( time < animDuration )
			{
				time += Time.deltaTime;
				SetSprite( (int) Mathf.Lerp( m_Current, targetIndex, time ) );
				yield return null;
			}
		}
		/// <summary>
		/// Animates the sprite until it reaches the /current + extraIndexes/ sprite index.
		/// </summary>
		public IEnumerator AnimateSpriteCo( int extraIndexes, float animDuration = 1f )
		{
			if( !enabled || sprites.Length == 0 || animDuration == 0f )
				yield break;
			int changes = 0;
			float interval = animDuration / extraIndexes;
			while( changes < extraIndexes )
			{
				yield return new WaitForSeconds( interval );
				NextSprite ();
				changes++;
            }
        }
	}
}