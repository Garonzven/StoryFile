//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DDK.Base.Statics;
using DDK.Base.Classes;
using System;
using DDK.Base.Animations;
using UnityEngine.EventSystems;
using DDK.Base.Components;
using DDK.Base.Fx.Transitions;
using MovementEffects;


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// GameObject class extension.
    /// </summary>
	public static class GameObjExt 
    {
		#region GET
		public static GameObject GetChild( this GameObject obj, string name )
		{
			return obj.transform.Find( name ).gameObject;
		}		
		public static GameObject GetChild( this GameObject obj, int index )
		{
			int count = obj.ChildCount();
			if( count > 0 && index < count )
				return obj.transform.GetChild( index ).gameObject;
			else return null;
		}
		public static GameObject GetNextSibling( this GameObject obj, bool mustBeActive = false )
		{
			int siblingIndex = obj.transform.GetSiblingIndex();
			GameObject parent = obj.GetParent();
			GameObject child = parent.GetChild( siblingIndex + 1 );
			if( !child.IsActiveInHierarchy() && mustBeActive )
			{
				return child.GetNextSibling( mustBeActive );
			}
			else return child;
		}
		public static List<GameObject> GetSiblings( this GameObject obj, bool includeSelf = true, bool includeInactive = false )
		{
			GameObject parent = obj.GetParent();
			List<GameObject> siblings = new List<GameObject>();
			siblings = parent.GetChildren( false, includeInactive ).ToList();
			if( !includeSelf )
			{
				siblings.Remove( obj );
			}
			return siblings;
		}
		/// <summary>
		/// Gets the children. If obj is null, null is returned.
		/// </summary>
		public static GameObject[] GetChildren( this GameObject obj, bool includeSubChildren = false, 
		                                       bool includeInactive = false )
		{
			if( !obj )	return null;
			List<GameObject> children = new List<GameObject>( obj.ChildCount(includeSubChildren) );
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
		/// Gets the children excluding the ones containing the specified strings in their name.
		/// </summary>
		/// <returns>The children.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="excludeWhichContain">Name exclutions.</param>
		public static List<GameObject> GetChildren( this GameObject obj, string[] excludeWhichContain )
		{
			//GameObject[] children = new GameObject[obj.ChildCount()];
			List<GameObject> children = new List<GameObject>();
			for( int i=0; i<obj.ChildCount(); i++ )
			{
				if( !excludeWhichContain.Contains<string>( obj.GetChild( i ).name ) )
					children.Add( obj.GetChild(i) );
			}
			return children;
		}		
		/// <summary>
		/// Gets the children containing the specified substrings in their name.
		/// </summary>
		/// <returns>The children.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="excludeWhichContain">Name exclutions.</param>
		public static List<GameObject> GetChildrenContaining( this GameObject obj, string[] containingSubstring )
		{
			//GameObject[] children = new GameObject[obj.ChildCount()];
			List<GameObject> children = new List<GameObject>();
			for( int i=0; i<obj.ChildCount(); i++ )
			{
				if( containingSubstring.Contains<string>( obj.GetChild( i ).name ) )
					children.Add( obj.GetChild(i) );
			}
			return children;
		}		
		/// <summary>
		/// Gets the children excluding the specified indexes.
		/// </summary>
		/// <returns>The children.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="excludeIndex">Exclution list of indexes.</param>
		public static GameObject[] GetChildren( this GameObject obj, int[] excludeIndex )
		{
			GameObject[] children = new GameObject[obj.ChildCount() - excludeIndex.Length];
			for( int i=0, j=0; i<obj.ChildCount(); i++, j++ )
			{
				if( !excludeIndex.Contains( i ) )
					children[j] = obj.GetChild(j);
				else j--;
			}
			return children;
		}
		/// <summary>
		/// Gets the children excluding the indexes between the specified range.
		/// </summary>
		/// <returns>The children.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="excludeFrom">Exclude from index (inclusive).</param>
		/// <param name="excludeTo">Exclude to index (exclusive).</param>
		public static GameObject[] GetChildren( this GameObject obj, int excludeFrom, int excludeTo )
		{
			if( excludeFrom > excludeTo && excludeTo > 0 )
			{
				Debug.LogWarning( "/excludeTo/ must be higher than /excludeFrom/. Returning null..", obj );
				return null;
			}
			if( excludeTo < 0 )
			{
				excludeTo = 0;
			}
			GameObject[] children = new GameObject[ obj.ChildCountActiveOnly() - (excludeTo - excludeFrom).Abs() ];
			if( excludeTo == 0 )
			{
				excludeTo = obj.ChildCount();
			}
			GameObject child = null;
			for( int i=0, j=0; i<obj.ChildCount(); i++, j++ )
			{
				if( ( i < excludeFrom && i >= excludeTo ) || i >= excludeTo )
				{
					child = obj.GetChild( i );
					if( !child.activeSelf )
					{
						j--;
						continue;
					}
					children[ j ] = obj.GetChild( i );
				}
				else j--;
			}
			return children;
		}
		/// <summary>
		/// Gets the children excluding the indexes between the specified range.
		/// </summary>
		/// <returns>The children.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="excludeFrom">Exclude from index (inclusive).</param>
		/// <param name="excludeTo">Exclude to index (exclusive).</param>
		public static List<T> GetChildren<T>( this GameObject obj, int excludeFrom, int excludeTo ) where T : Component
		{
			if( excludeFrom > excludeTo && excludeTo > 0 )
			{
				Debug.LogWarning( "/excludeTo/ must be higher than /excludeFrom/. Returning null..", obj );
				return null;
			}
			if( excludeTo < 0 )
			{
				excludeTo = 0;
			}
			List<T> children = new List<T>( obj.ChildCount() - (excludeTo - excludeFrom).Abs() );
			if( excludeTo == 0 )
			{
				excludeTo = obj.ChildCount();
			}
			GameObject child = null;
			T comp = default(T);
			for( int i=0; i<obj.ChildCount(); i++ )
			{
				if( ( i < excludeFrom && i >= excludeTo ) || i >= excludeTo )
				{
					child = obj.GetChild( i );
					if( !child.activeSelf )
						continue;
					comp = child.GetComponentInChildren<T>();
					if( !comp )
						continue;
					children.Add( comp );
				}
			}
			return children;
		}
		public static GameObject GetFirstParent( this GameObject obj )
		{
			if( obj.transform.parent != null )
				return GetFirstParent( obj.transform.parent.gameObject );
			else return obj;
		}
		public static GameObject GetFirstAncestorOfType<T>( this GameObject obj ) where T : Component
		{
			var firstFound = obj.GetCompInParent<T>();
			if( firstFound )
			{
				return firstFound.gameObject.GetFirstAncestorOfType<T>();
			}
			return obj;
		}
        /// <summary>
        /// Gets the specified ancestor. The index represents the ancestor, cero (0) is this gameObject, one (1) is the parent, 
        /// two (2) is the grand parent, and so on... If the value is two high the last valid ancestor is returned (the parent of all).
        /// </summary>
        public static GameObject GetAncestor( this GameObject obj, byte ancestorIndex )
        {
            GameObject ancestor = obj;
            for( int i=0; i<ancestorIndex; i++ )
            {
                GameObject parent = ancestor.GetParent();
                if( !parent )
                    break;
                ancestor = parent;
            }
            return ancestor;
        }
		public static T GetFirstComponentOfType<T>( this GameObject obj ) where T : Component
		{
			return obj.GetFirstAncestorOfType<T>().GetComponent<T>();
		}
		/// <summary>
		/// Gets the main texture from every material.
		/// </summary>
		/// <returns>The textures.</returns>
		/// <param name="obj">Object.</param>
		public static Texture[] GetTextures( this GameObject obj )
		{
			var mats = obj.GetComponent<Renderer>().GetTrueMaterials();
			Texture[] texs = new Texture[ mats.Length ];
			for( int i=0; i<mats.Length; i++ )
			{
				texs[ i ] = mats[ i ].mainTexture;
			}
			return texs;
		}
		public static Vector3 GetScreenPos( this GameObject obj, Camera cam = null )
		{
			if( cam == null )
				cam = Camera.main;
			return cam.WorldToScreenPoint( obj.Position() );
		}
		public static Vector2 GetScreenPos2( this GameObject obj, Camera cam = null )
		{
			return (Vector2) obj.GetScreenPos( cam );
		}
		/// <summary>
		/// Gets the prefab instance. NOTE: Make sure the object has a unique name. If no instance is found the prefab
		/// is returned
		/// </summary>
		/// <returns>The prefab instance, or the same prefab if no instance is found.</returns>
		/// <param name="prefab">Prefab.</param>
		public static GameObject GetPrefabInstance( this GameObject prefab )
		{
			if( prefab.IsPrefab() )
			{
				var instance = prefab.name.Find(true);
				if( instance )
					return instance;
				else
				{
					var instances = GameObject.FindObjectsOfType<IsPrefab>();
					int id = prefab.GetComponent<IsPrefab>().id;
					for( int i=0; i<instances.Length; i++ )
					{
						if( id == instances[i].id )
						{
							return instances[i].gameObject;
						}
					}
				}
			}
			else Debug.Log ( "Referenced object is not a prefab, returning the same object..." );
			return prefab;
		}
		public static T GetBehaviour<T>( this GameObject obj ) where T : Behaviour
		{
			if( obj )
				return obj.GetComponent<T>();
			else return default( T );
		}
		public static Behaviour GetBehaviour( this GameObject obj, Behaviour type ) 
		{
			if( obj )
				return obj.GetComponent( type.GetType() ) as Behaviour;
			else return null;
		}		
		public static Sprite GetSprite( this GameObject obj )
		{
			if( obj != null )
			{
				var ren = obj.GetComponent<SpriteRenderer>();
				if( ren )
				{
					return ren.sprite;
				}
			}
			return default(Sprite);
		}
		public static GameObject GetFirstActiveObj( this GameObject[] objs )
		{
			GameObject obj = null;
			for( int i=0; i<objs.Length; i++ )
			{
				if( objs[i].activeInHierarchy )
				{
					obj = objs[i];
					break;
				}
			}
			return obj;
		}		
		/// <summary>
		/// Gets the first sprite renderer (component) found in the objects array.
		/// </summary>
		/// <returns>The first sprite renderer.</returns>
		/// <param name="objs">Objects.</param>
		public static SpriteRenderer GetFirstSpriteRenderer( this GameObject[] objs )
		{
			SpriteRenderer ren = null;
			for( int i=0; i<objs.Length; i++ )
			{
				ren = objs[i].GetComponent<SpriteRenderer>();
				if( ren )
					break;
			}
			return ren;
		}		
		/// <summary>
		/// Gets the first sprite renderer (component) found in the object's children.
		/// </summary>
		/// <returns>The first sprite renderer.</returns>
		/// <param name="objs">Objects.</param>
		public static SpriteRenderer GetFirstSpriteRendererInChildren( this GameObject obj )
		{
			SpriteRenderer ren = null;
			for( int i=0; i<obj.ChildCount(); i++ )
			{
				ren = obj.GetChild(i).GetComponent<SpriteRenderer>();
				if( ren )
					break;
			}
			return ren;
		}		
		/// <summary>
		/// Gets the first child containing a sprite renderer (component).
		/// </summary>
		/// <returns>The first sprite renderer.</returns>
		/// <param name="objs">Objects.</param>
		public static GameObject GetFirstChildWithSpriteRenderer( this GameObject obj )
		{
			SpriteRenderer ren = null;
			for( int i=0; i<obj.ChildCount(); i++ )
			{
				ren = obj.GetChild(i).GetComponent<SpriteRenderer>();
				if( ren )
					return obj.GetChild(i);
			}
			return null;
		}
		public static GameObject GetParent( this GameObject obj )
		{
			if( obj )
			{
				if( obj.transform.parent )
				{
					return obj.transform.parent.gameObject;
				}
			}
			return null;
		}
		public static string[] GetNames( this IList<UnityEngine.Object> objs )
		{
			string[] names = new string[objs.Count];
			for( int i=0; i<objs.Count; i++ )
			{
				names[i] = objs[i].name;
			}
			return names;
		}
		/// <summary>
		/// Returns the object that matches the specified name, or null if non matches.
		/// </summary>
		public static GameObject GetNamed( this IList<GameObject> objs, string name )
		{
			for( int i=0; i<objs.Count; i++ )
			{
				if( objs[i].name.Equals ( name ) )
					return objs[i];
			}
			return null;
		}
		/// <summary>
		/// Gets the component in the specified children. Unity's default GetComponentInChildren() includes the component
		/// of the children's parent, this only returns the component in the children.
		/// </summary>
		public static T GetCompInChildren<T>( this GameObject obj, bool includeSubChildren = false, bool includeInactive = false ) where T : Component
		{
			var children = obj.GetChildren( includeSubChildren, includeInactive );
			for( int i=0; i<children.Length; i++ )
			{
				T comp = children[i].GetComponent<T>();
				if( comp )
				{
					return comp;
				}
			}
			return default( T );
		}
		/// <summary>
		/// Gets the behaviour in the specified children.
		/// </summary>
		public static T GetBehaviourInChildren<T>( this GameObject obj, bool includeSubChildren = false, bool includeInactive = false ) where T : Behaviour
		{
			var children = obj.GetChildren( includeSubChildren, includeInactive );
			for( int i=0; i<children.Length; i++ )
			{
				T comp = children[i].GetBehaviour<T>();
				if( comp )
				{
					return comp;
				}
			}
			return default( T );
		}
		/// <summary>
		/// Gets the components in the specified children. Unity's default GetComponentsInChildren() includes the components
		/// of the children's parent, this only returns the components in the children.
		/// </summary>
		public static List<T> GetCompsInChildren<T>( this GameObject obj, bool includeSubChildren = false, bool includeInactive = false ) where T : Component
		{
			var children = obj.GetChildren( includeSubChildren, includeInactive );
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
		/// <summary>
		/// Gets the components in children.
		/// </summary>
		/// <param name="includeInactive">If set to <c>true</c> include inactive children.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <typeparam name="TExclude">If the child has a component of this type, it will be excluded.</typeparam>
		public static T[] GetComponentsInChildren<T, TExclude>( this GameObject obj, bool includeInactive ) where T : Component where TExclude : Component
		{
			List<T> comps = obj.GetComponentsInChildren<T>( includeInactive ).ToList();
			for( int i=0; i<comps.Count; i++ )
			{
				if( comps[i].GetComponentInChildren<TExclude>() )
				{
					comps.Remove( comps[i] );
					i--;
				}
			}
			return comps.ToArray();
		}
		/// <summary>
		/// Searches for the specified type's component in all parents and returns the first found. Unity's default GetComponentInParent() 
		/// get only the component in the direct parent. It also works on prefab instances when obj is a prefab reference.
		/// </summary>
		/// <returns>The components in some parent.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetCompInParent<T>( this GameObject obj ) where T : Component
		{
			GameObject parent = obj.GetParent();
			if( parent )
			{
				T comp = parent.GetTrueComponent<T>();
				if( comp )
				{
					return comp;
				}
				return parent.GetCompInParent<T>();
			}
			else return null;
		}
		/// <summary>
		/// Searches for the specified type's component in this gameObject, all parents, and all children/subchildren (in that order) 
		/// and returns the first found. Unity's default GetComponentInParent() get only the component in the direct parent. 
		/// </summary>
		public static T GetCompInParentOrChildren<T>( this GameObject obj ) where T : Component
		{
			T comp = obj.GetComponent<T>();
			if( !comp )
			{
				comp = obj.GetCompInParent<T>();
				if( !comp )
				{
					comp = obj.GetCompInChildren<T>( true );
				}
			}
			return comp;
		}
		/// <summary>
		/// Searches for the specified type's component in this gameObject, all parents, and all children/subchildren (in that order) 
		/// and returns the first found. Unity's default GetComponentInParent() get only the component in the direct parent. 
		/// </summary>
		public static T GetBehaviourInParentOrChildren<T>( this GameObject obj ) where T : Behaviour
		{
			T comp = obj.GetBehaviour<T>();
			if( !comp )
			{
				comp = obj.GetBehaviourInParent<T>();
				if( !comp )
				{
					comp = obj.GetBehaviourInChildren<T>( true );
				}
			}
			return comp;
		}
		/// <summary>
		/// Searches for the specified type's behaviour in all parents and returns the first found. It also works on prefab
		/// instances when obj is a prefab reference.
		/// </summary>
		/// <returns>The behaviour in some parent.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetBehaviourInParent<T>( this GameObject obj ) where T : Behaviour
		{
			GameObject parent = obj.GetParent();
			if( parent )
			{
				T comp = parent.GetTrueComponent<T>();
				if( comp )
				{
					return comp;
				}
				return parent.GetCompInParent<T>();
			}
			else return null;
		}
		/// <summary>
		/// Gets the component of the specified type, if no comp is found in the specified object, its children and subchildren will be 
		/// searched until one returns it. NOTE: This includes the inactive children..
		/// </summary>
		/// <returns>The first found component.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetComponentIncludeChildren<T>( this GameObject obj )
		{
			T comp = obj.GetComponent<T>();
			if( comp == null )
			{
				var children = obj.GetChildren( true, true );
				for( int i=0; i<children.Length; i++ )
				{
					if( children[i].GetComponent<T>() != null )
					{
						return children[i].GetComponent<T>();
					}
				}
			}
			return default( T );
		}
		/// <summary>
		/// Gets the component, but it also checks if it references a prefab, if so, it returns the component from the 
		/// prefab instance.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="comp">Component.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetTrueComponent<T>( this GameObject obj ) where T : Component
		{
			if( !obj )
				return default(T);
			var comp = obj.GetComponent<T>();
			if( !comp )
				return default(T);
			return comp.GetTrueComponent<T>();
		}
		/// <summary>
		/// Gets the Behaviour, but it also checks if it references a prefab, if so, it returns the Behaviour from the 
		/// prefab instance.
		/// </summary>
		/// <returns>The Behaviour.</returns>
		/// <param name="comp">Behaviour.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetTrueBehaviour<T>( this GameObject obj ) where T : Behaviour
		{
			if( !obj )
				return default(T);
			var comp = obj.GetComponent<T>();
			if( !comp )
				return default(T);
			return comp.GetTrueBehaviour<T>();
		}
		/// <summary>
		/// Returns an array with the specified valid (not null) components (type) in this gameobjects.
		/// </summary>
		public static T[] GetComponents<T>( this IList<GameObject> objs )
		{
			T[] comps = new T[ objs.Count ];
			for( int i=0; i<comps.Length; i++ )
			{
				T comp = objs[i].GetComponent<T>();
				if( comp != null )
					comps[i] = comp;
			}
			return comps;
		}
		/// <summary>
		/// Gets the name of the object. It also checks if the referenced object is a prefab, if so, it returns the prefab's
		/// instance name in case there is one.
		/// </summary>
		/// <returns>The name.</returns>
		/// <param name="obj">Object.</param>
		public static string GetName( this GameObject obj )
		{
			string name = obj.name;
			if( obj.IsPrefab() )
			{
				var instance = obj.GetPrefabInstance();
				if( instance )
				{
					name = instance.name;
				}
			}
			return name;
		}
		/// <summary>
		/// Gets a random rotation.
		/// </summary>
		/// <returns>The random rotation.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="axis">Only the axis values higher than 0 will be calculated. This is just to tell this function 
		/// which axis are going to be affected.</param>
		/// <param name="minVal">Minimum value.</param>
		/// <param name="maxVal">Max value.</param>
		public static Vector3 GetRandomRotation( this GameObject obj, Vector3 axis, float minVal = 0f, float maxVal = 360f )
		{
			Vector3 ranRot = obj.transform.rotation.eulerAngles;
			for( int i=0; i<3; i++ )
			{
				var ranValue = UnityEngine.Random.Range( minVal, maxVal );
				if( axis[i] > 0f )
				{
					ranRot[i] = ranValue;
				}
			}
			return ranRot;
		}
        /// <summary>
        /// Gets a random scale.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="axis">Only the axis values higher than 0 will be calculated. This is just to tell this function 
        /// which axis are going to be affected.</param>
        /// <param name="minVal">Minimum value.</param>
        /// <param name="maxVal">Max value.</param>
        public static Vector3 GetRandomScale( this GameObject obj, Vector3 axis, float minVal = 0f, float maxVal = 2f )
        {
            Vector3 ranScale = obj.transform.localScale;
            var ranValue = UnityEngine.Random.Range( minVal, maxVal );
            for( int i=0; i<3; i++ )
            {
                if( axis[i] > 0f )
                {
                    ranScale[i] = ranValue;
                }
            }
            return ranScale;
        }
        /// <seealso cref="Fill()"/>
        public static GameObject[] GetGameObjects( this SearchableGameObject[] objs )
        {
            if( objs == null || objs.Length == 0 )
            {
                Debug.LogWarning("No objs... Returning empty array");
                return new GameObject[0];
            }
            GameObject[] _objs = new GameObject[ objs.Length ];
            for( int i=0; i<objs.Length; i++ )            
            {
                _objs[ i ] = objs[ i ].m_gameObject;
            }
            return _objs;
        }
		#endregion

		#region SET
		/// <summary>
		/// Sets the texture. If -matIndex- is left as -1 the final texture will be applied to each material.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="tex">Texture.</param>
		/// <param name="matIndex">The material index.</param>
		public static void SetTexture( this GameObject obj, Texture2D tex, int matIndex = -1 )
		{
			if( matIndex < 0 )
			{
				var mats = obj.GetComponent<Renderer>().materials;
				for( int i=0; i < mats.Length; i++ )
				{
					mats[i].mainTexture = tex;
				}
			}
			else obj.GetComponent<Renderer>().materials[matIndex].mainTexture = tex;
		}		
		/// <summary>
		/// Sets the texture. The -multiply- texture is multiplied with the original, they must have the same size. If -multiply- 
		/// is null, -tex- will be set to the object's main texture. If -matIndex- is left as -1 the final texture will be applied 
		/// to each material.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="tex">Texture.</param>
		/// <param name="multiply">The texture that will multiply with the original one.</param>
		/// <param name="matIndex">The material index.</param>
		public static void SetMultiplyTexture( this GameObject obj, Texture2D tex, Texture2D multiply = null, int matIndex = -1 )
		{
			if( tex != null && multiply != null )//MULTIPLY
			{
				Color[] original = tex.GetPixels();
				Color[] extra = multiply.GetPixels();
				for( int i=0; i<original.Length; i++ )
				{
					original[i] *= extra[i];
				}
				tex.SetPixels( original );
				tex.Apply();
			}
			obj.SetTexture( tex, matIndex );
		}		
		/// <summary>
		/// Sets the texture. The -multiply- texture is multiplied with the original, they must have the same size. If -multiply- 
		/// is null, -tex- will be set to the object's main texture. If -matIndex- is left as -1 the final texture will be applied 
		/// to each material.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="tex">Texture.</param>
		/// <param name="multiply">The texture that will multiply with the original one.</param>
		/// <param name="matIndex">The material index.</param>
		public static IEnumerator SetMultiplyTextureEndOfFrame( this GameObject obj, Texture2D tex, Texture2D multiply = null,
		                                                       int matIndex = -1 )
		{
			yield return null;
			obj.SetMultiplyTexture( tex, multiply, matIndex );
		}		
		/// <summary>
		/// Sets the texture. The -add- texture is added to the original, they must have the same size. If -add- 
		/// is null, -tex- will be set to the object's main texture. If -matIndex- is left as -1 the final texture will be applied 
		/// to each material.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="tex">Texture.</param>
		/// <param name="multiply">The texture that will multiply with the original one.</param>
		/// <param name="matIndex">The material index.</param>
		public static void SetAddTexture( this GameObject obj, Texture2D tex, Texture2D add = null, int matIndex = -1 )
		{
			if( tex != null && add != null )//ADD
			{
				Color[] original = tex.GetPixels();
				Color[] extra = add.GetPixels();
				for( int i=0; i<original.Length; i++ )
				{
					original[i] += extra[i];
				}
				tex.SetPixels( original );
				tex.Apply();
			}
			obj.SetTexture( tex, matIndex );
		}		
		/// <summary>
		/// Sets the textures. The -multiplers- textures are multiplied with the originals, they must have the same size. If -multiplers- 
		/// is null, -texs- will be set as the objects's main texture. If -matIndex- is left as -1 the final texture will be applied 
		/// to each material.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="tex">Texture.</param>
		/// <param name="multiply">The texture that will multiply with the original one.</param>
		/// <param name="matIndex">The material index.</param>
		public static void SetMultiplyTextures( this GameObject[] objs, Texture2D[] texs, Texture2D[] multipliers = null,
		                                       int matIndex = -1 )
		{
			for( int i = 0; i<objs.Length; i++ )
			{
				if( multipliers != null ){
					if( multipliers.Length == 0 ){
						objs[i].SetMultiplyTexture( texs[i], null, matIndex );
						continue;
					}
				}
				else {
					objs[i].SetMultiplyTexture( texs[i], null, matIndex );
					continue;
				}
				objs[i].SetMultiplyTexture( texs[i], multipliers[i], matIndex );
			}
		}		
		/// <summary>
		/// Sets the textures. The -add- textures are added with the originals, they must have the same size. If -add- 
		/// is null, -texs- will be set as the objects's main texture. If -matIndex- is left as -1 the final texture will be applied 
		/// to each material.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="tex">Texture.</param>
		/// <param name="multiply">The texture that will multiply with the original one.</param>
		/// <param name="matIndex">The material index.</param>
		public static void SetAddTextures( this GameObject[] objs, Texture2D[] texs, Texture2D[] add = null, int matIndex = -1 )
		{
			for( int i = 0; i<objs.Length; i++ )
			{
				if( add != null ){
					if( add.Length == 0 ){
						objs[i].SetAddTexture( texs[i], null, matIndex );
						continue;
					}
				}
				else {
					objs[i].SetAddTexture( texs[i], null, matIndex );
					continue;
				}
				objs[i].SetAddTexture( texs[i], add[i], matIndex );
			}
		}		
		public static void SetTexture( this GameObject obj, byte[] tex, int matIndex = -1 )
		{
			int size = (int)Mathf.Sqrt( tex.Length );
			Texture2D t = new Texture2D(size, size);
			t.LoadImage( tex );
			if( matIndex < 0 )
			{
				var mats = obj.GetComponent<Renderer>().GetTrueMaterials();
				for( int i=0; i < mats.Length; i++ )
				{
					mats[i].mainTexture = t;
				}
			}
			else obj.GetComponent<Renderer>().GetTrueMaterials()[matIndex].mainTexture = t;
		}		
		public static void SetTextures( this GameObject[] objs, byte[][] texs, int matIndex = -1 )
		{
			for( int i=0; i<objs.Length; i++ )
			{
				objs[i].SetTexture( texs[i], matIndex );
			}
		}		
		/// <summary>
		/// Sets the main texture of every material as the specified textures.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="tex">Texture.</param>
		public static void SetTextures( this GameObject obj, Texture[] texs )
		{
			/*if( texs != null )
		{
			var mats = obj.renderer.materials;
			for( int i=0; i<mats.Length; i++ )
			{
				if( texs[i] != null )
					mats[i].mainTexture = texs[i];
			}
		}*/

			var mats = obj.GetComponent<Renderer>().GetTrueMaterials();
			for( int i=0; i<mats.Length; i++ )
			{
				if( texs[i] != null )
					mats[i].mainTexture = texs[i];
			}
		}		
		/// <summary>
		/// Sets the main texture of every material as the specified texture.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="tex">Texture.</param>
		public static void SetTextures( this GameObject obj, Texture tex )
		{
			/*if( tex != null )
		{
			var mats = obj.renderer.materials;
			for( int i=0; i<mats.Length; i++ )
			{
				mats[i].mainTexture = tex;
			}
		}*/
			
			var mats = obj.GetComponent<Renderer>().GetTrueMaterials();
			for( int i=0; i<mats.Length; i++ )
			{
				mats[i].mainTexture = tex;
			}
		}
		/// <summary>
		/// Sets the object's position. If the object has a RectTransform the anchored position will be set instead, and
		/// /local/ won't be taken into account.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="pos">Position.</param>
		/// <param name="local">If set to <c>true</c> local.</param>
		/// <param name="checkRectTransform"> If true and the object has a RectTransform, its anchored positione will be returned instead. </param> 
		public static void SetPos( this GameObject obj, Vector3 pos, bool local = false, bool checkRectTransform = false )
		{
			if( obj == null )
				return;
			if( checkRectTransform )
			{
				var rt = obj.GetComponent<RectTransform>();
				if( rt )
				{
					rt.anchoredPosition3D = pos;
					return;
				}
			}
			if( local ) {
				obj.transform.localPosition = pos;
			}
			else obj.transform.position = pos;
		}
		/// <summary>
		/// Sets the specified axis value. If the object has a RectTransform the anchored position will be used instead.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="axis">Axis: 0=x, 1=y, 2=z.</param>
		/// <param name="value">Value.</param>
		public static void SetPos( this GameObject obj, byte axis, float value )
		{
			Vector3 originalPos = obj.transform.position;
			var rt = obj.GetComponent<RectTransform>();
			if( rt )
			{
				originalPos = rt.anchoredPosition3D;
			}
			var pos = new Vector3(
				( axis == 0 ) ? value : originalPos.x,
				( axis == 1 ) ? value : originalPos.y,
				( axis == 2 ) ? value : originalPos.z );
			if( rt )
			{
				rt.anchoredPosition3D = pos;
			}
			else obj.transform.position = pos;
		}
		/// <summary>
		/// Sets the object's rotation.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="pos">Rotation.</param>
		/// <param name="local">If set to <c>true</c> local.</param>
		public static void SetRot( this GameObject obj, Vector3 rot, bool local = false )
		{
			obj.SetRot( Quaternion.Euler( rot ), local );
		}
		/// <summary>
		/// Sets the object's rotation.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="pos">Rotation.</param>
		/// <param name="local">If set to <c>true</c> local.</param>
		public static void SetRot( this GameObject obj, Quaternion rot, bool local = false )
		{
			if( obj == null )
				return;
			if( local ) {
				obj.transform.localRotation = rot;
			}
			else obj.transform.rotation = rot;
		}
		/// <summary>
		/// Sets the alpha value of the shared material.
		/// </summary>
		public static void SetAlpha( this GameObject obj, float alpha = 0f, string colorPropName = "_TintColor" )
		{
			var mats = obj.GetComponent<Renderer>().GetTrueMaterials();
			for( int i=0; i < mats.Length; i++ )
			{
				if( mats[i] != null )
				{
					if( !mats[i].HasProperty( colorPropName ) )
						break;
					Color newColor = new Color( mats[i].GetColor(colorPropName).r, mats[i].GetColor(colorPropName).g,
					                           mats[i].GetColor(colorPropName).b, alpha );
					mats[i].SetColor( colorPropName, newColor );
				}
			}
		}
        /// <summary>
        /// Sets the alpha value of the gameObject's canvas group. NOTE: Use the canvas group directly to improve performance.
        /// </summary>
        public static void SetAlphaCanvasGroup( this GameObject obj, float alpha = 0f )
        {
            if( !obj )
                return;
            obj.GetComponent<CanvasGroup>().alpha = alpha;
        }
		/// <summary>
		/// Sets the sprite renderer's sprite texture.
		/// </summary>
		/// <param name="spriteObj">The sprite gameobject (the object containing the SpriteRenderer component).</param>
		/// <param name="tex">Texture.</param>
		public static void SetSpriteTexture( this GameObject spriteObj, Texture2D tex )
		{
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			block.SetTexture( "_MainTex", tex );
			spriteObj.GetComponent<SpriteRenderer>().SetPropertyBlock( block );
		}
		/// <summary>
		/// Sets the image's sprite texture by creating another sprite.
		/// </summary>
		/// <param name="spriteObj">The sprite gameobject (the object containing the Image component).</param>
		/// <param name="tex">Texture.</param>
		public static void SetUISpriteTexture( this GameObject spriteObj, Texture2D tex )
		{
			var image = spriteObj.GetComponent<Image>();
			var originalSprite = image.sprite;
			var newRect = new Rect( 0f, 0f, tex.width, tex.height );
			var newSprite = Sprite.Create( tex, newRect, (Vector2) originalSprite.bounds.center.normalized, originalSprite.pixelsPerUnit );
			image.sprite = newSprite;
			
		}
		/// <summary>
		/// Sets the sprite renderer's sprite.
		/// </summary>
		/// <param name="spriteObj">Sprite object.</param>
		/// <param name="sprite">Sprite.</param>
		/// <param name="nameObjAsSprite">If set to <c>true</c> the sprite object will be named as the specified sprite.</param>
		public static void SetSprite( this GameObject spriteObj, Sprite sprite, bool nameObjAsSprite = false )
		{
			spriteObj.GetComponent<SpriteRenderer>().sprite = sprite;
			if( nameObjAsSprite )
			{
				spriteObj.name = sprite.name;
			}
		}
        public static IEnumerator<float> SetActiveAfter( this GameObject obj, float time, bool active = true )
		{
			yield return Timing.WaitForSeconds( time );
			if( obj )
				obj.SetActive(active);
		}
		public static void SetActiveInHierarchyAfter( this GameObject obj, float time, bool active = true )
		{
			obj.SetActiveInHierarchyAfterCo( time, active ).Start();
		}
		public static IEnumerator SetActiveInHierarchyAfterCo( this GameObject obj, float time, bool active = true )
		{
			yield return new WaitForSeconds(time);
			obj.SetActiveInHierarchy(active);
		}
		public static void SetSpriteAlpha( this GameObject spriteObj, float alpha = 1f )
		{
			var sr = spriteObj.GetComponent<SpriteRenderer>();
			sr.color = new Color( sr.color.r, sr.color.g, sr.color.b, alpha );
		}		
		public static void SetSpritesAlpha( this GameObject[] objs, float opacity = 1f )
		{
			for( int i=0; i<objs.Length; i++ )
			{
				objs[i].SetSpriteAlpha( opacity );
			}
		}
		/// <summary>
		/// Sets the sibling sprite renderer color's alpha value, including this renderer.
		/// </summary>
		/// <param name="Graphic">Graphic.</param>
		/// <param name="alpha">The alpha value.</param>
		public static void SetSiblingSpritesAlpha( this GameObject spriteRenderer, float alpha, bool includeSiblingsSubChildren = false, 
		                                          bool includeInactive = false, bool includeThis = false )
		{
			var sr = spriteRenderer.GetComponent<SpriteRenderer>();
			if( sr )
			{
				sr.SetSiblingSpritesAlpha( alpha, includeSiblingsSubChildren, includeInactive, includeThis );
			}
			else Debug.LogWarning("The specified gameobject has no Sprite Renderer component");
		}
		public static void SetGraphicAlpha( this GameObject graphicObj, float alpha = 1f )
		{
			var sr = graphicObj.GetComponent<Graphic>();
			sr.color = new Color( sr.color.r, sr.color.g, sr.color.b, alpha );
		}
		public static void SetGraphicsAlpha( this GameObject[] objs, float opacity = 1f )
		{
			for( int i=0; i<objs.Length; i++ )
			{
				objs[i].SetGraphicAlpha( opacity );
			}
		}
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy. This also checks if the object is null.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static GameObject SetActiveInHierarchy( this GameObject obj, bool active = true, bool destroyIfClone = true, float after = 0f, bool immediate = false )
		{
			if( !obj )
				return null;
			bool isClone = false;
			if( active )
				obj.SetActive( active );
            if( active && !obj.activeInHierarchy )
			{
				GameObject instance = GameObject.Instantiate( obj, obj.transform.position, obj.transform.rotation ) as GameObject;
				instance.SetActive( true );
				return instance;
			}
            else if( !active && destroyIfClone )
			{
				if( obj.name.Contains( "(Clone)" ) )
				{
					if( immediate ) obj.DestroyImmediate();
					else obj.Destroy(after);
					isClone = true;
				}
				else//it might be a prefab
				{
					string name = obj.name;
					var clone = GameObject.Find( name+"(Clone)" );
					if( clone )
					{
						if( immediate ) clone.DestroyImmediate();
						else clone.Destroy(after);
						isClone = true;
					}
				}
			}
			if( !isClone ) obj.SetActive( active );
			return obj;
		}		
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy. This also checks if the object is null.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static T SetActiveInHierarchy<T>( this GameObject obj, bool active = true, bool destroyIfClone = true, float after = 0f, bool immediate = false ) where T : Component
		{
			return obj.SetActiveInHierarchy( active, destroyIfClone, after, immediate ).GetComponent<T>();
		}	
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy. This also checks if the object is null.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static GameObject SetActiveInHierarchyIncludeChildren( this GameObject obj, bool active = true, bool childrenActive = true, bool destroyIfClone = true, float after = 0f, bool immediate = false )
		{
			var gObj = obj.SetActiveInHierarchy( active, destroyIfClone, after, immediate );
			gObj.SetChildrenActive( childrenActive );
			return gObj;
		}		
		public static void SetActive( this IList<GameObject> objs, bool active = true )
		{
			for( int i=0; i<objs.Count; i++ )
			{
				objs[i].SetActive( active );
			}
		}		
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy.
		/// </summary>
		/// <returns>The objects.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static IEnumerable SetGetActiveInHierarchy( this IList<GameObject> objs, bool active = true, bool destroyIfClone = true, float after = 0f, bool immediate = false )
		{
			if( objs != null )
			{
				for( int i=0; i<objs.Count; i++ )
				{
					yield return objs[i].SetActiveInHierarchy( active, destroyIfClone, after, immediate );
				}
			}
		}
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy.
		/// </summary>
		/// <returns>The objects.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static IEnumerable SetGetActiveInHierarchyAfter( this IList<GameObject> objs, float time, bool active = true )
		{
			yield return new WaitForSeconds(time);
			foreach( var obj in objs.SetGetActiveInHierarchy(active) )
			{
				yield return obj;
			}
		}
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy.
		/// </summary>
		/// <returns>The objects.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static void SetActiveInHierarchy( this IList<GameObject> objs, bool active = true, bool destroyIfClone = true, float after = 0f, bool immediate = false )
		{
			if( objs != null )
			{
				for( int i=0; i<objs.Count; i++ )
				{
					objs[i].SetActiveInHierarchy( active, destroyIfClone, after, immediate );
				}
			}
		}		
		public static IEnumerator SetActiveInHierarchyAfterCo( this IList<GameObject> objs, float time, bool active = true )
		{
			yield return new WaitForSeconds(time);
			objs.SetActiveInHierarchy(active);
		}
		public static void SetChildrenActive( this GameObject obj, bool active = true )
		{
			if( obj )
			{
				for( int i=0; i<obj.ChildCount(); i++ )
				{
					obj.GetChild(i).SetActive(active);
				}
			}
		}
		public static void SetParent( this GameObject obj, Transform parent, bool worldPosStays = false )
		{
			if( !obj )
				return;
			obj.transform.SetParent( parent, worldPosStays );
		}		
		public static void SetParent( this IList<GameObject> objs, Transform parent, bool worldPosStays = false )
		{
			for( int i=0; i<objs.Count; i++ )
			{
				if( !objs[i] )
					return;
				objs[i].transform.SetParent( parent, worldPosStays );
			}
		}		
		public static void SetParent( this IList<GameObject> objs, GameObject parent )
		{
			objs.SetParent( parent.transform );
		}
		[Obsolete("GUI might be deprecated in the future, use the new UI available since unity 4.6 and call the SetText() function instead")]
		public static void SetGuiText( this GameObject obj, string txt )
		{
			var text = obj.GetComponentInChildren<GUIText>();
			if( !text )
				return;
			text.text = txt;
		}
		/// <summary>
		/// If the object or any of its children have a Text component, its text value will be set with the specified. On the First found Text component.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="txt">Text.</param>
		public static void SetText( this GameObject obj, string txt )
		{
			var text = obj.GetComponentInChildren<Text>();
			if( !text )
				return;
			text.text = txt;
		}	
		/// <summary>
		/// If the object or any of its children have a Text component, its value will be set with the specified. On the First found Text component.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="txt">Text.</param>
		public static void SetText( this GameObject obj, Text txt )
		{
			var text = obj.GetComponentInChildren<Text>();
			if( !text )
				return;
			text = txt;
		}	
		public static void SetAudioClip( this GameObject obj, AudioClip clip )
		{
			var src = obj.GetComponentInChildren<AudioSource>();
			if( !src )
				return;
			src.clip = clip;
		}		
		public static void SetAudioClipFromPath( this GameObject obj, string path )
		{
			AudioClip clip = Resources.Load<AudioClip>( path );
			obj.SetAudioClip( clip );
		}
		public static void SetSpriteOnLastActive( this IList<GameObject> objs, Sprite sprite, bool isImage = true )
		{
			for( int i = objs.Count-1; i >= 0; i-- )
			{
				if( objs[i].activeSelf )
				{
					if( isImage )
					{
						objs[i].SetImageSprite( sprite );
					}
					else objs[i].SetSprite( sprite );
					break;
				}
			}
		}		
		public static void SetSpriteOnFirstInactive( this IList<GameObject> objs, Sprite sprite, bool isImage = true )
		{
			for( int i=0; i<objs.Count; i++ )
			{
				if( !objs[i].activeSelf )
				{
					if( isImage )
					{
						objs[i].SetImageSprite( sprite );
					}
					else objs[i].SetSprite( sprite );
					break;
				}
			}
		}
		/// <summary>
		/// Sets the flags. Calling between frames may cause overhead.
		/// </summary>
		/// <param name="flags">Flags.</param>
		/// <param name="flag">If set to <c>true</c> the flags are activated; otherwise they are deactivated.</param>
		public static void SetFlagged( this GameObject obj, ValidationFlag.Flags flags, bool flag = true )
		{
			if( !obj )
				return;
			var _flag = obj.AddGetComponent<ValidationFlag>();
			_flag.SetFlagged( flags, flag );
		}
		/// <summary>
		/// Sets the flags.
		/// </summary>
		/// <param name="flags">Flags.</param>
		/// <param name="flag">If set to <c>true</c> the flags are activated; otherwise they are deactivated.</param>
		public static void SetFlagged( this ValidationFlag _flag, ValidationFlag.Flags flags, bool flag = true )
		{
			if( !_flag )
				return;
			if( flag )
				_flag.flags |= flags;
			else _flag.flags ^= ( _flag.flags & flags );
		}
        /// <summary>
        /// Sets the flags. Calling between frames may cause overhead.
        /// </summary>
        /// <param name="flags">Flags.</param>
        /// <param name="flag">If set to <c>true</c> the flags are activated; otherwise they are deactivated.</param>
        public static void SetFlagged( this GameObject[] objs, ValidationFlag.Flags flags, bool flag = true )
        {
            if( objs == null )
                return;
            var _flags = objs.AddGetComponents<ValidationFlag>();
            _flags.SetFlagged( flags, flag );
        }
        /// <summary>
        /// Sets the flags.
        /// </summary>
        /// <param name="flags">Flags.</param>
        /// <param name="flag">If set to <c>true</c> the flags are activated; otherwise they are deactivated.</param>
        public static void SetFlagged( this ValidationFlag[] _flags, ValidationFlag.Flags flags, bool flag = true )
        {
            if( _flags == null )
                return;
            for( int i=0; i<_flags.Length; i++ )
            {
                _flags[ i ].SetFlagged( flags, flag );
            }
        }
        public static SearchableGameObject[] Fill( this SearchableGameObject[] objs, GameObject[] gObjs )
        {
            if( gObjs == null || gObjs.Length == 0 )
            {
                Debug.LogWarning("No gameObjects... Returning null");
                return null;
            }
            objs = new SearchableGameObject[ gObjs.Length ];
            for( int i=0; i<gObjs.Length; i++ )            
            {
                objs[ i ] = new SearchableGameObject( gObjs[ i ] );
            }
            return objs;
        }
		#endregion

		#region ADD / GET
		/// <summary>
		/// Adds a new audio source to this game object, unless an audio source has already been added.
		/// </summary>
		/// <returns>The audio source.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="clip">Clip.</param>
		/// <param name="volume">Volume.</param>
		/// <param name="pitch">Pitch.</param>
		/// <param name="playOnAwake">If set to <c>true</c> play on awake.</param>
		public static AudioSource GetAddAudioSource( this GameObject obj, AudioClip clip, float volume = 1f, float pitch = 1f, bool playOnAwake = false )
		{
			AudioSource src = obj.GetAddAudioSource( volume, pitch, playOnAwake );
			src.clip = clip;
			return src;
		}
		/// <summary>
		/// Adds a new audio source to this game object, unless an audio source has already been added.
		/// </summary>
		/// <returns>The audio source.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="clip">Clip.</param>
		/// <param name="volume">Volume.</param>
		/// <param name="pitch">Pitch.</param>
		/// <param name="playOnAwake">If set to <c>true</c> play on awake.</param>
		public static AudioSource GetAddAudioSource( this GameObject obj, float volume = 1f, float pitch = 1f, bool playOnAwake = false )
		{
			if( !obj )
			{
				obj = GameObject.Find("Sfx");
				if( !obj )
				{
					obj = new GameObject("Sfx");
				}
			}
			//CHECK IF THE AUDIO SOURCE ALREADY EXISTS
			var src = obj.GetComponent<AudioSource>();
			//IF NOT, ADD IT
			if( !src )
			{
				src = obj.AddComponent<AudioSource>();
			}
			src.playOnAwake = playOnAwake;
			src.volume = volume;
			src.pitch = pitch;
			return src;
		}
		/// <summary>
		/// Adds a new audio source to this game object, unless an audio source with the same clip has already been added.
		/// </summary>
		/// <returns>The audio source.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="clip">Clip.</param>
		/// <param name="volume">Volume.</param>
		/// <param name="pitch">Pitch.</param>
		/// <param name="playOnAwake">If set to <c>true</c> play on awake.</param>
		public static AudioSource AddGetAudioSource( this GameObject obj, AudioClip clip, float volume = 1f, float pitch = 1f, bool playOnAwake = false )
		{
			if( !obj )
			{
				obj = GameObject.Find("Sfx");
				if( !obj )
				{
					obj = new GameObject("Sfx");
				}
			}
			//CHECK IF THE AUDIO SOURCE WITH THE CLIP ALREADY EXISTS
			var srcs = obj.GetComponents<AudioSource>();
			for( int i=0; i<srcs.Length; i++ )
			{
				if( srcs[i].clip == clip )
				{
					return srcs[i];
				}
			}
			//IF NOT, ADD IT
			var src = obj.AddComponent<AudioSource>();
			src.playOnAwake = playOnAwake;
			src.volume = volume;
			src.clip = clip;
			src.pitch = pitch;
			return src;
		}
		/// <summary>
		/// Adds new audio sources to this game object, unless an audio source with the same clip has already been added.
		/// </summary>
		/// <returns>The audio source.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="clips">The clips.</param>
		/// <param name="volumes">Volume.</param>
		/// <param name="pitches">Pitch.</param>
		/// <param name="playOnAwake">If set to <c>true</c> play on awake.</param>
		public static List<AudioSource> AddGetAudioSources( this GameObject obj, AudioClip[] clips, float[] volumes = null, float[] pitches = null, bool[] playOnAwake = null,
		                                                   bool clampVolArray = false, bool clampPitchesArray = false, bool clampOnAwakeArray = false )
		{
			List<AudioSource> audioSrcs = new List<AudioSource>();
			if( volumes == null )
			{
				volumes = new float[]{ 1f };
			}
			if( pitches == null )
			{
				pitches = new float[]{ 1f };
			}
			if( playOnAwake == null )
			{
				playOnAwake = new bool[]{ false };
			}
			//CHECK IF THE AUDIO SOURCE WITH THE CLIP ALREADY EXISTS
			var srcs = obj.GetComponents<AudioSource>();
			for( int i=0; i<clips.Length; i++ )
			{
				bool addItAnyWay = false;
				for( int j=0; j<srcs.Length; j++ )
				{
					if( srcs[j].clip == clips[i] )
					{
						audioSrcs.Add( srcs[j] );
						break;
					}
					else if( srcs.Length - 1 == j )
					{
						addItAnyWay = true;
					}
				}
				if( srcs.Length == 0 || addItAnyWay )
				{
					//IF NOT, ADD IT
					var src = obj.AddComponent<AudioSource>();
					int index = audioSrcs.Count;
					if( clampVolArray )
					{
						src.volume = volumes[ index.Clamp( 0, volumes.Length - 1 ) ];
					}
					else if( volumes.Length > index )
					{
						src.volume = volumes[index];
					}
					if( clampPitchesArray )
					{
						src.pitch = pitches[ index.Clamp( 0, pitches.Length - 1 ) ];
					}
					else if( pitches.Length > index )
					{
						src.pitch = pitches[index];
					}
					if( clampOnAwakeArray )
					{
						src.playOnAwake = playOnAwake[ index.Clamp( 0, playOnAwake.Length - 1 ) ];
					}
					else if( playOnAwake.Length > index )
					{
						src.playOnAwake = playOnAwake[index];
					}
					src.clip = clips[ index.Clamp( 0, clips.Length - 1 ) ];
					audioSrcs.Add( src );
				}
			}
			return audioSrcs;
		}
		/// <summary>
		/// Adds a new audio source to this game object, unless an audio source with the same clip has already been added.
		/// </summary>
		/// <returns>The audio source.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="clipsPaths">The clips path, if a path has multiple audio clips, a random one will be picked.</param>
		/// <param name="volumes">Volume.</param>
		/// <param name="pitches">Pitch.</param>
		/// <param name="playOnAwake">If set to <c>true</c> play on awake.</param>
		public static List<AudioSource> AddGetAudioSources( this GameObject obj, string[] clipsPaths, float[] volumes = null, float[] pitches = null, bool[] playOnAwake = null,
		                                                   bool clampVolArray = false, bool clampPitchesArray = false, bool clampOnAwakeArray = false )
		{
			List<AudioSource> audioSrcs = new List<AudioSource>();
			if( volumes == null )
			{
				volumes = new float[]{ 1f };
			}
			if( pitches == null )
			{
				pitches = new float[]{ 1f };
			}
			if( playOnAwake == null )
			{
				playOnAwake = new bool[]{ false };
			}
			//CHECK IF THE AUDIO SOURCE WITH THE CLIP ALREADY EXISTS
			var srcs = obj.GetComponents<AudioSource>();
			for( int i=0; i<clipsPaths.Length; i++ )
			{
				var clips = Resources.LoadAll<AudioClip>( clipsPaths[i] );
				AudioClip clip = null;
				if( clips.Length > 1 )
				{
					int ran = UnityEngine.Random.Range( 0, clips.Length );
					clip = clips[ran];
				}
				else clip = clips[0];
				
				bool addItAnyWay = false;
				for( int j=0; j<srcs.Length; j++ )
				{
					if( srcs[j].clip == clip )
					{
						audioSrcs.Add( srcs[j] );
						break;
					}
					else if( srcs.Length - 1 == j )
					{
						addItAnyWay = true;
					}
				}
				if( srcs.Length == 0 || addItAnyWay )
				{
					//IF NOT, ADD IT
					var src = obj.AddComponent<AudioSource>();
					int index = audioSrcs.Count;
					
					if( clampVolArray )
					{
						src.volume = volumes[ index.Clamp( 0, volumes.Length - 1 ) ];
					}
					else if( volumes.Length > index )
					{
						src.volume = volumes[index];
					}
					if( clampPitchesArray )
					{
						src.pitch = pitches[ index.Clamp( 0, pitches.Length - 1 ) ];
					}
					else if( pitches.Length > index )
					{
						src.pitch = pitches[index];
					}
					if( clampOnAwakeArray )
					{
						src.playOnAwake = playOnAwake[ index.Clamp( 0, playOnAwake.Length - 1 ) ];
					}
					else if( playOnAwake.Length > index )
					{
						src.playOnAwake = playOnAwake[index];
					}
					
					clips = Resources.LoadAll<AudioClip>( clipsPaths[ index.Clamp( 0, clipsPaths.Length - 1 ) ] );
					if( clips.Length > 1 )
					{
						int ran = UnityEngine.Random.Range( 0, clips.Length );
						clip = clips[ran];
					}
					else clip = clips[0];
					src.clip = clip;
					audioSrcs.Add( src );
				}
			}
			return audioSrcs;
		}
        /// <summary>
        /// Adds components of the specified type, if there is no other component already in each object.
        /// </summary>
        public static T[] AddGetComponents<T>( this GameObject[] objs ) where T : Component
        {
            if( objs == null )
                return default( T[] );
            T[] comps = new T[ objs.Length ];
            for( int i=0; i<objs.Length; i++ )
            {
                comps[ i ] = objs[ i ].AddGetComponent<T>();
            }
            return comps;
        }
		/// <summary>
		/// Adds a component of the specified type, if there is no other component already in the object.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T AddGetComponent<T>( this GameObject obj ) where T : Component
		{
			if( !obj )
				return default(T);
			var comp = obj.GetComponent<T>();
			if( !comp )
			{
				comp = obj.AddComponent<T>();
			}
			return comp;
		}
		/// <summary>
		/// Adds a component of the specified type, if there is no other component already in the object.
		/// </summary>
		/// <returns>The component.</returns>
		public static Component AddGetComponent( this GameObject obj, System.Type type )
		{
			var comp = obj.GetComponent( type );
			if( !comp )
			{
				comp = obj.AddComponent( type );
			}
			return comp;
		}
		/// <summary>
		/// Adds a component of the specified type, if there is no other component already in the object.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> AddGetComponentIncludeChildren<T>( this GameObject obj, bool includeInactive = false ) where T : Component
		{
			List<T> comps = new List<T>();
			if( !obj )
				return comps;
			comps.Add( obj.AddGetComponent<T>() );
			
			var children = obj.GetChildren( true, includeInactive );
			for( int i=0; i<children.Length; i++ )
			{
				comps.Add( children[i].AddGetComponent<T>() );
			}
			return comps;
		}
		#endregion
		
		#region ORDER
		public static void OrderSiblingsByNameDescending( this GameObject[] objs )
		{			
			GameObject temp;
			int _name = -1, _name2 = -1;
			for( int i=0; i<objs.Length; i++ ) //SORT OBJECTS
			{
				for ( int j=0; j<objs.Length - 1; j++ ) 
				{
					bool sort = false;
					if( int.TryParse( objs[ j ].name, out _name ) && int.TryParse( objs[ j+1 ].name, out _name2 ) )
					{
						if( _name < _name2 )
						{
							sort = true;
						}
					}
					else if( objs[ j ].name.CompareTo( objs[ j+1 ].name ) < 0 ) 
					{
						sort = true;
					}
					if( sort )
					{
						temp = objs[ j+1 ];
						objs[ j+1 ] = objs[ j ];
						objs[ j ] = temp;
					}
				}
			}
			for( int i=0; i<objs.Length; i++ ) //SET SIBLING INDEXES
			{
				objs[ i ].transform.SetSiblingIndex( i );
			}
		}
		public static void OrderSiblingsByNameAscending( this GameObject[] objs )
		{			
			GameObject temp;
			int _name = -1, _name2 = -1;
			for( int i=0; i<objs.Length; i++ ) //SORT OBJECTS
			{
				for ( int j=0; j<objs.Length - 1; j++ ) 
				{
					bool sort = false;
					if( int.TryParse( objs[ j ].name, out _name ) && int.TryParse( objs[ j+1 ].name, out _name2 ) )
					{
						if( _name > _name2 )
						{
							sort = true;
						}
					}
					else if( objs[ j ].name.CompareTo( objs[ j+1 ].name ) > 0 ) 
					{
						sort = true;
					}
					if( sort )
					{
						temp = objs[ j+1 ];
						objs[ j+1 ] = objs[ j ];
						objs[ j ] = temp;
					}
				}
			}
			for( int i=0; i<objs.Length; i++ ) //SET SIBLING INDEXES
			{
				objs[ i ].transform.SetSiblingIndex( i );
			}
		}
		#endregion

		#region TRANSFORM RELATED
		/// <summary>
		/// Gets this object's position. If it has a RectTransform the /anchoredPosition3D/ will be returned, and local
		/// won't be taken into account.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="local">If set to <c>true</c> local.</param>
		/// <param name="checkRectTransform"> If true and the object has a RectTransform, its anchored positione will be returned instead. </param> 
		public static Vector3 Position( this GameObject obj, bool local = false, bool checkRectTransform = false )
		{
			if( obj == null )
				return default( Vector3 );
			var rt = obj.GetComponent<RectTransform>();
			if( rt && checkRectTransform )
			{
				return rt.anchoredPosition3D;
			}
			if( local )
				return obj.transform.localPosition;
			return obj.transform.position;
		}
		/// <summary>
		/// Gets this object's position. If it has a RectTransform the /anchoredPosition/ will be returned, and local
		/// won't be taken into account.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="local">If set to <c>true</c> local.</param>
		/// <param name="checkRectTransform"> If true and the object has a RectTransform, its anchored positione will be returned instead. </param> 
		public static Vector2 Position2( this GameObject obj, bool local = false, bool checkRectTransform = false )
		{
			return (Vector2) obj.Position( local, checkRectTransform );
		}
		/// <summary>
		/// Gets this object's anchored position. If it doesn't have a RectTransform the /position/ will be returned.
		/// </summary>
		/// <param name="obj">Object.</param>
		public static Vector3 AnchoredPosition3D( this GameObject obj )
		{
			return obj.Position( false, true );
		}
		/// <summary>
		/// Gets this object's anchored position. If it doesn't have a RectTransform the /position/ will be returned.
		/// </summary>
		/// <param name="obj">Object.</param>
		public static Vector2 AnchoredPosition( this GameObject obj )
		{
			return obj.Position2( false, true );
		}
		/// <summary>
		/// Gets this object's rotation.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="local">If set to <c>true</c> local.</param>
		public static Quaternion Rotation( this GameObject obj, bool local = false )
		{
			if( obj == null )
				return default( Quaternion );
			if( local )
				return obj.transform.localRotation;
			return obj.transform.rotation;
		}
		/// <summary>
		/// Gets this object's rotation.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="local">If set to <c>true</c> local.</param>
		public static Vector3 RotationEuler( this GameObject obj, bool local = false )
		{
			return obj.Rotation( local ).eulerAngles;
		}
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="axis">The rotation axis.</param>
		public static void AnimRotateAroundTo( this GameObject obj, Vector3 point, Vector3 target, float duration, Vector3 axis )
		{
			obj.AnimRotateAroundToCo( point, target, duration, axis ).Start();
		}
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="axis">The rotation axis.</param>
		public static IEnumerator AnimRotateAroundToCo( this GameObject obj, Vector3 point, Vector3 target, float duration, Vector3 axis )
		{
			if( !obj )
				yield break;
			ValidationFlag validationFlags = obj.AddGetComponent<ValidationFlag>();
			if( validationFlags.IsFlagged( ValidationFlag.Flags.RotateAround ) )
				yield break;
			validationFlags.SetFlagged( ValidationFlag.Flags.RotateAround, true );

			float time = 0f;
			Vector3 targetDir = target - point;
			Vector3 initDir = obj.transform.position - point;
			float distance = Vector3.Distance( point, target );
			while( obj.transform.position.Distance( target ) >= 0.01f )
			{
				if( duration == 0f )
					break;
				if( time > duration )
					break;
				time += Time.deltaTime;
				obj.transform.position = point + Vector3.Lerp( initDir, targetDir, time / duration ).normalized * distance;				
				yield return null;
				if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.RotateAround ) )
					yield break;
				if( !obj )
					yield break;
			}
			obj.transform.position = target;

			validationFlags.SetFlagged( ValidationFlag.Flags.RotateAround, false );
			yield return null;
		}
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. The point transform's forward axis will be used as the rotation axis.
		/// NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		public static void AnimRotateAroundTo( this GameObject obj, Transform point, Vector3 target, float duration )
		{
			obj.AnimRotateAroundToCo( point, target, duration, point ).Start();
		}
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="axis">The rotation axis.</param>
		public static void AnimRotateAroundTo( this GameObject obj, Transform point, Vector3 target, float duration, Transform axis )
		{
			obj.AnimRotateAroundToCo( point, target, duration, axis ).Start();
		}
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. The point transform's forward axis will be used as the rotation axis.
		/// NOTE: Use ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		public static void AnimRotateAroundTo( this GameObject obj, Transform point, Transform target, float duration )
		{
			obj.AnimRotateAroundToCo( point, target, duration, point ).Start();
		}
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="axis">The rotation axis.</param>
		public static void AnimRotateAroundTo( this GameObject obj, Transform point, Transform target, float duration, Transform axis )
		{
			obj.AnimRotateAroundToCo( point, target, duration, axis ).Start();
		}
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="axis">THIS IS HASN'T BEEN IMPLEMENTED. The rotation axis.</param>
		public static IEnumerator AnimRotateAroundToCo( this GameObject obj, Transform point, Vector3 target, float duration, Transform axis )
		{
			if( !obj || !point )
				yield break;
			ValidationFlag validationFlags = obj.AddGetComponent<ValidationFlag>();
			if( validationFlags.IsFlagged( ValidationFlag.Flags.RotateAround ) )
				yield break;
			validationFlags.SetFlagged( ValidationFlag.Flags.RotateAround, true );

			float time = 0f;
			Vector3 targetDir = target - point.position;
			Vector3 initDir = obj.transform.position - point.position;
			float distance = Vector3.Distance( point.position, target );
			while( obj.transform.position.Distance( target ) >= 0.01f )
			{
				if( duration == 0f )
					break;
				if( time > duration )
					break;
				time += Time.deltaTime;
				obj.transform.position = point.position + Vector3.Lerp( initDir, targetDir, time / duration ).normalized * distance;				
				yield return null;
				if( !validationFlags.IsFlagged( ValidationFlag.Flags.RotateAround ) )
					yield break;
				if( !obj )
					yield break;
				if( !point )
					break;
			}
			obj.transform.position = target;

			validationFlags.SetFlagged( ValidationFlag.Flags.RotateAround, false );
			yield return null;

			//OLD CODE
			/*float time = 0f;
			int sign = ( obj.transform.position.y < target.y ) ? -1 : 1;
			Vector3 targetDir = target - point.position;
			while( obj.transform.position.Distance( target ) >= 0.01f )
			{
				time += Time.deltaTime;
				Vector3 currentDir = obj.transform.position - point.position;
				float angle = Vector3.Angle( targetDir, currentDir ) * sign;
				obj.transform.RotateAround( point.position, axis.forward, (time / duration) * angle * Time.deltaTime );
				yield return null;
				if( time > duration )
					duration *= 0.95f;
			}
			obj.transform.position = target;*/
		}
		/// <summary>
		/// Rotates this object around the specified /point/ until it reaches the specified /target/. NOTE: The target must
		/// be a point inside the orbit. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="duration">The animation's duration.</param>
		/// <param name="axis">The rotation axis.</param>
		public static IEnumerator AnimRotateAroundToCo( this GameObject obj, Transform point, Transform target, float duration, Transform axis )
		{
			if( !obj || !point || !target )
				yield break;
			ValidationFlag validationFlags = obj.AddGetComponent<ValidationFlag>();
			if( validationFlags.IsFlagged( ValidationFlag.Flags.RotateAround ) )
				yield break;
			validationFlags.SetFlagged( ValidationFlag.Flags.RotateAround, true );

			float time = 0f;
			Vector3 targetDir = target.position - point.position;
			Vector3 initDir = obj.transform.position - point.position;
			float distance = Vector3.Distance( point.position, target.position );
			while( obj.transform.position.Distance( target.position ) >= 0.01f )
			{
				if( duration == 0f )
					break;
				if( time > duration )
					break;
				time += Time.deltaTime;
				obj.transform.position = point.position + Vector3.Lerp( initDir, targetDir, time / duration ).normalized * distance;				
				yield return null;
				if( !validationFlags.IsFlagged( ValidationFlag.Flags.RotateAround ) )
					yield break;
				if( !obj || !target )
					break;
			}
			if( target )
				obj.transform.position = target.position;

			validationFlags.SetFlagged( ValidationFlag.Flags.RotateAround, false );
			yield return null;
		}
		/// <summary>
		/// Aligns this object's orientation until it matches the specified target's. This need to be called multiple times. The next rotation is automatically set.
		/// </summary>
		public static void LookRotation( this GameObject current, Transform target )
		{
			float angle = Vector3.Angle( current.transform.forward, target.forward );
			current.SetRot( Quaternion.RotateTowards( current.Rotation(), Quaternion.LookRotation( target.forward, target.up ), angle ) );
		}
		/// <summary>
		/// Aligns this object's orientation until it matches the specified target's. The duration of the animation is not precise.
		/// </summary>
		[Obsolete("Use RotateToCo() instead")]
		public static IEnumerator LookRotationCo( this GameObject current, float duration, Transform target )
		{
			float time = 0f;
			float angle = Vector3.Angle( current.transform.forward, target.forward );
			while( current.Rotation() != target.rotation )
			{
				time += Time.deltaTime;
				//current.transform.rotation = Quaternion.LookRotation( target.forward, target.up );
				current.SetRot( Quaternion.RotateTowards( current.Rotation(), Quaternion.LookRotation( target.forward, target.up ), (time / duration) * angle * Time.deltaTime ) );
				yield return null;
				if( !current || !target )
					yield break;
				if( time > duration )
					duration *= 0.95f;
				angle = Vector3.Angle( current.transform.forward, target.forward );
			}
			current.transform.rotation = target.rotation;
		}
		/// <summary>
		/// Rotates towards the specified target's orientation until it is reached. The duration of the animation is not precise. This calls LookRotationCo()
		/// </summary>
		[Obsolete("Use AnimRotateTo() instead")]
		public static void AnimLookRotation( this GameObject current, float duration, Transform target )
		{
#pragma warning disable 0618
			current.LookRotationCo( duration, target ).Start();
#pragma warning restore 0618
		}
		/// <summary>
		/// Aligns this object's orientation until it matches the specified target's or this object or its target is 
		/// destroyed. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// If this object's Rotate flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="useLerp">If set to <c>true</c> use Lerp which is faster than Slerp but might cause undesired behaviour.</param>
		public static IEnumerator RotateToCo( this GameObject current, float duration, Transform target, bool useLerp = false )
		{
			if( !current )
				yield break;
			ValidationFlag validationFlags = current.AddGetComponent<ValidationFlag>();
			if( validationFlags.IsFlagged( ValidationFlag.Flags.Rotate ) )
				yield break;
			validationFlags.SetFlagged( ValidationFlag.Flags.Rotate, true );

			float time = 0f;
			Quaternion initRot = current.Rotation();
            while( time <= duration )
			{
				if( duration == 0f )
					break;
				time += Time.deltaTime;
				if( useLerp )
					current.SetRot( Quaternion.Lerp( initRot, target.rotation, (time / duration) ) );
				else current.SetRot( Quaternion.Slerp( initRot, target.rotation, (time / duration) ) );
				yield return null;
                if( !validationFlags.IsFlagged( ValidationFlag.Flags.Rotate ) || !current )
					yield break;
				if( !target )
					break;
			}
			if( target )
				current.transform.rotation = target.rotation;

			validationFlags.SetFlagged( ValidationFlag.Flags.Rotate, false );
			yield return null;
		}
		/// <summary>
		/// Aligns this object's orientation until it matches the specified target's. NOTE: This uses ValidationFlag.cs 
		/// to prevent multiple calls on an object that is being rotated.
		/// </summary>
		/// <param name="useLerp">If set to <c>true</c> use Lerp which is faster than Slerp but might cause undesired behaviour.</param>
		public static void AnimRotateTo( this GameObject current, float duration, Transform target, bool useLerp = false )
		{
			current.RotateToCo( duration, target, useLerp ).Start();
		}
		/// <summary>
		/// Aligns this object's orientation until it matches the specified target's. NOTE: This uses ValidationFlag.cs
		/// to prevent multiple calls on an object that is being rotated.
		/// </summary>
		/// <param name="useLerp">If set to <c>true</c> use Lerp which is faster than Slerp but might cause undesired behaviour.</param>
		public static void AnimRotateTo( this GameObject current, float duration, GameObject target, bool useLerp = false )
		{
			current.RotateToCo( duration, target.transform, useLerp ).Start();
		}
		/// <summary>
		/// Rotate towards the specified target until it is reached, or this object is destroyed. NOTE: This uses
		/// ValidationFlag.cs to prevent multiple calls on an object that is being rotated.
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local rotation will be used.</param>
		public static IEnumerator RotateTowardsCo( this GameObject current, Quaternion target, float deltaSpeed = 1f, bool local = false )
		{
			if( !current )
				yield break;
			ValidationFlag validationFlags = current.AddGetComponent<ValidationFlag>();
			if( validationFlags.IsFlagged( ValidationFlag.Flags.Rotate ) )
				yield break;
			validationFlags.SetFlagged( ValidationFlag.Flags.Rotate, true );

			while( current.Rotation( local ) != target )
			{
				current.SetRot( Quaternion.RotateTowards( current.Rotation( local ), target, deltaSpeed ), local );
				yield return null;
				if( !validationFlags.IsFlagged( ValidationFlag.Flags.Rotate ) )
					yield break;
				if( !current )
					yield break;
			}

			validationFlags.SetFlagged( ValidationFlag.Flags.Rotate, false );
			yield return null;
		}
		/// <summary>
		/// Moves towards the specified target. This need to be called multiple times. The next position is automatically set.
		/// </summary>
		/// <returns>A value closer to the target.</returns>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static Vector3 MoveTowards( this GameObject current, Vector3 target, float deltaSpeed = 1f, bool local = false, bool useRectTransform = false )
		{
			var pos = Vector3.MoveTowards( current.Position( local, useRectTransform), target, deltaSpeed );
			current.SetPos( pos, local, useRectTransform );
			return pos;
		}
		/// <summary>
		/// Moves towards the specified target until it is reached, or this object or its target is destroyed. NOTE:
		/// This uses ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// If this object's Move flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static IEnumerator MoveTowardsCo( this GameObject current, Vector3 target, float deltaSpeed = 1f, bool local = false, bool useRectTransform = false )
		{
			if( !current )
				yield break;
			ValidationFlag validationFlags = current.AddGetComponent<ValidationFlag>();
			if( validationFlags.IsFlagged( ValidationFlag.Flags.Move ) )
				yield break;
			validationFlags.SetFlagged( ValidationFlag.Flags.Move, true );

			while( current.Position( local, useRectTransform ) != target )
			{
				current.SetPos( Vector3.MoveTowards( current.Position( local, useRectTransform ), target, deltaSpeed ), local, useRectTransform );
				yield return null;
				if( !validationFlags.IsFlagged( ValidationFlag.Flags.Move ) )
					yield break;
				if( !current )
					yield break;
				if( current.Position( local, useRectTransform ) == target ) //if( Vector3.Distance( current.Position( local, useRectTransform ), target ) < 0.001f )
				{
					break;
				}
			}
			current.SetPos( target, local, useRectTransform );

			validationFlags.SetFlagged( ValidationFlag.Flags.Move, false );
			yield return null;
		}
		/// <summary>
		/// Moves towards the specified target until it is reached, or this object or its target is destroyed. NOTE:
		/// This uses ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// If this object's Move flag is set to false, the coroutine will end.
		/// </summary>
		/// <returns>A value closer to the target.</returns>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static void AnimMoveTowards( this GameObject current, Vector3 target, float deltaSpeed = 1f, bool local = false, bool useRectTransform = false )
		{
			current.MoveTowardsCo( target, deltaSpeed, local, useRectTransform ).Start();
		}
		/// <summary>
		/// Moves towards the specified target until it is reached in the specified amount of time, or this object or 
		/// its target is destroyed. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// If this object's Move flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static IEnumerator MoveTowardsCo( this GameObject current, float duration, Vector3 target, bool local = false, bool useRectTransform = false )
		{
			if( !current )
				yield break;
			ValidationFlag validationFlags = current.AddGetComponent<ValidationFlag>();
			if( validationFlags.IsFlagged( ValidationFlag.Flags.Move ) )
				yield break;
			validationFlags.SetFlagged( ValidationFlag.Flags.Move, true );

			float time = 0f;
			//float distance = Vector3.Distance( current.Position( local, useRectTransform ), target );
			Vector3 initPos = current.Position( local, useRectTransform );
            while( time <= duration ) 
            //while( current.Position( local, useRectTransform ) != target )
			{
				if( duration == 0f )
					break;
				time += Time.deltaTime;
				current.SetPos( Vector3.Lerp( initPos, target, (time / duration) ), local, useRectTransform );
				yield return null;
                if( !validationFlags.IsFlagged( ValidationFlag.Flags.Move ) || !current )
					yield break;
			}
			current.SetPos( target, local, useRectTransform );

			validationFlags.SetFlagged( ValidationFlag.Flags.Move, false );
			yield return null;
		}
		/// <summary>
		/// Moves towards the specified target until it is reached in the specified amount of time, or this object or 
		/// its target is destroyed. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// If this object's Move flag is set to false, the coroutine will end.
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static IEnumerator MoveTowardsCo( this GameObject current, float duration, Transform target, bool local = false, bool useRectTransform = false )
		{
			if( !current || !target )
				yield break;
			ValidationFlag validationFlags = current.AddGetComponent<ValidationFlag>();
			if( validationFlags.IsFlagged( ValidationFlag.Flags.Move ) )
				yield break;
			validationFlags.SetFlagged( ValidationFlag.Flags.Move, true );
			
			float time = 0f;
			Vector3 initPos = current.Position( local, useRectTransform );
            while( time <= duration ) 
            //while( current.Position( local, useRectTransform ).Distance( target.position ) > 0.01f )
			{
				if( duration == 0f )
					break;
				time += Time.deltaTime;
				current.SetPos( Vector3.Lerp( initPos, target.position, time / duration ), local, useRectTransform );
				yield return null;
                if( !validationFlags.IsFlagged( ValidationFlag.Flags.Move ) || !current )
					yield break;
				if( !target )
					break;
			}
			if( target )
				current.SetPos( target.position, local, useRectTransform );
			
			validationFlags.SetFlagged( ValidationFlag.Flags.Move, false );
			yield return null;
		}
		/// <summary>
		/// Moves towards the specified target until it is reached in the specified amount of time, or this object or 
		/// its target is destroyed. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// If this object's Move flag is set to false, the coroutine will end. This calls MoveTowardsCo().
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static void AnimMoveTowards( this GameObject current, float duration, Vector3 target, bool local = false, bool useRectTransform = false )
		{
			current.MoveTowardsCo( duration, target, local, useRectTransform ).Start();
		}
		/// <summary>
		/// Moves towards the specified target until it is reached in the specified amount of time, or this object or 
		/// its target is destroyed. NOTE: This uses ValidationFlag.cs to prevent multiple calls on an object that is being moved.
		/// If this object's Move flag is set to false, the coroutine will end. This calls MoveTowardsCo().
		/// </summary>
		/// <param name="local">If set to <c>true</c> the local position will be used.</param>
		/// <param name="useRectTransform">If set to <c>true</c> the rect transform's anchored position will be used, if the object has the component.</param>
		public static void AnimMoveTowards( this GameObject current, float duration, Transform target, bool local = false, bool useRectTransform = false )
		{
			current.MoveTowardsCo( duration, target, local, useRectTransform ).Start();
		}
        /// <summary>
        /// Animates the scale of the specified object until the target scale is reached or this object is destroyed.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetScl">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target scale on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimScaleWithMultiplier( this GameObject obj, Vector3 targetScl, float duration, 
            AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( !obj )
                yield break;
            if( toggleAnimator )
            {
                obj.ToggleAnimator();
            }

            ValidationFlag validationFlags = obj.AddGetComponent<ValidationFlag>();
            if( validationFlags.IsFlagged( ValidationFlag.Flags.Scale ) )
                yield break;
            validationFlags.SetFlagged( ValidationFlag.Flags.Scale, true );

            if( multiplier == null || multiplier.keys.Length <= 1 )
                multiplier = AnimationCurve.Linear( 0f, 1f, duration, 1f );
            targetScl *= multiplier.keys[ multiplier.keys.Length - 1 ].value;

            Vector3 initialScl = obj.transform.localScale;
            float time = 0f;
            float multiply = 1f;//caching
            while( time <= duration )
            {
                time += Time.deltaTime;
                multiply = multiplier.Evaluate( Mathf.Lerp( 0f, 
                    multiplier.keys[ multiplier.keys.Length - 1 ].time, time / duration ) );
                obj.transform.localScale = Vector3.Lerp( initialScl, targetScl, time / duration ) * multiply;
                yield return null;
                if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.Scale ) || !obj )
                    yield break;
            }
            validationFlags.SetFlagged( ValidationFlag.Flags.Scale, false );
            yield return null;
        }
        /// <summary>
        /// Animates the scale of the specified objects until the target scales are reached. This yields for the specified duration.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetScl">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target scale on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimScaleWithMultiplier( this GameObject[] objs, Vector3 targetScl, float duration, 
            AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( objs == null )
                yield break;
            for( int i=0; i<objs.Length; i++ )
            {
                objs[ i ].AnimScaleWithMultiplier( targetScl, duration, multiplier, toggleAnimator ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        /// <summary>
        /// Animates the scale of the specified objects until the target scales are reached. This yields for the specified duration.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetScl">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target scale on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimScaleWithMultiplier( this GameObject[] objs, Vector3[] targetScales, float duration, 
            AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( objs == null )
                yield break;
            int targetScalesLength = targetScales.Length;
            for( int i=0; i<objs.Length; i++ )
            {
                if( i >= targetScalesLength )
                {
                    Utilities.LogWarning( "The /targetScales/ array is shorter than the /objs/ array" );
                    if( i == 0 )
                        yield break;
                    break;
                }
                objs[ i ].AnimScaleWithMultiplier( targetScales[i], duration, multiplier, toggleAnimator ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        /// <summary>
        /// Animates the scale of the specified objects until the target scales are reached. This yields for the specified duration.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetScl">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target scale on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimScaleWithMultiplier( this SearchableGameObject[] objs, Vector3 targetScl, float duration, 
            AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( objs == null )
                yield break;
            for( int i=0; i<objs.Length; i++ )
            {
                objs[ i ].m_gameObject.AnimScaleWithMultiplier( targetScl, duration, multiplier, toggleAnimator ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        /// <summary>
        /// Animates the scale of the specified objects until the target scales are reached. This yields for the specified duration.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetScales">Target scales.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target scale on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimScaleWithMultiplier( this SearchableGameObject[] objs, Vector3[] targetScales, float duration, 
            AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( objs == null )
                yield break;
            int targetScalesLength = targetScales.Length;
            for( int i=0; i<objs.Length; i++ )
            {
                if( i >= targetScalesLength )
                {
                    Utilities.LogWarning( "The /targetScales/ array is shorter than the /objs/ array" );
                    if( i == 0 )
                        yield break;
                    break;
                }
                objs[ i ].m_gameObject.AnimScaleWithMultiplier( targetScales[i], duration, multiplier, toggleAnimator ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        /// <summary>
        /// Animates the scale of the specified object until the target scale is reached or this object is destroyed.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetScl">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimScale( this GameObject obj, Vector3 targetScl, float duration, bool toggleAnimator = false )
        {
            yield return obj.AnimScaleWithMultiplier( targetScl, duration, null, toggleAnimator ).Start();
        }
        /// <summary>
        /// Animates the scale of the specified objects until the target scales are reached. This yields for the specified duration.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetScl">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimScale( this GameObject[] objs, Vector3 targetScl, float duration, bool toggleAnimator = false )
        {
            yield return objs.AnimScaleWithMultiplier( targetScl, duration, null, toggleAnimator ).Start();
        }
        /// <summary>
        /// Animates the scale of the specified objects until the target scales are reached. This yields for the specified duration.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetScl">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimScale( this SearchableGameObject[] objs, Vector3 targetScl, float duration, bool toggleAnimator = false )
        {
            yield return objs.AnimScaleWithMultiplier( targetScl, duration, null, toggleAnimator ).Start();
        }

        /// <summary>
        /// Animates the alpha of the specified object until the target alpha is reached or this object is destroyed. 
        /// It works with Graphics, Renderers, CanvasGroups, and RendererGroups.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="target">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target scale on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this GameObject obj, float target, float duration, 
            AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( !obj )
                yield break;
            Graphic g = obj.GetComponent<Graphic>();
            if( !g || !g.enabled )
            {
                Renderer ren = obj.GetComponent<Renderer>();
                if( !ren || !ren.enabled )
                {
                    CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
                    if( !canvasGroup )
                    {
                        RendererGroup renGroup = obj.GetComponent<RendererGroup>();
                        renGroup.AnimAlphaWithMultiplier( target, duration, multiplier, toggleAnimator ).Start();
                    }
                    else canvasGroup.AnimAlphaWithMultiplier( target, duration, multiplier, toggleAnimator ).Start();
                }
                else ren.AnimAlphaWithMultiplier( target, duration, multiplier, toggleAnimator ).Start();
            }
            else g.AnimAlphaWithMultiplier( target, duration, multiplier, toggleAnimator ).Start();
        }
        /// <summary>
        /// Animates the scale of the specified objects until the target scales are reached. This yields for the specified duration.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetScl">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target scale on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this GameObject[] objs, float target, float duration, 
            AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( objs == null )
                yield break;
            for( int i=0; i<objs.Length; i++ )
            {
                objs[ i ].AnimAlphaWithMultiplier( target, duration, multiplier, toggleAnimator ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        /// <summary>
        /// Animates the scale of the specified objects until the target scales are reached. This yields for the specified duration.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="target">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target scale on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this SearchableGameObject[] objs, float target, float duration, AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( objs == null )
                yield break;
            for( int i=0; i<objs.Length; i++ )
            {
                objs[ i ].m_gameObject.AnimAlphaWithMultiplier( target, duration, multiplier, toggleAnimator ).Start();
            }
            yield return new WaitForSeconds( duration );
        }

		public static IEnumerator SwitchPos( this GameObject obj1, GameObject obj2, float speed = 1f )
		{
			if( !obj1 || !obj2 )
				yield break;
			var pos1 = obj1.Position();
			var pos2 = obj2.Position();
			while( obj1.Position() != pos2 && obj2.Position() != pos1 )
			{
				yield return null;
				if( !obj1 || !obj2 )
					break;
				obj1.MoveTowards( pos2, speed );
				obj2.MoveTowards( pos1, speed );
			}
		}
		public static void ApplyRandomRot( this GameObject obj, Vector3 axis, float minVal = 0f, float maxVal = 360f )
		{
			Vector3 ranRot = obj.GetRandomRotation( axis, minVal, maxVal );
			obj.transform.rotation = Quaternion.Euler( ranRot );
		}
		public static void ApplyRot( this GameObject obj, Vector3 rot )
		{
			obj.transform.rotation = Quaternion.Euler( rot );
		}
        public static void ApplyRandomScale( this GameObject obj, Vector3 axis, float minVal = 0f, float maxVal = 2f )
        {
            Vector3 ranScale = obj.GetRandomScale( axis, minVal, maxVal );
            obj.transform.localScale = ranScale;
        }
		/// <summary>
		/// Orientates the specified object towards the specified direction, which can be a rigidbody velocity for example.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="target">Direction.</param>
		/// <param name="forwardAxis">Forward axis (for example: Vector3.up).</param>
		public static void OrientateTowards( this GameObject obj, Vector3 direction, Vector3 forwardAxis )
		{
			if( forwardAxis == Vector3.up )
			{
				obj.transform.up = direction;
			}
			else if( forwardAxis == Vector3.right )
			{
				obj.transform.right = direction;
			}
			else if( forwardAxis == Vector3.forward )
			{
				obj.transform.forward = direction;
			}
			else Debug.LogError( "OrientateTowards: forwardsAxis must be up, right, or forward" );
		}
		#endregion

		#region ANGLES
		/// <summary>
		/// Returns the angle from this object's Z forward axis to the target's position.
		/// </summary>
		public static float GetAngleTo( this GameObject obj, GameObject target )
		{
			return obj.GetAngleTo( target.transform );
		}
		/// <summary>
		/// Returns the angle from this object's Z forward axis to the target's position.
		/// </summary>
		public static float GetAngleTo( this GameObject obj, Transform target )
		{
			return Vector3.Angle( obj.transform.forward, target.position - obj.transform.position  );
		}
		/// <summary>
		/// Returns the angle from this object's X forward axis (transform's right) to the target's position.
		/// </summary>
		public static float GetAngle2DTo( this GameObject obj, GameObject target )
		{
			return obj.GetAngle2DTo( target.transform );
		}
		/// <summary>
		/// Returns the angle from this object's X forward axis (transform's right) to the target's position.
		/// </summary>
		public static float GetAngle2DTo( this GameObject obj, Transform target )
		{
			return Vector2.Angle( obj.transform.right, target.position - obj.transform.position  );
		}
		public static float GetSignedAngleTo( this GameObject obj, GameObject target, bool inDegrees = true )
		{
			var localTarget = obj.transform.InverseTransformPoint( target.transform.position );
			var angle = Mathf.Atan2(localTarget.x, localTarget.z);
			if( inDegrees )
			{
				angle *= Mathf.Rad2Deg;
			}
			return angle;
		}		
		public static float GetSignedAngleTo( this GameObject obj, Transform target, bool inDegrees = true )
		{
			return obj.GetSignedAngleTo( target.gameObject, inDegrees );
		}		
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Animates the color of the specified object's sprite renderer / image.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="target">Target.</param>
		/// <param name="duration">Animation Duration.</param>
		/// <param name="avoidAlpha">If true, the alpha channel is not taken in account.</param>
        public static IEnumerator<float> AnimColor( this GameObject obj, Color target, float duration = 0.4f,
		                                    bool avoidAlpha = true )
		{
			if( obj == null )
			{
                yield break;
			}
            Graphic graphic = obj.GetComponent<Graphic>();
            if( graphic )
            {
                graphic.AnimColor( target, duration, avoidAlpha ).Run();
            }
            else
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                if( renderer )
                {
                    renderer.AnimColor( target, duration, avoidAlpha ).Run();
                }
            }
            yield return Timing.WaitForSeconds( duration );
		}
		/// <summary>
		/// Animates the color of the specified object's sprite renderer / image.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="target">Target.</param>
		/// <param name="duration">Ping-Pong Animation's Duration, from start to end 
		/// (initial value to target - target to initial value).</param>
		/// <param name="repeat">The amount of times to repeat the ping-pong animation.</param>
		/// <param name="avoidAlpha">If true, the alpha channel is not taken in account.</param>
        public static IEnumerator<float> AnimColorPingPong( this GameObject obj, Color target, float duration = 0.4f, int repeat = 0,
		                                            bool avoidAlpha = true )
		{
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            Graphic graphic = obj.GetComponent<Graphic>();
			for( int i=0; i<repeat+1; i++ )
			{
                if( renderer )
				{
                    yield return Timing.WaitUntilDone( renderer.AnimColorPingPong( target, duration, avoidAlpha, repeat ).Run() );
				}
                else if( graphic )
				{
                    yield return Timing.WaitUntilDone( graphic.AnimColorPingPong( target, duration, avoidAlpha, repeat ).Run() );
				}
			}
		}		
		/// <summary>
		/// Animates the alpha value of the specified object's sprite renderer / graphic.
		/// </summary>
		public static void AnimAlpha( this GameObject obj, float target, float duration = 0.4f, bool includeChildren = false, bool includeSubChildren = false )
		{
            if( obj == null )
            {
                return;
            }
            Color current = Color.white;
            Color targetColor = new Color( current.r, current.g, current.b, target );
            Graphic graphic = obj.GetComponent<Graphic>();
            if( graphic )
            {
                current = graphic.color;
                graphic.AnimColor( targetColor, duration, false ).Run();
            }
            else
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                current = renderer.color;
                if( renderer )
                {
                    renderer.AnimColor( targetColor, duration, false ).Run();
                }
            }
            if( includeChildren )
            {
                GameObject[] children = obj.GetChildren( includeSubChildren );
                children.AnimAlpha( target, duration, includeChildren, includeSubChildren );
            }
		}
		/// <summary>
		/// Animates the alpha value of the specified objects sprite renderer / graphic.
		/// </summary>
		public static void AnimAlpha( this GameObject[] objs, float target, float duration = 0.4f, bool includeChildren = false, bool includeSubChildren = false )
		{
			if( objs == null )
				return;
			for( int i=0; i<objs.Length; i++ )
			{
				objs[i].AnimAlpha( target, duration, includeChildren, includeSubChildren );
			}
		}
		/// <summary>
		/// Animates the alpha value of the specified object canvas group's.
		/// </summary>
		/// <param name="obj">Object containing a canvas group.</param>
		/// <param name="target">Target.</param>
		/// <param name="duration">Animation Duration.</param>
		public static void AnimAlphaCanvasGroup( this GameObject obj, float target, float duration = 0.4f )
		{
			var canvas = obj.GetComponent<CanvasGroup>();
			if( canvas )
			{
                canvas.AlphaTo( target, duration ).Start();
			}
			else Debug.LogWarning("There is no CanvasGroup in the specified object /"+obj.name+"/");
		}
		/// <summary>
		/// Animates the alpha value of the specified object's sprite renderer / graphic.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="target">Target.</param>
		/// <param name="duration">Ping-Pong Animation's Duration, from start to end 
		/// (initial value to target - target to initial value).</param>
		/// <param name="repeat">The amount of times to repeat the ping-pong animation.</param>
		public static void AnimAlphaPingPong( this GameObject obj, float target, float duration = 0.4f, int repeat = 0 )
		{
            Color current = Color.white;
            Color targetColor = current;
            Graphic graphic = obj.GetComponent<Graphic>();
            if( graphic )
            {
                current = graphic.color;
                targetColor = new Color( current.r, current.g, current.b, target );
                graphic.AnimColorPingPong( targetColor, duration, false, repeat ).Run();
            }
            else
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                current = renderer.color;
                if( renderer )
                {
                    targetColor = new Color( current.r, current.g, current.b, target );
                    renderer.AnimColorPingPong( targetColor, duration, false, repeat ).Run();
                }
            }
		}
		public static IEnumerator AnimSpriteAlpha( this GameObject obj, float from, float to, float duration )
		{
			float t = 0f;
			
			while (t<0.99f)
			{
				yield return null;
				t = Mathf.Clamp01(t + Time.deltaTime / duration);
				if( obj )
					obj.SetSpriteAlpha( from * (1 - t) );
			}
			while (t>0f)
			{
				yield return null;
				t = Mathf.Clamp01(t - Time.deltaTime / duration);
				if( obj )
					obj.SetSpriteAlpha( to * (1 - t) );
			}
		}
		#endregion

		#region DESTROY
		public static void DestroyChildren( this GameObject obj, int excludeSiblingFrom, int excludeSiblingTo, bool activeChildrenOnly = true )
		{
			for( int  i=0; i<obj.ChildCount(); i++ )
			{
				if( i >= excludeSiblingFrom && i < excludeSiblingTo )
					continue;
				if( activeChildrenOnly && !obj.GetChild(i).activeInHierarchy )
					continue;
				obj.DestroyChild( i );
			}
		}		
		public static void DestroyChildren( this GameObject obj, string[] exclude = null, bool activeChildrenOnly = true )
		{
			for( int  i=0; i<obj.ChildCount(); i++ )
			{
				if( exclude.Contains( obj.GetChild(i).name ) )
					continue;
				if( activeChildrenOnly && !obj.GetChild(i).activeInHierarchy )
					continue;
				obj.DestroyChild( i );
			}
		}
		public static void DestroyChildren( this GameObject obj, float delay, int excludeSiblingFrom, int excludeSiblingTo, bool activeChildrenOnly = true )
		{
			obj._DestroyChildren( delay, excludeSiblingFrom, excludeSiblingTo, activeChildrenOnly ).Start();
		}
		public static void DestroyChildren( this GameObject obj, float after, string[] exclude = null, bool activeChildrenOnly = true )
		{
			obj._DestroyChildren( after, exclude, activeChildrenOnly ).Start();
		}
		private static IEnumerator _DestroyChildren( this GameObject obj, float after, int excludeSiblingFrom, int excludeSiblingTo, bool activeChildrenOnly = true )
		{
			yield return new WaitForSeconds( after );
			obj.DestroyChildren( excludeSiblingFrom, excludeSiblingTo, activeChildrenOnly );
		}
		private static IEnumerator _DestroyChildren( this GameObject obj, float after, string[] exclude = null, bool activeChildrenOnly = true )
		{
			yield return new WaitForSeconds( after );
			obj.DestroyChildren( exclude, activeChildrenOnly );
		}		
		public static void DestroyChildrenImmediate( this GameObject obj, string[] exclude, bool activeChildrenOnly = true )
		{
            GameObject child = null;
			for( int  i=0; i<obj.ChildCount(); i++ )
			{
                child = obj.GetChild(i);
                if( exclude.Contains( child.name ) )
					continue;
                if( activeChildrenOnly && !child.activeInHierarchy )
					continue;
                child.DestroyImmediate();
			}
		}		
		/// <summary>
		/// Destroy the specified obj and after the specified seconds, if seconds are lower than 0 the object won't be destroyed.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="after">After.</param>
		public static void Destroy( this GameObject obj, float after = 0f )
		{
			if( obj == null || after < 0f )
				return;
			Behaviour.Destroy( obj, after );
		}		
		public static void DestroyImmediate( this GameObject obj )
		{
			if( obj )
				Behaviour.DestroyImmediate( obj );
		}		
		public static void DestroyChild( this GameObject obj, int index )
		{
			obj.GetChild( index ).Destroy();
		}		
		public static void DestroyChildImmediate( this GameObject obj, int index )
		{
			obj.GetChild( index ).DestroyImmediate();
		}
		/// <summary>
		/// Destroy every object inside the specified list
		/// </summary>
		public static void Destroy( this IList<GameObject> objs, float after = 0f )
		{
			if( objs == null )
				return;
			for( int i=0; i<objs.Count; i++ )
			{
				if( objs[i] )
				{
					objs[i].Destroy( after );
				}
			}
		}
		#endregion

		#region IS
		/// <summary>
		/// Checks if this is a prefab.
		/// </summary>
		/// <returns><c>true</c> if its prefab; otherwise, <c>false</c>.</returns>
		/// <param name="obj">Object.</param>
		public static bool IsPrefab( this GameObject obj )
		{
			if( !obj )
			{
				return false;
			}
			if( obj.IsActiveInHierarchy() )
			{
				return false;
			}
			
			var prefabs = obj.AddGetComponentIncludeChildren<IsPrefab>(true);
			if( prefabs[0] )
			{
                prefabs[0].CheckPrefab();
                if( prefabs[0] == null )
                    return false;
				for( int i=0; i<prefabs.Count; i++ )
				{
					if( !prefabs[i].idWasSet )
					{
						prefabs[i].id = DDK.Base.Components.IsPrefab.nextValidId++;
						prefabs[i].idWasSet = true;
					}
				}
				if( !prefabs[0].isInstance )
					return true;
			}
			return false;
		}
		/// <summary>
		/// Also checks for clones (instances). If obj is null, false is returned.
		/// </summary>
		/// <returns><c>true</c> if the specified obj is active in hierarchy; otherwise, <c>false</c>.</returns>
		/// <param name="obj">Object.</param>
		public static bool IsActiveInHierarchy( this GameObject obj )
		{
			bool active = false;
			if( !obj )
				return active;
			active = obj.activeInHierarchy;
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
		/// Determines if the specified object is 2D (has a sprite renderer).
		/// </summary>
		public static bool Is2D( this GameObject obj )
		{
            var sr = obj.GetComponentInChildren<SpriteRenderer>();
			return ( sr != null ) ? true : false;
		}
		/// <summary>
		/// Determines if the specified obj exceeds any screen bound (bounds are calculated in the Device.cs static class).
		/// </summary>
		/// <returns><c>true</c> if any bound is exceeded; otherwise, <c>false</c>.</returns>
		/// <param name="obj">Object.</param>
		public static bool IsAnyBoundExceeded( this GameObject obj )
		{
			if( obj.transform.position.y > Device.yBound )//up bound
			{
				return true;
			} 
			else if ( obj.transform.position.y < Device._yBound )//down bound
			{
				return true;
			} 	
			if( obj.transform.position.x > Device.xBound )//right bound
			{
				return true;
			} 
			else if ( obj.transform.position.x < Device._xBound )//left bound
			{
				return true;
			}
			return false;
		}
        public static bool IsInLayerMask(this GameObject obj, LayerMask mask)
        {
            return ((mask.value & (1 << obj.layer)) > 0);
        }
		public static bool IsAnyAncestorNamed( this GameObject obj, string name )
		{
			if( !obj )
				return false;
			if( obj.name.Equals( name ) )
				return true;
			else return obj.GetParent().IsAnyAncestorNamed( name );
		}
        #endregion

        #region ENABLE / DISABLE
        /// <summary>
        /// Enables / Disables the component (specified type) of the specified game object.
        /// </summary>
        /// <param name="objs">Object.</param>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        /// <param name="prefabsCompsIds">Prefabs component identifier. This allows to identify prefab components that
        /// are duplicated in the same prefab.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Enable<T>( this GameObject obj, bool enabled = true, int prefabsCompsId = 0 ) where T : Behaviour
		{
			obj.GetTrueComponent<T>().SetEnabled( enabled, prefabsCompsId );
		}
		/// <summary>
		/// Enables / Disables the components (specified type) of the specified game objects.
		/// </summary>
		/// <param name="objs">Objects.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		/// <param name="prefabsCompsIds">Prefabs components identifiers. This allows to identify prefab components that
		/// are duplicated in the same prefab.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void Enable<T>( this IList<GameObject> objs, bool enabled = true, IList<int> prefabsCompsIds = null ) where T : Behaviour
		{
			for( int i=0; i<objs.Count; i++ )
			{
				if( prefabsCompsIds != null )
				{
					if( prefabsCompsIds.Count > i )
					{
						objs[i].GetComponent<T>().SetEnabled( enabled, prefabsCompsIds[i] );
					}
				}
				else objs[i].GetComponent<T>().SetEnabled( enabled );
			}
		}
		public static void EnableAfter( this Behaviour[] comps, bool enabled = true, float after = 1f )
		{
			comps._EnableAfter( enabled, after ).Start();
		}
		
		public static void EnableAfter( this Renderer[] comps, bool enabled = true, float after = 1f )
		{
			comps._EnableAfter( enabled, after ).Start();
		}
		private static IEnumerator _EnableAfter( this Behaviour[] comps, bool enabled = true, float after = 1f )
		{
			yield return new WaitForSeconds( after );
			for( int i=0; i<comps.Length; i++ )
			{
				if( comps[i] != null )
				{
					comps[i].SetEnabled( enabled );
				}
			}
		}
		private static IEnumerator _EnableAfter( this Renderer[] comps, bool enabled = true, float after = 1f )
		{
			yield return new WaitForSeconds( after );
			for( int i=0; i<comps.Length; i++ )
			{
				if( comps[i] != null )
				{
					comps[i].SetEnabled( enabled );
				}
			}
		}
		/// <summary>
		/// Disables the collider.
		/// </summary>
		/// <returns>The collider.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="duration">Duration.</param>
		public static IEnumerator DisableCollider( this GameObject obj, float duration = 0f )
		{
			if( obj != null )
			{
				if( duration == 0 )
				{
					if( obj.Is2D() )
						obj.GetComponent<Collider2D>().enabled = false;
					else obj.GetComponent<Collider>().enabled = false;
				}
				else if( obj.Is2D() )
				{
					obj.GetComponent<Collider2D>().enabled = false;
					yield return new WaitForSeconds( duration );
					if( obj != null )
						obj.GetComponent<Collider2D>().enabled = true;
				}
				else
				{
					obj.GetComponent<Collider>().enabled = false;
					yield return new WaitForSeconds( duration );
					if( obj != null )
						obj.GetComponent<Collider>().enabled = true;
				}
			}
		}		
		/// <summary>
		/// Disables the collider. If duration = 0, it is just disabled.
		/// </summary>
		/// <returns>The collider.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="duration">Duration.</param>
		public static IEnumerator DisableFor( this GameObject obj, Behaviour component, float duration = 0f )
		{
			if( obj )
			{
				if( duration == 0 )
				{
					obj.GetBehaviour( component ).enabled = false;
				}
				else
				{
					obj.GetBehaviour(component).enabled = false;
					yield return new WaitForSeconds( duration );
					if( obj )
						obj.GetBehaviour(component).enabled = true;
				}
			}
		}
		public static IEnumerator EnableBehaviourAfter<T>( this GameObject obj, bool enable = true, float after = 0f ) where T : Behaviour
		{
			yield return new WaitForSeconds( after );
			obj.GetComponent<T>().enabled = enable;
		}
		public static IEnumerator EnableBehaviourAfter<T>( this GameObject obj, int compIndex, bool enable = true, float after = 0f ) where T : Behaviour
		{
			yield return new WaitForSeconds( after );
			var comps = obj.GetComponents<T>();
			comps[compIndex].enabled = enable;
		}
		#endregion

		#region COMPONENTS COPY
		/// <summary>
		/// Copies the specified component to every object in the array.
		/// </summary>
		/// <param name="objs">Objects.</param>
		/// <param name="comp">Component.</param>
		public static void CopyComponent( this GameObject[] objs, Behaviour comp, bool enabled = true )
		{
			//var t = comp.GetType();
			for( int i=0; i<objs.Length; i++ )
			{
				comp.CopyTo( objs[i], enabled );
				/*var c = objs[i].AddComponent(t);
				c = comp;*/
			}
		}
		/// <summary>
		/// Copies the transform from the specified to this object's.
		/// </summary>
		public static void CopyTransformFrom( this GameObject obj, Transform t )
		{
			if( !obj || !t )
				return;
			obj.transform.position = t.position;
			obj.transform.rotation = t.rotation;
			obj.transform.localScale = t.localScale;
		}
		/// <summary>
		/// Copies the transform from the specified to this object's.
		/// </summary>
		public static void CopyTransformFrom( this GameObject obj, GameObject obj2 )
		{
			if( !obj || !obj2 )
				return;
			obj.CopyTransformFrom( obj2.transform );
		}
		/// <summary>
		/// Copies the transform to the specified from this object's.
		/// </summary>
		public static void CopyTransformTo( this GameObject obj, GameObject obj2 )
		{
			if( !obj || !obj2 )
				return;
			obj2.CopyTransformFrom( obj );
		}
		/// <summary>
		/// Copies the transform to the specified from this object's.
		/// </summary>
		public static void CopyTransformTo( this GameObject obj, Transform t )
		{
			if( !obj || !t )
				return;
			obj.CopyTransformTo( t.gameObject );
		}
		#endregion

		#region MISC
		public static GameObject InstantiateInsideArea2D( this GameObject obj, Collider2D area, float z )
		{
			return (GameObject) GameObject.Instantiate( obj, area.GetRandomPointInCollider2D( z ), obj.transform.rotation );
		}
		public static GameObject InstantiateInsideArea( this GameObject obj, Collider area, float y )
		{
			return (GameObject) GameObject.Instantiate( obj, area.GetRandomPointInCollider( y ), obj.transform.rotation );
		}
		public static GameObject InstantiateInsideArea( this GameObject obj, Collider area )
		{
			return (GameObject) GameObject.Instantiate( obj, area.GetRandomPointInCollider(), obj.transform.rotation );
		}
        public static GameObject InstantiateOnTargetWithSameScale( this GameObject obj, Transform target, Transform parent )
        {
            if( !obj )
            {
                Debug.LogWarning ( "The specified obj is null" );
                return null;
            }
            if( !target )
            {
                Debug.LogWarning ( "The specified target Transform is null", obj );
                return null;
            }
            GameObject instance = (GameObject) GameObject.Instantiate<GameObject>( obj );
            instance.SetParent( parent );
            instance.transform.position = target.position;
            instance.transform.localScale = target.localScale;
            return instance;
        }
        public static GameObject InstantiateOnTargetWithSameScaleAndParent( this GameObject obj, Transform target )
        {
            return obj.InstantiateOnTargetWithSameScale( target, target.parent );
        }
        public static T InstantiateOnTargetWithSameScale<T>( this GameObject obj, Transform target, Transform parent ) where T : Component
        {
            if( !obj )
            {
                Debug.LogWarning ( "The specified obj is null" );
                return null;
            }
            if( !target )
            {
                Debug.LogWarning ( "The specified target Transform is null", obj );
                return null;
            }
            return obj.InstantiateOnTargetWithSameScale( target, parent ).GetComponentInChildren<T>();
        }
        public static T InstantiateOnTargetWithSameScaleAndParent<T>( this GameObject obj, Transform target ) where T : Component
        {
            return obj.InstantiateOnTargetWithSameScale<T>( target, target.parent );
        }
		/// <summary>
		/// Enables or disables the object's Animator.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		public static void ToggleAnimator( this GameObject obj )
		{
			if( !obj )
                return;
            var animator = obj.GetComponent<Animator>();
            if( animator != null )
            {
                animator.enabled = !animator.enabled;
            }
		}
		/// <summary>
		/// Find the specified obj in the list. Returns null if it is not found.
		/// </summary>
		/// <param name="objs">Objects.</param>
		/// <param name="name">Name.</param>
		public static GameObject Find( this IList<GameObject> objs, string name )
		{
			for( int i=0; i<objs.Count; i++ )
			{
				if( objs[i].name.Equals( name ) )
					return objs[i];
			}
			return null;
		}
		public static GameObject RemoveChild( this GameObject obj, int index, bool destroy = false )
		{
			var child = obj.GetChild(index);
			if( destroy )
			{
				child.Destroy();
			}
			else child.transform.parent = null;
			return child;
		}
		public static void ShowSprite( this GameObject spriteObj, bool show = true )
		{
			var sr = spriteObj.GetComponent<SpriteRenderer>();
			sr.color = new Color( sr.color.r, sr.color.g, sr.color.b, ( ( show ) ? 1f : 0f ) );
		}		
		public static IEnumerator ShowSpriteAfter( this GameObject spriteObj, float delay, bool show = true )
		{
			yield return new WaitForSeconds( delay );
			spriteObj.ShowSprite( show );
		}
		public static IEnumerator MakeColliderTrigger( this GameObject obj, float duration = 0f )
		{
			if( duration == 0 )
			{
				if( obj.Is2D() )
					obj.GetComponent<Collider2D>().isTrigger = true;
				else obj.GetComponent<Collider>().isTrigger = true;
			}
			else if( obj.Is2D() )
			{
				if( !obj.GetComponent<Collider2D>().isTrigger )
				{
					obj.GetComponent<Collider2D>().isTrigger = true;
					yield return new WaitForSeconds( duration );
					obj.GetComponent<Collider2D>().isTrigger = false;
				}
			}
			else
			{
				if( !obj.GetComponent<Collider>().isTrigger )
				{
					obj.GetComponent<Collider>().isTrigger = true;
					yield return new WaitForSeconds( duration );
					obj.GetComponent<Collider>().isTrigger = false;
				}
			}
		}
		public static float DistanceTo( this GameObject obj1, GameObject obj2 )
		{
			return Vector3.Distance( obj1.Position(), obj2.Position() );
		}
		public static void Unparent( this IList<GameObject> objs )
		{
			for( int i=0; i<objs.Count; i++ )
			{
				objs[i].transform.SetParent( null );
			}
		}
		public static int Length( this GameObject[] objs, bool countOnlyActives = true, bool inHierarchy = true )
		{
			int count = 0;
			for( int i=0; i<objs.Length; i++ )
			{
				if( inHierarchy )
				{
					if( objs[i].activeInHierarchy )
					{
						count++;
					}
				}
				else
				{
					if( objs[i].activeSelf )
					{
						count++;
					}
				}
			}
			return count;
		}
		public static List<GameObject> Instantiate( this GameObject[] objs, Transform parent = null, bool worldPosStays = false )
		{
			List<GameObject> instances = new List<GameObject>();
			for( int i=0; i<objs.Length; i++ )
			{
				instances.Add( (GameObject) GameObject.Instantiate( objs[i], objs[i].Position(), objs[i].transform.rotation ) );
				if( parent != null )
				{
					instances[i].SetParent( parent, worldPosStays );
				}
			}
			return instances;
		}
		/// <summary>
		/// Checks if this object or any ancestor contains the specified string in their names.
		/// </summary>
		/// <returns><c>true</c>, if this object or any ancestor contains the specified string in his name, <c>false</c> otherwise.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="name">Name.</param>
		public static bool AnyAncestorContains( this GameObject obj, string name )
        {
			if( !obj.name.Contains( name ) )
			{
				var parent = obj.GetParent();
				if( parent )
				{
					return parent.AnyAncestorContains( name );
				}
				return false;
			}
			return true;
		}
		public static void DeactivateLastActive( this IList<GameObject> objs )
		{
			for( int i = objs.Count-1; i >= 0; i-- )
			{
				if( objs[i].activeSelf )
				{
					objs[i].SetActive( false );
					break;
				}
			}
		}
		public static void ActivateFirstInactive( this IList<GameObject> objs )
		{
			for( int i=0; i<objs.Count; i++ )
			{
				if( !objs[i].activeSelf )
				{
					objs[i].SetActive( true );
					break;
				}
			}
		}
		public static int ChildCount( this GameObject obj, bool includeSubChildren = false )
		{
			if( !obj )
				return 0;
			
			if( includeSubChildren )
			{
				int count = obj.transform.childCount;
				for( int i=0; i < obj.transform.childCount; i++ )
				{
					count += obj.GetChild(i).ChildCount( true );
				}
				return count;
			}
			return obj.transform.childCount;
		}
		public static int ChildCountActiveOnly( this GameObject obj, bool includeSubChildren = false )
		{
			if( !obj )
				return 0;
			
			int count = 0;
			if( includeSubChildren )
			{
				for( int i=0; i < obj.transform.childCount; i++ )
				{
					count += obj.GetChild(i).ChildCountActiveOnly( true );
				}
				return count;
			}
			for( int i=0; i<obj.transform.childCount; i++ )
			{
				if( obj.GetChild( i ).IsActiveInHierarchy() )
				{
					count++;
				}
			}
			return count;
		}
		/// <summary>
		/// Calling this between frames may cause an overhead.
		/// </summary>
		public static bool IsFlagged( this GameObject obj, ValidationFlag.Flags flagType )
		{
			if( !obj )
				return false;
			var flag = obj.AddGetComponent<ValidationFlag>();
			return flag.IsFlagged( flagType );
		}
		public static bool IsFlagged( this ValidationFlag flag, ValidationFlag.Flags flagType )
		{
			if( !flag )
				return false;
			if( (flag.flags & flagType) == flagType )
				return true;
			return false;
		}
		#endregion

	}
}