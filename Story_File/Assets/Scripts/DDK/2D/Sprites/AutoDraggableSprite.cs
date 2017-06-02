//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Statics;
using UnityEngine.UI;
using DDK.Base.Misc;
using DDK.Base.Events;



namespace DDK.ObjManagement._2D {

	/// <summary>
	/// This goes on the mouse pointer prefab to simulate a picked up UI element being dragged to a target (drag tutorial..).
	/// </summary>
	[RequireComponent( typeof( MoveTowards ) )]
	public class AutoDraggableSprite : MonoBehaviour {

		/// <summary>
		/// The name of the object containing the items.
		/// </summary>
		[Tooltip("The name of the object containing the items")]
		public string spritesParentName;
		public Vector3 iconScale = new Vector3(1f, 1f, 1f);
		public float iconDepth = 0f;
		public float showDelay = 0f;
		public float hideDelay = 3f;
		[Space(10f)]
		public float destroyAfter = 3f;
		public bool destroyOnMouseDown = true;
		[Indent(1)]
		public bool uiEventSystemMustBeActive = true;
		[Space(10f)]
		[Tooltip("Holds a reference to the path were the icon's picked up images are stored. They MUST contain the name " +
			"of the original sprite on theirs. Ex: Original sprite is named \"ballFloor\" then the pickedUp sprite might " +
	         "be named \"ballPickedUp\"")]
		public PathHolder.Index iconsPickedUpPath;



		/// <summary>
		/// The sprite to drag.
		/// </summary>
		internal Sprite sprite;
		private Image _firstChildrenImg;
		private GameObject _icon;
		private bool _iconVisible;
		private GameObject _spritesParent;



		// Use this for initialization
		void Start () {

			//StartCoroutine( StartCoroutines() );
			Init();
			
			StartCoroutine( ShowIcon() );
			StartCoroutine( HideIcon() );
		}

		public void OnDestroy()
		{
			if( _iconVisible )
			{
				_firstChildrenImg.SetAlpha(1f);
			}
		}



		private void Init()
		{
			_spritesParent = spritesParentName.Find();
			//yield return new WaitForSeconds( 0.01f );

			//INITIAL SETUP

			//ADD AN OnDestroyController
			var onDestroyController = gameObject.AddGetComponent<OnDestroyController>();
			onDestroyController.destroyAfter = destroyAfter;
			onDestroyController.destroyOnMouseDown = destroyOnMouseDown;
			onDestroyController.uiEventSystemMustBeActive = uiEventSystemMustBeActive;
			//GET THE ICON'S SPRITE
			string spritePath = iconsPickedUpPath.path;
			_firstChildrenImg = _spritesParent.GetChild(0).GetComponent<Image>();//The animation must go from the first child to the target
			sprite = spritePath.GetResourceWithSubstring<Sprite>( _firstChildrenImg.sprite.name );
			if( !sprite )
			{
				sprite = _firstChildrenImg.sprite;
			}
			//CREATE AND SETUP ICON
			_icon = new GameObject("tuto_icon");
			_icon.SetParent( transform );
			_icon.layer = gameObject.layer;//should be UI layer
			//_icon.tag = tag;

			var renderer = _icon.AddComponent<SpriteRenderer>();
			_icon.SetPos( 2, iconDepth );
			_icon.SetSpriteAlpha(0f);
			renderer.sprite = sprite;
			renderer.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
			_icon.transform.localScale = iconScale;
		}

		public IEnumerator ShowIcon()
		{
			yield return new WaitForSeconds( showDelay );
			_iconVisible = true;
			_firstChildrenImg.SetAlpha(0.4f);
			_icon.SetSpriteAlpha( 1f );
		}

		public IEnumerator HideIcon()
		{
			yield return new WaitForSeconds( hideDelay );
			_iconVisible = false;
			_firstChildrenImg.SetAlpha(1f);
			_icon.SetSpriteAlpha( 0f );
		}

	}

}
