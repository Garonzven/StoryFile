using System.Collections;
using DDK.Base.Extensions;
using UnityEngine;


namespace DDK.Base.Classes
{
    /// <summary>
    ///     Class for storing transforms information without a gameObject instance
    /// </summary>
    public class StoredTransform
    {
        public Vector3 forward, right, up;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;

        public Vector3 localPosition;
        public Quaternion localRotation;


        public StoredTransform ( Transform t )
        {
            forward = t.forward;
            right = t.right;
            up = t.up;
            position = t.position;
            rotation = t.rotation;
            localScale = t.localScale;
            localPosition = t.localPosition;
            localRotation = t.localRotation;
        }
        public StoredTransform ( StoredTransform t )
        {
            forward = t.forward;
            right = t.right;
            up = t.up;
            position = t.position;
            rotation = t.rotation;
            localScale = t.localScale;
            localPosition = t.localPosition;
            localRotation = t.localRotation;
        }
        public StoredTransform() {}


        public void ExtractTo ( Transform t )
        {
            t.localPosition = localPosition;
            t.localRotation = localRotation;
            t.position = position;
            t.rotation = rotation;
            t.localScale = localScale;
        }
        public void ExtractFrom ( Transform t )
        {
            forward = t.forward;
            right = t.right;
            up = t.up;
            position = t.position;
            rotation = t.rotation;
            localScale = t.localScale;
            localPosition = t.localPosition;
            localRotation = t.localRotation;
        }
        /// <summary>
        ///     <see cref="TransformExt.AlignToTargetCo" /> for the main camera
        ///     using <see cref="StoredTransform" /> as destiny.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="useSlerp">if set to <c>true</c> [use slerp].</param>
        /// <returns></returns>
        public IEnumerator CameraAlignToCo ( float duration = 1f, bool useSlerp = true )
        {
            var camera = Camera.main.transform;
            yield return camera.AlignToTargetCo ( position, rotation, duration, useSlerp );
        }
        /// <summary>
        ///     <see cref="TransformExt.LookAtTargetCo" />
        ///     for the main camera using <see cref="StoredTransform" /> as destiny.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="useSlerp">if set to <c>true</c> [use slerp].</param>
        /// <returns></returns>
        public IEnumerator CameraLookAtCo (  float duration = 1f, bool useSlerp = true )
        {
            var camera = Camera.main.transform;
            yield return camera.LookAtTargetCo ( position, duration, useSlerp ) ;
        }
        public GameObject Instantiate ( string name )
        {
            var instance = new GameObject ( name );
            instance.transform.position = position;
            instance.transform.localScale = position;
            instance.transform.rotation = rotation;
            return instance;
        }
    }
}