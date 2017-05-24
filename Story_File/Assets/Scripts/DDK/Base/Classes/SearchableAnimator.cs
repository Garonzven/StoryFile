//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;


namespace DDK.Base.Classes 
{
    /// <summary>
    /// This class represents an Animator, and it can be used to reference an Animator without losing its reference since
    /// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
    /// </summary>
	[System.Serializable]
	public class SearchableAnimator : SearchableBehaviour<Animator> 
	{
		public SearchableAnimator(){}
		public SearchableAnimator( string objName )
		{
			if( !string.IsNullOrEmpty( objName ) )
				base.objName = objName;
		}
	}
}
