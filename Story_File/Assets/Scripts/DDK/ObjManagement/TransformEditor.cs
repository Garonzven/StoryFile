using UnityEngine;

namespace DDK.ObjManagement
{
    /// <summary>
    /// Improves Transform handling in the editor.
    /// </summary>
    [ExecuteInEditMode]
    public class TransformEditor : MonoBehaviour
    {
        [System.Flags]
        public enum TransformComponent
        {
            Position = 1 << 0,
            Rotation = 1 << 1,
            Scale = 1 << 2,
            Forward = 1 << 3,
            Right = 1 << 4,
            Up = 1 << 5
        }

        [SerializeField]
        private Transform _source;
        [SerializeField]
        private Transform _target;

        public Transform Source
        {
            get
            {
                if ( !_source )
                {
                    _source = transform;
                }

                return _source;
            }
        }

        public Transform Target
        {
            get
            {
                if ( !_target )
                {
                    _target = transform;
                }

                return _target;
            }
        }

        [InspectorButton ( "_AlignWith" , true )]
        public bool alignWith;

        [InspectorButton ( "_LookAt", true )]
        public bool lookAt;

        [InspectorButton ( "_Copy", true )]
        public bool copy;
        [EnumFlags]
        public TransformComponent copyTransformComponent;

        private void _AlignWith()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(Source, Source.name);
#endif
            Source.position = Target.position;
            Source.rotation = Target.rotation;
        }

        private void _LookAt()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(Source, Source.name);
#endif
            Source.LookAt ( Target );
        }

        private void _Copy()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(Source, Source.name);
#endif

            if ( ( copyTransformComponent & TransformComponent.Position ) == TransformComponent.Position )
            {
                Source.position = Target.position;
            }

            if ( ( copyTransformComponent & TransformComponent.Rotation ) == TransformComponent.Rotation )
            {
                Source.rotation = Target.rotation;
            }

            if ( ( copyTransformComponent & TransformComponent.Scale ) == TransformComponent.Scale )
            {
                Source.localScale = Target.localScale;
            }

            if ( ( copyTransformComponent & TransformComponent.Forward ) == TransformComponent.Forward )
            {
                Source.forward = Target.forward;
            }

            if ( ( copyTransformComponent & TransformComponent.Right ) == TransformComponent.Right )
            {
                Source.right = Target.right;
            }

            if ( ( copyTransformComponent & TransformComponent.Up ) == TransformComponent.Up )
            {
                Source.up = Target.up;
            }
        }
    }
}