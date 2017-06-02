//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;



namespace DDK.ObjManagement {

	/// <summary>
    /// Attach to an object to make it rotate forever. You can also use Unity's version ( Standard Assets / Utility / AutoMoveAndRotate.cs )
	/// </summary>
	public class RotateForever : MonoBehaviour {

		public Space space = Space.Self;
		[Tooltip("The rotation amounts")]
		public Vector3 amounts = new Vector3( 0f, 0f, 0.1f );
		[Tooltip("A value between the below (negative variation) and this will be added to the amounts")]
		[IndentAttribute(1)]
		public Vector3 positiveVariation;
		[Tooltip("A value between this and the positive variation will be added to the amounts")]
		[IndentAttribute(1)]
		public Vector3 negativeVariation;
		[Tooltip("The amounts minimum positive value")]
		[IndentAttribute(1)]
		public Vector3 minAmounts = new Vector3( 0f, 0f, 1f );
		[Tooltip("The amounts maximum positive value")]
		[IndentAttribute(1)]
		public Vector3 maxAmounts = new Vector3( 360f, 360f, 360f );
		[Tooltip("The amounts maximum (closer to 0) negative value")]
		[IndentAttribute(1)]
		public Vector3 minNegativeAmounts = new Vector3( -360f, -360f, -360f );
		[Tooltip("The amounts minimum (farther from 0) negative value")]
		[IndentAttribute(1)]
		public Vector3 maxNegativeAmounts = new Vector3( 0f, 0f, -1f );
		[Tooltip("If true, the variation will only be calculated once")]
		[IndentAttribute(1)]
		public bool constantVariation;
		[Tooltip("If true, the Rotation will be assigned in the LateUpdate() instead of the Update()")]
		public bool lateUpdate = true;
		[Tooltip("This helps keeping a rotating camera (skybox) independent from the main camera's rotation. If rotating a skybox camera, then set this to true.")]
		public bool fixMainCam;				
		
		
		
		internal Vector3 _finalAmounts;
		internal bool _blockStart;
		protected Vector3 _rotAmount;
		protected Vector3 _rotValue;		



		[ContextMenu("Set Skybox Rotation")]
		public void SetSkyboxRotation()
		{
			amounts = new Vector3( 0f, 0.01f, 0f );
			positiveVariation = new Vector3( 0f, 0.01f, 0f );
			negativeVariation = new Vector3( 0f, -0.01f, 0f );
			minAmounts = new Vector3( 0f, 0.005f, 0f );
			maxAmounts = new Vector3( 0f, 0.1f, 0f );
			minNegativeAmounts = new Vector3( 0f, -0.01f, 0f );
			maxNegativeAmounts = new Vector3( 0f, -0.1f, 0f );
			fixMainCam = true;
		}


		
		void Start () {

			if( !_blockStart )
			{
				FixNegativeVariation();
				UpdateFinalAmounts();
				minNegativeAmounts.NegateValues();
				maxNegativeAmounts.NegateValues();
			}
		}
		
		// Update is called once per frame
		void Update () {

			if( !lateUpdate )
			{
				Rotate();
			}
		}

		void LateUpdate()
		{
			if( lateUpdate )
			{
				Rotate();
			}
		}



		void Rotate()
		{
			if (!constantVariation) {
				UpdateFinalAmounts ();
			} else
				_finalAmounts = amounts;
			if( fixMainCam )
			{
				_rotAmount += ( _finalAmounts * Time.deltaTime * 100f );
				var mainCamEuler =  Camera.main.transform.rotation.eulerAngles;
				_rotValue = mainCamEuler + _rotAmount;
				transform.rotation = Quaternion.Euler ( _rotValue );
			}
			else transform.Rotate( _finalAmounts * Time.deltaTime * 100f, space );
		}

		protected void FixNegativeVariation()
		{
			for( int i=0; i<3; i++ )
			{
				if( negativeVariation[i] > 0f )
				{
					negativeVariation[i] *= -1f;
				}
			}
		}

		protected void UpdateFinalAmounts()
		{
			_finalAmounts = amounts.RandomAdd( negativeVariation, positiveVariation ).ClampMultiple( maxAmounts, minAmounts, minNegativeAmounts, maxNegativeAmounts );
		}

	}

}
