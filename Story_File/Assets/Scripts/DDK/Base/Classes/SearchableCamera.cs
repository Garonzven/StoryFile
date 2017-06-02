using UnityEngine;
using System.Collections;


namespace DDK.Base.Classes 
{
    /// <summary>
    /// This class represents a Camera, and it can be used to reference a Camera without losing its reference since
    /// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
    /// </summary>
	[System.Serializable]
	public class SearchableCamera : SearchableBehaviour<Camera> 
	{
		public SearchableCamera(){}
		public SearchableCamera( string objName )
		{
			if( !string.IsNullOrEmpty( objName ) )
				base.objName = objName;
		}
	}
}