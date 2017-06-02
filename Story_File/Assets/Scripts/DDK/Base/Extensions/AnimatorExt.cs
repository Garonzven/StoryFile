//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Statics;
using MovementEffects;


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// Animator class extension.
    /// </summary>
	public static class AnimatorExt 
    {
		#region STOP
		/// <summary>
		/// Rebinds and stops (speed = 0) the specified animator.
		/// </summary>
		/// <param name="animator">Animator.</param>
		/// <param name="propName">Property name.</param>
		public static void Stop( this Animator animator, string propName, bool rebind = true )
		{
			int current = animator.GetInteger( propName );
			if( rebind )
				animator.Rebind();
			animator.SetInteger( propName, current );
			animator.speed = 0f;
		}
		/// <summary>
		/// Stops this Animator.
		/// </summary>
		/// <param name="animationName">If not null, and the animation is playing, the animator will be stopped after it ends playing.</param>
		public static IEnumerator StopAnim( this Animator animator, string animationName = null)
		{
			while( animator.IsStateActive( animationName ) )
			{
				yield return null;
			}
			animator.StopPlayback();
		}
		#endregion
				
		#region IS
		/// <summary>
		/// Returns true if the current animator state matches the specified.
		/// </summary>
		public static bool IsStateActive( this Animator animator, string stateName, int layerIndex = 0 )
		{
			return animator.GetCurrentAnimatorStateInfo( layerIndex ).IsName( stateName );
		}
		/// <summary>
		/// Returns true if the current state's animation is done.
		/// </summary>
		public static bool IsAnimDone( this Animator ani, int layer = 0 )
		{
			var aniState = ani.GetCurrentAnimatorStateInfo( layer );
			return ( aniState.normalizedTime == 1 ) ? true : false;
		}
		#endregion

		#region GET
		public static float GetStateLength( this Animator ani, int currentLayer )
		{
			var aniState = ani.GetCurrentAnimatorStateInfo( currentLayer );
			return aniState.length; //aniState.normalizedTime - 1;
		}
		public static float GetStateNormalizedTime( this Animator ani, int currentLayer )
		{
			var aniState = ani.GetCurrentAnimatorStateInfo( currentLayer );
			return aniState.normalizedTime;
		}
		/// <summary>
		/// Get the total length (duration) of the specified clips.
		/// </summary>
		/// <returns>The total length.</returns>
		/// <param name="clips">Clips.</param>
		public static float GetClipsTotalLength( this IList<AnimationClip> clips )
		{
			return clips.GetClipsTotalLength( 0, clips.Count );
		}
		/// <summary>
		/// Gets the total length ( duration ) of the specified clips. If any of the specified indexes is wrong it will be clamped to a valid value.
		/// </summary>
		/// <param name="from">From ( clip index).</param>
		/// <param name="to">To ( clip index ).</param>
		public static float GetClipsTotalLength( this IList<AnimationClip> clips, int from, int to )
		{
			float totalLength = 0f;
			to = to.Clamp( 1, clips.Count );
			from = from.Clamp( 0, to - 1 );
			for( int i=from; i<to; i++ )
			{
				if( !clips[i] )
					continue;
				totalLength += clips[i].length;
			}
			return totalLength;
		}
		#endregion

		#region SET
		/// <summary>
		/// Sets the value of a property named -direction- deppending on the specified angle from 0 to 360. Starting at south in counterclockwise and ending at southwest (maximum 8 values).
		/// </summary>
		/// <param name="angle">Angle.</param>
		/// <param name="counterclockwiseValues">Counterclockwise values. Order example, first 90º angles: 1, 2, 3, 4. The, diagonal angles: 11, 12, 13, 14. If using the example values this can be left as null</param>
		public static void SetDirection( this Animator animator, float angle, bool hasDiagonalDirections = false, int[] counterclockwiseValues = null )
		{
			angle = Mathf.Clamp( angle, 0f, 360f );
			if( counterclockwiseValues == null )
			{
				counterclockwiseValues = new int[]{ 1, 2, 3, 4, 11, 12, 13, 14 };
			}
			
			if( angle >= 45f && angle <= 135f )//north
			{
				animator.SetInteger("direction", counterclockwiseValues[2]);
			}
			if ( angle >= 225f && angle <= 315f )//south
			{
				animator.SetInteger("direction", counterclockwiseValues[0]);
			}
			if ( angle >= 315f || angle <= 45f )//east
			{
				animator.SetInteger("direction", counterclockwiseValues[1]);
			}
			if ( angle >= 135f && angle <= 225f )//west
			{
				animator.SetInteger("direction", counterclockwiseValues[3]);
			}
			
			if( hasDiagonalDirections )
			{
				if ( angle >= 22.5f && angle <= 67.5f )//NE
				{
					animator.SetInteger("direction", counterclockwiseValues[5]);
				}
				if (angle >= 112.5f && angle <= 157.5f )//NW
				{
					animator.SetInteger("direction", counterclockwiseValues[6]);
				}
				if ( angle >= 292.5f && angle <= 337.5 )//SE
				{
					animator.SetInteger("direction", counterclockwiseValues[4]);
				}
				if ( angle >= 202.5f && angle <= 247.5f )//SW
				{
					animator.SetInteger("direction", counterclockwiseValues[7]);
				}
			}
		}

		public static void SetEnabledAfter( this IList<Animator> animators, bool enabled = true, float after = 1f )
		{
			if( animators == null )
				return;
            animators.SetEnabledAfterCo( enabled, after ).Run();
		}
        public static IEnumerator<float> SetEnabledAfterCo( this IList<Animator> animators, bool enabled = true, float after = 1f )
		{
			yield return Timing.WaitForSeconds( after );
			if( animators == null )
				yield break;
			for( int i=0; i<animators.Count; i++ )
			{
				if( animators[i] != null )
				{
					animators[i].enabled = enabled;
				}
			}
		}
		public static void SetInt( this IList<Animator> animators, string paramName, int value )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Count; i++ )
			{
				if( animators[i] )
				{
					if( !animators[i].HasParam( paramName ) )
						continue;
					animators[i].SetIntValue( paramName, value );
				}
			}
		}	
		public static void SetFloat( this IList<Animator> animators, string paramName, float value )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Count; i++ )
			{
				if( animators[i] )
				{
					if( !animators[i].HasParam( paramName ) )
						continue;
					animators[i].SetFloatValue( paramName, value );
				}
			}
		}	
		public static void SetBool( this IList<Animator> animators, string paramName, bool value )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Count; i++ )
			{
				if( animators[i] )
				{
					if( !animators[i].HasParam( paramName ) )
						continue;
					animators[i].SetBoolValue( paramName, value );
				}
			}
		}	
		public static void SetTrigger( this IList<Animator> animators, string paramName )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Count; i++ )
			{
				if( animators[i] )
				{
					if( !animators[i].HasParam( paramName ) )
						continue;
					animators[i].SetTriggerValue( paramName );
				}
			}
		}	
		/// <summary>
		/// Sets the specified bool parameter.
		/// </summary>
		/// <param name="duration">If equal or below cero (0) it won't be taken into account. Hence, the animator's
		/// param will stay /value/.</param>
		public static void SetBoolForDuration( this Animator controller, string paramName, bool value, float duration )
		{
			if( duration <= 0f )
				controller.SetBoolValue( paramName, value );
			else controller.SetBoolForDurationCo( paramName, value, duration ).Start();
		}
		/// <summary>
		/// Sets the specified bool parameter.
		/// </summary>
		/// <param name="duration">If equal or below cero (0) it won't be taken into account. Hence, the animator's
		/// param will stay /value/.</param>
		public static IEnumerator SetBoolForDurationCo( this Animator controller, string paramName, bool value, float duration )
		{
			if( !controller.enabled )
				yield break;
			controller.SetBoolValue( paramName, value );
			yield return new WaitForSeconds( duration );
			controller.SetBoolValue( paramName, !value );
		}
		/// <summary>
		/// Sets the bool param's value. If the parameter doesn't exist, this does nothing.
		/// </summary>
		public static void SetBoolValue( this Animator anim, string param, bool value )
		{
			anim.SetBool( param, value, 1f );
		}
		/// <summary>
		/// Sets the bool param's value. If the parameter doesn't exist, this does nothing.
		/// </summary>
		/// <param name="ani">Animator.</param>
		/// <param name="param">Parameter name.</param>
		/// <param name="value">Param's value.</param>
		/// <param name="animatorSpeed">Animator's speed.</param>
		/// <param name="resetSpeedAfterAnimDone">If set to <c>true</c> reset speed after animation is done.</param>
		/// <param name="currentLayer">Current layer.</param>
		public static void SetBool( this Animator ani, string param, bool value, float animatorSpeed, bool resetSpeedAfterAnimDone = true, int currentLayer = 0 )
		{
			if( !ani.HasParam( param ) )
				return;
			if( resetSpeedAfterAnimDone )
			{
				ani._SetBool( param, value, animatorSpeed, currentLayer ).Start();
			}
			else
			{
				ani.speed = animatorSpeed;
				ani.SetBool( param, value );
			}
		}		
		private static IEnumerator _SetBool( this Animator ani, string param, bool value, float animatorSpeed, int currentLayer )
		{
            if( ani == null )
                yield break;
			float iniSpeed = ani.speed;
			ani.speed = animatorSpeed;
			ani.SetBool( param, value );
			var aniState = ani.GetCurrentAnimatorStateInfo( currentLayer );
			while( aniState.normalizedTime < 1 )//wait for animation to end
			{
				yield return null;
			}
			ani.speed = iniSpeed;
		}
		/// <summary>
		/// Sets the trigger param's value. If the parameter doesn't exist, this does nothing.
		/// </summary>
		public static void SetTriggerValue( this Animator anim, string param )
		{
			anim.SetTrigger( param, 1f );
		}
		/// <summary>
		/// Sets the trigger param's value. If the parameter doesn't exist, this does nothing.
		/// </summary>
		/// <param name="ani">Animator.</param>
		/// <param name="param">Parameter name.</param>
		/// <param name="animatorSpeed">Animator's speed.</param>
		/// <param name="resetSpeedAfterAnimDone">If set to <c>true</c> reset speed after animation is done.</param>
		/// <param name="currentLayer">Current layer.</param>
		public static void SetTrigger( this Animator ani, string param, float animatorSpeed, bool resetSpeedAfterAnimDone = true, int currentLayer = 0 )
		{
            if( !ani || !ani.HasParam( param ) )
				return;
            if( ani.speed != animatorSpeed && resetSpeedAfterAnimDone )
			{
                ani._SetTrigger( param, animatorSpeed, currentLayer ).Run();
			}
			else
			{
				ani.speed = animatorSpeed;
				ani.SetTrigger( param );
			}
		}		
        private static IEnumerator<float> _SetTrigger( this Animator ani, string param, float animatorSpeed, int currentLayer )
		{
            if( ani == null )
                yield break;
			float iniSpeed = ani.speed;
			ani.speed = animatorSpeed;
			ani.SetTrigger( param );
			var aniState = ani.GetCurrentAnimatorStateInfo( currentLayer );
			while( aniState.normalizedTime < 1 )//wait for animation to end
			{
				yield return 0f;
			}
			ani.speed = iniSpeed;
		}
		/// <summary>
		/// Sets the float param's value. If the parameter doesn't exist, this does nothing.
		/// </summary>
		public static void SetFloatValue( this Animator anim, string param, float value )
		{
			anim.SetFloat( param, value, 1f );
		}
		/// <summary>
		/// Sets the float param's value. If the parameter doesn't exist, this does nothing.
		/// </summary>
		/// <param name="ani">Animator.</param>
		/// <param name="param">Parameter name.</param>
		/// <param name="value">Param's value.</param>
		/// <param name="animatorSpeed">Animator's speed.</param>
		/// <param name="resetSpeedAfterAnimDone">If set to <c>true</c> reset speed after animation is done.</param>
		/// <param name="currentLayer">Current layer.</param>
		public static void SetFloat( this Animator ani, string param, float value, float animatorSpeed, bool resetSpeedAfterAnimDone = true, int currentLayer = 0 )
		{
            if( !ani || !ani.HasParam( param ) )
				return;
            if( ani.speed != animatorSpeed && resetSpeedAfterAnimDone )
			{
				ani._SetFloat( param, value, animatorSpeed, currentLayer ).Run();
			}
			else
			{
				ani.speed = animatorSpeed;
				ani.SetFloat( param, value );
			}
		}		
        private static IEnumerator<float> _SetFloat( this Animator ani, string param, float value, float animatorSpeed, int currentLayer )
		{
            if( null == ani ) yield break;

			float iniSpeed = ani.speed;
			ani.speed = animatorSpeed;
			ani.SetFloat( param, value );
			var aniState = ani.GetCurrentAnimatorStateInfo( currentLayer );
			while( aniState.normalizedTime < 1 )//wait for animation to end
			{
				yield return 0f;
			}
			ani.speed = iniSpeed;
		}
		/// <summary>
		/// Sets the integer param's value. If the parameter doesn't exist, this does nothing.
		/// </summary>
		public static void SetIntValue( this Animator anim, string param, int value )
		{
			anim.SetInt( param, value, 1f );
		}
		/// <summary>
		/// Sets the integer param's value. If the parameter doesn't exist, this does nothing.
		/// </summary>
		/// <param name="ani">Animator.</param>
		/// <param name="param">Parameter name.</param>
		/// <param name="value">Param's value.</param>
		/// <param name="animatorSpeed">Animator's speed.</param>
		/// <param name="resetSpeedAfterAnimDone">If set to <c>true</c> reset speed after animation is done.</param>
		/// <param name="currentLayer">Current layer.</param>
		public static void SetInt( this Animator ani, string param, int value, float animatorSpeed, bool resetSpeedAfterAnimDone = true, int currentLayer = 0 )
		{
			if( !ani.HasParam( param ) )
				return;
			if( resetSpeedAfterAnimDone )
			{
				ani._SetInt( param, value, animatorSpeed, currentLayer ).Start();
			}
			else
			{
				ani.speed = animatorSpeed;
				ani.SetInteger( param, value );
			}
		}		
		private static IEnumerator _SetInt( this Animator ani, string param, int value, float animatorSpeed, int currentLayer )
		{
			float iniSpeed = ani.speed;
			ani.speed = animatorSpeed;
			ani.SetInteger( param, value );
			var aniState = ani.GetCurrentAnimatorStateInfo( currentLayer );
			while( aniState.normalizedTime < 1 )//wait for animation to end
			{
				yield return null;
			}
			ani.speed = iniSpeed;
		}
		public static void SetBool( this Animator animator, string args = "paramName:T", char argsSplitter = ':' )
		{
			if( !animator )
				return;
			string[] _args = args.Split( argsSplitter );
			if( _args.Length != 2 )
			{
				Debug.LogError ("Wrong arguments! They must follow the pattern: paramName + /argsSplitter/ + bool value (True, False, T, F)");
				return;
			}
			string paramName = _args[0];
			string value = _args[1];
			animator.SetBoolValue( paramName, value.ToBool() );
		}
		public static void SetInt( this Animator animator, string args = "paramName:1", char argsSplitter = ':' )
		{
			if( !animator )
				return;
			string[] _args = args.Split( argsSplitter );
			if( _args.Length != 2 )
			{
				Debug.LogError ("Wrong arguments! They must follow the pattern: paramName + /argsSplitter/ + int value");
				return;
			}
			string paramName = _args[0];
			string value = _args[1];
			animator.SetIntValue( paramName, value.ToInt() );
		}
		public static void SetFloat( this Animator animator, string args = "paramName:1f", char argsSplitter = ':' )
		{
			if( !animator )
				return;
			string[] _args = args.Split( argsSplitter );
			if( _args.Length != 2 )
			{
				Debug.LogError ("Wrong arguments! They must follow the pattern: paramName + /argsSplitter/ + float value");
				return;
			}
			string paramName = _args[0];
			string value = _args[1];
			animator.SetFloatValue( paramName, value.ToFloat() );
		}
		public static void SetBool( this IList<Animator> animators, string args = "paramName:T", char argsSplitter = ':' )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Count; i++ )
			{
				animators[i].SetBool( args, argsSplitter );
			}
		}
		public static void SetInt( this IList<Animator> animators, string args = "paramName:1", char argsSplitter = ':' )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Count; i++ )
			{
				animators[i].SetInt( args, argsSplitter );
			}
		}
		public static void SetFloat( this IList<Animator> animators, string args = "paramName:1f", char argsSplitter = ':' )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Count; i++ )
			{
				animators[i].SetFloat( args, argsSplitter );
			}
		}
        public static IEnumerator<float> AnimateFloatToCo( this Animator animator, string paramName, float target, float duration )
        {
            if( !animator )
                yield break;
            float time = Time.deltaTime;
            float from = animator.GetFloat( paramName );
            //float wait = (duration * 0.1f).Clamp( 0.05f, 0.2f );//This will improve this coroutine's performance
            while( time < duration )
            {
                animator.SetFloatValue( paramName, Mathf.Lerp( from, target, time / duration ) );
                yield return 0f;
                time += Time.deltaTime;
                if( !animator )
                    break;
            }
        }
		public static void AnimateFloatTo( this Animator animator, string paramName, float target, float duration )
		{
			if( !animator )
				return;
            animator.AnimateFloatToCo( paramName, target, duration ).Run();
		}
		public static void AnimateFloatTo( this Animator animator, string args = "paramName:1f:1f", char argsSplitter = ':' )
		{
			if( !animator )
				return;
			string[] _args = args.Split( argsSplitter );
			if( _args.Length != 3 )
			{
				Debug.LogError ("Wrong arguments! They must follow the pattern: paramName "+ argsSplitter +
				                " float target "+ argsSplitter +" float duration");
				return;
			}
			string paramName = _args[0];
			string target = _args[1];
			string duration = _args[2];
			animator.AnimateFloatTo( paramName, target.ToFloat(), duration.ToFloat() );
		}
		public static IEnumerator AnimateLayerWeightToCo( this Animator animator, int layerIndex, float target, float duration )
		{
			if( !animator )
				yield break;
			float time = 0f;
			float currentWeight = animator.GetLayerWeight( layerIndex );
			while( time < duration )
			{
				time += Time.deltaTime;
				animator.SetLayerWeight( layerIndex, Mathf.Lerp( currentWeight, target, time / duration ) );
				yield return null;
			}
		}
		public static void AnimateLayerWeightTo( this Animator animator, int layerIndex, float target, float duration )
		{
			animator.AnimateLayerWeightToCo( layerIndex, target, duration ).Start();
		}
		public static void AnimateLayerWeightTo( this Animator animator, string args = "paramName:1f:1f", char argsSplitter = ':' )
		{
			if( !animator )
				return;
			string[] _args = args.Split( argsSplitter );
			if( _args.Length != 3 )
			{
				Debug.LogError ("Wrong arguments! They must follow the pattern: layerIndex "+ argsSplitter +
				                " float target "+ argsSplitter +" float duration");
				return;
			}
			string layerIndex = _args[0];
			string target = _args[1];
			string duration = _args[2];
			animator.AnimateLayerWeightTo( layerIndex.ToInt(), target.ToFloat(), duration.ToFloat() );
		}
		/// <summary>
		/// Sets the speed value for all this animators
		/// </summary>
		public static void SetSpeed( this List<Animator> animators, float speed )
		{
			animators.ToArray().SetSpeed( speed );
		}
		/// <summary>
		/// Sets the speed value for all this animators
		/// </summary>
		public static void SetSpeed( this Animator[] animators, float speed )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Length; i++ )
			{
				if( !animators[i] )
					continue;
				animators[i].speed = speed;
			}
		}
		/// <summary>
		/// Sets/Divides the speed value for all this animators by the specified /divisor/
		/// </summary>
		public static void SetSpeedDivide( this List<Animator> animators, float divisor )
		{
			animators.ToArray().SetSpeedDivide( divisor );
		}
		/// <summary>
		/// Sets/Divides the speed value for all this animators by the specified /divisor/
		/// </summary>
		public static void SetSpeedDivide( this Animator[] animators, float divisor )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Length; i++ )
			{
				if( !animators[i] )
					continue;
				animators[i].speed /= divisor;
			}
		}
		/// <summary>
		/// Sets/Multiplies the speed value for all this animators by the specified /divisor/
		/// </summary>
		public static void SetSpeedMultiply( this List<Animator> animators, float multiplier )
		{
			animators.ToArray().SetSpeedMultiply( multiplier );
		}
		/// <summary>
		/// Sets/Multiplies the speed value for all this animators by the specified /divisor/
		/// </summary>
		public static void SetSpeedMultiply( this Animator[] animators, float multiplier )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Length; i++ )
			{
				if( !animators[i] )
					continue;
				animators[i].speed *= multiplier;
			}
		}
		public static void SetLayersWeight( this IList<Animator> animators, int layerIndex, float value )
		{
			if( animators == null )
				return;
			value = Mathf.Clamp01( value );
			for( int i=0; i<animators.Count; i++ )
			{
				if( !animators[i] )
					continue;
				animators[i].SetLayerWeight( layerIndex, value );
			}
		}
		#endregion

		#region MISC
		public static void Enable( this Animator[] animators, bool enabled = true )
		{
            if( animators == null || animators.Length == 0 )
                return;
			for( int i=0; i<animators.Length; i++ )
			{
				animators[i].enabled = enabled;
			}
		}
		
		public static void Enable( this List<Animator> animators, bool enabled = true )
		{
			for( int i=0; i<animators.Count; i++ )
			{
				if( animators[i] != null )
				{
					animators[i].enabled = enabled;
				}
			}
		}
		/// <summary>
		/// Waits for the current state's animation to end. This can be yielded to wait for something inside a coroutine.
		/// </summary>
		public static IEnumerator WaitForAnimToEnd( this Animator ani, int layer = 0 )
		{
			var aniState = ani.GetCurrentAnimatorStateInfo( layer );
			while( aniState.normalizedTime < 1 )
			{
				yield return null;
			}
		}
        //[System.Obsolete("This overloads the performance, try something else")]
		public static bool HasParam( this Animator animator, string paramName )
		{
            return true;
            //BAD FOR PERFORMANCE
			/*if( !animator )
			{
				Debug.LogWarning ("The specified animator is null");
				return false;
			}
			AnimatorControllerParameter param;
			for (int i = 0; i < animator.parameters.Length; i++) 
			{
				param = animator.parameters[i];
				if( param.name.Equals( paramName ) )
					return true;
			}
			return false;*/
		}
		public static AnimatorControllerParameter GetParam( this Animator animator, string paramName )
		{
			AnimatorControllerParameter param;
			for (int i = 0; i < animator.parameters.Length; i++) 
			{
				param = animator.parameters[i];
				if( param.name.Equals( paramName ) )
					return param;
			}
			Debug.LogWarning ( "Param: \""+ paramName +"\" doesn't exist in the specified Animator", animator.gameObject );
			return null;
		}
		public static void CrossFade( this IList<Animator> animators, string stateName, float transitionDuration, int layer )
		{
			if( animators == null )
				return;
			for( int i=0; i<animators.Count; i++ )
			{
				if( !animators[i] )
					continue;
				animators[i].CrossFade( stateName, transitionDuration, layer );
			}
		}
		#endregion

	}
}