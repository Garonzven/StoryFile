using DDK.Base.Extensions;

using UnityEngine;
using UnityEngine.UI;


namespace DDK.Base.Events
{
    [RequireComponent(typeof(Button))]
    public class OnClickExplosion2D : MonoBehaviour
    {
        [Header("Explosion Parameters")]
        public float radius;
        public float force;
        [Header("Query Parameters")]
        public LayerMask layer;
        public float distance = 100f;
        public float minDepth = 0;
        public float maxDepth = 1;
        [Header("Debug")]
        public bool drawGizmos = true;


        // Use this for initialization
        void Start()
        {
            var button = GetComponent<Button>();

            if ( button )
            {
                button.onClick.AddListener(Explosion);
            }
        }


        public void Explosion()
        {
            var hits = Physics2D.CircleCastAll(transform.position, radius,
                transform.forward, distance, layer, minDepth, maxDepth);

            for ( int i = 0; i < hits.Length; i++ )
            {
                // ignore non rigid bodies and self
                if ( hits[i].rigidbody != null && hits[i].transform != transform )
                {
                    hits[i].rigidbody.AddExplosionForce(force,
                        transform.position, radius);
                }
            }
        }
        public void OnDrawGizmosSelected()
        {
            if ( !drawGizmos )
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}