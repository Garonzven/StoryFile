//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK;
using DDK.Base.Extensions;


namespace DDK.Base.Classes
{
    /// <summary>
    /// This class represents a GameObject, and it can be used to reference a GameObject without losing its reference since
    /// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
    /// </summary>
	[System.Serializable]
	public class SearchableGameObject 
	{		
		[Tooltip("If /obj/ is null, an object with this name will be searched and set as the spawn's parent")]
		[ShowIf( "_IsTargetInvalid" )]
		public string objName;
		[Tooltip("If true, the /name/ will be used as a Tag when searching")]
		[ShowIf( "_IsTargetInvalid", 1 )]
		public bool isTag;
		[SerializeField]
		private GameObject _obj;
		
		
		public SearchableGameObject(){}
		public SearchableGameObject( string objName )
		{
			if( !string.IsNullOrEmpty( objName ) )
				this.objName = objName;
		}
        public SearchableGameObject( GameObject obj )
        {
            if( obj )
                this.m_gameObject = obj;
        }
		
		
		/// <summary>
		/// The object, if not referenced, then an object with the specified name/tag will be searched and used/set/returned, which might also be null.
		/// </summary>
		public GameObject m_gameObject {
			get{
				if( !_obj )
					_obj = ( isTag ) ? objName.FindWithTag<GameObject>() : objName.Find();
				return _obj;
			}
			set{
				if( value )
				{
					objName = value.name;
					_obj = value;
				}
			}
		}
		/// <summary>
		/// The object's transform, if not referenced, then an object with the specified name/tag will be searched and used/set/returned, which might also be null.
		/// </summary>
		public Transform m_transform {
			get{
				if( !m_gameObject )
					return null;
				return m_gameObject.transform;
			}
		}
		
		
		/// <summary>
		/// Validation function.
		/// </summary>
		protected bool _IsTargetInvalid()
		{
			bool invalid = _obj == null;
			if( m_gameObject )//Keep the object's name updated in case it changes
			{
				objName = m_gameObject.name;
			}
			return invalid;
		}
		
		
		
		public T GetComponent<T>()
		{
			if( !m_gameObject )
			{
				Debug.LogWarning ("GetComponent<T>(): The specified Searchable GameObject is null.. Returning Default Type");
				return default(T);
			}
			return m_gameObject.GetComponent<T>();
		}
		public T GetComponentInChildren<T>()
		{
			if( !m_gameObject )
			{
				Debug.LogWarning ("GetComponentInChildren<T>(): The specified Searchable GameObject is null.. Returning Default Type");
				return default(T);
			}
			return m_gameObject.GetComponentInChildren<T>();
		}
		public T[] GetComponentsInChildren<T>()
		{
			if( !m_gameObject )
			{
				Debug.LogWarning ("GetComponentInChildren<T>(): The specified Searchable GameObject is null.. Returning Default Type");
				return default(T[]);
			}
			return m_gameObject.GetComponentsInChildren<T>();
		}
		/// <summary>
		/// Gets the components ONLY in children, excluding this object.
		/// </summary>
		public T[] GetComponentsInChildrenExcludeThis<T>( bool includeSubChildren = true, bool includeInactive = false ) where T : Component
		{
			if( !m_gameObject )
			{
				Debug.LogWarning ("GetComponentInChildren<T>(): The specified Searchable GameObject is null.. Returning Default Type");
				return default(T[]);
			}
			return m_gameObject.GetCompsInChildren<T>( includeSubChildren, includeInactive ).ToArray();
		}
	}

}