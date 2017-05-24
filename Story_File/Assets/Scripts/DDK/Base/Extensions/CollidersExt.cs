using UnityEngine;
using System.Collections.Generic;
using System.Collections;


namespace DDK.Base.Extensions
{
    public static class CollidersExt 
    {
        /// <summary>
        /// Returns a random world point inside the given Collider
        /// </summary>
        public static Vector3 GetRandomPointInCollider( this Collider area )
        {
            Bounds bounds = area.bounds;
            Vector3 center = bounds.center;
            var y = UnityEngine.Random.Range( center.y - bounds.extents.y, center.y + bounds.extents.y );
            return area.GetRandomPointInCollider( y );
        }
        /// <summary>
        /// Returns a random world point inside the given Collider
        /// </summary>
        public static Vector3 GetRandomPointInCollider( this Collider area, float y )
        {
            Bounds bounds = area.bounds;
            Vector3 center = bounds.center;

            var x = UnityEngine.Random.Range( center.x - bounds.extents.x, center.x + bounds.extents.x );
            var z = UnityEngine.Random.Range( center.z - bounds.extents.z, center.z + bounds.extents.z );

            return new Vector3(x, y, z);
        }
        /// <summary>
        /// Returns a random world point inside the given Collider2D
        /// </summary>
        public static Vector3 GetRandomPointInCollider2D( this Collider2D area, float z = 0f )
        {
            Bounds bounds = area.bounds;
            Vector3 center = bounds.center;

            var x = UnityEngine.Random.Range( center.x - bounds.extents.x, center.x + bounds.extents.x );
            var y = UnityEngine.Random.Range( center.y - bounds.extents.y, center.y + bounds.extents.y );

            return new Vector3(x, y, z);
        }
        /// <summary>
        /// Enables or disables the specified component, if it references a prefab the prefab instance will be searched;
        /// make sure the instance has a unique name if multiple instances are active.
        /// </summary>
        /// <param name="comp">Comp.</param>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        /// <param name="prefabCompId">Prefab comp identifier.</param>
        public static void SetEnabled( this Collider comp, bool enabled = true, int prefabCompId = 0 )
        {
            if( comp.IsPrefab() )
            {
                var instance = comp.name.Find();
                if( instance )
                {
                    var cmps = instance.GetComponents( comp.GetType() );
                    if( cmps != null )
                    {
                        for( int j=0; j<cmps.Length; j++ )
                        {
                            if( j == prefabCompId )
                            {
                                var c = cmps[j] as Renderer;
                                c.enabled = enabled;
                            }
                        }
                    }
                }
            }
            else if( comp ) { 
                comp.enabled = enabled;
            }
        }
        public static void SetEnabled( this IList<Collider> comps, bool enabled = true, int prefabCompId = 0 )
        {
            if( comps == null )
                return;
            for( int i=0; i<comps.Count; i++ )
            {
                comps[i].SetEnabled( enabled, prefabCompId );
            }
        }
        /// <summary>
        /// Enables or disables the specified component, if it references a prefab the prefab instance will be searched;
        /// make sure the instance has a unique name if multiple instances are active.
        /// </summary>
        /// <param name="comp">Comp.</param>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        /// <param name="prefabCompId">Prefab comp identifier.</param>
        public static void DisableFor( this Collider comp, float duration = 1f, int prefabCompId = 0 )
        {
            comp._DisableFor( duration, prefabCompId ).Start();
        }
        private static IEnumerator _DisableFor( this Collider comp, float duration = 1f, int prefabCompId = 0 )
        {
            comp.SetEnabled( false, prefabCompId );
            yield return new WaitForSeconds( duration );
            comp.SetEnabled( true, prefabCompId );
        }
    }

}