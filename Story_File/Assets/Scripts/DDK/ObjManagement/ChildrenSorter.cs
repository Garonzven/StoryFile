//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;



namespace DDK.ObjManagement {

	/// <summary>
	/// Attach to a game object to sort its childrens hierarchy order.
	/// </summary>
	public class ChildrenSorter : MonoBehaviourExt {

		[HelpBoxAttribute()]
		public string msg = "This sorts the children OnEnable() and then disables this component and executes the specified events";
		[InspectorButtonAttribute( "Sort", true )]
		public bool sort;
		public float eventsDelay;
		public ComposedEvent onDone = new ComposedEvent();


		// Use this for initialization
		void OnEnable () {

			Sort();
			enabled = false;
			onDone.Invoke ( eventsDelay );
		}


		/// <summary>
		/// Sorts the children. Events are not executed agfter sort is done, and this component is not disabled. Call OnEnable()
		/// if that's desired.
		/// </summary>
		public void Sort()
		{
			GameObject[] children = gameObject.GetChildren();
			int[] ranIndexes = new int[children.Length];
			//FILL RANDOM INDEXES
			for( int i=0; i<children.Length; i++ )
			{
				var ranIndex = Random.Range( 0, children.Length );
				if( !ranIndexes.Contains<int>( ranIndex ) )
				{
					ranIndexes[i] = ranIndex;
				}
				else if( ranIndex != 0 )
				{
					i--;
					continue;
				}
			}
			//SORT
			for( int i=0; i<children.Length; i++ )
			{
				children[i].transform.SetSiblingIndex( ranIndexes[i] );
			}
		}
	}

}
