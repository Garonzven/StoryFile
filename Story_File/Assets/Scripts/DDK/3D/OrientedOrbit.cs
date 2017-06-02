using DDK.Base.Classes;

using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DDK._3D
{
    /// <summary>
    /// Rotates around a target making an orbit.
    /// The plane formed by this orbit is oriented
    /// by a <see cref="PlaneNormal"/>. This orbit
    /// can be translated relative to the target.
    /// </summary>
    /// <seealso cref="PlaneNormal" />
    public class OrientedOrbit : PlaneNormal
    {
        public Vector3 Center
        {
            get
            {
                if ( _center )
                {
                    return _center.position + translateCenter;
                }

                _center = center.Reference;

                if ( !_center )
                {
                    enabled = false;
                }

                return _center.position + translateCenter;
            }
        }

        public Vector3 translateCenter;
        private Transform _center;
        public LateReference center;
        public float radius = 3f;

        public float stepSpeed = 0.5f;
        public float rotationSpeed = 80f;

        public bool drawOrbit;

        // Use this for initialization
        void Start()
        {
            var dir = ( transform.position - Center ).normalized;

            if ( dir == Vector3.zero )
                dir = _center.forward; 

            transform.position = dir * radius + Center;
        }

        // Update is called once per frame
        void Update()
        {
            SetPlaneNormal();
            transform.RotateAround ( Center, planeNormal,
                                     rotationSpeed * Time.deltaTime );
            var dir = ( transform.position - Center ).normalized;
            var pos = dir * radius + Center;
            transform.position = Vector3.MoveTowards ( transform.position, pos,
                                 Time.deltaTime * stepSpeed );
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if ( drawOrbit )
            {
                Handles.color = Color.red;
                Handles.DrawWireDisc ( Center, planeNormal, radius );
            }
        }
#endif
    }
}