//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;


namespace DDK.Base.UI 
{
	/// <summary>
	/// This allows controlling the sprite's outline visibility and color. Image's Material MUST contain the Outline_2DSprite shader which can be found here: 
	/// http://answers.unity3d.com/questions/680443/shader-edit-help-2d-outline-coloring.html
	/// </summary>
	[RequireComponent( typeof( Image ) )]
	[ExecuteInEditMode]
	public class ImageSpriteOutline : MonoBehaviour 
    {
		public bool executeInPlayMode;
		[Space(5f)]
		public Shader outlineShader;
		public Color outlineColor;
		[Range( 0f, 0.012f )]
		public float xSpread = 0.007f;
		[Range( 0f, 0.012f )]
		public float ySpread = 0.007f;
		public Color ambientLight;


		internal Image _img {
			get {
				return GetComponent<Image>();
			}
		}


		// Use this for initialization
		void Start () {}
		
		// Update is called once per frame
		void Update () 
        {
			if( !Application.isPlaying )
			{
				if( !executeInPlayMode )
					return;
				SetOutline();
			}
			else if( enabled )
			{
				SetOutline();
				SetAmbientLight();
			}
		}
		void OnDisable()
		{
			_img.material = null;
		}


		public void SetOutline()
		{
			if( !outlineShader )
			{
				//outlineMatSet = false;
				return;
			}
			if( !_img.material.shader != outlineShader )
			{
				_img.material = new Material( outlineShader );
				xSpread = _img.material.GetFloat( "_OutLineSpreadX" );
				ySpread = _img.material.GetFloat( "_OutLineSpreadY" );
				//outlineMatSet = true;
			}
			_img.material.SetColor( "_Color", outlineColor );
			_img.material.SetFloat( "_OutLineSpreadX", xSpread );
			_img.material.SetFloat( "_OutLineSpreadY", ySpread );
		}
		public void OutlineFadeIn( float speed = 1f )
		{
			StartCoroutine( _OutlineFadeIn( speed ) );
		}
		public void OutlineFadeOut( float speed = 1f )
		{
			StartCoroutine( _OutlineFadeOut( speed ) );
		}
		public void SetAmbientLight()
		{
			if( ambientLight != default( Color ) )
			{
				RenderSettings.ambientLight = ambientLight;
			}
		}


		public IEnumerator _OutlineFadeIn( float speed = 1f )
		{
			while( outlineColor.a < 1f )
			{
				outlineColor = new Color( outlineColor.r, outlineColor.g, outlineColor.b, ( outlineColor.a + speed * Time.deltaTime ).Clamp01() );
				SetOutline();
				yield return null;
			}
		}		
		public IEnumerator _OutlineFadeOut( float speed = 1f )
		{
			while( outlineColor.a > 0f )
			{
				outlineColor = new Color( outlineColor.r, outlineColor.g, outlineColor.b, ( outlineColor.a - speed * Time.deltaTime ).Clamp01() );
				SetOutline();
				yield return null;
			}
		}
	}
}
