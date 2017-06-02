//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.UI;
using DDK.Base.Extensions;


namespace DDK.Base.UI
{
	/// <summary>
    /// Renames the object it is attached to, as the Image or SpriteRenderer's component's sprite's name.
    /// </summary>
	[ExecuteInEditMode]
	public class NameAsSprite : NameAs 
    {	
        #if UNITY_EDITOR
		public bool ommitSpriteName;


		protected Sprite _sprite 
        {
			get
            {
				var img = GetComponentInChildren<Image>();
				if( img )
				{
					return img.sprite;
				}
				var ren = GetComponentInChildren<SpriteRenderer>();
				if( !ren )
				{
					enabled = false;
					return null;
				}
				return ren.sprite;
			}
		}


		protected override void OnEnable() 
        {			
			base.OnEnable();
			if( !_sprite )
				enabled = false;
		}

		// Update is called once per frame
		void Update () 
        {			
			if( stopUpdate || !_sprite )
				return;
			if( _sprite.name != null )
			{
                if( ommitSpriteName )
					name = Prefix + Suffix;
				else name = Prefix + _sprite.name + Suffix;
				if( clamp > 0 )
				{
					name = name.Clamp( clamp );
				}
			}
		}
		#endif
	}
}