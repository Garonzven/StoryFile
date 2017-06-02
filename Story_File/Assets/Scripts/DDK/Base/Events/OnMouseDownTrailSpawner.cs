//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK;
using DDK.Base.Statics;
using DDK.Base.Misc;


namespace DDK.Base.Events 
{
	/// <summary>
	/// Creates and attaches a TrailRenderer to this object when mouse is down. It destroys the renderers when they finish fading out.
	/// </summary>
	public class OnMouseDownTrailSpawner : MonoBehaviour 
    {
		[Tooltip("In case you don't want to reference it directly")]
		public PathHolder.Index trailPrefabPath;
		[Tooltip("If a prefab, it will be attached to this object (parented)")]
		public GameObject trail;					
		
		
		private TrailRenderer _lastTrail;
		private GameObject _lastInstance;
		private int _spawns;
		
		
		// Use this for initialization
		void Start () 
        {
			if( trail )
				return;
			if( !string.IsNullOrEmpty( trailPrefabPath.path ) )
			{
				GameObject _trail = Resources.Load<GameObject>( trailPrefabPath.path );
				if( _trail )
				{
					trail = _trail;
					return;
				}
			}
            Utilities.LogWarning ("There is no /trailPrefab/ specified, and the specified /trailPrefabPath/ is empty. Disabling component...", gameObject);
			enabled = false;
		}		
		// Update is called once per frame
		void Update ()
        {			
			if( Input.GetMouseButtonDown(0) )
			{
				CreateTrail();
			}
			else if( Input.GetMouseButtonUp(0) )
			{
				_DestroyTrail( _lastInstance );
			}
		}
				
		
		
		public void CreateTrail()
		{
			_spawns++;
			_lastInstance = trail.SetActiveInHierarchy();
			_lastTrail = _lastInstance.GetComponent<TrailRenderer>();
			_lastTrail.enabled = true;
			if( _lastInstance.GetParent() != gameObject )
			{
				_lastInstance.SetParent( transform );//Attach to this object
                _lastInstance.SetPos( Vector3.zero, true );
			}
		}		
        public void DestroyCurrentTrail()
        {
            _DestroyTrail( _lastInstance );
        }

		
        protected void _DestroyTrail( GameObject trailObj )
        {
            if( !trailObj )
                return;
            trailObj.SetParent( null );//Detach
            trailObj.transform.position = transform.position;
            trailObj.SetActiveInHierarchy( false, true, _lastTrail.time );
            //Destroy( trailObj, _lastTrail.time );
            _ReduceSpawnsCount( _lastTrail.time ).Start();
        }

		private IEnumerator _ReduceSpawnsCount( float delay )
		{
			yield return new WaitForSeconds( delay );
			_spawns--;
		}			
	}
}