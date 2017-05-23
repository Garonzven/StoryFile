//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK;
using DDK.Base.Extensions;


namespace DDK.Base.Classes
{
	/// <summary>
	/// This class represents a Component, and it can be used to reference a Component without losing its reference since
	/// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
	/// </summary>
	[System.Serializable]
	public class SearchableComponent<T> where T : Component 
	{		
		[Tooltip("If /obj/ is null, an object with this name will be searched and set as the spawn's parent")]
		[ShowIf( "_IsTargetInvalid" )]
		public string objName = "";
		[Tooltip("If true, the /name/ will be used as a Tag when searching")]
		[ShowIf( "_IsTargetInvalid", 1 )]
		public bool isTag;
		[SerializeField]
		protected T _obj;
		
		
		public SearchableComponent(){}
		public SearchableComponent( string objName )
		{
			if( !string.IsNullOrEmpty( objName ) )
				this.objName = objName;
		}
		
		
		/// <summary>
		/// The object, if not referenced, then an object with the specified name/tag will be searched and used/set/returned, which might also be null.
		/// </summary>
		public T m_object {
			get{
				if( !_obj )
					_obj = ( isTag ) ? objName.FindWithTag<T>() : objName.Find<T>();
				return _obj;
			}
			set{
				if( value )
				{
					_obj = value;
					objName = _obj.name;
				}
			}
		}
		/// <summary>
		/// The object, if not referenced, then an object with the specified name/tag will be searched and used/set/returned, which might also be null.
		/// </summary>
		public GameObject m_gameObject {
			get{
				if( !m_object )
				{
					var obj = objName.Find();
					if( obj )
					{
						_obj = obj.GetCompInChildren<T>();
					}
					if( !_obj )
						return null;
				}
				return m_object.gameObject;
			}
		}
		/// <summary>
		/// The object, if not referenced, then an object with the specified name/tag will be searched and used/set/returned, which might also be null.
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
			if( m_object )//Keep the object's name updated in case it changes
			{
				objName = m_gameObject.name;
			}
			return invalid;
		}
		
		
		
		public T GetComponent()
		{
			if( !m_object )
			{
				Debug.LogWarning ("GetComponent<T>(): The specified Searchable Behaviour is null.. Returning Default Type");
				return default(T);
			}
			return m_object.GetComponent<T>();
		}
		public T GetComponentInChildren()
		{
			if( !m_object )
			{
				Debug.LogWarning ("GetComponentInChildren<T>(): The specified Searchable Behaviour is null.. Returning Default Type");
				return default(T);
			}
			return m_object.GetComponentInChildren<T>();
		}
		public T[] GetComponentsInChildren()
		{
			if( !m_object )
			{
				Debug.LogWarning ("GetComponentInChildren<T>(): The specified Searchable Behaviour is null.. Returning Default Type");
				return default(T[]);
			}
			return m_object.GetComponentsInChildren<T>();
		}
		/// <summary>
		/// Gets the components ONLY in children, excluding this object.
		/// </summary>
		public TComponent[] GetComponentsInChildrenExcludeThis<TComponent>( bool includeSubChildren = true, bool includeInactive = false ) where TComponent : Component
		{
			if( !m_gameObject )
			{
				Debug.LogWarning ("GetComponentInChildren<T>(): The specified Searchable GameObject is null.. Returning Default Type");
				return default(TComponent[]);
			}
			return m_gameObject.GetCompsInChildren<TComponent>( includeSubChildren, includeInactive ).ToArray();
		}
	}
	
}
