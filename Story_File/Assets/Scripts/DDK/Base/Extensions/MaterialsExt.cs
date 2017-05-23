//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using MovementEffects;


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// Extension class for materials manipulation.
    /// </summary>
	public static class MaterialsExt 
    {		
		#region GET
		/// <summary>
		/// Gets the opacity of the _TintColor property.
		/// </summary>
		/// <returns>
		/// The opacity.
		/// </returns>
		/// <param name='go'>
		/// Game Object.
		/// </param>
		public static float GetOpacity(this GameObject go)
		{
			return go.GetComponent<Renderer>().sharedMaterial.GetColor("_TintColor").a;
		}
		/// <summary>
		/// Gets the opacity of the specified color property.
		/// </summary>
		/// <returns>
		/// The opacity.
		/// </returns>
		/// <param name='go'>
		/// Game Object.
		/// </param>
		/// <param name='colorPropName'>
		/// Color property name.
		/// </param>
		public static float GetOpacity(this GameObject go, string colorPropName)
		{
			return go.GetComponent<Renderer>().sharedMaterial.GetColor(colorPropName).a;
		}
		public static Material[] GetMaterials( this GameObject obj, bool useSharedMaterial = false )
		{
			if( !obj )
				return null;
			var _renderers = obj.GetComponentsInChildren<Renderer>();
			if( _renderers == null )
			{
				Debug.LogWarning( "This object has no Renderer", obj );
				return null;
			}
			if( obj.IsPrefab() )
			{
				Debug.LogWarning( "This object is a prefab..", obj );
				return null;
			}
			var materials = new List<Material>();
			if( useSharedMaterial )
			{
				for( int i=0; i<_renderers.Length; i++ )
				{
					materials.AddRange( _renderers[i].sharedMaterials );
				}
			}
			else
			{
				for( int i=0; i<_renderers.Length; i++ )
				{
					materials.AddRange( _renderers[i].materials );
				}
			}
			return materials.ToArray();
		}
		public static Shader[] GetShaders( this IList<Material> materials )
		{
			if( materials == null )
				return null;
			var shaders = new List<Shader>();
			for( int i=0; i<materials.Count; i++ )
			{
				shaders.Add( materials[i].shader );
			}
			return shaders.ToArray();
		}
		public static Shader[] GetShaders( this GameObject obj, bool useSharedMaterials = false )
		{
			if( !obj )
				return null;
			var materials = obj.GetMaterials( useSharedMaterials );
			if( materials != null )
				return materials.GetShaders();
			return null;
		}
		#endregion

		
		#region SET
		/// <summary>
		/// Sets a material's opacity.
		/// </summary>
		/// <param name='go'>
		/// The game object that contains the material.
		/// </param>
		/// <param name='colorPropName'>
		/// Color property name.
		/// </param>
		/// <param name='newOpacity'>
		/// New opacity.
		/// </param>
		public static void SetOpacity(this GameObject go, string colorPropName, float newOpacity)
		{
			float r = go.GetComponent<Renderer>().sharedMaterial.GetColor(colorPropName).r;
			float g = go.GetComponent<Renderer>().sharedMaterial.GetColor(colorPropName).g;
			float b = go.GetComponent<Renderer>().sharedMaterial.GetColor(colorPropName).b;
			go.GetComponent<Renderer>().sharedMaterial.SetColor(colorPropName,new Color(r,g,b,newOpacity));
		}		
		public static void SetColors( this IList<Material> mats, Color target, string colorPropName = "_TintColor" )
		{
			for( int i=0; i<mats.Count; i++ )
			{
				mats[i].SetColor( colorPropName, target );
			}
		}	
		public static void SetAlpha( this Material mat, float target )
		{
			if( !mat )
				return;
			Color color = mat.color;
			color.a = target;
			mat.color = color;
		}
		public static void SetAlpha( this IList<Material> mats, float target )
		{
			if( mats == null )
				return;
			for( int i=0; i<mats.Count; i++ )
			{
				mats[i].SetAlpha( target );
			}
		}
		public static void SetShaders( this IList<Material> materials, IList<Shader> shaders )
		{
			if( materials == null || shaders == null )
				return;
			if( materials.Count != shaders.Count )
			{
				Debug.LogWarning ("The shaders count differs from this materials count");
			}
			for( int i=0; i<materials.Count; i++ )
			{
				if( materials[i] == null )
					continue;
				materials[i].shader = shaders[i];
			}
		}
		public static void SetShader( this IList<Material> materials, Shader shader )
		{
			if( materials == null || !shader )
				return;
			for( int i=0; i<materials.Count; i++ )
			{
				if( materials[i] == null )
					continue;
				materials[i].shader = shader;
			}
		}
		public static void SetShader( this GameObject obj, Shader shader )
		{
			Material[] materials = obj.GetMaterials();
			if( materials == null || !shader )
				return;
			materials.SetShader( shader );
		}
		#endregion


		#region ANIMATIONS
		/// <summary>
		/// Animates the color of the specified materials.
		/// </summary>
        public static IEnumerator<float> AnimColors( this Material[] materials, Color target, float duration = 0.4f,
		                                    bool avoidAlpha = true, UnityAction callback = null )
		{
			if( materials == null )
				yield break;
			for( int i=0; i<materials.Length; i++ )
			{
                materials[i].AnimColor( target, duration, avoidAlpha, null ).Run();
			}
			if( materials == null || callback == null )
				yield break;
            yield return Timing.WaitForSeconds( duration );
			callback.Invoke();
		}	
		/// <summary>
		/// Animates the color of the specified material.
		/// </summary>
        public static IEnumerator<float> AnimColor( this Material mat, Color target, float duration = 0.4f,
		                                    bool avoidAlpha = true, UnityAction callback = null )
		{
			if( !mat )
				yield break;

            Color iniColor = mat.color;
            Color current = iniColor;
            float t = Time.deltaTime;
            float wait = (duration * 0.1f).Clamp( 0.05f, 0.1f );//This will improve this coroutine's performance
            while ( current != target )
            {
                if( !mat )//IT MIGHT GET DESTROYED WHILE ANIMATING
                {
                    yield break;
                }
                current = iniColor.Lerp( target, t / duration, avoidAlpha ); 
                mat.color = current;
                yield return Timing.WaitForSeconds( wait );
                t += wait;
            }
			if( !mat || callback == null )
				yield break;
			callback.Invoke();
		}	
        /// <summary>
        /// Animates the color (back and forth) of the specified material
        /// </summary>
        /// <param name="uiElement">User interface element.</param>
        /// <param name="target">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="avoidAlpha">If true, the alpha channel is not taken in account.</param>
        public static IEnumerator<float> AnimColorPingPong( this Material mat, Color target, float duration = 0.4f,
            bool avoidAlpha = true, int repeat = 0, UnityAction callback = null )
        {
            if( !mat )
                yield break;
            
            Color iniColor = mat.color;
            for( int i=0; i<repeat + 1; i++ )
            {
                mat.AnimColor( target, duration * 0.5f, avoidAlpha ).Run();
                mat.AnimColor( iniColor, duration * 0.5f, avoidAlpha ).Run();   
            }

            if( !mat || callback == null )
                yield break;
            yield return Timing.WaitForSeconds( duration );
            callback.Invoke();
        }
		#endregion
		
	}
}