//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;




namespace DDK._3D {

	/// <summary>
	/// Allows properly rotating the camera in editor mode. Sometimes if you try to rotate a camera using the Transform component it
	/// doesn't rotate properly.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent( typeof( Camera ) )]
	public class CameraEditorRotation : MonoBehaviour {

		public Vector3 rotation = new Vector3( 0f, 0f, 0f );
		public bool local;



		// Use this for initialization
		void Start () {

		}
		
#if UNITY_EDITOR
		// Update is called once per frame
		void Update () {

			if( !Application.isPlaying )
			{
				if( local )
				{
					GetComponent<Camera>().transform.localRotation = Quaternion.Euler( rotation );
				}
				else
				{
					GetComponent<Camera>().transform.rotation = Quaternion.Euler( rotation );
				}
			}
		}
#endif

	}

}
