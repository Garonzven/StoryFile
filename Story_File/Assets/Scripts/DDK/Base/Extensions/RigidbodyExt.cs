using UnityEngine;


namespace DDK.Base.Extensions
{
    public static class RigidbodyExt 
    {
        /// <summary>
        /// Adds a random force that is applied from the mouse position to the specified object's.
        /// </summary>
        /// <param name="rigidbody2d">Rigidbody2d.</param>
        /// <param name="multiplier">.</param>
        public static void AddRandomForceFromClick( this Rigidbody2D rigidbody2d, float randomMultiplierMaxLimit = 1f, 
            float randomMultiplierMinLimit = 0.1f )
        {
            Vector2 randomForce = ( Vector2 )( rigidbody2d.transform.position - Input.mousePosition.GetWorldPoint() );
            randomForce.Normalize();
            randomForce *= Random.Range( randomMultiplierMinLimit, randomMultiplierMaxLimit );
            rigidbody2d.AddForce( randomForce );
        }
        public static void AddExplosionForce( this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, 
            float explosionRadius )
        {
            Vector3 dir = ( body.transform.position - explosionPosition );
            float wearoff = 1 - ( dir.magnitude / explosionRadius );
            body.AddForce( dir.normalized * explosionForce * wearoff );
        }
        public static void AddExplosionForce( this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, 
            float explosionRadius, float upliftModifier )
        {
            Vector3 dir = (body.transform.position - explosionPosition);
            float wearoff = 1 - (dir.magnitude / explosionRadius);
            Vector3 baseForce = dir.normalized * explosionForce * wearoff;
            body.AddForce(baseForce);

            float upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            body.AddForce(upliftForce);
        }
    }
}
