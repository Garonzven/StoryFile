using UnityEngine;
using System.Collections;
#if USE_FINAL_IK
using RootMotion.FinalIK;
#endif


namespace DDK.Base.Classes 
{
#if USE_FINAL_IK
    /// <summary>
    /// This class represents an IK, and it can be used to reference an IK without losing its reference since
    /// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
    /// </summary>
	[System.Serializable]
	public class SearchableIK : SearchableBehaviour<IK> 
	{
		public SearchableIK(){}
		public SearchableIK( string objName )
		{
			if( !string.IsNullOrEmpty( objName ) )
				base.objName = objName;
		}
		public SearchableIK( IK obj )
		{
			if( !obj )
				base.m_object = obj;
		}
	}
#endif
}
