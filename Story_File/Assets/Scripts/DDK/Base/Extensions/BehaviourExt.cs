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
    /// Behaviour class extension.
    /// </summary>
	public static class BehaviourExt 
    {		
		#region GET / ADD
		public static GameObject GetChild( this Behaviour obj, string name )
		{
			return obj.transform.Find( name ).gameObject;
		}		
		public static GameObject GetChild( this Behaviour obj, int index )
		{
			return obj.gameObject.GetChild( index );
		}
		public static GameObject GetNextSibling( this Behaviour obj, bool mustBeActive = false )
		{
			return obj.gameObject.GetNextSibling( mustBeActive );
		}
		public static List<GameObject> GetSiblings( this Behaviour obj, bool includeSelf = true, bool includeInactive = false )
		{
            return obj.gameObject.GetSiblings( includeSelf, includeInactive );
		}
		/// <summary>
		/// Gets the children. If obj is null, null is returned.
		/// </summary>
		public static GameObject[] GetChildren( this Behaviour obj, bool includeSubChildren = false, 
		                                       bool includeInactive = false )
		{
			return obj.gameObject.GetChildren( includeSubChildren, includeInactive );
		}		
		/// <summary>
		/// Gets the children excluding the ones containing the specified strings in their name.
		/// </summary>
		/// <returns>The children.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="excludeWhichContain">Name exclutions.</param>
		public static List<GameObject> GetChildren( this Behaviour obj, string[] excludeWhichContain )
		{
			return obj.gameObject.GetChildren( excludeWhichContain );
		}		
		/// <summary>
		/// Gets the children containing the specified substrings in their name.
		/// </summary>
		/// <returns>The children.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="excludeWhichContain">Name exclutions.</param>
		public static List<GameObject> GetChildrenContaining( this Behaviour obj, string[] containingSubstring )
		{
			return obj.gameObject.GetChildrenContaining( containingSubstring );
		}		
		/// <summary>
		/// Gets the children excluding the specified indexes.
		/// </summary>
		/// <returns>The children.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="excludeIndex">Exclution list of indexes.</param>
		public static GameObject[] GetChildren( this Behaviour obj, int[] excludeIndex )
		{
			return obj.GetChildren( excludeIndex );
		}
		public static GameObject GetFirstParent( this Behaviour obj )
		{
			return obj.gameObject.GetFirstParent();
		}
		public static GameObject GetFirstAncestorOfType<T>( this Behaviour obj ) where T : Component
		{
			return obj.gameObject.GetFirstAncestorOfType<T>();
		}
		public static T GetFirstComponentOfType<T>( this Behaviour obj ) where T : Component
		{
			return obj.GetFirstAncestorOfType<T>().GetComponent<T>();
		}
		/// <summary>
		/// Gets the main texture from every material.
		/// </summary>
		/// <returns>The textures.</returns>
		/// <param name="obj">Object.</param>
		public static Texture[] GetTextures( this Behaviour obj )
		{
			return obj.gameObject.GetTextures();
		}
		/// <summary>
		/// Gets or add a component. Usage example:
		/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
		/// </summary>
		public static T GetOrAddBehaviour<T> (this Behaviour child) where T: Behaviour {
			T result = child.GetComponent<T>();
			if (result == null) {
				result = child.gameObject.AddComponent<T>();
			}
			return result;
		}
		
		/// <summary>
		/// Gets or add a component. Usage example:
		/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
		/// </summary>
		public static T GetOrAddComponent<T> (this Component child) where T: Component {
			T result = child.GetComponent<T>();
			if (result == null) {
				result = child.gameObject.AddComponent<T>();
			}
			return result;
		}
		
		/// <summary>
		/// Gets or add a component. Usage example:
		/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
		/// </summary>
		public static T GetOrAddComponent<T> ( this Behaviour child ) where T: Component 
        {
			T result = child.GetComponent<T>();
			if (result == null) {
				result = child.gameObject.AddComponent<T>();
			}
			return result;
		}
		
		/// <summary>
		/// Gets or add a component. Usage example:
		/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
		/// </summary>
		public static T GetOrAddBehaviour<T> ( this Component child ) where T: Behaviour 
        {
			T result = child.GetComponent<T>();
			if (result == null) {
				result = child.gameObject.AddComponent<T>();
			}
			return result;
		}
		#endregion GET / ADD

		#region IS
		/// <summary>
		/// Returns true if the referenced behaviour is in a prefab.
		/// </summary>
		/// <returns><c>true</c> if the behaviour is in prefab; otherwise, <c>false</c>.</returns>
		/// <param name="comp">component to check.</param>
		public static bool IsPrefab( this Behaviour comp )
		{
			if( !comp )
				return false;
			return comp.gameObject.IsPrefab();
		}
		
		/// <summary>
		/// Returns true if the referenced component is in a prefab.
		/// </summary>
		/// <returns><c>true</c> if the component is in prefab; otherwise, <c>false</c>.</returns>
		/// <param name="comp">component.</param>
		public static bool IsPrefab(this Component comp)
		{
			if( !comp )
				return false;
			return comp.gameObject.IsPrefab();
		}
		/// <summary>
		/// Checks if a level is available (added to Build Settings and enabled). This returns true if not in the Editor.
		/// </summary>
		/// <param name="logMsgs">If set to <c>true</c> a message will be displayed if the scene hasn't been added
		/// to the Build Settings, or if it's disabled.</param>
		/// <param name="context"> The context object for the logged messages </parma>
		public static bool IsLevelAvailable( this Behaviour behaviour, string levelName, bool logMsgs = false )
		{
			return Utilities.IsLevelAvailable( levelName, logMsgs, behaviour.gameObject );
		}
		/// <summary>
		/// Returns true if this Behaviour is null; otherwise, false.
		/// </summary>
		/// <param name="warningMsg">Warning message to show if the behaviour is null.</param>
		public static bool IsNull( this Behaviour behaviour, string warningMsg = "The specified Behaviour is null", Object context = null )
		{
			if( !behaviour )
			{
				Debug.LogWarning ( warningMsg, context );
				return true;
			}
			return false;
		}
		#endregion IS

		#region GET
		/// <summary>
		/// Gets the component, but it also checks if it references a prefab, if so, it returns the component from the 
		/// prefab instance.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="comp">Component.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetTrueComponent<T>( this Component comp ) where T : Component
		{
			if( !comp )
			{
				return null;
			}
			if( comp.gameObject.IsPrefab() )
			{
				return comp.gameObject.GetPrefabInstance().GetComponent<T>();
			}
			else return comp.GetComponent<T>();
		}
		
		/// <summary>
		/// Gets the Behaviour, but it also checks if it references a prefab, if so, it returns the Behaviour from the 
		/// prefab instance.
		/// </summary>
		/// <returns>The Behaviour.</returns>
		/// <param name="comp">Behaviour.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetTrueBehaviour<T>( this Behaviour comp ) where T : Behaviour
		{
			if( !comp )
			{
				return null;
			}
			if( comp.gameObject.IsPrefab() )
			{
				return comp.gameObject.GetPrefabInstance().GetComponent<T>();
			}
			else return comp.GetComponent<T>();
		}
		
		public static GameObject GetParent( this Behaviour comp )
		{
			if( comp )
			{
				return comp.transform.parent.gameObject;
			}
			else return null;
		}
		public static Rigidbody GetRigidbody( this Behaviour behaviour )
		{
			return behaviour.GetComponent<Rigidbody>();
		}
		/// <summary>
		/// Gets the components in children.
		/// </summary>
		/// <param name="includeInactive">If set to <c>true</c> include inactive children.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <typeparam name="TExclude">If the child has a component of this type, it will be excluded.</typeparam>
		public static T[] GetComponentsInChildren<T, TExclude>( this Behaviour comp, bool includeInactive ) where T : Component where TExclude : Component
		{
			List<T> comps = comp.GetComponentsInChildren<T>().ToList();
			for( int i=0; i<comps.Count; i++ )
			{
				if( comps[i].GetComponent<TExclude>() )
				{
					comps.Remove( comps[i] );
					i--;
				}
			}
			return comps.ToArray();
		}
		#endregion GET

		#region SET
		/// <summary>
		/// Sets the name of the parent. It also checks if the referenced component's parent is a prefab, if so it changes 
		/// the prefab's instance parent name.
		/// </summary>
		/// <param name="comp">Component.</param>
		/// <param name="name">New parent name.</param>
		public static void SetParentName( this Behaviour comp, string name )
		{
			if( !comp )
				return;
			var parent = comp.GetParent();
			if( parent )
			{
				if( parent.IsPrefab() )
				{
					parent = parent.GetPrefabInstance();
				}
				parent.name = name;
			}
			else Debug.Log ("There is no parent to rename");
		}
		
		public static void SetEnabled( this IList<Behaviour> comps, bool enabled = true, int prefabCompId = 0 )
		{
			if( comps == null )
				return;
			for( int i=0; i<comps.Count; i++ )
			{
				comps[i].SetEnabled( enabled, prefabCompId );
			}
		}
		/// <summary>
		/// Enables/Disables this behaviour.
		/// </summary>
		/// <param name="exclude">The name of the objects to exclude.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		/// <param name="prefabCompId">Prefab component identifier.</param>
		public static void SetEnabledExclude( this IList<Behaviour> comps, IList<string> exclude, bool enabled = true, int prefabCompId = 0 )
		{
			if( comps == null )
				return;
			for( int i=0; i<comps.Count; i++ )
			{
				if( exclude.Contains( comps[i].name ) )
					continue;
				comps[i].SetEnabled( enabled, prefabCompId );
			}
		}
		
		/// <summary>
		/// Enables or disables the specified component, if it references a prefab the prefab instance will be searched;
		/// make sure the instance has a unique name if multiple instances are active.
		/// </summary>
		/// <param name="comp">Comp.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		/// <param name="prefabCompId">Prefab comp identifier.</param>
		public static void SetEnabled( this Behaviour comp, bool enabled = true, int prefabCompId = 0 )
		{
			if( !comp )
				return;
			if( comp.IsPrefab() )
			{
				var instance = comp.name.Find();
				if( instance )
				{
					var cmps = instance.GetComponents( comp.GetType() );
					if( cmps != null )
					{
						for( int j=0; j<cmps.Length; j++ )
						{
							if( j == prefabCompId )
							{
                                var c = (Behaviour) cmps[j];
								c.enabled = enabled;
							}
						}
					}
				}
			}
			else if( comp ) 
            {
				comp.enabled = enabled;
			}
		}
		
		/// <summary>
		/// Enables or disables the specified component, if it references a prefab the prefab instance will be searched;
		/// make sure the instance has a unique name if multiple instances are active.
		/// </summary>
		/// <param name="comp">Comp.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		/// <param name="prefabCompId">Prefab comp identifier.</param>
		public static void SetEnabledAfter( this Behaviour comp, bool enabled = true, float after = 1f, int prefabCompId = 0 )
		{
			comp._SetEnabledAfter( enabled, after, prefabCompId ).Start();
		}
		#endregion SET

		#region DESTROY
		public static void DestroyChildren( this Behaviour obj, int excludeSiblingFrom, int excludeSiblingTo, bool activeChildrenOnly = true )
		{
			obj.gameObject.DestroyChildren( excludeSiblingFrom, excludeSiblingTo, activeChildrenOnly );
		}		
		public static void DestroyChildren( this Behaviour obj, string[] exclude = null, bool activeChildrenOnly = true )
		{
			obj.gameObject.DestroyChildren( exclude, activeChildrenOnly );
		}
		public static void DestroyChildren( this Behaviour obj, float delay, int excludeSiblingFrom, int excludeSiblingTo, bool activeChildrenOnly = true )
		{
			obj._DestroyChildren( delay, excludeSiblingFrom, excludeSiblingTo, activeChildrenOnly ).Start();
		}
		public static void DestroyChildren( this Behaviour obj, float after, string[] exclude = null, bool activeChildrenOnly = true )
		{
			obj._DestroyChildren( after, exclude, activeChildrenOnly ).Start();
		}
		private static IEnumerator _DestroyChildren( this Behaviour obj, float after, int excludeSiblingFrom, int excludeSiblingTo, bool activeChildrenOnly = true )
		{
			yield return new WaitForSeconds( after );
			obj.DestroyChildren( excludeSiblingFrom, excludeSiblingTo, activeChildrenOnly );
		}
		private static IEnumerator _DestroyChildren( this Behaviour obj, float after, string[] exclude = null, bool activeChildrenOnly = true )
		{
			yield return new WaitForSeconds( after );
			obj.DestroyChildren( exclude, activeChildrenOnly );
		}		
		public static void DestroyChildrenImmediate( this Behaviour obj, string[] exclude, bool activeChildrenOnly = true )
		{
			obj.gameObject.DestroyChildrenImmediate( exclude, activeChildrenOnly );
		}		
		/// <summary>
		/// Destroy the specified behaviour and after the specified seconds, if seconds are lower than 0 the object won't be destroyed.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="after">After.</param>
		public static void Destroy( this Behaviour obj, float after = 0f )
		{
			MonoBehaviour.Destroy( obj, after );
		}		
		public static void DestroyImmediate( this Behaviour obj )
		{
			MonoBehaviour.DestroyImmediate( obj );
		}		
		public static void DestroyChild( this Behaviour obj, int index )
		{
			obj.GetChild( index ).Destroy();
		}		
		public static void DestroyChildImmediate( this Behaviour obj, int index )
		{
			obj.GetChild( index ).DestroyImmediate();
		}
		public static void Destroy( this IList<Behaviour> objs, float after = 0f )
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

		#region MISC
		/// <summary>
		/// Enables or disables the specified component, if it references a prefab the prefab instance will be searched;
		/// make sure the instance has a unique name if multiple instances are active.
		/// </summary>
		/// <param name="comp">Comp.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		/// <param name="prefabCompId">Prefab comp identifier.</param>
		public static void DisableFor( this Behaviour comp, float duration = 1f, int prefabCompId = 0 )
		{
            comp._DisableFor( duration, prefabCompId ).Run();
		}
		/// <summary>
		/// Starts the ienumerable as an ienumerator (coroutine) in the specified -comp- (script).
		/// </summary>
		/// <returns>The coroutine.</returns>
		/// <param name="comp">Component (script).</param>
		/// <param name="ienum">Ienumerable.</param>
		public static Coroutine StartCoroutine( this MonoBehaviour comp, System.Collections.IEnumerable ienum )
		{
			return comp.StartCoroutine( ienum.GetEnumerator() );
		}
		public static void EnableColliders( this Behaviour This,  bool enable, string[] excludeObjs = null )
		{
			var colls = GameObject.FindObjectsOfType<Collider2D>();
			for( int i=0; i<colls.Length; i++ )
			{
				if( excludeObjs != null ) {
					if( excludeObjs.Contains<string>( colls[i].name ) )
						continue;
				}
				colls[i].SetEnabled( enable );
			}
		}
        /// <summary>
        /// Enables / Disables the component (specified type) of the specified object.
        /// </summary>
        /// <param name="objs">Object.</param>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        /// <param name="prefabsCompsIds">Prefabs component identifier. This allows to identify prefab components that
        /// are duplicated in the same prefab.</param>
        /// <typeparam name="T">The 1st type parameter. The type of the behaviour that will be /enabled/ </typeparam>
        /// <typeparam name="T2">The 2nd type parameter. The type of this object </typeparam>
        public static void Enable<T, T2>( this T2 obj, bool enabled = true, int prefabsCompsId = 0 ) where T : Behaviour where T2 : Behaviour
        {
            if( obj )
                return;
            obj.GetTrueComponent<T>().SetEnabled( enabled, prefabsCompsId );
        }
        /// <summary>
        /// Enables / Disables the components (specified type) of the specified objects.
        /// </summary>
        /// <param name="objs">Objects.</param>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        /// <param name="prefabsCompsIds">Prefabs components identifiers. This allows to identify prefab components that
        /// are duplicated in the same prefab.</param>
        /// <typeparam name="T">The 1st type parameter. The type of the behaviour that will be /enabled/ </typeparam>
        public static void Enable<T>( this IList<Behaviour> objs, bool enabled = true, IList<int> prefabsCompsIds = null ) where T : Behaviour
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
        /// <summary>
        /// Enables / Disables the components (specified type) of the specified objects.
        /// </summary>
        /// <param name="objs">Objects.</param>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        /// <param name="prefabsCompsIds">Prefabs components identifiers. This allows to identify prefab components that
        /// are duplicated in the same prefab.</param>
        /// <typeparam name="T">The 1st type parameter. The type of the behaviour that will be /enabled/ </typeparam>
        public static void EnableAfter<T>( this IList<Behaviour> objs, bool enabled, float delay, IList<int> prefabsCompsIds = null ) where T : Behaviour
        {
            for( int i=0; i<objs.Count; i++ )
            {
                if( prefabsCompsIds != null )
                {
                    if( prefabsCompsIds.Count > i )
                    {
                        objs[i].GetComponent<T>().SetEnabledAfter( enabled, delay, prefabsCompsIds[i] );
                    }
                }
                else objs[i].GetComponent<T>().SetEnabledAfter( enabled, delay );
            }
        }
		#endregion

		#region PRIVATE
		private static IEnumerator _SetEnabledAfter( this Behaviour comp, bool enabled = true, float after = 1f, int prefabCompId = 0 )
		{
			yield return new WaitForSeconds( after );
			comp.SetEnabled( enabled, prefabCompId );
		}
        private static IEnumerator<float> _DisableFor( this Behaviour comp, float duration = 1f, int prefabCompId = 0 )
		{
			comp.SetEnabled( false, prefabCompId );
			yield return Timing.WaitForSeconds( duration );
			comp.SetEnabled( true, prefabCompId );
		}
		public static int ChildCount( this Behaviour obj, bool includeSubChildren = false )
		{
			return obj.gameObject.ChildCount( includeSubChildren );
		}
		public static int ChildCountActiveOnly( this Behaviour obj, bool includeSubChildren = false )
		{
			return obj.gameObject.ChildCountActiveOnly( includeSubChildren );
		}
		#endregion PRIVATE

	}

}
