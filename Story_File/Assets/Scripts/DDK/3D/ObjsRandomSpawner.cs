//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using System.Collections.Generic;



namespace DDK._3D {

	/// <summary>
	/// Attach to an object to manage objs spawning in a 3D world.
	/// </summary>
	public class ObjsRandomSpawner : MonoBehaviour {

		[Tooltip("Objects to spawn")]
		public GameObject[] objs = new GameObject[0];
		public string objsParent;
		[Tooltip("The zone were the objects will be randomly spawned. Keep in mind that the objects will spawn in the spawn's zone Y position.")]
		public BoxCollider spawnZone;

		[Space(5f)]
		[Tooltip("If true, the objects will be moved depending on the values specified below. NOTE: If you want movement to be smoother user a rigidbody instead.")]
		public bool move;
		[ShowIfAttribute( "_Move", 1 )]
		public Vector3 minMovement = new Vector3( 0.01f, 0f, 0.01f );
		[ShowIfAttribute( "_Move", 1 )]
		public Vector3 maxMovement = new Vector3( 0.1f, 0f, 0.1f );
		[ShowIfAttribute( "_Move", 1 )]
		public bool local = true;
		[Header("Spawns Random Local Scale")]
		public bool matchXY;
		public Vector3 minScale = new Vector3( 1f, 1f, 1f );
		public Vector3 maxScale = new Vector3( 1f, 1f, 1f );

		[Space(5f)]
		[Tooltip("The max amount of spawns that can exist at a time")]
		public int maxSpawns = 20;
		[Tooltip("In seconds")]
		public float spawnRate = 1f;
					

		protected bool _Move()
		{
			return move;
		}
		
		
		protected int _activeSpawns;
		protected List<GameObject> _objs;
		protected GameObject _objsParent;
				
				
		
		// Use this for initialization
		public void Start () {

			if( spawnZone == null )
			{
				Debug.LogWarning( "There is no Spawn Zone..." );
				enabled = false;
			}

			InvokeRepeating( "Spawn", 0f, spawnRate );
			_objs = new List<GameObject>( maxSpawns );
			_objsParent = GameObject.Find( objsParent );
		}
		
		// Update is called once per frame
		public void Update () {
			
			for( int i=0; i<_objs.Count; i++ )
			{
				if( _objs[i] == null )
				{
					_objs.RemoveAt( i );
					ObjDestroyed();
					continue;
				}
				//MOVEMENT
				if( move )
				{
					if( local )
					{
						_objs[i].transform.localPosition = _objs[i].transform.localPosition.RandomAdd( minMovement, maxMovement );
					}
					else
					{
						_objs[i].transform.position = _objs[i].transform.position.RandomAdd( minMovement, maxMovement );
					}
				}
			}
		}				
		
		
		
		public virtual void Spawn()
		{
			if( _activeSpawns < maxSpawns )
			{
				if( objs.Length == 0 )
					return;
				GameObject spawn = objs.GetRandom<GameObject>().InstantiateInsideArea( spawnZone );

				if( _objsParent )
				{
					spawn.SetParent( _objsParent.transform, true );
				}
				spawn.transform.localScale = spawn.transform.localScale.GetRandom( minScale, maxScale );
				if( matchXY )
				{
					float scl = spawn.transform.localScale.x;
					spawn.transform.localScale = new Vector3( scl, scl, spawn.transform.localScale.z );
				}
				_objs.Add( spawn );

				_activeSpawns++;
			}
		}
		
		private void ObjDestroyed()
		{
			_activeSpawns--;
		}
	}
	
}
