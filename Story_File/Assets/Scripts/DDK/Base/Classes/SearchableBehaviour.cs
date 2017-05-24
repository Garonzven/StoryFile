//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK;
using DDK.Base.Extensions;


namespace DDK.Base.Classes
{
	/// <summary>
	/// This class represents a Behaviour, and it can be used to reference a Behaviour without losing its reference since
	/// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
	/// </summary>
	[System.Serializable]
	public class SearchableBehaviour<T> : SearchableComponent<T> where T : Behaviour
	{		
		public SearchableBehaviour(){}
		public SearchableBehaviour( string objName )
		{
			if( !string.IsNullOrEmpty( objName ) )
				base.objName = objName;
		}
	}    
}