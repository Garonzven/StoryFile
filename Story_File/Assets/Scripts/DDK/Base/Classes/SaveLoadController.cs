//By: Daniel Soto
//4dsoto@gmail.com
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
//You must include these namespaces
//to use BinaryFormatter
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;



namespace DDK.Base.Classes 
{
	/// <seealso cref="Persistent"/>
	public class SaveLoadController<T> 
    {		
		/// <summary>
		/// Holds the last saved or loaded object.
		/// </summary>
		internal T obj;
		
		/// <summary>
		/// Save the specified obj with the specified key (playerName), if saveToPersistentDataPath is true the obj will be
		/// saved to the device's persistent data path. Check PlayerPrefs docs for more information.
		/// </summary>
		/// <param name="playerName">Player name.</param>
		/// <param name="obj">Object.</param>
		/// <param name="saveToPersistentDataPath">If set to <c>true</c> save to persistent data path; otherwise, you must manually call PlayerPrefs.Save().</param>
		public void Save(string playerName, T obj, bool saveToPersistentDataPath = false)
		{
			#if UNITY_IOS
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
			#endif
			//Get a binary formatter
			var b = new BinaryFormatter();
			//Create an in memory stream
			var m = new MemoryStream();
			//Save the objs
			b.Serialize(m, obj);
			//Add it to player prefs
			PlayerPrefs.SetString(playerName, Convert.ToBase64String( m.GetBuffer() ) );
			if( saveToPersistentDataPath ) PlayerPrefs.Save();
			
			/*Persistent.objs[playerName] =*/ this.obj = obj;
		}
        /// <summary>
        /// Load the specified obj from PlayerPrefs or the persistent data path.
        /// </summary>
        /// <param name="playerName">Player name.</param>
        public T Load(string playerName)
        {
			#if UNITY_IOS
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
			#endif
			//Get the data
			var data = PlayerPrefs.GetString(playerName);
			//If not blank then load it
			if(!string.IsNullOrEmpty(data))
			{
				//Binary formatter for loading back
				var b = new BinaryFormatter();
				//Create a memory stream with the data
				var m = new MemoryStream(Convert.FromBase64String(data));
				//Load back the objs
				obj = (T)b.Deserialize(m);
				//Persistent.objs[playerName] = obj;
				return obj;
			}
			return default(T);//empty string
		}
		/// <summary>
		/// Serialize the specified object.
		/// </summary>
		public string Serialize( T obj )
		{
			#if UNITY_IOS
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
			#endif
			//Get a binary formatter
			var b = new BinaryFormatter();
			//Create an in memory stream
			var m = new MemoryStream();
			//Save the objs
			b.Serialize(m, obj);
			return Convert.ToBase64String( m.GetBuffer() );
		}
		/// <summary>
		/// Deserialize the specified data. Returns null if the specified string is null or empty
		/// </summary>
		public T Deserialize( string data )
		{
			#if UNITY_IOS
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
			#endif
			//If not blank then load it
			if( !string.IsNullOrEmpty( data ) )
			{
				//Binary formatter for loading back
				var b = new BinaryFormatter();
				//Create a memory stream with the data
				var m = new MemoryStream( Convert.FromBase64String( data ) );
				//Load back the objs
				obj = (T)b.Deserialize(m);
				return obj;
			}
			return default( T );
		}

	}
}