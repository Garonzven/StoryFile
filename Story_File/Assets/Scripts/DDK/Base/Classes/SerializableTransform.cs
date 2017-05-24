//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;


namespace DDK.Base.Classes 
{
	[System.Serializable]
	public class SerializableTransform 
    {
		public SerializableTransform( Transform t )
		{
			_Init( t );
		}


		public Vec<float> position;
		public Vec<float> rotation;
		public Vec<float> scale;


		private void _Init( Transform transform )
		{
			position = transform.position.Serialize();
			rotation = transform.rotation.eulerAngles.Serialize();
			scale = transform.localScale.Serialize();
		}
		
		public void DeserializeInto( Transform transform )
		{
			transform.position = position.Deserialize3();
			transform.rotation = Quaternion.Euler( rotation.Deserialize3() );
			transform.localScale = scale.Deserialize3();
		}
	}
}