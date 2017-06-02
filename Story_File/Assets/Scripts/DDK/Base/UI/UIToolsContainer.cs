//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.UI;
using DDK.Base.Extensions;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

using DDK.Base.SoundFX;
using DDK.Base.Classes;
using DDK.Base.Managers;
using DDK.Base.Events;
using DDK.Base.Statics;

#if USE_OBJ_MANAGEMENT
using DDK.ObjManagement;
#endif


namespace DDK.UI 
{
	/// <summary>
	/// Attach this to an object that will hold tools inside a container. For example: A game with a UI Backpack
	/// holding multiple items that the user can use throughout the game.
	/// </summary>
	public class UIToolsContainer : MonoBehaviourExt 
    {
		[System.Serializable]
		public class UISfx 
        {
			[Tooltip("The name of the button that must be clicked for the specified clip to be played. NOTE: If empty," +
				"the clip will be played on all tool buttons that don't have a specific sfx set")]
			public string toolButtonName;
			public Sfx sfx;
		}


        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string msg = "Ensure the tools have a unique name";
        #endif
		public SearchableGameObject toolsContainer = new SearchableGameObject( "Backpack" );
		[Tooltip("The Graphic that will be replaced when the current tool is set. The tools are the /toolsContainer/'s children Buttons")]
		public SearchableGraphic currentTool = new SearchableGraphic( "CurrentTool" );

		[Space(10f)]
		public UISfx[] onClickSfx = new UISfx[0];

		/// <summary>
		/// The duration of the glow ping-pong animations.
		/// </summary>
		[Tooltip("The duration of the glow ping-pong animations")]
		[Header( "Glows Settings" )]
		public float glowAnimationsDuration = 1.5f;
		/// <summary>
		/// The amount of times the glow will repeat its ping-pong animation when animating
		/// </summary>
		[Tooltip("The amount of times the glow will repeat its ping-pong animation when animating")]
		public int repeatGlow = 1;
		/// <summary>
		/// The seconds before glowing the respective UI button if the user hasn't clicked on it.
		/// </summary>
		[Tooltip("The seconds before glowing again the respective UI button if the user hasn't clicked on it")]
        public float glowInterval = 2f;
        	

		protected Transform _toolsParent;
		private UIPopUpMenuController _toolsContainer;
		public UIPopUpMenuController ToolsContainer 
        {
			get
            {
				_CheckWarning();
				return _toolsContainer;
			}
			protected set
            {
				_toolsContainer = value;
			}
		}
		private Button _toolsContainerBt;
		public Button ToolsContainerBt 
        {
			get
            {
				_CheckWarning();
				return _toolsContainerBt;
			}
			protected set
            {
				_toolsContainerBt = value;
			}
		}
		private Graphic _currentTool;
		public Graphic CurrentTool 
        {
			get
            {
				_CheckWarning();
				return _currentTool;
			}
			protected set
            {
				_currentTool = value;
			}
		}
		private Button _currentToolBt;
		public Button CurrentToolBt 
        {
			get
            {
				_CheckWarning();
				return _currentToolBt;
			}
			protected set
            {
				_currentToolBt = value;
			}
		}
		private Dictionary<string, Button> _tools;
		/// <summary>
		/// All the Buttons representing a Tool. Each key is the button's gameObject's name, so make sure they are unique!
		/// </summary>
		public Dictionary<string, Button> Tools 
        {
			get
            {
				_CheckWarning();
				return _tools;
			}
			protected set
            {
				_tools = value;
			}
		}
		private Dictionary<string, GameObject> _glows;
		/// <summary>
		/// Holds each UI element's glow object. This element is animated to show a glowing animated, meaning the user 
		/// should click on the element.
		/// </summary>
		public Dictionary<string, GameObject> Glows 
        {
			get
            {
				_CheckWarning();
                return _glows;
            }
			protected set
            {
				_glows = value;
			}
        }
		private Dictionary<GameObject, bool> _clicked = new Dictionary<GameObject, bool>();
		/// <summary>
		/// All the buttons inside the /toolsContainer/ (including the toolsContainer). When a button (key) is clicked its value will be true.
		/// </summary>
		/// <value>The clicked.</value>
		public Dictionary<GameObject, bool> Clicked 
        {
			get
            {
				_CheckWarning();
                return _clicked;
            }
			protected set
            {
				_clicked = value;
			}
        }


        private const string GLOW = "Glow";



		void Awake()
		{
			#region VALIDATE / ASSIGN
			if( !toolsContainer.m_gameObject )
			{
                Utilities.LogWarning( "Tools Container hasn't been assigned!", gameObject );
				enabled = false;
				return;
			}
			ToolsContainer = toolsContainer.GetComponent<UIPopUpMenuController>();
			if( !ToolsContainer )
			{
                Utilities.LogWarning( "The Tools Container has no UIPopUpMenuController.cs attached", toolsContainer.m_gameObject );
				enabled = false;
				return;
			}
			if( !currentTool.m_gameObject )
			{
                Utilities.LogWarning( "Current Tool hasn't been assigned!", gameObject );
				enabled = false;
				return;
			}
			ToolsContainerBt = toolsContainer.GetComponent<Button>();
			CurrentTool = currentTool.m_object;
			CurrentToolBt = currentTool.m_gameObject.GetComponentInChildren<Button>();
			var tools = toolsContainer.GetComponentsInChildrenExcludeThis<Button>();
            Tools = new Dictionary<string, Button>();
			//FILL THE GLOWS AND TOOLS DICTIONARY
			Glows = new Dictionary<string, GameObject>();
            Glows.Add( toolsContainer.objName, toolsContainer.m_transform.Find( toolsContainer.objName + GLOW ).gameObject );
			if( CurrentTool )
			{
                Glows.Add( currentTool.objName, currentTool.m_transform.Find( CurrentTool.transform.parent.name + GLOW  ).gameObject );
				Tools.Add( currentTool.objName, CurrentToolBt );
			}
			for( int i=0; i<tools.Length; i++ )
			{
				if( i == 0 )
				{
					_toolsParent = tools[i].transform.parent;
				}
                Glows.Add( tools[i].name, tools[i].transform.Find( tools[i].name + GLOW ).gameObject );
				Tools.Add( tools[i].name, tools[i] );
			}
			//FILL THE CLICKED DICTIONARY
			Clicked = new Dictionary<GameObject, bool>();
			Clicked.Add( toolsContainer.m_gameObject, false );
			Clicked.Add( currentTool.m_gameObject, false );
			for( int i=0; i<tools.Length; i++ )
			{
				Clicked.Add( tools[i].gameObject, false );
            }
            //ADD BACKPACK AND TOOLS EVENTS
            ToolsContainerBt.onClick.AddListener( () => {
                _OnToolClick( ToolsContainerBt.gameObject );
            } );
            for (int i = 0; i < tools.Length; i++)
            {
                var go = tools[i].gameObject;
                tools[i].onClick.AddListener( () => { _OnToolClick( go ); } );
            }
            #endregion
        }
		void Start(){}//To allow this component to be disabled


		/// <summary>
		/// Logs a warning message into the console if this component is disabled.
		/// </summary>
		protected void _CheckWarning()
		{
			if( enabled )
				return;
            Utilities.LogWarning( "This UIToolsContainer is disabled, check if a variable hasn't been set", gameObject );
		}
		/// <summary>
		/// Returns true if the specified /toolName/ is not null/empty, and a tool with that name is inside the
		/// Tools Container.
		/// </summary>
		/// <returns><c>true</c>, if is tool name valid was _ed, <c>false</c> otherwise.</returns>
		/// <param name="toolName">Tool name.</param>
		protected bool _IsToolNameValid( string toolName )
		{
			if( string.IsNullOrEmpty( toolName ) )
			{
                Utilities.LogWarning ( "No tool name has been specified", gameObject );
				return false;
			}
			if( !Tools.ContainsKey( toolName ) )
			{
                Utilities.LogWarning ( string.Format( "There is no Tool named: {0}", toolName ), gameObject );
				return false;
			}
			return true;
		}


        #region ACTIONS
        public void ResetClickListeners()
        {
            ToolsContainerBt.onClick.RemoveAllListeners();
            ToolsContainerBt.onClick.AddListener( () => {
                _OnToolClick( ToolsContainerBt.gameObject );
            } );
            for (int i = 0; i < Tools.Count; i++)
            {
                Button button = Tools.ElementAt(i).Value;

                if (button == null) 
                    continue;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener( () => {
                    _OnToolClick( button.gameObject );
                } );
            }
        }
        public void ShowTools()
		{
			ToolsContainer.ShowMenu();
		}
		public void HideTools()
		{
			ToolsContainer.HideMenu();
		}
		/// <summary>
		/// Makes the specified UI button glow.
		/// </summary>
		/// <param name="btName">The name of the object that should glow.</param>
		public void Glow( string btName )
		{
			if( string.IsNullOrEmpty( btName ) || !Glows.ContainsKey( btName ) )
				return;
			Glows[ btName ].SetActive( true );//in case it has been deactivated
			Glows[ btName ].AnimAlphaPingPong( 1f, glowAnimationsDuration, repeatGlow );
		}
		/// <summary>
		/// Stops the specified UI button's glow animation.
		/// </summary>
		/// <param name="btName">The name of the object that should stop glowing.</param>
		public void StopGlow( string btName )
		{
			if( string.IsNullOrEmpty( btName ) || !Glows.ContainsKey( btName ) )
				return;
			Glows[ btName ].SetActive( false );
		}
		/// <summary>
		/// Make the specified UI button (name) glows, and wait for the user to click on it.
		/// </summary>
		public void GlowWaitForClick( string btName )
		{
            StartCoroutine( GlowWaitForClickCo( btName, false ) );
		}
		/// <summary>
		/// Make the Tools Container glow and wait for the user to click on it.
		/// </summary>
		public void GlowWaitForToolsContainerClick()
		{
			StartCoroutine( GlowWaitForClickCo( toolsContainer.m_gameObject ) );
		}
		public void SetCurrentTool( string toolName, bool show )
		{
			if( toolName.Equals( toolsContainer.objName ) || !Tools.ContainsKey( toolName ) )
				return;
			CurrentTool.SetGraphicMainValue( Tools[ toolName ].targetGraphic );
			if( show )
			{
				ShowCurrentTool();
			}
		}
		public void SetAndShowCurrentTool( string toolName )
		{
			SetCurrentTool( toolName, true );
		}
		public void SetCurrentTool( string toolName )
		{
			SetCurrentTool( toolName, false );
		}
		public void SetCurrentTool( Graphic tool )
		{
			if( !tool )
				return;
			SetCurrentTool( tool.name );
		}
		public void SetCurrentTool( GameObject toolObj )
		{
			if( !toolObj )
				return;
			SetCurrentTool( toolObj.name );
		}
		public void AddOnClickListenersToCurrentTool( UnityEngine.Events.UnityAction[] actions )
		{
			for( int i=0; i<actions.Length; i++ )
			{
				if( actions[i] == null )
					continue;
				CurrentToolBt.onClick.AddListener( actions[i] );
			}
		}
		public void AddOnClickListenersToCurrentTool( Button actions )
		{
			CurrentToolBt.onClick.AddListener( actions.onClick.Invoke );
		}
		public void AddOnClickListenersToCurrentTool( SingleComposedEvent actions )
		{
			CurrentToolBt.onClick.AddListener( actions.Invoke );
		}
		public void AddOnClickListenersToCurrentTool( ComposedEvent actions )
		{
			CurrentToolBt.onClick.AddListener( () => actions.Invoke() );
		}
        public void AddOnClickListenersToToolsContainer( Button actions )
        {
            ToolsContainerBt.onClick.AddListener( actions.onClick.Invoke );
        }
        public void AddOnClickListenersToToolsContainer( SingleComposedEvent actions )
        {
            ToolsContainerBt.onClick.AddListener( actions.Invoke );
        }
        public void AddOnClickListenersToToolsContainer( ComposedEvent actions )
        {
            ToolsContainerBt.onClick.AddListener( () => actions.Invoke() );
        }
        public void ShowCurrentTool()
		{
			CurrentTool.enabled = true;
			CurrentTool.AnimAlpha( 1f );
		}
		public void HideCurrentTool()
		{
			CurrentTool.AnimAlpha( 1f, 0.4f, () => CurrentTool.enabled = false );
		}
		/// <summary>
		/// This will just instantiate/activate the specified prefab and parent it to the /_toolsParent/.
		/// </summary>
		public void AddTool( GameObject toolPrefab )
		{
			if( toolPrefab == null )
			{
                Utilities.LogWarning ( "AddTool: No tool prefab has been specified", gameObject );
				return;
			}
			GameObject tool = toolPrefab.SetActiveInHierarchy();
			tool.name = toolPrefab.name;
			tool.SetParent( _toolsParent );
			var toolBt = tool.GetComponent<Button>();
			if( toolBt == null )
			{
                Utilities.LogWarning ( "AddTool: The specified tool prefab has no Button component", gameObject );
				return;
			}
			Tools.Add( tool.name, toolBt );
			toolBt.onClick.AddListener( () => {
				_OnToolClick( tool );
			} );
			Clicked.Add( tool, false );
			//ADD GLOW
			if( Glows.Count == 0 )
				return;
            Glows.Add( tool.name, tool.transform.Find( tool.name + GLOW ).gameObject );
		}
		/// <summary>
		/// This will search for the specified tool and delete the object from the container.
		/// </summary>
		public void DeleteTool( string toolName )
		{
			if( !_IsToolNameValid( toolName ) )
				return;
			Destroy ( Tools[ toolName ].gameObject );
			Tools.Remove( toolName );
		}
		/// <summary>
		/// This will search for the specified tool and remove its Image's sprite. The name of the tool will remain the
		/// same until the slot gets replaced by another tool.
		/// </summary>
		public void RemoveTool( string toolName )
		{
			if( !_IsToolNameValid( toolName ) )
				return;
            Image tool = (Image) Tools[ toolName ].targetGraphic;
            if( tool )
			{
				tool.sprite = null;
			}
			else
			{
                Utilities.LogWarning ( "The specified tool \"" + toolName + "\" has no Image", gameObject );
			}
		}
		/// <summary>
		/// This will search for the specified tool and replace its Image's sprite. The name of the tool will be the
		/// name of the sprite.
		/// </summary>
		public void ReplaceTool( string toolToReplace, Sprite newTool )
		{
			if( newTool == null )
			{
                Utilities.LogWarning ( "ReplaceTool: No Sprite has been specified", gameObject );
				return;
			}
			if( !_IsToolNameValid( toolToReplace ) )
				return;
			RemoveTool( toolToReplace );
            Image tool = (Image) Tools[ toolToReplace ].targetGraphic;
            if( tool )
			{
				tool.sprite = newTool;
			}
		}
		#endregion

		#region ACTIONS COROUTINES
		/// <summary>
		/// Make the specified UI button (name) glows, and wait for the user to click on it.
		/// </summary>
		public IEnumerator GlowWaitForClickCo( string btName, bool isTag )
		{
			if( Tools.ContainsKey( btName ) )
			{
				yield return StartCoroutine( GlowWaitForClickCo( Tools[ btName ].gameObject ) );
				yield break;
			}
			GameObject bt = isTag ? btName.FindWithTag() : btName.Find();
			yield return StartCoroutine( GlowWaitForClickCo( bt ) );
		}
		/// <summary>
		/// Make the specified UI button glow, and wait for the user to click on it.
		/// </summary>
		public IEnumerator GlowWaitForClickCo( GameObject button )
		{
			if( !button )
				yield break;
			var bt = button.GetComponent<Button>();
			if( !bt )
			{
                Utilities.LogWarning ("The specified /button/ has no Button component");
				yield break;
			}
			//MAKE THE BUTTON INTERACTABLE
			bt.interactable = true;
			
            float time;
			while( !Clicked[ button ] )
			{
				Glow( button.name );
				
                time = Time.time + ( glowAnimationsDuration * (repeatGlow + 1) ) + glowInterval;
                while( time > Time.time )
				{
					//IF IT ISN'T THE BACKPACK BUTTON OR THE CURRENT TOOL BUTTON, BACKPACK'S POP-UP MENU MUST BE ACTIVE
					if( button != toolsContainer.m_gameObject && button != currentTool.m_gameObject && !ToolsContainer.isActive )
					{
                        yield return GlowWaitForToolsContainerClickCo();
					}
					else if( Clicked[ button ] )
					{
						if( button != toolsContainer.m_gameObject ) //Hide Menu when button is clicked
						{
							HideTools();
						}
						break;
					}
					yield return null;
				}
				yield return null;
			}
			StopGlow( button.name );
			Clicked[ button ] = false;
			bt.interactable = false;
		}
		/// <summary>
		/// Make the backpack glow and wait for the user to click on it.
		/// </summary>
		public IEnumerator GlowWaitForToolsContainerClickCo()
		{
			yield return StartCoroutine( GlowWaitForClickCo( toolsContainer.m_gameObject ) );
		}
		#endregion

		#region PROTECTED/PRIVATE EVENTS
		/// <summary>
		/// This is called when clicking on tool.
		/// </summary>
        /// <param name="button">The gameObject (button) being clicked.</param>
		protected void _OnToolClick( GameObject button )
		{
			if( !button )
				return;
			Clicked[ button ] = true;
			SetCurrentTool( button );
			//PLAY CLIP
			for( int i=0; i<onClickSfx.Length; i++ )
			{
				if( onClickSfx[i].toolButtonName.Equals( button.name ) || string.IsNullOrEmpty( onClickSfx[i].toolButtonName ) )
				{
					SfxManager.PlaySfx( onClickSfx[i].sfx, true, gameObject );
					break;
				}
			}
			StartCoroutine( _ResetClicked( button ) );
        }
		protected IEnumerator _ResetClicked( GameObject button, float delay = 0.1f )
		{
			yield return new WaitForSeconds( delay );
			if( !button || !Clicked.ContainsKey( button ) )
				yield break;
			Clicked[ button ] = false;
		}
        #endregion

    }
}
