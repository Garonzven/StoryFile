using System.Collections;

using DDK.Base.Animations;
using DDK.Base.Classes;

using UnityEngine;
using System.Collections.Generic;
using DDK.Base.Fx.Transitions;

namespace DDK.Base.Extensions
{
    // <summary>
    /// Transform class extension.
    /// </summary>
    public static class TransformExt
    {
		#region ROTATING
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="axis">The rotation axis.</param>
		public static void AnimRotateAroundTo ( this Transform obj, Vector3 point, Vector3 target,
		                                       float duration, Vector3 axis )
		{
			obj.gameObject.AnimRotateAroundToCo ( point, target, duration, axis ).Start();
		}
		
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. The point tranform's forward axis will be used as the rotation axis.
		/// NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		public static void AnimRotateAroundTo ( this Transform obj, Transform point, Vector3 target, float duration )
		{
			obj.gameObject.AnimRotateAroundToCo ( point, target, duration, point ).Start();
		}
		
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="axis">The rotation axis.</param>
		public static void AnimRotateAroundTo ( this Transform obj, Transform point,
		                                       Vector3 target, float duration, Transform axis )
		{
			obj.gameObject.AnimRotateAroundToCo ( point, target, duration, axis ).Start();
		}		
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. The point tranform's forward axis will be used as the rotation axis.
		/// NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		public static void AnimRotateAroundTo ( this Transform obj, Transform point, Transform target, float duration )
		{
			obj.gameObject.AnimRotateAroundToCo ( point, target, duration, point ).Start();
		}		
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="axis">The rotation axis.</param>
		public static void AnimRotateAroundTo ( this Transform obj, Transform point,
		                                       Transform target, float duration, Transform axis )
		{
			obj.gameObject.AnimRotateAroundToCo ( point, target, duration, axis ).Start();
		}		
		/// <summary>
		/// This must be called multiple times as transform.RotateAround().
		/// </summary>
		public static void RotateAroundEllipse ( this Transform obj, Ellipse ellipse, float speed /*, Vector3 normal*/ )
		{
			ellipse._alpha -= speed;
			float X = ellipse._Center.x + ( ellipse.a * Mathf.Cos ( ellipse._alpha * 0.005f ) );
			float Y = ellipse._Center.y + ( ellipse.b * Mathf.Sin ( ellipse._alpha * 0.005f ) );
			//obj.position = new Vector3( X * ( 1f - normal.x ), Y * ( 1f - normal.y ), obj.position.z * normal.z );
			obj.position = new Vector3 ( X, Y, obj.position.z );
		}
        /// <summary>
        /// Rotates this transform only in the specified axes.
        /// </summary>
        public static void RotateAxes( this Transform t, Vector3 axes, Vector3 rotation )
        {
            if( !t )
                return;
            for( int i=0; i<3; i++ )
            {
                if( axes[i] == 0f )
                    rotation.x = t.rotation.eulerAngles.x;
            }
            t.rotation = Quaternion.Euler( rotation );
        }
		/// <summary>
		/// Rotates this object around the specified /ellipse/'s center, until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		public static void AnimRotateAroundEllipseTo ( this Transform obj, Ellipse ellipse, Vector3 target, 
		                                              float duration )
		{
			obj.AnimRotateAroundEllipseToCo ( ellipse, target, duration ).Start();
		}
		/// <summary>
		/// Rotates this object around the specified /ellipse/'s center, until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		public static IEnumerator AnimRotateAroundEllipseToCo ( this Transform obj, Ellipse ellipse, Vector3 target, 
		                                                       float duration )
		{
			if( !obj )
				yield break;
			ValidationFlag validationFlags = obj.GetOrAddComponent<ValidationFlag>();
			if ( validationFlags.IsFlagged ( ValidationFlag.Flags.RotateAround ) )
				yield break;
			
			obj.SetFlagged ( ValidationFlag.Flags.RotateAround, true );
			float time = 0f;
			Vector3 targetDir = target - ellipse._Center;
			Vector3 startDir = obj.transform.position - ellipse._Center;
			float distanceToTarget = Vector3.Distance ( ellipse._Center, target );
			
			//float distanceToStart = Vector3.Distance( ellipse._center, obj.position );
			while ( obj.transform.position.Distance ( target ) >= 0.01f )
			{
				if ( duration == 0f )
				{ break; }
				
				if ( time > duration )
				{ break; }
				
				time += Time.deltaTime;
				obj.transform.position = ellipse._Center + Vector3.Lerp (
					startDir, targetDir, time / duration ).normalized * ( distanceToTarget );
				yield return null;
				if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.RotateAround ) )
					yield break;
				if( !obj )
					yield break;
			}
			
			obj.transform.position = target;
			validationFlags.SetFlagged ( ValidationFlag.Flags.RotateAround, false );
			yield return null;
		}
		/// <summary>
		/// Aligns this object's orientation until it matches the specified target's. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="useLerp">If set to <c>true</c> use Lerp which is faster than Slerp but might cause undesired behaviour.</param>
		public static void AnimRotateTo ( this Transform current, float duration, Transform target, bool useLerp = false )
		{
			current.gameObject.RotateToCo ( duration, target, useLerp ).Start();
		}
		/// <summary>
		/// Aligns this object's orientation until it matches the specified target's. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="useLerp">If set to <c>true</c> use Lerp which is faster than Slerp but might cause undesired behaviour.</param>
		public static void AnimRotateTo ( this Transform current, float duration, GameObject target, bool useLerp = false )
		{
			current.gameObject.RotateToCo ( duration, target.transform, useLerp ).Start();
		}
		/// <summary>
		/// Turns to the transform source to the direction
		/// of target
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="speed">The speed.</param>
		/// <returns></returns>
		public static IEnumerator TurnToDirectionCo ( this Transform source, Transform target, float speed = 1f )
		{
			var srcRot = source.rotation;
			var direction = ( target.position - source.position ).normalized;
			var angle = Vector3.Angle ( source.forward, direction );
			var duration = angle / speed;
			var time = 0f;
			
			while ( time < duration )
			{
				time += Time.deltaTime;
				source.rotation = srcRot * Quaternion.AngleAxis( angle * ( time / duration ), source.up );
				yield return null;
			}
		}
		
		public static IEnumerator TurnToDirectionCo ( this Transform source, Vector3 target, float speed = 1f )
		{
			var srcRot = source.rotation;
			var direction = ( target - source.position ).normalized;
			var angle = Vector3.Angle ( source.forward, direction );
			var duration = angle / ( speed * 180f );
			var time = 0f;
			
			while ( time < duration )
			{
				time += Time.deltaTime;
				source.rotation = srcRot * Quaternion.AngleAxis( angle * ( time / duration ), source.up );
				yield return null;
			}
		}
		/// <summary>
		///     Rotates a object around the axis.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="axis">The axis.</param>
		/// <param name="duration">The duration.</param>
		/// <param name="speed">The speed.</param>
		/// <returns></returns>
		public static IEnumerator RotateInAxisCo ( this Transform source, Vector3 axis, float duration, float speed )
		{
			var time = 0f;
			
			while ( time < duration )
			{
				time += Time.deltaTime;
				source.rotation *= Quaternion.AngleAxis ( speed * Time.deltaTime, axis );
				yield return null;
			}
		}
		/// <summary>
		///     Rotates to target.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="duration">The duration.</param>
		/// <param name="useSlerp">if set to <c>true</c> [use slerp].</param>
		/// <returns></returns>
		public static IEnumerator RotateToTargetCo ( this Transform source,
		                                            Quaternion target, float duration = 1f, bool useSlerp = false )
		{
			var time = 0f;
			var init = source.rotation;
			
			while ( time < duration )
			{
				time += Time.deltaTime;
				source.rotation = useSlerp
					? Quaternion.Slerp ( init, target, time / duration )
						: Quaternion.Lerp ( init, target, time / duration );
				yield return null;
			}
			
			source.rotation = target;
		}
		#endregion
        
		#region POSITIONING
		/// <summary>
		/// Moves towards the specified target. This need to be called multiple times. The next position is automatically set.
		/// </summary>
		/// <returns>A value closer to the target.</returns>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static Vector3 MoveTowards ( this Transform current, Vector3 target,
		                                   float deltaSpeed = 1f, bool local = false, bool useRectTransform = false )
		{
			return current.gameObject.MoveTowards ( target, deltaSpeed, local, useRectTransform );
		}
		/// <summary>
		/// Moves towards the specified target. NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static IEnumerator MoveTowardsCo ( this Transform current, Vector3 target, float deltaSpeed = 1f, 
		                                         bool local = false, bool useRectTransform = false )
		{
			yield return current.gameObject.MoveTowardsCo ( target, deltaSpeed, local, useRectTransform ).Start();
		}
		/// <summary>
		/// Moves towards the specified target until it is reached. This calls MoveTowardsCo(). NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// If this object's Move flag is set to false, the coroutine will end.
		/// </summary>
		/// <returns>A value closer to the target.</returns>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static void AnimMoveTowards ( this Transform current, Vector3 target, float deltaSpeed = 1f, 
		                                    bool local = false, bool useRectTransform = false )
		{
			current.MoveTowardsCo ( target, deltaSpeed, local, useRectTransform ).Start();
		}
		/// <summary>
		/// Moves towards the specified target. NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static IEnumerator MoveTowardsCo ( this Transform current, float duration, Vector3 target, 
		                                         bool local = false, bool useRectTransform = false )
		{
			if( !current )
				yield break;
			yield return current.gameObject.MoveTowardsCo ( duration, target, local, useRectTransform ).Start();
		}
		/// <summary>
		/// Moves towards the specified target until it is reached in the specified amount of time, or this object or 
		/// its target is destroyed. NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static IEnumerator MoveTowardsCo ( this Transform current, float duration, Transform target, 
		                                         bool local = false, bool useRectTransform = false )
		{
			if( !current || !target )
				yield break;
			yield return current.gameObject.MoveTowardsCo ( duration, target, local, useRectTransform ).Start();
		}
		/// <summary>
		/// Moves towards the specified target until it is reached. This calls MoveTowardsCo(). NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// If this object's Move flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static void AnimMoveTowards ( this Transform current, float duration, Vector3 target, bool local = false,
		                                    bool useRectTransform = false )
		{
			if( !current )
				return;
			current.MoveTowardsCo ( duration, target, local, useRectTransform ).Start();
		}
		/// <summary>
		/// Moves towards the specified target until it is reached. This calls MoveTowardsCo(). NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// If this object's Move flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static void AnimMoveTowards ( this Transform current, float duration, Transform target, 
		                                    bool local = false, bool useRectTransform = false )
		{
			if( !current || !target )
				return;
			current.MoveTowardsCo ( duration, target, local, useRectTransform ).Start();
		}
		/// <summary>
		///     Moves the object in the specified direction.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="direction">The direction.</param>
		/// <param name="duration">The duration.</param>
		/// <param name="speed">The speed.</param>
		/// <returns></returns>
		public static IEnumerator MoveInDirectionCo ( this Transform source, Vector3 direction, float duration = 1f,
		                                             float speed = 1f )
		{
			var time = 0f;
			var step = direction.normalized;
			
			while ( time < duration )
			{
				time += Time.deltaTime;
				source.position = Vector3.MoveTowards ( source.position, source.position
				                                       + step, Time.deltaTime * speed );
				yield return null;
			}
		}
		/// <summary>
		///     Moves the object to the target.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="duration">The duration.</param>
		/// <param name="useSlerp">if set to <c>true</c> [use slerp].</param>
		/// <returns></returns>
		public static IEnumerator MoveToTargetCo ( this Transform source, Vector3 target, float duration = 1f,
		                                          bool useSlerp = false )
		{
			var time = 0f;
			var init = source.position;
			
			while ( time < duration )
			{
				time += Time.deltaTime;
				source.position = useSlerp
					? Vector3.Slerp ( init, target, time / duration )
						: Vector3.Lerp ( init, target, time / duration );
				yield return null;
			}
			
			source.position = target;
		}
		/// <summary>
		/// Returns the distance from this transform to the specified /b/
		/// </summary>
		public static float Distance( this Transform a, Transform b )
		{
			if( !a || !b )
			{
				return 0f;
			}
			return a.position.Distance( b.position );
		}
		#endregion
        
		#region ALIGNMENTS
		/// <summary>
		/// Aligns this transform with the specified target. This needs to be called multiple times. The next position and orientation are automatically set.
		/// </summary>
		public static void AlignWith ( this Transform current, Transform target, float deltaSpeed )
		{
			if( !current || !target )
				return;
			current.gameObject.MoveTowards ( target.position, deltaSpeed );
			current.gameObject.LookRotation ( target );
		}
		/// <summary>
		/// Aligns this transform with the specified target. This needs to be called multiple times. The next position and orientation are automatically set.
		/// </summary>
		/// <param name="targetPos"> Overrides the target's position </param>
		public static void AlignWith ( this Transform current, Vector3 targetPos, Transform target, float deltaSpeed )
		{
			if ( !current || !target )
			{ return; }
			
			current.gameObject.MoveTowards ( targetPos, deltaSpeed );
			current.gameObject.LookRotation ( target );
		}
		/// <summary>
		/// Aligns this transform with the specified target. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being aligned.
		/// If this object's Align flag is set to false, the coroutine will end.
		/// </summary>
		public static IEnumerator AlignCo ( this Transform current, Transform target, float duration )
		{
			if ( !current || !target )
				yield break;

			ValidationFlag validationFlags = current.GetOrAddComponent<ValidationFlag>();
			if ( validationFlags.IsFlagged ( ValidationFlag.Flags.Align ) )
				yield break;
			
			validationFlags.SetFlagged ( ValidationFlag.Flags.Align, true );
			current.gameObject.AnimMoveTowards ( duration, target );
			current.AnimRotateTo ( duration, target );

			float time = 0f;
			while ( time <= duration )
			{
				time += Time.deltaTime;
				yield return null;
				if ( !current || !validationFlags.IsFlagged ( ValidationFlag.Flags.Align ) )
					yield break;
				if( !target )
					break;
			}
			validationFlags.SetFlagged ( ValidationFlag.Flags.Align, false );
			yield return null;
		}
		/// <summary>
		/// Aligns this transform with the specified target. This calls AlignCo(). NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being aligned.
		/// If this object's Align flag is set to false, the coroutine will end.
		/// </summary>
		public static void Align ( this Transform current, Transform target, float duration )
		{
			current.AlignCo ( target, duration ).Start();
		}
		/// <summary>
		///     Aligns to target.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="position">The position.</param>
		/// <param name="rotation">The rotation.</param>
		/// <param name="duration">The duration.</param>
		/// <param name="useSlerp">if set to <c>true</c> [use slerp].</param>
		/// <returns></returns>
		public static IEnumerator AlignToTargetCo ( this Transform source,
		                                           Vector3 position, Quaternion rotation, float duration = 1f,
		                                           bool useSlerp = true )
		{
			var time = 0f;
			var rota = source.rotation;
			var posi = source.position;
			
			while ( time < duration )
			{
				time += Time.deltaTime;
				source.rotation = useSlerp
					? Quaternion.Slerp ( rota, rotation, time / duration )
						: Quaternion.Lerp ( rota, rotation, time / duration );
				source.position = useSlerp
					? Vector3.Slerp ( posi, position, time / duration )
						: Vector3.Lerp ( posi, position, time / duration );
				yield return null;
			}
			
			source.rotation = rotation;
			source.position = position;
		}
		#endregion
        
		#region LOOK AT
		public static void LookAtPos ( this Transform t, Vector3 axis, Vector3 worldPos )
		{
			if ( axis == Vector3.up ) { t.LookAt ( new Vector3 ( worldPos.x, t.position.y, worldPos.z ) ); }
			else if ( axis == Vector3.right ) { t.LookAt ( new Vector3 ( t.position.x, worldPos.y, worldPos.z ) ); }
			else if ( axis == Vector3.forward ) { t.LookAt ( new Vector3 ( worldPos.x, worldPos.y, t.position.z ) ); }
			else if ( axis == new Vector3 ( 0f, 1f, 1f ) ) { t.LookAt ( new Vector3 ( worldPos.x, t.position.y, t.position.z ) ); }
			else if ( axis == new Vector3 ( 1f, 1f, 0f ) ) { t.LookAt ( new Vector3 ( t.position.x, t.position.y, worldPos.z ) ); }
			else if ( axis == new Vector3 ( 1f, 0f, 1f ) ) { t.LookAt ( new Vector3 ( t.position.x, worldPos.y, t.position.z ) ); }
		}
		public static void LookAt ( this Transform t, Vector3 axis, Transform target )
		{
			if ( axis == Vector3.up ) { t.LookAt ( new Vector3 ( target.position.x, t.position.y, target.position.z ) ); }
			else if ( axis == Vector3.right ) { t.LookAt ( new Vector3 ( t.position.x, target.position.y, target.position.z ) ); }
			else if ( axis == Vector3.forward ) { t.LookAt ( new Vector3 ( target.position.x, target.position.y, t.position.z ) ); }
			else if ( axis == new Vector3 ( 0f, 1f, 1f ) ) { t.LookAt ( new Vector3 ( target.position.x, t.position.y, t.position.z ) ); }
			else if ( axis == new Vector3 ( 1f, 1f, 0f ) ) { t.LookAt ( new Vector3 ( t.position.x, t.position.y, target.position.z ) ); }
			else if ( axis == new Vector3 ( 1f, 0f, 1f ) ) { t.LookAt ( new Vector3 ( t.position.x, target.position.y, t.position.z ) ); }
		}
		/// <summary>
		/// Animates the specified transform until it's looking at the specified target. This calls LookAtCo.
		/// NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// </summary>
		public static void LookAt ( this Transform t, Transform target, float duration )
		{
			t.LookAtCo ( target, duration ).Start();
		}
		/// <summary>
		/// Animates the specified transform until it's looking at the specified target.
		/// NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// </summary>
		public static IEnumerator LookAtCo ( this Transform t, Transform target, float duration )
		{
			if( !t )
				yield break;
			ValidationFlag validationFlags = t.GetOrAddComponent<ValidationFlag>();
			if ( validationFlags.IsFlagged ( ValidationFlag.Flags.Rotate ) )
				yield break;
			
			validationFlags.SetFlagged ( ValidationFlag.Flags.Rotate, true );
			float time = Time.deltaTime;
			
			while ( time <= duration )
			{
				var startRotation = t.rotation;
				var endRotation = Quaternion.LookRotation ( target.position - t.position );
				
				if ( startRotation == endRotation )
				{ break; }
				
				t.rotation = Quaternion.Slerp ( startRotation, endRotation, time / duration );
				yield return null;
				if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.Rotate ) )
					yield break;
				if( !t )
					yield break;
				time += Time.deltaTime;
			}
			
			t.LookAt ( target );
			validationFlags.SetFlagged ( ValidationFlag.Flags.Rotate, false );
		}
		/// <summary>
		/// Animates the specified transform until it's looking at the specified target. This calls LookAtCo.
		/// NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// </summary>
		public static void LookAt ( this Transform t, Vector3 target, float duration )
		{
			t.LookAtCo ( target, duration ).Start();
		}
		/// <summary>
		/// Animates the specified transform until it's looking at the specified target.
		/// NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// </summary>
		public static IEnumerator LookAtCo ( this Transform t, Vector3 target, float duration )
		{
			if( !t )
				yield break;

			ValidationFlag validationFlags = t.GetOrAddComponent<ValidationFlag>();
			if ( validationFlags.IsFlagged ( ValidationFlag.Flags.Rotate ) )
				yield break;
			
			validationFlags.SetFlagged ( ValidationFlag.Flags.Rotate, true );
			float time = Time.deltaTime;
			
			while ( time <= duration )
			{
				var startRotation = t.rotation;
				var endRotation = Quaternion.LookRotation ( target - t.position );
				
				if ( startRotation == endRotation )
				{ break; }
				
				t.rotation = Quaternion.Slerp ( startRotation, endRotation, time / duration );
				yield return null;
				if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.Rotate ) )
					yield break;
				if( !t )
					yield break;
				time += Time.deltaTime;
			}
			
			t.LookAt ( target );
			validationFlags.SetFlagged ( ValidationFlag.Flags.Rotate, false );
		}
		/// <summary>
		///     Looks at target.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="duration">The duration.</param>
		/// <param name="useSlerp">if set to <c>true</c> [use slerp].</param>
		/// <returns></returns>
		public static IEnumerator LookAtTargetCo ( this Transform source, Vector3 target,
		                                          float duration = 1f, bool useSlerp = true )
		{
			var time = 0f;
			var lookAt = Quaternion.LookRotation ( target - source.position );
			var init = source.rotation;
			
			while ( time < duration )
			{
				time += Time.deltaTime;
				source.rotation = useSlerp
					? Quaternion.Slerp ( init, lookAt, time / duration )
						: Quaternion.Lerp ( init, lookAt, time / duration );
				yield return null;
			}
			
			source.rotation = lookAt;
		}
		#endregion

		#region SET
		/// <summary>
		/// Sets the flags.
		/// </summary>
		/// <param name="flags">Flags.</param>
		/// <param name="flag">If set to <c>true</c> the flags are activated; otherwise they are deactivated.</param>
		public static void SetFlagged ( this Transform obj, ValidationFlag.Flags flagType, bool flag = true )
		{
			if ( !obj )
			{ return; }
			
			obj.gameObject.SetFlagged ( flagType, flag );
		}
		#endregion

		#region GET
		/// <summary>
		/// Returns the closest transform.
		/// </summary>
		public static Transform GetClosest( this Transform t, IList<Transform> transforms )
		{
			Transform closest = null;
			for( int i=0; i<transforms.Count; i++ )
			{
				if( transforms[i] == null )
					continue;
				if( closest == null || t.Distance( transforms[i] ) < t.Distance( closest ) )
				{
					closest = transforms[i];
				}
			}
			return closest;
		}
        /// <summary>
        /// Returns the farthest transform.
        /// </summary>
        public static Transform GetFarthest( this Transform t, IList<Transform> transforms )
        {
            Transform farthest = null;
            for( int i=0; i<transforms.Count; i++ )
            {
                if( transforms[i] == null )
                    continue;
                if( farthest == null || t.Distance( transforms[i] ) > t.Distance( farthest ) )
                {
                    farthest = transforms[i];
                }
            }
            return farthest;
        }
        /// <summary>
        /// Gets the children. If obj is null, null is returned.
        /// </summary>
        public static Transform[] GetChildren( this Transform obj, bool includeSubChildren = false, 
            bool includeInactive = false )
        {
            if( !obj )  return null;
            List<Transform> children = new List<Transform>( obj.ChildCount(includeSubChildren) );
            if( includeSubChildren )
            {
                var firstChildren = obj.GetChildren(false, includeInactive);
                children.AddRange( firstChildren );
                for( int i=0; i<firstChildren.Length; i++ )
                {
                    children.AddRange( firstChildren[i].GetChildren( true, includeInactive ) );
                }
                return children.ToArray();
            }
            else
            {
                for( int i=0; i<obj.ChildCount(); i++ )
                {
                    var child = obj.GetChild(i);
                    if( includeInactive )
                    {
                        children.Add( child );
                    }
                    else if ( child.IsActiveInHierarchy() ) children.Add( child );
                }
            }
            return children.ToArray();
        }   
        /// <summary>
        /// Gets the components in the specified children. Unity's default GetComponentsInChildren() includes the components
        /// of the children's parent, this only returns the components in the children.
        /// </summary>
        public static List<T> GetCompsInChildren<T>( this Transform obj, bool includeSubChildren = false, bool includeInactive = false ) where T : Component
        {
			Transform[] children = obj.GetChildren( includeSubChildren, includeInactive );
            List<T> comps = new List<T>();
            for( int i=0; i<children.Length; i++ )
            {
                var comp = children[i].GetComponent<T>();
                if( comp )
                {
                    comps.Add( comp );
                }
            }
            return comps;
        }
		public static Transform GetSubChild( this Transform t, params int[] subIndex )
		{
			if( subIndex == null )
			{
				Debug.LogWarning("The /subIndex/ params array is null, returning null");
				return null;
			}
			Transform child = t;
			for( int i=0; i<subIndex.Length; i++ )
			{
				child = child.GetChild( subIndex[i] );
			}
			return child;
		}
		#endregion

        #region COMPONENTS COPY
        /// <summary>
        /// Copies the transform from the specified to this object's.
        /// </summary>
        public static void CopyTransformFrom ( this Transform t, Transform t2 )
        {
            if ( !t || !t2 )
            { return; }

            t.gameObject.CopyTransformFrom ( t2 );
        }
        /// <summary>
        /// Copies the transform from the specified to this object's.
        /// </summary>
        public static void CopyTransformFrom ( this Transform t, GameObject obj2 )
        {
            if ( !t || !obj2 )
            { return; }

            t.CopyTransformFrom ( obj2.transform );
        }
        /// <summary>
        /// Copies the transform to the specified from this object's.
        /// </summary>
        public static void CopyTransformTo ( this Transform t, GameObject obj2 )
        {
            if ( !t || !obj2 )
            { return; }

            obj2.CopyTransformFrom ( t );
        }
        /// <summary>
        /// Copies the transform to the specified from this object's.
        /// </summary>
        public static void CopyTransformTo ( this Transform t, Transform t2 )
        {
            if ( !t || !t2 )
            { return; }

            t.CopyTransformTo ( t2.gameObject );
        }
        #endregion

        #region ANGLES
        public static float GetAngleTo ( this Transform obj, GameObject target )
        {
            return Vector3.Angle ( obj.forward, target.transform.position - obj.position );
        }
        public static float GetAngleTo ( this Transform obj, Transform target )
        {
            return Vector3.Angle ( obj.forward, target.position - obj.position );
        }
        public static float GetSignedAngleTo ( this Transform obj, GameObject target, bool inDegrees = true )
        {
            return obj.gameObject.GetSignedAngleTo ( target, inDegrees );
        }
        public static float GetSignedAngleTo ( this Transform obj, Transform target, bool inDegrees = true )
        {
            return obj.gameObject.GetSignedAngleTo ( target.gameObject, inDegrees );
        }
        #endregion

        #region IS
        public static bool IsPrefab ( this Transform This )
        {
            if ( !This )
            { return false; }

            return This.gameObject.IsPrefab();
        }
		/// <summary>
		/// Calling this between frames may cause an overhead.
		/// </summary>
		public static bool IsFlagged ( this Transform obj, ValidationFlag.Flags flagType )
		{
			if ( !obj )
			{ return false; }
			
			return obj.gameObject.IsFlagged ( flagType );
		}
        /// <summary>
        /// Also checks for clones (instances). If obj is null, false is returned.
        /// </summary>
        /// <returns><c>true</c> if the specified obj is active in hierarchy; otherwise, <c>false</c>.</returns>
        /// <param name="obj">Object.</param>
        public static bool IsActiveInHierarchy( this Transform obj )
        {
            bool active = false;
            if( !obj )
                return active;
            active = obj.gameObject.activeInHierarchy;
            //Might be prefab, check if Fading cause that would mean a level might be loading and this will throw an error
            if( !active && !AutoFade._Fading )
            {
                var instance = GameObject.Find( obj.name+"(Clone)" );
                if( instance )//is prefab and is active
                {
                    return true;
                }
            }
            return active;
        }
        /// <summary>
        /// Determines if this is looking at the specified target.
        /// </summary>
        /// <param name="transform">This transform.</param>
        /// <param name="target">The target this transform should be looking at.</param>
        /// <param name="maxAngle">Max angle between this transform's forward axis and the specified target.</param>
        public static bool IsLookingAt( this Transform transform, Transform target, float maxAngle = 1f )
        {
            return transform.IsLookingAt( target.position, maxAngle );
        }
        /// <summary>
        /// Determines if this is looking at the specified target.
        /// </summary>
        /// <param name="transform">This transform.</param>
        /// <param name="target">The target this transform should be looking at.</param>
        /// <param name="maxAngle">Max angle between this transform's forward axis and the specified target.</param>
        public static bool IsLookingAt( this Transform transform, Vector3 target, float maxAngle = 1f )
        {
            if( !transform ) 
            { 
                return false; 
            }
            if( Vector3.Angle( transform.forward, target - transform.position ) <= maxAngle ) 
            { 
                return true; 
            }
            return false;
        }
        #endregion

        #region MISC
		public static Transform FindChildRecursive ( this Transform obj, string childName)
		{
			foreach(Transform child in obj)
			{
				if(child.name == childName )
					return child;
				var result = child.FindChildRecursive(childName);
				if (result != null)
					return result;
			}
			return null;
		}

        public static int ChildCount( this Transform obj, bool includeSubChildren = false )
        {
            if( !obj )
                return 0;

            if( includeSubChildren )
            {
                int count = obj.childCount;
                for( int i=0; i < obj.childCount; i++ )
                {
                    count += obj.GetChild(i).ChildCount( true );
                }
                return count;
            }
            return obj.childCount;
        }
        #endregion
    }
}
