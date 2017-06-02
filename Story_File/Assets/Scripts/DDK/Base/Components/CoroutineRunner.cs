//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;


namespace DDK.Base.Components 
{
	/// <summary>
	/// DO NOT ATTACH DIRECTLY. This class allows OtherExt.cs Start IEnumerator extension method to start coroutines on an object that won't be destroyed 
	/// until the game finishes executing.
	/// </summary>
	[ExecuteInEditMode]
	public class CoroutineRunner : MonoBehaviour 
    {
		void Awake()
		{
			DontDestroyOnLoad( gameObject );
		}

		//FIX BUG THAT SOMETIMES PREVENTS OBJECT FROM BEING DESTROYED
#if UNITY_EDITOR
		void Update()
		{
			if( !Application.isPlaying )
			{
				DestroyImmediate( gameObject );
			}
		}
#endif		
	}
}
