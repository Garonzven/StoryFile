//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;


namespace DDK._3D 
{
	/// <summary>
	/// Attach to an object to specify a destruction zone. The objects with the specified tags will be destroyed when
	/// they enter the zone (trigger).
	/// </summary>
	[RequireComponent( typeof( BoxCollider ) )]
	public class DestructionZone : MonoBehaviour 
    {
		public string[] objsWithTags = new string[]{ "Untagged" };



		// Use this for initialization
		void Start () 
        {
			GetComponent<BoxCollider>().isTrigger = true;
		}


		public void OnTriggerEnter( Collider other )
		{
			if( objsWithTags.Contains( other.tag ) )
			{
				Destroy( other.gameObject );
			}
		}
	}
}
