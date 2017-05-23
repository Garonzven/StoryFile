//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.EventSystems;
using DDK.Base.Extensions;
using UnityEngine.UI;
using System.Collections.Generic;
using DDK.Base.Classes;
using DDK.Base.ScriptableObjects;
using System.Collections;


namespace DDK.Base.UI 
{
	/// <summary>
	/// Controls the popUp / hide animator animations of a UI menu. Attach to the button that will popUp the menu.
	/// </summary>
	[RequireComponent( typeof( Button ) )]
	public class UIPopUpMenuController : MonoBehaviourExt, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler 
	{
		public enum Transition { ToLeft, ToRight };

		[System.Serializable]
		public class TitleText 
		{
			public LanguageDictionary[] languageDictionary;
			[Tooltip("If index is below cero (0), a random one will be used")]
			[NotLessThan( 0 )]
			public int index = 0;
		}



		public bool hideOnClickOutside = true;
		[Space(5f)]
		//[Header("PopUp Menu")]
		[Tooltip("This MUST be the Panel (containing an image) that represents the menu")]
		public SearchableGameObject popUpMenu = new SearchableGameObject();
		public bool destroyMenuChildrenWhenHidden;
		public bool destroyMenuSubChildrenWhenHidden;
		[Space(2f)]
		[Tooltip("This will only be used if any of the Destroy Menu... toggles is true")]
		[ShowIfAttribute( "_DestroyMenuChildrenOrSubChildren" )]
		public string[] excludeFromDestroy;
		[Header("Menu Bar")]
		[Tooltip("The object that represents the menu title")]
		public SearchableGameObject title = new SearchableGameObject( "Title" );
		[Tooltip("The text that will be set as title when the popUp menu is active")]
		[ShowIfAttribute( "titleTextIsObjName", true )]
		public TitleText titleText = new TitleText();
		[Tooltip("If true, the specified /title/'s text will be this object's name")]
		public bool titleTextIsObjName;
		[Space(5f)]
		[Header("Animations")]
		[Tooltip("When visible, the object is always positioned at 0. Make sure the pivot is properly located")]
		[ShowIfAttribute( "_IsPopUpControllerNull" )]
		public Transition transition;
		[ShowIfAttribute( "_IsPopUpControllerNull" )]
		public float animDuration = 0.3f;
		[Space(10f)]
		[Tooltip("If null, it will be searched in this object and its children. If not null, it will be used instead of the previous Animations")]
		public Animator popUpController;
		/// <summary>
		/// The Animator's (bool) show popup param name.
		/// </summary>
		[Tooltip("The Animator's (bool) show popup param name")]
		[ShowIfAttribute( "_IsPopUpControllerNull", true, 1 )]
		public string showParamName = "show";


		protected bool _DestroyMenuChildrenOrSubChildren()
		{
			return destroyMenuChildrenWhenHidden || destroyMenuSubChildrenWhenHidden;
		}
		protected bool _IsPopUpControllerNull()
		{
			return popUpController == null;
		}


		/// <summary>
		/// This object's Button
		/// </summary>
		protected Button _trigger 
		{
			get{ return GetComponent<Button>(); }
		}
		protected GameObject _currentMenu;
		protected Vector2 _currentHiddenPos;
		protected Vector2 _currentVisiblePos;
		protected Vector2 _nextHiddenPos;
		protected Vector2 _nextVisiblePos;
		/// <summary>
		/// Prevents the menu from being hidden when clicking over it.
		/// </summary>
		protected bool _pointerIsInside;
		/// <summary>
		/// Helps preventing the pop up menu to toggle multiple times in a short interval causing buggy results.
		/// </summary>
		protected float _lastToggle;
		protected string _previousTitle;
		/// <summary>
		/// This contains the buttons that will be disabled if /hideOnClickOutside/ is true and the menu is active.
		/// </summary>
		protected Button[] _otherBts 
		{
			get
			{
				var objs = GameObject.FindObjectsOfType<Button>();
				return objs.RemoveContained<Button>( popUpMenu.m_gameObject.GetComponentsInChildren<Button>() ).RemoveNonInteractable().ToArray();
			}
		}
		/// <summary>
		/// true if the pop-up menu is active.
		/// </summary>
		internal bool isActive { private set; get; }


		protected static Text _title;
		/// <summary>
		/// This helps to easily navigate through multiple pop-up menus.
		/// </summary>
		public static UIPopUpMenuController[] menus 
		{
			get{ return GameObject.FindObjectsOfType<UIPopUpMenuController>(); }
		}
		/// <summary>
		/// This helps to easily navigate through multiple pop-up menus.
		/// </summary>
		public static List<UIPopUpMenuController> _previousMenus;



		// Use this for initialization
		void Start () 
		{
			//INIT
			if( !popUpController )
			{
				popUpController = GetComponent<Animator>();
				if( !popUpController && gameObject.ChildCount() > 0 )
				{
					popUpController = gameObject.GetChild(0).GetComponent<Animator>();
				}
			}
			if( !_currentMenu )
			{
				_currentMenu = gameObject.GetFirstAncestorOfType<Image>();
			}
			if( popUpMenu.m_gameObject && hideOnClickOutside )
			{
				//EVENTS
				var _popUpPanel = popUpMenu.m_gameObject.GetComponentInChildren<Image>();
				_popUpPanel.AddEvent( (a) => {
					_pointerIsInside = true;
				}, EventTriggerType.PointerEnter );
				_popUpPanel.AddEvent( (a) => {
					_pointerIsInside = false;
				}, EventTriggerType.PointerExit );
			}
			if( _previousMenus == null )
			{
				_previousMenus = new List<UIPopUpMenuController>();
			}
			if( title.m_gameObject )
			{
				_title = title.m_gameObject.GetComponentInChildren<Text>();
			}
			if( _trigger )//THIS BUTTON'S EVENT
			{
				_trigger.onClick.AddListener( ToggleMenu );
			}
			//TRANSITION
			var canvasScaler = GetComponentInParent<CanvasScaler>();
			_currentVisiblePos = new Vector2( 0f, _currentMenu.AnchoredPosition().y );
			_nextVisiblePos = new Vector2( 0f, popUpMenu.m_gameObject.AnchoredPosition().y );
			float offsetMultiplier = 1f;
			if( popUpMenu.m_gameObject )
			{
				offsetMultiplier = popUpMenu.GetComponent<RectTransform>().sizeDelta.x / canvasScaler.referenceResolution.x;
			}
			switch( transition )
			{
			case Transition.ToLeft:
				_currentHiddenPos = new Vector2( -canvasScaler.referenceResolution.x, _currentMenu.AnchoredPosition().y );
				_nextHiddenPos = new Vector2( canvasScaler.referenceResolution.x * offsetMultiplier, popUpMenu.m_gameObject.AnchoredPosition().y );
				break;
			case Transition.ToRight:
				_currentHiddenPos = new Vector2( canvasScaler.referenceResolution.x, _currentMenu.AnchoredPosition().y );
				_nextHiddenPos = new Vector2( -canvasScaler.referenceResolution.x * offsetMultiplier, popUpMenu.m_gameObject.AnchoredPosition().y );
				break;
			}
		}		
		// Update is called once per frame
		void Update () 
		{
			if( Input.GetMouseButtonDown(0) && !_pointerIsInside && hideOnClickOutside )
			{
				HideMenu();
			}
		}
		#region EVENTS
		public void OnPointerClick( PointerEventData data )
		{
			if( data.pointerEnter.name.Equals( name ) )//TOGGLE WHEN CLICKING THE MENU OPTION
			{
				ToggleMenu();
			}
		}		
		public void OnPointerEnter( PointerEventData data )
		{
			_pointerIsInside = true;
		}		
		public void OnPointerExit( PointerEventData data )
		{
			_pointerIsInside = false;
		}
		#endregion EVENTS


		#region FUNCTIONS
		public void ShowMenu()
		{
			_ShowMenu( true );
		}
		public void HideMenu()
		{
			_ShowMenu( false );
		}	
		public void ToggleMenu()
		{
			if( !_trigger.interactable )
				return;
			if( Time.time < _lastToggle + animDuration )
				return;
			if( popUpController )
			{
				//isActive = !popUpController.GetBool( showParamName );
				_ShowMenu( !isActive );
			}
			else if( popUpMenu.m_gameObject && _trigger )
			{
				_ShowMenu( !isActive );
			}
			_lastToggle = Time.time;
		}		
		/// <summary>
		/// Shows the previous UIPopUpMenu.
		/// </summary>
		/// <param name="hideThis"> If true, this UIPopUpMenu will be hidden after calling this function </param>
		public void PreviousMenu( bool hideThis )
		{
			var lastMenu = _previousMenus.GetLastItem<UIPopUpMenuController>();
			if( !lastMenu )
				return;
			if( hideThis )
			{
				HideMenu();
			}
			lastMenu.HideMenu();
		}
		#endregion FUNCTIONS


		#region PROTECTED/PRIVATE FUNCTIONS
		protected void _ShowMenu( bool show )
		{
			if( show == isActive )
				return;
			isActive = show;
			if( popUpController )//ANIMATOR
			{
				popUpController.SetBool( showParamName, show );
				return;
			}
			else if( popUpMenu.m_gameObject )//CUSTOM ANIMATION
			{
				Vector2 popUpTarget = _nextHiddenPos;
				Vector2 currentTarget = _currentVisiblePos;
				if( show )
				{
					popUpTarget = _nextVisiblePos;
					currentTarget = _currentHiddenPos;
				}
				popUpMenu.m_gameObject.AnimMoveTowards( animDuration, popUpTarget, false, true );
				if( !hideOnClickOutside )
				{
					_currentMenu.AnimMoveTowards( animDuration, currentTarget, false, true );
				}
			}
			else return;

			if( isActive )
			{
				if( !hideOnClickOutside && !_previousMenus.Contains( this ) )
				{
					_previousMenus.Add( this );
				}
				else if ( hideOnClickOutside )
				{
					_otherBts.SetEnabled( false );
				}
				if( _title )
				{
					_previousTitle = _title.text;
					if( titleTextIsObjName )
					{
						_title.text = name;
					}
					else if( titleText.languageDictionary != null && titleText.languageDictionary.Length > 0 )
					{
						_title.text = titleText.languageDictionary.GetCurrentLanguageDictionary().GetTextAt( titleText.index );
					}
				}
			}
			else 
			{
				if( /*hideOnClickOutside &&*/ _title )
				{
					_title.text = _previousTitle;
				}
				if( !hideOnClickOutside && _previousMenus.Contains( this ) )
				{
					_previousMenus.Remove( this );
				}
				else if( hideOnClickOutside )
				{
					_otherBts.EnableAfter( true, 0.1f );
				}
				if( destroyMenuSubChildrenWhenHidden )
				{
					for( int i=0; i<popUpMenu.m_gameObject.ChildCountActiveOnly(); i++ )
					{
						StartCoroutine( DestroyMenuChildren( popUpMenu.m_gameObject.GetChild( i ) ) );
					}
				}
				else if( destroyMenuChildrenWhenHidden )
				{
					StartCoroutine( DestroyMenuChildren( popUpMenu.m_gameObject ) );
				}
			}
		}

		private IEnumerator DestroyMenuChildren( GameObject menu )
		{
			while( menu.IsInsideItsCanvas() )
			{
				yield return null;
			}
			menu.DestroyChildren( excludeFromDestroy, true );
		}
		#endregion
	}
}
