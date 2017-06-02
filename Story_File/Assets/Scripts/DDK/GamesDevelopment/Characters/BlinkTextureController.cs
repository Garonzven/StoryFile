//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Extensions;
using DDK.Base.Statics;


namespace DDK.GamesDevelopment.Characters 
{
	/// <summary>
	/// This controls a character's eyes blink animation
	/// </summary>
	[RequireComponent( typeof( SkinnedMeshRenderer ) )]
	public class BlinkTextureController : MonoBehaviour 
    {
		[Tooltip("The texture to set when blinking")]
		public Texture2D blink;
		[Tooltip("If /blink/ is null, this path will be used to load the texture from the Resources folder")]
		[ShowIfAttribute( "_IsBlinkNull" )]
		public string texturePath;
		public float blinkDuration = 0.2f;
		[Range(0f,1f)]
		[Tooltip("Chances of blinking twice, in % (1 = 100%)")]
		public float blinkTwiceChances = 0.3f;
		[IndentAttribute(1)]
		public float secondBlinkDelay = 0.2f;
		[Tooltip("Blink every... seconds")]
		public float blinkRate = 5f;
		[Tooltip("A value between /blinkRate/ and the result of its addition with this, will be the real Blink Rate")]
		public float blinkRateVariation = 3f;


		protected bool _IsBlinkNull()
		{
			return blink == null;
		}


		protected SkinnedMeshRenderer _renderer;
		protected Texture _original;
        protected Material _SharedMaterial
        {
            get
            {
                return _renderer.sharedMaterial;
            }
        }
		protected float _time;
		protected float _rate;
        protected IEnumerator<float> _blinkCoroutine;

        private WaitForSeconds _waitBlink;
        private WaitForSeconds _waitSecondBlink;


		// Use this for initialization
		void Start () 
        {
			if( !blink )
			{
				if( string.IsNullOrEmpty( texturePath ) )
				{
                    Utilities.LogWarning ("No /blink/ texture has been set, and the specified path is empty");
					enabled = false;
					return;
				}
				blink = Resources.Load<Texture2D>( texturePath );
				if( !blink )
				{
					Utilities.LogWarning ("No /blink/ texture has been set, and no texture was found in the specified path");
					enabled = false;
					return;
				}
			}
			_renderer = GetComponentInParent<SkinnedMeshRenderer>();
			CalculateRate();
            _waitBlink = new WaitForSeconds( blinkDuration );
            _waitSecondBlink = new WaitForSeconds( secondBlinkDelay );
		}		
		// Update is called once per frame
		void Update () 
        {
            if( !enabled )//Some other script might be handling the eyes texture
                return;
			_time += Time.deltaTime;
			if( _time >= _rate  )
			{
				_time = 0f;
				CalculateRate();
				Blink ();
			}
		}


		public void Blink()
		{
            if( !enabled )
                return;
            Texture original = _SharedMaterial.mainTexture;
            if( original.name.Equals( blink.name ) )
                return;
            _original = original;
            _Blink().Start();
		}

        protected IEnumerator _Blink()
		{
			_SharedMaterial.mainTexture = blink;
            yield return _waitBlink;
            if( !enabled || _SharedMaterial.mainTexture != blink )//Some other script might be handling the eyes texture
                yield break;
			_SharedMaterial.mainTexture = _original;

			if( blinkTwiceChances >= Random.value )
			{
				float _chances = blinkTwiceChances;
				blinkTwiceChances = 0f;//prevent extra blinks
                yield return _waitSecondBlink;
                if( !enabled || _SharedMaterial.mainTexture != _original )//Some other script might be handling the eyes texture
                    yield break;
                yield return _Blink().Start();
                blinkTwiceChances = _chances;
			}
		}
		protected void CalculateRate()
		{
			_rate = Random.Range( blinkRate, blinkRate + blinkRateVariation );
		}
	}
}
