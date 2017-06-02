//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using System.Collections.Generic;
using MovementEffects;


namespace DDK.Handlers
{
    /// <summary>
    /// Allows calling multiple Renderer related functions.
    /// </summary>
	public class RendererHandler : MonoBehaviour 
    {
		[System.Serializable]
		public class ColorAnimation
		{
			public Color target = Color.green;
			public float duration = 1f;
			public bool loop;
		}


		[SerializeField]
		[ShowIfAttribute( "_UpdateRendererReference" )] //JUST TO UPDATE THE REFERENCE IF MISSING, without making this class ExecuteInEditMode
		private bool checkRendererRef;

		[HelpBoxAttribute]
		public string msg = "If disabled, no actions will be invoked";
        [DisplayNameAttribute( "Renderer" )]
		public SearchableRenderer sRenderer;
		[Tooltip("This is used to split the arguments of some functions")]
		public char argsSeparator = ':';
		public float animsDuration = 1f;


		protected bool _UpdateRendererReference()
		{
			if( !sRenderer.m_object )//JUST UPDATE THE REFERENCE
				sRenderer.m_object = m_Renderer;
			return false;
		}
		
		
		private Renderer _renderer;
		public Renderer m_Renderer 
        {
			get
            {
				sRenderer.objName = gameObject.name; //TODO ADDED by Germain, this is a temporary "fix", do a proper fix
				//Debug.LogError( "Fix this the proper way!" );
                if( !_renderer )
				{
					_renderer = sRenderer.m_object;
                    if( !_renderer )
					{
						_renderer = GetComponentInChildren<Renderer>();
						sRenderer.m_object = _renderer;
					}
                }
                return _renderer;
            }
        }
		public GameObject m_GameObject 
        {
			get
            {
				if( !m_Renderer )
				{
					return null;
				}
				return _renderer.gameObject;
            }
        }


        void Start() {}//Allows enabling/disabling this component


        #region GET
        /// <summary>
        /// Returns the Renderer's Material's texture.
        /// </summary>
        /// <param name="fromSharedMaterial">If set to <c>true</c> the texture from the shared material will be returned 
        /// preventing another material instance from being created.</param>
        public Texture2D GetTexture( bool fromSharedMaterial = true )
        {
            if( !m_Renderer )
            {
                Debug.LogWarning ("No Renderer is being referenced. Returning null..", gameObject );
                return null;
            }
            if( fromSharedMaterial )
                return (Texture2D) m_Renderer.sharedMaterial.mainTexture;
            return (Texture2D) m_Renderer.material.mainTexture;
        }
        #endregion

        #region SET
        /// <summary>
        /// Sets the texture. If -matIndex- is left as -1 the final texture will be applied to each material.
        /// </summary>
        /// <param name="tex">Texture.</param>
        /// <param name="matIndex">The material index.</param>
        public void SetTexture( Texture2D tex, int matIndex = -1 )
        {
            if( !enabled )
                return;
            if( !m_Renderer )
            {
                Debug.LogWarning ("No Renderer is being referenced", gameObject );
                return;
            }
            m_GameObject.SetTexture( tex, matIndex );
        }
        /// <summary>
        /// Sets the main texture of every material as the specified texture.
        /// </summary>
        /// <param name="tex">Texture.</param>
        public void SetTexture( Texture2D tex )
        {
            if( !enabled )
                return;
            if( !m_Renderer )
            {
                Debug.LogWarning ("No Renderer is being referenced", gameObject );
                return;
            }
            m_GameObject.SetTextures( tex );
        }
        /// <summary>
        /// Sets the main texture of every material as the specified textures.
        /// </summary>
        public void SetTexture( Texture2D[] textures )
        {
            if( !enabled )
                return;
            if( !m_Renderer )
            {
                Debug.LogWarning ("No Renderer is being referenced", gameObject );
                return;
            }
            m_GameObject.SetTextures( textures );
        }
        /// <summary>
        /// Sets the main texture of every or a specific material as the specified texture. If -matIndex- is left as -1 
        /// the texture will be applied to each material.
        /// </summary>
        /// <param name="textures">The Textures.</param>
        /// <param name="index">The texture index.</param>
        /// <param name="matIndex">The material index.</param>
        public void SetTexture( Texture2D[] textures, int index, int matIndex = -1 )
        {
            if( !enabled )
                return;
            if( !m_Renderer )
            {
                Debug.LogWarning ("No Renderer is being referenced", gameObject );
                return;
            }
            m_GameObject.SetTexture( textures[ index ], matIndex );
        }
        /// <summary>
        /// Sets the main texture of every material as the specified textures.
        /// </summary>
        public void SetTexture( List<Texture2D> textures )
        {
            SetTexture( textures.ToArray() );
        }
        /// <summary>
        /// Sets the main texture of every or a specific material as the specified texture. If -matIndex- is left as -1 
        /// the texture will be applied to each material.
        /// </summary>
        /// <param name="textures">The Textures.</param>
        /// <param name="index">The texture index.</param>
        /// <param name="matIndex">The material index.</param>
        public void SetTexture( List<Texture2D> textures, int index, int matIndex = -1 )
        {
            SetTexture( textures.ToArray(), index, matIndex );
        }
        #endregion

		#region ANIMATIONS
        public IEnumerator AnimateColor( ColorAnimation animation )
        {
            Color original = m_Renderer.GetTrueMaterial().color;
            bool _loop = animation.loop;
            do {
                Timing.RunCoroutine( m_Renderer.AnimColor( animation.target, animation.duration, false, null ) );
                yield return new WaitForSeconds( animation.duration );
                if( _loop )
                {
                    Timing.RunCoroutine( m_Renderer.AnimColor( original, animation.duration, false, null ) );
                    yield return new WaitForSeconds( animation.duration );
                }
            } while( animation.loop );
        }
        public IEnumerator AnimateColors( ColorAnimation animation )
        {
            Material[] originals = m_Renderer.GetTrueMaterials();
            for( int i=0; i<originals.Length; i++ )
            {
                Color original = originals[i].color;
                bool _loop = animation.loop;
                do {
                    Timing.RunCoroutine( m_Renderer.AnimColor( animation.target, animation.duration, false, null ) );
                    yield return new WaitForSeconds( animation.duration );
                    if( _loop )
                    {
                        Timing.RunCoroutine( m_Renderer.AnimColor( original, animation.duration, false, null ) );
                        yield return new WaitForSeconds( animation.duration );
                    }
                } while( animation.loop );
            }
        }
        #endregion
    }
}
