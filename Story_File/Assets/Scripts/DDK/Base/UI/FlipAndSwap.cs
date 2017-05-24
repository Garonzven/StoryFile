//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;
using DDK.Base.Misc;
using DDK.Base.Managers;
using DDK.Base.SoundFX;
using DDK.Base.Classes;
using System.Collections.Generic;


namespace DDK.Base.UI 
{
	/// <summary>
	/// Allows flipping a card (image's sprite) and swapping its sprite.
	/// </summary>
	[RequireComponent( typeof( Image ) )]
	public class FlipAndSwap : MonoBehaviour 
	{
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "If this object contains a UI Button component, its OnClick() event will be automatically added to the button's onClick Listener";
        #endif

		/// <summary>
		/// The sprite that will replace the current one when swapped.
		/// </summary>
		[Space(30f)]
		[Tooltip("The sprite that will replace the current one when swapped.")]
		public Sprite replacement;
		[Tooltip("If the child has an Image, this will replace the current sprite when swapped.")]
		public Sprite childReplacement;
		public float flipSpeed = 3f;
		public Vector3 flipAxis = new Vector3( 0f, 1f, 0f );
		[Tooltip("If using a World Space Canvas the card might be on a table, and it needs to be offset when rotating so part of the card doesn't get inside the table. A" +
			" good value might be half of the parent layout's cell size, if using a grid panel")]
		public Vector3 flipPosOffset = new Vector3( 0f, 0f, 0f );
		[Tooltip("The amount of objects, holding this script, that can be flipped at a time")]
		public int flippedLimit = 2;
		public Sfx cardFlip;

		[Space(10f)]
		[Tooltip("If the card's parent has a grid layout with a Contraint set to Fixed Column Count and the main camera has a perspective projection, this will be added to" +
			" the sprite's swap angle (90) depending on the column index from the center, meaning the first column will be the middle column, the columns next to it will be" +
			" the second, and so on... The first column will always swap at 90 degrees")]
		public float flipAngleOffset = 15.4f;

		[Space(5f)]
		public AudioClip cardDescription;
		[Tooltip("The object description's path. NOTE: The name of the clip must be the same as the replacement's / sprite's, it will be appended to the specified path so the correct clip can be found")]
		[ShowIfAttribute( "_NoCardDescription", 1 )]
		public PathHolder.Index cardDescriptionSfx;

		[Header("Events")]
		public bool disableThisCompAfterFlipped = false;
		public bool disableOnClickAfterFlipped = true;
		public bool notInteractableWhenFlipped = true;
		[Tooltip( "This is invoked on Swap(), when the image is been replaced" )]
		public ComposedEvent onSwap = new ComposedEvent();
		[Tooltip( "This is invoked on Swap(), when the image is been set back to the original" )]
		public ComposedEvent onSwapBack = new ComposedEvent();


		#region VALIDATION FUNCTIONS
		protected bool _NoCardDescription()
		{
			return cardDescription == null;
		}
		#endregion


		protected Sprite _original;
		protected Sprite _originalChild;
		protected Quaternion _originalRot;
		protected Vector3 _originalPos;

		internal Image _img;
		internal Image _imgChild;
		internal Text _text;
		internal RectTransform _rt;
		internal Animator _ani;
		internal bool _flipping;
		internal bool _flipped;

		public bool Flipped
		{
			get { return _flipped; }
		}
		/// <summary>
		/// Is true if the card is flipped or being flipped. From its card back to its front.
		/// </summary>
		internal bool WillBeFlipped 
        {
			get
            {
				return ( (!_flipped && _flipping) || (_flipped && !_flipping) ) ? true : false;
			}
		}
		internal float _FlipSpeed 
        {
			get
            {
				return flipSpeed * Time.deltaTime * 100f;
			}
		}
		protected float _SwapDegree 
        {
			get 
            {
				float swapDegree = 90f;
				if( Camera.main.orthographic )
					return swapDegree;
				var gridLayout = transform.parent.GetComponent<GridLayoutGroup>();
				if( gridLayout )
				{
					if( gridLayout.constraint != GridLayoutGroup.Constraint.FixedColumnCount )
					{
						Debug.LogWarning("The GridLayout must have a Fixed Column Count Constrait for the sprite swap to be done properly by" +
							"using the /flipAngleOffset/ FlipAndSwap's value", transform.parent);
						return swapDegree;
					}
					int columns = gridLayout.constraintCount;
					int siblingIndex = transform.GetSiblingIndex();
					int middleColumn = (int)(columns * 0.5f);
					int currentRow = (int)(siblingIndex / columns);
					int currentColumn = siblingIndex - ( middleColumn + ( columns * currentRow ) );
					return swapDegree + flipAngleOffset * currentColumn;
				}
				return swapDegree;
			}
		}
		public Button _Bt { get; private set; }


        /// <summary>
        /// The number of FlipAndSwap that are about to be flipped (if not yet flipped).
        /// </summary>
		public static byte willBeFlippedCount;
        /// <summary>
        /// If true, the OnClick event won't be executed.
        /// </summary>
		public static bool blockOnClick;
        /// <summary>
        /// A list of all active instances of this component.
        /// </summary>
        public static List<FlipAndSwap> allFlipAndSwap;



		void Awake()
		{
            if( allFlipAndSwap == null )
            {
                allFlipAndSwap = new List<FlipAndSwap>();
            }
            allFlipAndSwap.Add( this );
			_img = GetComponent<Image>();
			_imgChild = gameObject.GetCompInChildren<Image>();
			_text = GetComponentInChildren<Text>();
			_rt = GetComponent<RectTransform>();
			_ani = GetComponent<Animator>();
		}
		// Use this for initialization
		void Start () 
		{
			_original = _img.sprite;
			if( _imgChild )
				_originalChild = _imgChild.sprite;
			_originalRot = _rt.localRotation;

			_Bt = GetComponent<Button>();
			if( _Bt )
			{
				_Bt.onClick.AddListener( OnClick );
			}
		}
		public void OnClick()
		{
			if( enabled && !blockOnClick )
			{
				if( disableOnClickAfterFlipped && _flipped )
					return;
				StartCoroutine( Flip_Swap() );	
			}
		}
        void OnEnable()
        {
            if( allFlipAndSwap.Contains( this ) )
                return;
            allFlipAndSwap.Add( this );
        }
        void OnDisable()
        {
            allFlipAndSwap.Remove( this );
        }

		public void StartFlipAndSwap()
		{
			StartCoroutine( Flip_Swap() );
		}
		public IEnumerator Flip_Swap()
		{
			if( !_flipping && ( willBeFlippedCount < flippedLimit || _flipped ) )
			{
				_flipping = true;
				SfxManager.PlaySfx( cardFlip, true, gameObject );
				if( !_flipped )
				{
					willBeFlippedCount++;
				}
				else if( willBeFlippedCount > 0 ) willBeFlippedCount--;
				//Half flip
				var target = Quaternion.Euler( _originalRot.eulerAngles + ( flipAxis * _SwapDegree ) );
				_originalPos = _rt.anchoredPosition3D;
				var targetOffset = _originalPos + flipPosOffset;
				yield return StartCoroutine( gameObject.MoveTowardsCo( targetOffset, _FlipSpeed * 4f, false, true ) );
				yield return StartCoroutine( gameObject.RotateTowardsCo( target, _FlipSpeed, true ) );

				Swap();
				//Other half flip
				yield return StartCoroutine( gameObject.RotateTowardsCo( _originalRot, _FlipSpeed, true ) );
				yield return StartCoroutine( gameObject.MoveTowardsCo( _originalPos, _FlipSpeed * 4f, false, true ) );

				_flipped = !_flipped;
				_flipping = false;
				if( _flipped && disableThisCompAfterFlipped )
				{
					enabled = false;
				}
				if( _flipped && _Bt.enabled )
				{
					SfxManager.PlayClip( "Voices", cardDescription, true, gameObject );
				}
			}
		}
		/// <summary>
		/// Swaps the image's sprite only if there is a replacement.
		/// </summary>
		public void Swap()
		{
			if( !replacement )
			{
				Debug.LogWarning  ("Swap(): No replacement has been set..", gameObject );
				return;
			}

			if( !_flipped )
			{
				_img.sprite = replacement;
				if( _imgChild )
					_imgChild.sprite = childReplacement;
				if( cardDescription == null )
				{
					cardDescription = _GetCardDescription( replacement.name );
				}
				onSwap.Invoke();
			}
			else
			{
				_img.sprite = _original;
				if( _imgChild )
					_imgChild.sprite = _originalChild;
				onSwapBack.Invoke();
			}
			if( notInteractableWhenFlipped )
			{
				ToggleInteractable();
			}
		}
		public void ToggleInteractable()
		{
			_Bt.interactable = !_Bt.interactable;
		}

        protected AudioClip _GetCardDescription( string replacementName )
		{
			AudioClip cardDescription;
			cardDescription = Resources.Load<AudioClip>( cardDescriptionSfx.path + "/" + replacementName );
			if( !cardDescription && _text && !replacementName.Equals( _text.text ) )
			{
				return _GetCardDescription( _text.text );
			}
			return cardDescription;
		}

	}

}
