//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.GamesDevelopment {

	/// <summary>
	/// Attach to an object with children to randomly switch their positions. To stop the switching set the internal 
	/// variable _stop to true.
	/// </summary>
	public class ChildrenPositionSwitchController : MonoBehaviour {

		/// <summary>
		/// Name of the children to exclude.
		/// </summary>
		public string[] excludeChildrenContaining;
		/// <summary>
		/// Pairs to switch at a time. Example: If this is 2, and switchSpeed is 1, two pairs of objects will be switched
		/// per second.
		/// </summary>
		[Tooltip("Pairs to switch at a time. Example: If this is 2, and switchSpeed is 1, two pairs of objects will be switched per second")]
		public int switchPairs = 1;
		/// <summary>
		/// The times a switch can be done in a second. Example: 0.5f = two switches per second.
		/// </summary>
		[Tooltip("The times a switch can be done in a second. Example: 0.5f = two switches per second.")]
		public float switchSpeed = 1f;
		/// <summary>
		/// The switch speed. Higher values will make the switching objects move faster.
		/// </summary>
		[Tooltip("The switch speed. Higher values will make the switching objects move faster.")]
		public float switchMoveSpeed = 1f;
		/// <summary>
		/// If true the objects to switch must share the X position.
		/// </summary>
		[Tooltip("If true the objects to switch must share the X position.")]
		public bool mustShareX = true;
		/// <summary>
		/// If true the objects to switch must share the Y position.
		/// </summary>
		[Tooltip("If true the objects to switch must share the Y position.")]
		public bool mustShareY = true;
		/// <summary>
		/// If true the objects to switch must share the Z position.
		/// </summary>
		[Tooltip("If true the objects to switch must share the Z position.")]
		public bool mustShareZ = false;
		/// <summary>
		/// If true, the mustShare... variables are evaluated as atLeastShare.. Meaning that just one of them needs to be true, 
		/// if its respective variable is also.
		/// </summary>
		[Tooltip("If true, the mustShare... variables are evaluated as atLeastShare.. Meaning that just one of them needs to be true, if its respective variable is also.")]
		public bool mustIsJustAtLeast = true;
		/// <summary>
		/// The max separation between the objects to switch.
		/// </summary>
		[Tooltip("The max separation between the objects to switch.")]
		public float maxSeparation = 10f;
		[Tooltip("The initial delay before starting to switch")]
		public float delay;
		[Tooltip("If this becomes true, the switching will be paused.")]
		public bool pause;
		
		
		
		
		
		/// <summary>
		/// This must become true for the switching to stop.
		/// </summary>
		internal bool _stop;
		
		
		
		
		
		
		
		// Use this for initialization
		void Start () {
			
			StartCoroutine( StartSwitching() );
		}
		
		// Update is called once per frame
		void Update () {
			
		}
		
		
		
		
		
		
		public IEnumerator StartSwitching()
		{
			yield return null;
			yield return new WaitForSeconds( delay );
			while( true )
			{
				yield return null;
				while( pause || !enabled )
				{
					yield return null;
				}
				if( _stop ) 
					break;
				
				//GET PAIRS AND SWITCH THEM
				var children = gameObject.GetChildren( excludeChildrenContaining );
				switchPairs = Mathf.Clamp( switchPairs, 1, children.Count );
				for( int i=0; i<switchPairs; i++ )
				{
					int pair1 = Random.Range( 0, children.Count );
					int pair2;
					while( true )//PAIR 2 MUST BE DIFFERENT FROM PAIR 1
					{
						pair2 = Random.Range( 0, children.Count );
						if( pair2 != pair1 )
						{
							if( children[pair1].DistanceTo( children[pair2] ) < maxSeparation )//VALIDATE
							{
								bool Break = false;
								if( mustShareX )
								{
									if( children[pair1].Position().x.CloseTo( children[pair2].Position().x, 0.0001f ) )
										Break = true;
									if( mustIsJustAtLeast && Break )
									{
										break;
									}
								}
								if( mustShareY )
								{
									if( children[pair1].Position().y.CloseTo( children[pair2].Position().y, 0.0001f ) )
										Break = true;
									if( mustIsJustAtLeast && Break )
									{
										break;
									}
								}
								if( mustShareZ )
								{
									if( children[pair1].Position().z.CloseTo( children[pair2].Position().z, 0.0001f ) )
										Break = true;
								}
								if( Break )
									break;
							}
						}
						yield return null;
					}
					//SWITCH
					yield return StartCoroutine( SwitchPos( children[pair1], children[pair2] ) );
				}
				
				yield return new WaitForSeconds( switchSpeed );
			}
		}

		/// <summary>
		/// Switchs the specified objs positions, animating each object to the other's position.
		/// </summary>
		public IEnumerator SwitchPos( GameObject obj1, GameObject obj2 )
		{
			var pos1 = obj1.Position();
			var pos2 = obj2.Position();
			while( obj1.Position() != pos2 && obj2.Position() != pos1 )
			{
				yield return null;
				obj1.MoveTowards( pos2, switchMoveSpeed );
				obj2.MoveTowards( pos1, switchMoveSpeed );
			}
		}
		
		
		
	}


}