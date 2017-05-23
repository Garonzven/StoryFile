//By: Daniel Soto
//4dsoto@gmail.com
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine;


namespace DDK.Base.Classes 
{	
	[System.Serializable]
	public class EnableDisableState 
    {
		public EnableDisable enable = new EnableDisable();
		public EnableDisable disable = new EnableDisable();


		/// <summary>
		/// True if the states were set.
		/// </summary>
		internal bool _areSet;
		/// <summary>
		/// True if the states are being set. this becomes true while the states delay hasn't passed.
		/// </summary>
		internal bool _areBeingSet;
		
		

		/// <summary>
		/// Sets the states.
		/// </summary>
		public void SetStates()
		{
			if( this == null )
				return;
			if( enable == null || disable == null )
				return;
			enable.Enable();
			disable.Enable( false );
			_Set().Start();
		}
		/// <summary>
		/// Reverts the states.
		/// </summary>
		public void SetStatesRevert()
		{
			if( this == null )
				return;
			if( enable == null || disable == null )
				return;
			enable.Enable( false );
			disable.Enable();
			_Set( true ).Start();
		}


		IEnumerator _Set( bool reverting = false )
		{
			_areBeingSet = true;
			yield return new WaitForSeconds( ( enable.delay > disable.delay ) ? enable.delay : disable.delay );
			_areSet = !reverting;
			_areBeingSet = false;
		}
	}
}
