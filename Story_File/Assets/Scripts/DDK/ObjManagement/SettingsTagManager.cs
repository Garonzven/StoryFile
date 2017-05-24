//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.ObjManagement {
	
	/// <summary>
	/// Allows setting all tags, this will ease the tags creation process when joining multiple projects.
	/// </summary>
	public class SettingsTagManager : MonoBehaviour {//THIS HAS AN EDITOR CLASS
		
		public string[] _tags;
		[Tooltip("This only works in Editor Mode")]
		public bool createTags;

	}
	
}
