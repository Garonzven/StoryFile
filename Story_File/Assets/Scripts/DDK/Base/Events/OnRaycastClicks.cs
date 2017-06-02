//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Classes;
using DDK.Base.Statics;


namespace DDK.Base.Events 
{
	/// <summary>
	/// Attach to an object that will handle multiple raycast on click events.
	/// </summary>
	public class OnRaycastClicks : MonoBehaviour 
    {		
		[System.Serializable]
		public class Target 
        {			
			public SearchableGameObject target = new SearchableGameObject();
			[Tooltip("The mouse button index that will be used to click on the /target/")]
			public int btIndex = 0;
			public ComposedEvent onMouseDown = new ComposedEvent();
			public ComposedEvent onMouseUp = new ComposedEvent();
		}
		
		
		public Target[] targets = new Target[0];

		
        void Start () {} //Allows enabling/disabling this component
		// Update is called once per frame
		void Update () 
        {			
			CheckHits();
		}
		
		void CheckHits()
		{
			for( int i=0; i<targets.Length; i++ )
			{
				if( !targets[i].target.m_gameObject )
					continue;
				CheckHitOnMouseDown( targets[i], targets[i].btIndex );
				CheckHitOnMouseUp( targets[i], targets[i].btIndex );
			}
		}
		void CheckHitOnMouseDown( Target target, int btIndex )
		{
			if( !Input.GetMouseButtonDown( btIndex ) )
				return;
            CheckHit( target, btIndex, target.onMouseDown );
		}
		void CheckHitOnMouseUp( Target target, int btIndex )
		{
			if( !Input.GetMouseButtonUp( btIndex ) )
				return;
            CheckHit( target, btIndex, target.onMouseUp );
		}
        void CheckHit( Target target, int btIndex, ComposedEvent action )
		{
			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hitInfo );
			if(! hit ) 
				return;
			Utilities.Log( "Hit " + hitInfo.transform.gameObject.name );
			if( hitInfo.transform.gameObject == target.target.m_gameObject )
			{
                action.Invoke();
			}
		}
		
	}
}