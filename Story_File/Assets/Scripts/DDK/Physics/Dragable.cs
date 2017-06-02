using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace DDK._Physics
{
    public class Dragable : MonoBehaviour
    {

        public float spring = 50.0f;
        public float damper = 5.0f;
        public float drag = 10.0f;
        public float angularDrag = 5.0f;
        public float distance = 0.2f;
        public bool attachToCenterOfMass = false;
        public LayerMask checkLayer;
        [Tooltip ( "If enabled dragging will only be possible when" +
                   "the EventSystem is enabled" )]
        public bool boundToEventSystem = true;

        private static SpringJoint _springJoint;
        private EventSystem _eventSystem;

        private void Awake()
        {
            _eventSystem = EventSystem.current;

            if ( !_eventSystem )
            {
                _eventSystem = FindObjectOfType<EventSystem>();

                if ( !_eventSystem )
                {
                    Debug.LogWarning ( "Disabling bound to event" +
                                       " system, no EventSystem found." );
                    boundToEventSystem = false;
                }
            }
        }

        private void Start()
        {
            if ( checkLayer.value == 0 )
            {
                checkLayer.value = 1 << gameObject.layer;
            }
        }

        private void Update()
        {
            if ( boundToEventSystem && !_eventSystem.enabled )
            {
                return;
            }

            // Make sure the user pressed the mouse down
            if ( !Input.GetMouseButtonDown ( 0 ) )
            {
                return;
            }

            var mainCamera = FindCamera();
            // We need to actually hit an object
            RaycastHit hit = new RaycastHit();

            if (
                !Physics.Raycast ( mainCamera.ScreenPointToRay ( Input.mousePosition ).origin,
                                   mainCamera.ScreenPointToRay ( Input.mousePosition ).direction, out hit, 100,
                                   checkLayer ) )
            {
                return;
            }

            // unlike standard DragRigidbody this one only
            // drags the associated gameObject to the script
            if ( hit.transform.gameObject != gameObject )
            {
                return;
            }

            // We need to hit a rigidbody that is not kinematic
            if ( !hit.rigidbody || hit.rigidbody.isKinematic )
            {
                return;
            }

            if ( !_springJoint )
            {
                var go = new GameObject ( "Rigidbody Dragger" );
                Rigidbody body = go.AddComponent<Rigidbody>();
                _springJoint = go.AddComponent<SpringJoint>();
                body.isKinematic = true;
            }

            _springJoint.transform.position = hit.point;
            _springJoint.anchor = Vector3.zero;
            _springJoint.spring = spring;
            _springJoint.damper = damper;
            _springJoint.maxDistance = distance;
            _springJoint.connectedBody = hit.rigidbody;
            StartCoroutine ( "DragObject", hit.distance );
        }


        private IEnumerator DragObject ( float distance )
        {
            var oldDrag = _springJoint.connectedBody.drag;
            var oldAngularDrag = _springJoint.connectedBody.angularDrag;
            _springJoint.connectedBody.drag = drag;
            _springJoint.connectedBody.angularDrag = angularDrag;
            var mainCamera = FindCamera();

            while ( Input.GetMouseButton ( 0 ) )
            {
                var ray = mainCamera.ScreenPointToRay ( Input.mousePosition );
                _springJoint.transform.position = ray.GetPoint ( distance );
                yield return null;
            }

            if ( _springJoint.connectedBody )
            {
                _springJoint.connectedBody.drag = oldDrag;
                _springJoint.connectedBody.angularDrag = oldAngularDrag;
                _springJoint.connectedBody = null;
            }
        }


        private Camera FindCamera()
        {
            if ( GetComponent<Camera>() )
            {
                return GetComponent<Camera>();
            }

            return Camera.main;
        }
    }
}
