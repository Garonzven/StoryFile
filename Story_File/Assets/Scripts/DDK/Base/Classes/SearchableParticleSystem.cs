//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Classes;


namespace DDK.Base.Classes
{
    /// <summary>
    /// This class represents a ParticleSystem, and it can be used to reference a ParticleSystem without losing its reference since
    /// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
    /// </summary>
	[System.Serializable]
	public class SearchableParticleSystem : SearchableComponent<ParticleSystem>
	{
		public SearchableParticleSystem(){}
		public SearchableParticleSystem( string objName )
		{
			if( !string.IsNullOrEmpty( objName ) )
				base.objName = objName;
		}
	}
}
