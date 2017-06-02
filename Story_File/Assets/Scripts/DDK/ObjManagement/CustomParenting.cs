//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.ObjManagement {

	/// <summary>
	/// Parents an object and allows to customize the affected values of the transform.
	/// </summary>
	[ExecuteInEditMode]
	public class CustomParenting : MonoBehaviour {

		public Transform parent;
		[Tooltip("If true and the object has a RectTransform, its anchored position will be used instead")]
		public bool useRectTransform;
		[Space(5f)]
		[Header("Position")]
		public bool px;
		public bool py;
		public bool pz;
		public Vector3 offset;
		[Space(5f)]
		[Header("Rotation")]
		public bool rx;
		public bool ry;
		public bool rz;
		[Space(5f)]
		[Header("UI")]
		public bool w;
		public bool h;
		public Vector2 extraWH;		


		protected RectTransform _rt;

		
		
		// Use this for initialization
		void Start () {
			
			_rt = gameObject.GetComponent<RectTransform>();
		}
		
		// Update is called once per frame
		void Update () {
			
			if( parent )
			{
				#region POSITIONING
				var position = gameObject.Position( false, useRectTransform );
				float X = position.x;
				float Y = position.y;
				float Z = position.z;
				if( px ){
					X = parent.position.x;
				}
				if( py ){
					Y = parent.position.y;
				}
				if( pz ){
					Z = parent.position.z;
				}
				
				gameObject.SetPos( new Vector3( X, Y, Z ) + offset, useRectTransform );
				#endregion
				#region ROTATING
				X = transform.eulerAngles.x;
				Y = transform.eulerAngles.y;
				Z = transform.eulerAngles.z;
				if( rx ){
					X = parent.eulerAngles.x;
				}
				if( ry ){
					Y = parent.eulerAngles.y;
				}
				if( rz ){
					Z = parent.eulerAngles.z;
                }
                
				transform.eulerAngles = new Vector3( X, Y, Z );
                #endregion
#if UNITY_EDITOR
				#region UI
				if( GetComponent<RectTransform>() )
				{
					UI( GetComponent<RectTransform>() );
				}
				#endregion
#else
				#region UI
				if( _rt )
				{
					UI( _rt );
				}
				#endregion
#endif
            }
            
        }



		void UI( RectTransform rt )
		{
			float W = rt.rect.width;
			float H = rt.rect.height;
			if( w ){
				W = parent.GetComponent<RectTransform>().rect.width;
			}
			if( h ){
				H = parent.GetComponent<RectTransform>().rect.height;
			}
			
			rt.SetSize( new Vector2( W, H ) + extraWH );
		} 
        
        
    }
    
    
}