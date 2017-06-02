//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;


namespace DDK.Base.Classes 
{
    /// <summary>
    /// This class represents a Renderer, and it can be used to reference a Renderer without losing its reference since
    /// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
    /// </summary>
	[System.Serializable]
	public class SearchableRenderer : SearchableComponent<Renderer> 
	{
		public SearchableRenderer(){}
		public SearchableRenderer( string objName )
		{
			if( !string.IsNullOrEmpty( objName ) )
				base.objName = objName;
        }
    }
}