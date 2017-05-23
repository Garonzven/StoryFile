//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Classes;


namespace DDK.Base.ScriptableObjects 
{
	[CreateAssetMenu( fileName = "Transforms", menuName = "Scriptable Objects/Transforms")]
	public class Transforms : ScriptableObject 
    {		
		public IntTransformDictionary transforms;
	}
}
