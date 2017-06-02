using DDK.Base.Extensions;

using UnityEngine;

namespace DDK._3D
{
    /// <summary>
    /// This class describes a normal oriented plane
    /// for all <see cref="MonoBehaviour"/>s that need
    /// an oriented plane for logic.
    /// </summary>
    /// <seealso cref="DDK.Extensions.MonoBehaviourExt" />
    public class PlaneNormal : MonoBehaviourExt
    {
        public enum UseAsPlaneNormal
        {
            Custom,
            TransformForward,
            TransformBackward,
            TransformRight,
            TransformLeft,
            TransformUp,
            TransformDown
        }

        [Header ( "Plane Normal" )]
        public UseAsPlaneNormal useAsPlaneNormal;
        public Transform asTransformPlane;
        public Vector3 planeNormal;

        private float _gizmoCircleRadius = 1f;

        public float GizmoCircleRadius
        {
            get { return _gizmoCircleRadius; }
            set { _gizmoCircleRadius = value; }
        }

        public void SetPlaneNormal()
        {
            var source = transform;

            if ( asTransformPlane )
            {
                source = asTransformPlane;
            }
            else
            {
                asTransformPlane = transform;
            }

            switch ( useAsPlaneNormal )
            {
                case UseAsPlaneNormal.TransformForward:
                    planeNormal = source.forward;
                    break;

                case UseAsPlaneNormal.TransformRight:
                    planeNormal = source.right;
                    break;

                case UseAsPlaneNormal.TransformUp:
                    planeNormal = source.up;
                    break;

                case UseAsPlaneNormal.TransformBackward:
                    planeNormal = -source.forward;
                    break;

                case UseAsPlaneNormal.TransformLeft:
                    planeNormal = -source.right;
                    break;

                case UseAsPlaneNormal.TransformDown:
                    planeNormal = -source.up;
                    break;
            }
        }
    }
}