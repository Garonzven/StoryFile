//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Extensions;
using DDK.Base.Statics;
using DDK.Base.Misc;


namespace DDK.Base.Animations 
{
	/// <summary>
	/// Attach this to a gameobject that needs a texture animation. Ex: A Character with texture facial expressions.
	/// </summary>
	public class AnimTextures : MonoBehaviour 
    {
		[System.Serializable]
		public class TextureAnimation 
        {
			[Tooltip("The name of the parameter that the Animator must have set to true for this texture animation to run.")]
			public string paramName;
			[Tooltip("The textures prefix. Ex: If the animation is made up by textures named Talk_001, Talk_002, Talk_003... " +
				"and so on, then this prefix should be \"Talk_\". NOTE: If it's not an animation, but a single texture, then " +
				"just put the name of the texture")]
			public string texPrefix;
            /// <summary>
            /// Rate in seconds.
            /// </summary>
			[Tooltip("IN SECONDS, This just applies to animations")]
			[DisplayNameAttribute("Rate")]
			public float speed = 0.4f;
			public float checkRate = float.MaxValue;


			byte _current = 0;
			bool _othersReset = false;
			internal List<Texture2D> textures;
			/// <summary>
			/// Returns true if the textures conform an animation.
			/// </summary>
			bool _IsAnimation 
            {
				get{ return textures.Count > 1; }
			}

            const byte CERO = 0;

			/// <summary>
			/// After calling this function the specified parameter name will be checked, if true this animation will run
			/// or the texture just changed, in case it is not an animation.
			/// </summary>
			/// <param name="obj">Object.</param>
			/// <param name="animator">Animator.</param>
			public void CheckRunAnim( GameObject obj, Animator animator )
			{
				_CheckRunAnim( obj, animator ).Start();
			}
			public IEnumerator CheckRunAnimOnce( GameObject obj, Animator animator )
			{
				var animTexs = obj.GetComponentInChildren<AnimTextures>();//holds the animations that need to be reset
				if( animator.GetBool( paramName ) )
				{
					//SET OTHER PARAMS TO FALSE
					if( !_othersReset )
					{
						var parameters = animator.parameters;
						for( int i=0; i<parameters.Length; i++ )
						{
							if( parameters[i].type == AnimatorControllerParameterType.Bool && !parameters[i].name.Equals( paramName )
							   && animTexs.HasParam( parameters[i].name ) )
							{
								animator.SetBool( parameters[i].name, false );
							}
						}
						_othersReset = true;
					}
					
					if( _IsAnimation )
					{
                        _current = (++_current).ClampOpposite( CERO, (byte)(textures.Count - 1) );
						obj.SetTexture( textures[ _current ] );
                        yield return new WaitForSeconds( speed );
					}
					else
					{
						obj.SetTexture( textures[ 0 ] );
                        yield return new WaitForSeconds( checkRate );
					}
				}
				else 
				{
					_othersReset = false;
                    yield return new WaitForSeconds( checkRate );
				}
			}

			private IEnumerator _CheckRunAnim( GameObject obj, Animator animator )
			{
				while( true )
				{
					if( animator == null )
					{
						break;
					}
					yield return CheckRunAnimOnce( obj, animator ).Start();
				}
			}
		}		



        /// <summary>
        /// This are set to null in Start().
        /// </summary>
		[Tooltip("If empty, they will be searched in the specified path")]
		public Texture2D[] textures = new Texture2D[0];
		[Tooltip("This will be searched in the Resources folder")]
		[ShowIfAttribute( "_IsTexturesEmpty" )]
		public PathHolder.Index texturesPath;
		[Tooltip("If null, it will be searched in this object and its parents/children, in that order")]
		public Animator animator;
		public  TextureAnimation[] animations = new TextureAnimation[0];


		protected bool _IsTexturesEmpty()
		{
            if( !enabled )
                return true;
			_ValidateAnimator();//called in editor
			if( textures == null )
				return true;
			return textures.Length == 0;
		}


		// Use this for initialization
		void Start () 
        {
			if( _IsTexturesEmpty() ) //GET ALL TEXTURES
			{
				textures = Resources.LoadAll<Texture2D>( texturesPath.path );
			}
			if( textures.Length == 0 )
			{
				if( string.IsNullOrEmpty( texturesPath.path ) )
                    Utilities.LogWarning("No /textures/ have been found in the specified path.. Disabling component", gameObject);
                else Utilities.LogWarning("No /textures/ have been specified.. Disabling component", gameObject);
				enabled = false;
				return;
			}
			_ValidateAnimator();
			Init();
			textures = null;//Release memory
		}



		public void Init()
		{
			for( int i=0; i<animations.Length; i++ )//ASSIGN THE ANIMATIONS'S TEXTURES
			{
				animations[i].textures = textures.GetWhichContain( animations[i].texPrefix );
				animations[i].CheckRunAnim( gameObject, animator );
			}
		}
		/// <summary>
		/// Returns true, if the specified param is inside the TextureAnimation array
		/// </summary>
		public bool HasParam( string param )
		{
			for( int i=0; i<animations.Length; i++ )
			{
				if( animations[i].paramName.Equals( param ) )
					return true;
			}
			return false;
		}
		/// <summary>
		/// Call this if you want to check which animation should be played right now. If any is true, it will be played immediately.
		/// </summary>
		public void CheckAllAnimations()
		{
			for( int i=0; i<animations.Length; i++ )//ASSIGN THE ANIMATIONS'S TEXTURES
			{
				animations[i].CheckRunAnimOnce( gameObject, animator ).Start();
			}
		}


		protected void _ValidateAnimator()
		{
			if( !animator )
			{
				animator = GetComponentInParent<Animator>();
				if( !animator )
				{
					animator = GetComponentInChildren<Animator>();
				}
			}
			if( !animator )
			{
                Utilities.LogWarning ( "No Animator component was found in this object," +
					"its parent or any of its children.. Disabling component", gameObject );
				enabled = false;
			}
		}



		/// <summary>
		/// Call this if you want to check which animation should be played right now. If any is true, it will be played immediately.
		/// </summary>
		public static void CheckAll( AnimTextures[] animations )
		{
			if( animations == null || animations.Length == 0 )
				return;
			for( int i=0; i<animations.Length; i++ )
			{
				animations[i].CheckAllAnimations();
			}
		}
		/// <summary>
		/// Call this if you want to check which animation should be played right now. If any is true, it will be played immediately.
		/// </summary>
		/// <param name="obj"> The parent of the objects containing an AnimTextures component </param>
		public static void CheckAll( GameObject obj )
		{
			var animations = obj.GetComponentsInChildren<AnimTextures>();
			CheckAll( animations );
		}

	}

}
