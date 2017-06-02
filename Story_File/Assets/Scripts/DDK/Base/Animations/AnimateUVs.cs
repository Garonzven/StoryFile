using UnityEngine;


namespace DDK.Base.Animations 
{
    /// <summary>
    /// Animates the UVs of a MeshRenderer's Material's main Texture.
    /// </summary>
	[RequireComponent( typeof( MeshRenderer ) )]
	public class AnimateUVs : MonoBehaviour 
    {
		public Vector2 speed = new Vector2( 1f, 0f ) * 0.1f;


		MeshRenderer _mr;
		Material _mat;
		Vector2 _offset;


		/// <summary>
        /// Initialize private variables.
        /// </summary>
		void Start () 
        {
			_mr = GetComponent<MeshRenderer>();			
			_mat = _mr.material;			
		}		
		/// <summary>
        /// Updates the offset of the MeshRenderer's Material's main Texture.
        /// </summary>
		void Update () 
        {			
			_offset = _mat.mainTextureOffset;
			
			_offset.x += speed.x * 0.001f;
			_offset.y += speed.y * 0.001f;
			
			_mat.mainTextureOffset = _offset;
		}
	}
}
