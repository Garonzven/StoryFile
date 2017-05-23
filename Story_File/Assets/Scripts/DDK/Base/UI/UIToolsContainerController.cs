//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using UnityEngine.UI;
using DDK.Base.Events;
using System.Collections;


namespace DDK.UI
{
	/// <summary>
	/// This allows controlling a UIToolsContainer from any other object. Useful when you need to avoid direct 
	/// references, in cases such as Asset Bundles workflow.
	/// </summary>
	/// <seealso cref="UIToolsContainer"/>
    /// <seealso cref="DDK.Base.Fx.Transitions.LevelLoader"/>
	public class UIToolsContainerController : MonoBehaviourExt
	{		
		#region CLASSES
        [System.Serializable]
        public class SearchableUIToolsContainer : SearchableBehaviour<UIToolsContainer> 
        {
            public SearchableUIToolsContainer(){}
            public SearchableUIToolsContainer( string objName )
            {
                if( !string.IsNullOrEmpty( objName ) )
                    base.objName = objName;
            }
        }
        [System.Serializable]
        public class ToolButton 
        {
            [ShowIfAttribute( "_UpdateName" )]
            public string name;
            public SearchableGameObject toolButton = new SearchableGameObject();
            public ComposedEvent onClick = new ComposedEvent();

            protected bool _UpdateName()
            {
                name = toolButton.objName;
                return false;
            }
        }
        #endregion


		public SearchableUIToolsContainer toolsContainer = new SearchableUIToolsContainer( "MainUICanvas" );
		[Header( "Events" )]
		public ToolButton[] toolsButtons = new ToolButton[0];
		/// <summary>
		/// If ReplaceTool( Sprite ) is called, this will specify which tool will be replaced. Call
		/// SetNextToolToReplace( string ) to set this value.
		/// </summary>
		[ReadOnlyAttribute]
		[Tooltip("If ReplaceTool( Sprite ) is called, this will specify which tool will be replaced. Call" +
			" SetNextToolToReplace( string ) to set this value.")]
		[Space( 10f )]
		public string nextToolToReplace;


		protected UIToolsContainer _toolsContainer 
        {
			get
            {
				return toolsContainer.m_object;
			}
		}


		// Use this for initialization
		void Start () 
        {
			//ADD TOOLS EVENTS
			for( int i=0; i<toolsButtons.Length; i++ )
			{
				string toolName = toolsButtons[ i ].name;
				if( !_toolsContainer.Tools.ContainsKey( toolName ) )
				{
                    Debug.LogWarning( string.Format( "No tool named {0} was found inside the container", toolName ), gameObject );
					continue;
				}
			    var button = toolsButtons[i];

                _toolsContainer.Tools[ toolName ].onClick.AddListener( () => {
                    button.onClick.Invoke();
                } );
            }
		}


		#region ACTIONS
	    public void ResetClickListeners()
	    {
	        _toolsContainer.ResetClickListeners();
	    }
		public void SetCurrentTool( string toolName, bool show )
		{
			_toolsContainer.SetCurrentTool( toolName, show );
		}
		public void SetAndShowCurrentTool( string toolName )
		{
			_toolsContainer.SetAndShowCurrentTool( toolName );
		}
		public void SetCurrentTool( string toolName )
		{
			_toolsContainer.SetCurrentTool( toolName );
		}
		public void SetCurrentTool( Graphic tool )
		{
			_toolsContainer.SetCurrentTool( tool );
		}
		public void SetCurrentTool( GameObject toolObj )
		{
			_toolsContainer.SetCurrentTool( toolObj );
		}
		public void AddOnClickListenersToCurrentTool( UnityEngine.Events.UnityAction[] actions )
		{
			_toolsContainer.AddOnClickListenersToCurrentTool( actions );
		}
		public void AddOnClickListenersToCurrentTool( Button actions )
		{
			_toolsContainer.AddOnClickListenersToCurrentTool( actions );
		}
		public void AddOnClickListenersToCurrentTool( SingleComposedEvent actions )
		{
			_toolsContainer.AddOnClickListenersToCurrentTool( actions );
		}
		public void AddOnClickListenersToCurrentTool( ComposedEvent actions )
		{
			_toolsContainer.AddOnClickListenersToCurrentTool( actions );
		}
        public void AddOnClickListenersToToolsContainer(Button actions)
        {
            _toolsContainer.AddOnClickListenersToToolsContainer(actions);
        }
        public void AddOnClickListenersToToolsContainer(SingleComposedEvent actions)
        {
            _toolsContainer.AddOnClickListenersToToolsContainer(actions);
        }
        public void AddOnClickListenersToToolsContainer(ComposedEvent actions)
        {
            _toolsContainer.AddOnClickListenersToToolsContainer(actions);
        }
        public void ShowCurrentTool()
		{
			_toolsContainer.ShowCurrentTool();
		}
		public void HideCurrentTool()
		{
			_toolsContainer.HideCurrentTool();
		}
		public void ShowTools()
		{
			_toolsContainer.ShowTools();
		}
		public void HideTools()
		{
			_toolsContainer.HideTools();
		}
		/// <summary>
		/// This will just instantiate/activate the specified prefab and parent it to the /_toolsParent/.
		/// </summary>
		public void AddTool( GameObject toolPrefab )
		{
			_toolsContainer.AddTool( toolPrefab );
		}
		/// <summary>
		/// This will search for the specified tool and delete the object from the container.
		/// </summary>
		public void DeleteTool( string toolName )
		{
			_toolsContainer.DeleteTool( toolName );
		}
		/// <summary>
		/// This will search for the specified tool and remove its Image's sprite. The name of the tool will remain the
		/// same until the slot gets replaced by another tool.
		/// </summary>
		public void RemoveTool( string toolName )
		{
			_toolsContainer.RemoveTool( toolName );
		}
		public void SetNextToolToReplace( string toolName )
		{
			nextToolToReplace = toolName;
		}
		/// <summary>
		/// This will search for the specified /nextToolToReplace/ (as set by SetNextToolToReplace(string) ) and 
		/// replace its Image's sprite. The name of the tool will be the name of the sprite.
		/// </summary>
		public void ReplaceTool( Sprite newTool )
		{
			ReplaceTool( nextToolToReplace, newTool );
		}
		/// <summary>
		/// This will search for the specified tool and replace its Image's sprite. The name of the tool will be the
		/// name of the sprite.
		/// </summary>
		public void ReplaceTool( string toolToReplace, Sprite newTool )
		{
			_toolsContainer.ReplaceTool( toolToReplace, newTool );
		}
		/// <summary>
		/// Makes the specified UI button glow.
		/// </summary>
		/// <param name="btName">The name of the object that should glow.</param>
		public void Glow( string btName )
		{
			_toolsContainer.Glow( btName );
		}
		/// <summary>
		/// Stops the specified UI button's glow animation.
		/// </summary>
		/// <param name="btName">The name of the object that should stop glowing.</param>
		public void StopGlow( string btName )
		{
			_toolsContainer.StopGlow( btName );
		}
		/// <summary>
		/// Make the specified UI button (name) glows, and wait for the user to click on it.
		/// </summary>
		public void GlowWaitForClick( string btName )
		{
			_toolsContainer.GlowWaitForClick( btName );
		}
		/// <summary>
		/// Make the specified UI button (name) glows, and wait for the user to click on it.
		/// </summary>
		public void GlowWaitForClick( string btName, bool isTag )
		{
			StartCoroutine( _toolsContainer.GlowWaitForClickCo( btName, isTag ) );
		}
		/// <summary>
		/// Make the Tools Container glow and wait for the user to click on it.
		/// </summary>
		public void GlowWaitForToolsContainerClick()
		{
			_toolsContainer.GlowWaitForToolsContainerClick();
		}
		/// <summary>
		/// Make the specified UI button (name) glows, and wait for the user to click on it.
		/// </summary>
		public IEnumerator GlowWaitForClickCo( string btName )
		{
			yield return _toolsContainer.GlowWaitForClickCo( btName, false ).Start();
		}
		/// <summary>
		/// Make the specified UI button (name) glows, and wait for the user to click on it.
		/// </summary>
		public IEnumerator GlowWaitForClickCo( string btName, bool isTag )
		{
			yield return _toolsContainer.GlowWaitForClickCo( btName, isTag ).Start();
		}
		/// <summary>
		/// Make the Tools Container glow and wait for the user to click on it.
		/// </summary>
		public IEnumerator GlowWaitForToolsContainerClickCo()
		{
			yield return _toolsContainer.GlowWaitForToolsContainerClickCo().Start();
		}
		#endregion
	}
}
