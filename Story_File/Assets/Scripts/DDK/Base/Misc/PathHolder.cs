using UnityEngine;
using System.Collections;


namespace DDK.Base.Misc 
{

	/// <summary>
	/// THIS SHOULD ONLY HAVE ONE INSTANCE. This is used to reference multiple paths for resources loading, to avoid modifying multiple 
	/// references in multiple objects or fix them when they get missing. In other words, this centrilizes all paths references.
	/// <para></para>
	/// Usage Example:
	/// <para></para>
	/// - Attach to an object, and add some paths.
	/// <para></para>
	/// - Set a public PathHolder.Index or int variable on any script to allow specifying the Path Holder's index (path) that wants to be referenced/accessed.
	/// <para></para>
	/// - Call the variable's path property or PathHolder.GetPath(index) to get the path.
	/// </summary>
	public class PathHolder : MonoBehaviour 
    {
		[System.Serializable]
		public class Index 
        {
			[Tooltip("The index that represents a Path Holder's path")]
			public int pathIndex = -1;


			/// <summary>
			/// Returns true if the specified index is valid. If it is, but the path is empty it will return false.
			/// </summary>
			public bool isValid 
            {
				get
                {
					if( !_Instance )
					{
						return false;
					}
					if( pathIndex < Instance.paths.Length )
					{
						if( !string.IsNullOrEmpty( GetPath( pathIndex ) ) )
						{
							return true;
						}
					}
					return false;
				}
			}
			public string path 
            {
				get{ return PathHolder.GetPath( pathIndex ); }
			}
		}


		public string[] paths;


		private static PathHolder _Instance;
		public static PathHolder Instance 
        {
			get
            {
				if( !_Instance )
				{
					Debug.LogWarning("There is no active Path Holder, returning null...");
					return null;
				}
				return _Instance;
			}
		}


		void Awake()
		{
			if( _Instance )
			{
				Debug.LogError("THERE SHOULD ONLY BE ONE INSTANCE OF THIS CLASS", gameObject);
				Destroy( this );
				return;
			}
			_Instance = this;
		}


		public static string GetPath( int index )
		{
			if( index < 0 )
			{
				return "";
			}
			if( index < Instance.paths.Length )
			{
				return Instance.paths[index];
			}
			else Debug.LogError("The specified index exceeds the paths array length, returning empty string...", Instance.gameObject);
			return "";
		}
		public static void DestroyInstance()
		{
			Destroy( _Instance );
		}
	}
}
