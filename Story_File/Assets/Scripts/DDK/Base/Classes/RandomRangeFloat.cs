//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;


namespace DDK.Base.Classes 
{	
	[System.Serializable]
	public class RandomRangeFloat 
    {
        [ShowIfAttribute( "_hideRandomSign", true )]
		public bool randomSign;
        [NotLessThan( 0f, null, "_boundValues" )]
		public float from = 0f;
        [NotMoreThan( 1f, null, "_boundValues" )]
		public float to = 1f;


		#pragma warning disable 0414
        /// <summary>
        /// If true, /from/ and /to/ will be bounded to a range between 0 and 1 Eg: /from/ won't go below 0, and /to/ won't go higher than 1
        /// </summary>
        private bool _boundValues;
        /// <summary>
        /// Editor, don't touch.
        /// </summary>
        private bool _hideRandomSign;
		#pragma warning restore 0414

		
        public RandomRangeFloat( bool boundValues = false ) 
        { 
            _boundValues = boundValues;
        }
        public RandomRangeFloat( bool hideRandomSign, bool boundValues = false ) 
        { 
            _boundValues = boundValues;
            _hideRandomSign = hideRandomSign;
        }
        public RandomRangeFloat( float from, float to, bool randomSign = false, bool boundValues = false )
		{
			this.from = from;
			this.to = to;
			this.randomSign = randomSign;
            _boundValues = boundValues;
		}
        public RandomRangeFloat( bool hideRandomSign, float from, float to, bool boundValues = false ) : 
        this( hideRandomSign, boundValues )
        {
            this.from = from;
            this.to = to;
        }

		

		/// <summary>
		/// Returns a value between /from/ and /to/
		/// </summary>
		public float GetRandom()
		{
			return GetRandom( randomSign );
		}
		/// <summary>
		/// Returns a value between /from/ and /to/
		/// </summary>
		public float GetRandom( bool randomSign )
		{
			int sign = 1;
			if( randomSign )
			{
				sign = ( Random.value > 0.5f ) ? -1 : 1;
			}
			return UnityEngine.Random.Range( from, to ) * sign;
		}
		/// <summary>
		/// Get a Vector3 with random values.
		/// </summary>
		/// <param name="uniform">If set to <c>true</c> all the values will be the same (x = y = z).</param>
		public Vector3 GetRandomVec3( bool uniform = true, bool randomSign = false )
		{
			if( uniform )
			{
				int sign = 1;
				if( randomSign )
				{
					sign = ( Random.value > 0.5f ) ? -1 : 1;
                }
                float value = GetRandom();
				return new Vector3( value * sign, value * sign, value * sign );
			}
			else return new Vector3( GetRandom( randomSign ), GetRandom( randomSign ), GetRandom( randomSign ) );
		}
		/// <summary>
		/// Get a Vector3 with random values.
		/// </summary>
		/// <param name="uniform">If set to <c>true</c> all the values will be the same (x = y = z).</param>
		public Vector2 GetRandomVec2( bool uniform = true, bool randomSign = false )
		{
			return (Vector2) GetRandomVec3( uniform, randomSign );
		}
	}
}
