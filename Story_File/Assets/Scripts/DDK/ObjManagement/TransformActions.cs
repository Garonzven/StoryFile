//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.ObjManagement
{
	/// <summary>
	/// Transform actions to be called by events.
	/// </summary>
	public class TransformActions : MonoBehaviour {

		[HelpBoxAttribute]
		public string msg = "If this is disabled, the invoked actions won't be executed.";
        public SearchableGameObject m_obj;


		// Use this for initialization
		void Start () {
			
            if( !m_obj.m_transform )
            {
                m_obj.m_gameObject = gameObject;
            }
		}


		#region SET
		public void SetTransform( Transform t )
		{
			if( !enabled || !t )
				return;
            m_obj.m_transform.CopyTransformFrom( t );
		}
		public void SetPosition( Vector3 pos )
		{
			if( !enabled )
				return;
            m_obj.m_transform.position = pos;
		}
		public void SetLocalPosition( Vector3 pos )
		{
			if( !enabled )
				return;
            m_obj.m_transform.localPosition = pos;
		}
		public void SetRotation( Vector3 euler )
		{
			if( !enabled )
				return;
            m_obj.m_transform.rotation = Quaternion.Euler( euler );
		}
		public void SetLocalRotation( Vector3 euler )
		{
			if( !enabled )
				return;
            m_obj.m_transform.localRotation = Quaternion.Euler( euler );
		}
		public void SetLocalScale( Vector3 scale )
		{
			if( !enabled )
				return;
            m_obj.m_transform.localScale = scale;
		}
		#endregion

		#region ADD
		public void AddToTransform( Transform t )
		{
			if( !enabled || !t )
				return;
            m_obj.m_transform.position += t.position;
            Vector3 newRot = m_obj.m_transform.rotation.eulerAngles + t.rotation.eulerAngles;
            m_obj.m_transform.rotation = Quaternion.Euler( newRot );
            m_obj.m_transform.localScale += t.localScale;
		}
		public void AddToPosition( Vector3 pos )
		{
			if( !enabled )
				return;
            m_obj.m_transform.position += pos;
		}
		public void AddToLocalPosition( Vector3 pos )
		{
			if( !enabled )
				return;
            m_obj.m_transform.localPosition += pos;
		}
		public void AddToRotation( Vector3 euler )
		{
			if( !enabled )
				return;
            Vector3 newRot = m_obj.m_transform.rotation.eulerAngles + euler;
            m_obj.m_transform.rotation = Quaternion.Euler( newRot );
		}
		public void AddToLocalRotation( Vector3 euler )
		{
			if( !enabled )
				return;
            Vector3 newRot = m_obj.m_transform.localRotation.eulerAngles + euler;
            m_obj.m_transform.localRotation = Quaternion.Euler( newRot );
		}
		public void AddToLocalScale( Vector3 scale )
		{
			if( !enabled )
				return;
            m_obj.m_transform.localScale += scale;
		}
		#endregion
	}
}
