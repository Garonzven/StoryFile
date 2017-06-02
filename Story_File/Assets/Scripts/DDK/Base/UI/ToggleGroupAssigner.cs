//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Classes;
using UnityEngine.UI;


namespace DDK.Base.UI
{
    /// <summary>
    /// Allows setting up a Toggle's ToggleGroup. This works with indirect references.
    /// </summary>
	[RequireComponent( typeof( Toggle ) )]
	public class ToggleGroupAssigner : MonoBehaviour 
	{
		[System.Serializable]
		public class SearchableToggleGroup : SearchableBehaviour<ToggleGroup> 
		{
			public SearchableToggleGroup(){}
			public SearchableToggleGroup( string objName )
			{
				if( !string.IsNullOrEmpty( objName ) )
					base.objName = objName;
			}
		}


		[Tooltip("If true, this toggle's group will be set only if it has no group")]
		public bool onlyIfNoGroup = true;
		public bool onEnable;
		public SearchableToggleGroup toggleGroup = new SearchableToggleGroup();

		protected Toggle _toggle;


		void Awake()
		{
			_toggle = GetComponent<Toggle>();
		}
		void OnEnable()
		{
			if( !onEnable )
				return;
			Assign();
		}
		// Use this for initialization
		void Start () {

			if( onEnable )
				return;
			Assign();
		}


		public void Assign()
		{
			if( onlyIfNoGroup && _toggle.group )
			{
				return;
			}
			_toggle.group = toggleGroup.m_object;
		}
	}
}
