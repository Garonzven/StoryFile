//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK;


namespace DDK._Physics._2D
{
	[RequireComponent( typeof( Rigidbody2D ) )]
	public class RandomRigidbodyParams2D : MonoBehaviour 
    {
		[HelpBoxAttribute]
		public string msg = "If /from/ and /to/ values are the same no value will be applied";
		[Space(10f)]
        [ShowIfAttribute( "onStart", true )]
        public bool onAwake = false;
        [ShowIfAttribute( "onAwake", true )]
        public bool onStart = false;
		[Header("Gravity")]
		[DisplayNameAttribute( "Random Sign" )]
		public bool randomSignG;
		public float fromGravity;
		public float toGravity;
		[Header("Velocity")]
		[DisplayNameAttribute( "Random Sign" )]
		public bool randomSignV;
		public Vector2 fromVelocity;
		public Vector2 toVelocity;


		protected Rigidbody2D _rigidbody;



		void Awake()
		{
			//INIT
			_rigidbody = GetComponent<Rigidbody2D>();

            if( onAwake )
			{
				ApplyAll();
			}
		}
		// Use this for initialization
		void Start ()
        {
			if( onStart )
			{
				ApplyAll();
			}
		}


        /// <summary>
        /// Applies all the specified random params to the Rigidbody2D.
        /// </summary>
		public void ApplyAll()
		{
			_ApplyRandomGravity();
			_ApplyRandomVelocity();
		}
		void _ApplyRandomGravity()
		{
			if( fromGravity == toGravity )
				return;
			_rigidbody.gravityScale = Random.Range( fromGravity, toGravity ).RandomSign( randomSignG );
		}
		void _ApplyRandomVelocity()
		{
			if( fromVelocity == toVelocity )
				return;
			_rigidbody.drag = 0f;
			_rigidbody.velocity = new Vector2( Random.Range( fromVelocity.x, toVelocity.x ), Random.Range( fromVelocity.y, toVelocity.y ) ).RandomSign2( randomSignV );
		}
	}

}
