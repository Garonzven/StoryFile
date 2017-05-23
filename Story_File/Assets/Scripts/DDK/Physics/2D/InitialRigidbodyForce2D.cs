using UnityEngine;

namespace DDK._Physics._2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class InitialRigidbodyForce2D : MonoBehaviour
    {
        [Header("Initial Force Setup")]
        public Vector2 initialForce;

        public ForceMode2D forceMode;
        public bool relativeForce;

        [Header("Random Option")]
        public bool randomInitialForce = true;
        public float randomForceIntensity = 1f;

        private Rigidbody2D _rigidbody2D;

        // Use this for initialization
        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();

            if ( randomInitialForce )
            {
                initialForce = Random.insideUnitCircle.normalized *
                               randomForceIntensity;
            }

            _AddInitialForce();
        }

        void _AddInitialForce()
        {
            if ( relativeForce )
            {
                _rigidbody2D.AddRelativeForce(initialForce, forceMode);
            }
            else
            {
                _rigidbody2D.AddForce(initialForce, forceMode);
            }
        }
    }
}