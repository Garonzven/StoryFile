//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace DDK.Base.Components 
{
	/// <summary>
	/// This is used by other classes and extension methods to allow finding inactive gameObjects, which can't be found 
    /// by using GameObject.Find... functions.
	/// </summary>
	public class InactivesHolder : MonoBehaviour 
    {
        #if UNITY_EDITOR
        [HelpBoxAttribute]
        public string info = "This is used by other classes to allow finding inactive objects";
        #endif
		[Space(20f)]
		public GameObject[] inactiveObjs;


		/// <summary>
		/// The first Instance.
		/// </summary>
        public static InactivesHolder Instance { get; private set; }

		private static List<GameObject> _inactiveObjs;



		void Awake()
		{
			if( Instance == null )
			{
				_inactiveObjs = new List<GameObject>();
				Instance = this;
			}
			//THIS MIGHT BE ANOTHER INSTANCE
			_inactiveObjs.AddRange( inactiveObjs );
		}
		void Start () {} //Allows enabling/disabling this component



		/// <summary>
		/// Gets the inactive objects. Returns an empty array if InactivesHolder has no instance, or if it has no objects.
		/// </summary>
		/// <returns>The inactive objects.</returns>
		public static List<GameObject> GetInactiveObjs()
		{
			if( !Instance )
			{
				return new List<GameObject>(0);
			}
			return _inactiveObjs;
		}
		/// <summary>
		/// Gets the inactive object containing the specified tag. Returns /null/ if InactivesHolder has no instance, or if it has no objects.
		/// </summary>
		public static GameObject GetWithTag( string tag )
		{
			if( !Instance )
				return null;

			var objs = GetInactiveObjs();
			for( int i=0; i<objs.Count; i++ )
			{
				if( !objs[i] ) continue;
				if( objs[i].CompareTag( tag ) )
					return objs[i];
			}
			return null;
		}
		/// <summary>
		/// Gets the inactive objects containing the specified tag. Returns an empty list if InactivesHolder has no instance, or if it has no objects.
		/// </summary>
		public static List<GameObject> GetByTag( string tag )
		{
			if( !Instance )
				return new List<GameObject>();

			List<GameObject> objs = new List<GameObject>( GetInactiveObjs() );

			var iObjs = GetInactiveObjs();
            for( int i=0; i<objs.Count; i++ )
			{
				if( !iObjs[i].CompareTag( tag ) )
					objs.RemoveAt( i-- );
			}
			return objs;
		}
	}
}
