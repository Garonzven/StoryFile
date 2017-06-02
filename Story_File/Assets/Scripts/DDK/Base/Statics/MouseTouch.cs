//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;


namespace DDK.Base.Statics 
{
	public static class MouseTouch 
    {		
		/// <summary>
		/// Returns true if two touches are close. If only one touch is detected it will return true. If no touches are detected 
		/// but a mouse button with same index as firstTouchID or firstTouchID+1 is detected it will return true.
		/// </summary>
		/// <returns><c>true</c>, if touches are close, or only one touch is detected, <c>false</c> otherwise.</returns>
		/// <param name="maxDistance">Maximum distance.</param>
        /// <param name="minDistance">Minimum distance.</param>
		/// <param name="firstTouchID">First touch ID. Second one will follow ( First touch ID + 1 ).</param>
		public static bool AreTouchesClose( float maxDistance, float minDistance = 0f, byte firstTouchID = 0 )
		{
			if( Input.touchCount == firstTouchID+2 )
			{
				float dist = Vector2.Distance( Input.touches[firstTouchID].position, Input.touches[firstTouchID+1].position );
				return dist <= maxDistance && dist >= minDistance;
			}
			else if( ( Input.GetMouseButton(firstTouchID) || Input.GetMouseButton(firstTouchID+1) ) && !Device.isTouchable ) 
				return true;
			return false;
		}		
		public static bool IsAnyMouseButtonDown()
		{
			for( byte i=0; i<3; i++ )
				if( !Device.isTouchable )
					if( Input.GetMouseButton(i) ) return true;
			return false;
		}
		/// <summary>
        /// Returns the touch/mouse position in pixel coordinates.
        /// </summary>
        /// <param name="touchID"> The touch (finger) Identifier (index) </param>
		public static Vector2 Position( int touchID = 0 )
		{
			if( Input.touchCount > touchID )
			{
				return Input.touches[touchID].position;
			}
			else return Input.mousePosition;
		}
		/// <summary>
        /// Returns the touch/mouse position in world coordinates. If the /cam/ is null, the main camera will be used.
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
		public static Vector3 GetWorldPoint ( Camera cam = null, int touchID = 0 )
		{
			if( cam == null )
				cam = Camera.main;
			return cam.ScreenToWorldPoint( Position( touchID ) );
		}		
		/// <summary>
		/// Returns the object being touched if the mouse is up on the object, null otherwise. NOTE: object must have a collider.
		/// </summary>
		public static GameObject UpOnObject( int buttonIndex = 0 )
		{
			if( Input.GetMouseButtonUp( buttonIndex ) )
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit))
				{
					return hit.collider.gameObject;
				}				
			}
			return null;
		}		
	}
}