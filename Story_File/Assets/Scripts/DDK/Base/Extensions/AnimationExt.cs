//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Statics;


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// Animation class extension.
    /// </summary>
	public static class AnimationExt 
    {				
		public static IEnumerator Stop( this Animation anim )
		{
			while( anim.isPlaying ){
				yield return null;
			}
			anim.Stop();
		}
		
		public static void SetSpeed( this Animation anim, float speed )
		{
			foreach( AnimationState state in anim )
			{
				state.speed = speed;
			}
		}
		
		public static void SetAnimTime( this Animation animation, float time )
		{
			foreach ( AnimationState state in animation ) {
				state.time = time;
			}
		}

		public static IEnumerator SetEnabledAfter( this Animation[] anims, bool enabled = true, float after = 1f )
		{
			yield return new WaitForSeconds( after );
			for( int i=0; i<anims.Length; i++ )
			{
				if( anims[i] != null )
				{
					anims[i].enabled = enabled;
				}
			}
		}
		
		public static IEnumerator PlayAfter( this Animation anim, float after )
		{
			yield return new WaitForSeconds( after );
			anim.PlayAnim();
		}
				
		/// <summary>
		/// Play the animation, checks if its null.
		/// </summary>
		/// <param name="anim">Animation.</param>
		public static void PlayAnim( this Animation anim )
		{
			if( anim != null )
			{
				anim.Play();
			}
		}		
		
	}
}