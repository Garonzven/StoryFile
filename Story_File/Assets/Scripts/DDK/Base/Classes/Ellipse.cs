//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.Base.Classes 
{
    /// <summary>
    /// Representation of an Ellipse
    /// </summary>
	[System.Serializable]
	public class Ellipse 
    {
		[SerializeField]
		protected Transform center;
		[Tooltip("Semimajor axis")]
		public float a = 2f;
        [Tooltip("Semiminor axis")]
        public float b = 1f;


        internal float _alpha;


		public Vector3 _Center 
        {
			get
            {
				if( !center )
					return Vector3.zero;
				else return center.position;
			}
		}


		public void SetCenter( Transform center )
		{
			if( !center )
				return;
			this.center = center;
		}

		/// <summary>
		/// When using transform.RotateAround() for circular orbits, an angle is specified. For the ellipse to rotate with the same angle/speed 
		/// as that function, you must use the speed calculated by this. NOTE: This is not completely precise..
		/// </summary>
		/// <returns>The speed from angle.</returns>
		/// <param name="angle">Angle.</param>
		public float GetSpeedFromAngle( float angle )
		{
			return ( angle * Mathf.PI * ( a / b ) );
		}
    }
}
