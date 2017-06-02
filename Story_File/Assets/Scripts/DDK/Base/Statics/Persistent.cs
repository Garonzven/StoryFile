//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.Base.Statics 
{
	/// <summary>
	/// This class allows saving and loading objects to the persistent data path.
	/// </summary>
	public static class Persistent 
    {
		/// <summary>
		/// Save the specified obj with the specified key (playerName), if saveToPersistentDataPath is true the obj will be
		/// saved to the device's persistent data path; otherwise, you must manually call PlayerPrefs.Save(). Check PlayerPrefs docs for more information.
		/// </summary>
		/// <param name="playerName">Player name.</param>
		/// <param name="obj">Object.</param>
		/// <param name="saveToPersistentDataPath">If set to <c>true</c> save to persistent data path.</param>
		public static void Save<T>( string playerName, T obj, bool saveToPersistentDataPath = false )
		{
			var manager = new SaveLoadController<T>();
			manager.Save( playerName, obj, saveToPersistentDataPath );
		}
		/// <summary>
		/// Load the specified obj from PlayerPrefs or the persistent data path.
		/// </summary>
		/// <param name="playerName">Player name.</param>
		public static T Load<T>( string playerName )
		{
			var manager = new SaveLoadController<T>();
			return manager.Load( playerName );
		}
		/// <summary>
		/// Serialize the specified object.
		/// </summary>
		public static string Serialize<T>( T obj )
		{
			var manager = new SaveLoadController<T>();
			return manager.Serialize( obj );
		}
		/// <summary>
		/// Deserialize the specified data.
		/// </summary>
		public static T Deserialize<T>( string data )
		{
			var manager = new SaveLoadController<T>();
			return manager.Deserialize( data );
		}            					
	}
}