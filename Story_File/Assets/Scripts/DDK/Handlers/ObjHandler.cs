//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using DDK.Base.Fx.Transitions;
using DDK.Base.ScriptableObjects;
using DDK.Base.Statics;


namespace DDK.Handlers {

	public class ObjHandler : MonoBehaviourExt {

		[HelpBoxAttribute]
		public string msg = "If disabled, the actions won't be executed";
		public SearchableGameObject obj = new SearchableGameObject();

		[Header("Children To Exclude")]
		[Tooltip("If DestroyChildren() is called, the children named as the specified will be excluded")]
		public string[] excludeNamed;
		[Tooltip("If DestroyChildren() is called, the children between /From/ and /To/ sibling indexes will be excluded. If both have the same value non are excluded")]
		[ShowIfAttribute( "IsExcludeNamedValid", 1 )]
		public int excludeSiblingsFrom;
		[Tooltip("If DestroyChildren() is called, the children between /From/ and /To/ sibling indexes will be excluded. If both have the same value non are excluded")]
		[ShowIfAttribute( "IsExcludeNamedValid", 1 )]
		public int excludeSiblingsTo;


		// Use this for initialization
		void Start () {
			
		}


		#region VALIDATIONS
		protected bool IsObjValid()
		{
			return obj.m_gameObject || !string.IsNullOrEmpty( obj.objName );
		}
		protected bool IsExcludeNamedValid()
		{
			return excludeNamed != null && excludeNamed.Length > 0 && ( obj.m_gameObject || !string.IsNullOrEmpty( obj.objName ) );
		}
		#endregion

		#region CHILDREN HANDLING
		/// <summary>
		/// Destroys the children of the specified object.
		/// </summary>
		public void DestroyChildrenAfter( float delay )
		{
			if( !enabled )
				return;
			DestroyChildrenAfter( obj.m_gameObject, delay );
		}
		/// <summary>
		/// Destroys the children of the specified object.
		/// </summary>
		public void DestroyChildren()
		{
			if( !enabled )
				return;
			DestroyChildren( obj.m_gameObject );
		}
		/// <summary>
		/// Destroys the children of the specified object.
		/// </summary>
		public void DestroyChildren( GameObject obj )
		{
			if( !enabled )
				return;
			if( excludeNamed.Length > 0 )
			{
				DestroyChildren( obj, excludeNamed );
			}
			else
			{
				obj.DestroyChildren( excludeSiblingsFrom, excludeSiblingsTo );
			}
		}
		/// <summary>
		/// Destroys the children of the specified object.
		/// </summary>
		/// <param name="exclude"> the name of the children to exclude </param>
		public void DestroyChildrenAfter( GameObject obj, float delay )
		{
			if( !enabled )
				return;
			if( excludeNamed.Length > 0 )
			{
				obj.DestroyChildren( delay, excludeNamed );
			}
			else
			{
				obj.DestroyChildren( delay, excludeSiblingsFrom, excludeSiblingsTo );
			}
		}
		/// <summary>
		/// Destroys the children of the specified object.
		/// </summary>
		/// <param name="exclude"> the name of the children to exclude </param>
		public void DestroyChildren( GameObject obj, string[] exclude )
		{
			if( !enabled )
				return;
			if( excludeNamed.Length > 0 )
			{
				obj.DestroyChildren( exclude );
			}
			else
			{
				obj.DestroyChildren( excludeSiblingsFrom, excludeSiblingsTo );
			}
		}
		/*public void SaveChildrenIntoPlayerPrefs( GameObject obj )
		{
			if( !obj )
				return;
			GameObject[] children = obj.GetChildren();
			Persistent.Save<GameObject[]>( obj.name, children, true );
		}
		public void LoadReplaceChildrenInPlayerPrefs( GameObject obj )
		{
			if( !obj )
				return;
			GameObject[] children = GetChildrenInPlayerPrefs();
			if( children == null || children.Length == 0 )
			{
				return;
			}
			GameObject[] _children = obj.GetChildren();
			_children.Destroy();
			for( int i=0; i<children.Length; i++ )
			{
				children[i].SetParent( obj.transform, false );
			}
		}
		public void LoadChildrenInPlayerPrefs( GameObject obj )
		{
			if( !obj )
				return;
			var children = Persistent.Load<GameObject[]>( obj.name );
			for( int i=0; i<children.Length; i++ )
			{
				children[i].SetParent( obj.transform, false );
			}
		}
		public GameObject[] GetChildrenInPlayerPrefs( GameObject obj )
		{
			if( !obj )
				return null;
			return Persistent.Load<GameObject[]>( obj.name );
		}
		public void SaveChildrenIntoPlayerPrefs()
		{
			SaveChildrenIntoPlayerPrefs( obj.m_gameObject );
		}
		public void LoadReplaceChildrenInPlayerPrefs()
		{
			LoadReplaceChildrenInPlayerPrefs( obj.m_gameObject );
		}
		public void LoadChildrenInPlayerPrefs()
		{
			LoadChildrenInPlayerPrefs( obj.m_gameObject );
		}
		public GameObject[] GetChildrenInPlayerPrefs()
		{
			return GetChildrenInPlayerPrefs( obj.m_gameObject );
		}*/
		#endregion

		#region STATES
		/// <summary>
		/// Enables the specified behaviour in the referenced /obj/
		/// </summary>
		public void EnableBehaviour( int index )
		{
			EnableBehaviour( index, true );
		}
		/// <summary>
		/// Enables the specified behaviour in the referenced /obj/
		/// </summary>
		public void DisableBehaviour( int index )
		{
			EnableBehaviour( index, false );
		}
		/// <summary>
		/// Enables all the behaviours in the referenced /obj/.
		/// </summary>
		public void EnableAllBehaviours( bool enable )
		{
			if( !obj.m_gameObject )
				return;
			Behaviour[] behaviours = obj.m_gameObject.GetComponents<Behaviour>();
			for( int i=0; i<behaviours.Length; i++ )
			{
				behaviours[ i ].enabled = enable;
			}
		}

		/// <summary>
		/// Enables/Disabled the specified behaviour in the referenced /obj/
		/// </summary>
		public void EnableBehaviour( int index, bool enabled )
		{
			if( !obj.m_gameObject )
				return;
			Behaviour[] behaviours = obj.m_gameObject.GetComponents<Behaviour>();
			for( int i=0; i<behaviours.Length; i++ )
			{
				if( index != i )
					continue;
				behaviours[ i ].enabled = enabled;
			}
		}
		public void ActivateHandledObj( bool activate, float delay )
		{
			obj.m_gameObject.SetActiveInHierarchy( activate, true, delay );
		}
		public void ActivateHandledObj( bool activate, float delay, bool destroyIfClone, bool immediately = false )
		{
			obj.m_gameObject.SetActiveInHierarchy( activate, destroyIfClone, delay, immediately );
		}
		public void ActivateHandledObj( float delay )
		{
			ActivateHandledObj( true, delay );
		}
		public void DeactivateHandledObj( float delay )
		{
			ActivateHandledObj( false, delay );
		}
		public void DestroyHandledObj( float delay )
		{
			if( delay < 0 )
				DestroyImmediate( obj.m_gameObject );
			Destroy( obj.m_gameObject, delay );
		}
		#endregion

		#region MISC
		public void SetTransform( Transform target )
		{
			SetTransform( obj, target );
		}
		#endregion
	}

}
