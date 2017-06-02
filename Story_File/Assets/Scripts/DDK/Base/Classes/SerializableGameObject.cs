//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;



namespace DDK.Base.Classes {

	/// <summary>
	/// Allows Serializing important attributes of a gameobject. You can create a class with other attributes that inherits
	/// from this.
	/// </summary>
	[System.Serializable]
	public class SerializableGameObject {
		
		protected Vec<float>[] TRANSFORM = new Vec<float>[3];
		//If Camera
		protected float orthoSize;
		protected float fieldOfView;
		protected Vec<float> bgColor;
		
		public SerializableGameObject() {}
		public SerializableGameObject( GameObject gameobj )
		{
			TRANSFORM = gameobj.transform.Serialize();
			if( gameobj.GetComponent<Camera>() )
			{
				orthoSize = gameobj.GetComponent<Camera>().orthographicSize;
				fieldOfView = gameobj.GetComponent<Camera>().fieldOfView;
				bgColor = gameobj.GetComponent<Camera>().backgroundColor.Serialize();
			}
		}
		
		/// <summary>
		/// Deserialize the gameobject. NOTE: Use DeserializeTo() for better results.
		/// </summary>
		/// <param name="hasCamera">If set to <c>true</c> has camera.</param>
		public GameObject Deserialize(bool hasCamera = false)
		{
			GameObject go = new GameObject();
			go.transform.position = TRANSFORM[0].Deserialize3();
			go.transform.eulerAngles = TRANSFORM[1].Deserialize3();
			go.transform.localScale = TRANSFORM[2].Deserialize3();
			if( hasCamera )
			{
				go.AddComponent<Camera>();
				go.GetComponent<Camera>().orthographicSize = orthoSize;
				go.GetComponent<Camera>().fieldOfView = fieldOfView;
				go.GetComponent<Camera>().backgroundColor = bgColor.DeserializeColor();
			}
			return go;
		}
		/// <summary>
		/// Deserializes to the specified gameobject.
		/// </summary>
		/// <returns>The specified gameobject.</returns>
		/// <param name="gameobj">Gameobject.</param>
		public GameObject DeserializeTo(GameObject gameobj)
		{
			gameobj.transform.position = TRANSFORM[0].Deserialize3();
			gameobj.transform.eulerAngles = TRANSFORM[1].Deserialize3();
			gameobj.transform.localScale = TRANSFORM[2].Deserialize3();
			if( gameobj.GetComponent<Camera>() != null )
			{
				gameobj.GetComponent<Camera>().orthographicSize = orthoSize;
				gameobj.GetComponent<Camera>().fieldOfView = fieldOfView;
				gameobj.GetComponent<Camera>().backgroundColor = bgColor.DeserializeColor();
			}
			return gameobj;
		}
		
		
	}


}