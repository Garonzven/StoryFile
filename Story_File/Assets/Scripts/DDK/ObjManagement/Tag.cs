//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.ObjManagement 
{
	/// <summary>
	/// Allows setting an object's tag, this will remain exporting the object to another project which doesn't happen 
	/// when manually creating a tag from the inspector.
	/// </summary>
    public class Tag : MonoBehaviour //THIS HAS AN EDITOR CLASS
    {
        [SerializeField]
        private string _tag;
        [Tooltip("This only works in Editor Mode")]
		public bool createTag;



		// Use this for initialization
		void Awake () 
        {
			tag = _tag;
		}
	}

}
