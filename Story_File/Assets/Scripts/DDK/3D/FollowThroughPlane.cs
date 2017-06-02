using DDK.Base.Classes;

using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DDK._3D
{

    /// <summary>
    /// Follow through plane makes a transform follow another
    /// but its movements are projected onto a normal oriented
    /// 3D plane.
    /// </summary>
    /// <seealso cref="DDK._3D.PlaneNormal" />
    public class FollowThroughPlane : PlaneNormal
    {
        [Header ( "Follow Options" )]
        public LateReference source;
        public LateReference target;
        /// <summary>
        /// The smoothing factor for movement.
        /// </summary>
        public float smoothSpeed = 5f;
        /// <summary>
        /// If this is true the source transform will keep its difference
        /// distance with target thus translating the source point with the
        /// initial difference between the source position and the target
        /// position
        /// </summary>
        [Tooltip ( "If this is true the source transform will keep its diffe" +
                   "rence distance with target thus translating the source p" +
                   "oint with the initial difference between the source posi" +
                   "tion and the target position" )]
        public bool preserveInitialDistance = true;

        private Vector3 _initialDifference;
        public bool drawDebug;
        // Use this for initialization
        void Start()
        {
            if ( !source.Reference )
            {
                source.Reference = transform;
            }

            if ( preserveInitialDistance )
            {
                _initialDifference = source.Reference.position -
                                     target.Reference.position;
            }
        }

        // Update is called once per frame
        void LateUpdate()
        {
            SetPlaneNormal();
            var targetPos = target.Reference.position;

            if ( preserveInitialDistance )
            {
                targetPos = targetPos + _initialDifference;
            }

            var movement = targetPos - source.Reference.position;
            var projected = Vector3.ProjectOnPlane ( movement , planeNormal );
            var final = source.Reference.position + projected;
            source.Reference.position = Vector3.Slerp ( source.Reference.position,
                                        final, Time.deltaTime *
                                        smoothSpeed );
        }

        public void SetTarget ( Transform newtarget )
        {
            target.Reference = newtarget;
        }

        public void OnDrawGizmos()
        {
#if UNITY_EDITOR

            if ( drawDebug )
            {
                SetPlaneNormal();
                Handles.DrawWireDisc ( source.Reference.position, planeNormal, 1f );
            }

#endif
        }
    }
}