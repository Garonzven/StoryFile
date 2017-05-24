//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.UI;
using DDK.Base.Extensions;


namespace DDK.Base.UI
{
	/// <summary>
	/// Renames the object it is attached to, as the Text component's text value.
	/// </summary>
	[ExecuteInEditMode]
	public class NameAsTextValue : NameAs 
    {
        #if UNITY_EDITOR
		public bool ommitTextValue;


		protected Text _text 
        {
			get{ return GetComponentInChildren<Text>(); }
		}
		
		
		protected override void OnEnable() 
        {			
			base.OnEnable();
			if( !_text )
				enabled = false;
		}

		// Update is called once per frame
		void Update () 
        {			
			if( stopUpdate || !_text )
				return;
			if( _text.text != null )
			{
				if( ommitTextValue )
					name = Prefix + Suffix;
				else name = Prefix + _text.text + Suffix;
				if( clamp > 0 )
				{
					name = name.Clamp( clamp );
				}
			}
		}
		#endif
	}
}
