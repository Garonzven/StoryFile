//By: Cesar Rosales
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.Shaders {
	
	public class ShaderChanger : MonoBehaviour {

        [Tooltip("The object that will be handled. If null, this object is used.")]
        public SearchableGameObject obj;
        public bool useSharedMaterial;
		//A materials to skip variable can be implemented here..

		protected Renderer[] _renderers;
		protected Material[][] _materials;
		protected Material[][] _originalMaterials;
		protected Shader[][] _originalShaders;
		protected bool _valid = true;

		public bool withChildren = true;

		void Start()
		{
			if( obj.m_gameObject.IsPrefab() )
			{
				_valid = false;
				Debug.LogWarning( "This object is a prefab..", gameObject );
				return;
			}
			if (withChildren)
				_renderers = obj.GetComponentsInChildren<Renderer> ();
			else
				_renderers = new Renderer[] { obj.GetComponent<Renderer> () };
			if( _renderers == null )
			{
				_valid = false;
				Debug.LogWarning( "This object has no Renderer", gameObject );
				return;
			}
			_materials = new Material[_renderers.Length][];
			_originalMaterials = new Material[_renderers.Length][];
			_originalShaders = new Shader[_renderers.Length][];
			for (int i = 0; i < _renderers.Length; i++)
			{
				if (useSharedMaterial)
					_originalMaterials [i] = _renderers [i].sharedMaterials;
				else
					_originalMaterials [i] = _renderers [i].materials;
				_originalShaders [i] = _originalMaterials [i].GetShaders ();
			}
			if( _originalMaterials == null || _originalMaterials.Length == 0 )
			{
				_valid = false;
				Debug.LogWarning ( "The object has no materials", gameObject );
				return;
			}
		}

		/// <summary>
		/// Applies the specified shader to all materials.
		/// </summary>
		public void SetShader( Shader target )
		{
			if( !_valid || !target )
				return;
			for (int i = 0; i < _renderers.Length; i++)
			{
				if (useSharedMaterial)
					_renderers [i].sharedMaterials.SetShader (target);
				else 
					_renderers [i].materials.SetShader (target);
			}
		}
		/// <summary>
		/// Applies the original shaders to all materials.
		/// </summary>
		public void ResetShaders()
		{
			if( !_valid || _originalMaterials == null )
				return;
			for (int i = 0; i < _renderers.Length; i++)
			{
				if (useSharedMaterial)
					_renderers [i].sharedMaterials.SetShaders (_originalShaders [i]);
				else
					_renderers [i].materials.SetShaders (_originalShaders [i]);
			}
		}

		/// <summary>
		/// Applies the specified material to object.
		/// </summary>
		public void SetMaterial ( Material target )
		{
			if( !_valid || !target )
				return;
			for (int i = 0; i < _renderers.Length; i++)
			{
				if (useSharedMaterial)
					_renderers [i].sharedMaterials = new Material[] { target };
				else
					_renderers [i].materials = new Material[] { target };
			}
		}

		/// <summary>
		/// Applies the original materials to object.
		/// </summary>
		public void ResetMaterials ()
		{
			if( !_valid || _originalMaterials == null )
				return;
			for (int i = 0; i < _renderers.Length; i++)
			{
				if (useSharedMaterial)
					_renderers [i].sharedMaterials = _originalMaterials[i];
				else
					_renderers [i].materials = _originalMaterials[i];
			}
		}
	}
}