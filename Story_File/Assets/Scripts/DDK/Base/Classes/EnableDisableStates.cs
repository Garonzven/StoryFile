//By: Daniel Soto
//4dsoto@gmail.com
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine;

namespace DDK.Base.Classes 
{
	[System.Serializable]
	public class EnableDisableStates 
    {		
		public EnableDisableState onEnabled;
		public EnableDisableState onDisable;


		/// <summary>
		/// True if the states were set.
		/// </summary>
		internal bool _areSet;



		/// <summary>
		/// Sets the states.
		/// </summary>
		public void SetStates( bool enabled )
		{
			if( enabled )
			{
				onEnabled.SetStates();
			}
			else onDisable.SetStates();
			_Set().Start();
		}


		IEnumerator _Set()
		{
			while( onEnabled != null && !onEnabled._areSet ||
                   onDisable != null && !onDisable._areSet)
				yield return null;
			_areSet = true;
        }
	}
}
