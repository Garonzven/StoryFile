//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Managers;
using DDK.Base.SoundFX;
using DDK.Base.Classes;



namespace DDK.ObjManagement {

	/// <summary>
	/// Moves an object towards a target.
	/// </summary>
	public class MoveTowards : MonoBehaviour {//THIS HAS AN EDITOR CLASS
		
		public float delay;
		[Tooltip("This might not be necessary. If true and the object has a RectTransform component, then its anchored position will be used for calculations")]
		public bool useRectTransform;
		public bool fromNameInstead;
		public bool fromTagInstead;
		[Tooltip("The name of the object from which this should move towards")]
		public string fromName;
		[Tooltip("The tag of the object from which this should move towards")]
		public string fromTag;
		public GameObject from;

		public float speed = 1f;
		public int frameRate = 1;
		[Tooltip("This are played after last target reached")]
		public Sfx[] sfx = new Sfx[0];
		public bool stopAllSoundsBeforePlayingSfx;
		//EVENTS
		[Tooltip("If true, the ActivateAf... objects will be searched by their names (searching also for clones)")]
		public bool activateByNamesInstead = true;
		[Tooltip("Each index represents a target")]
		public string[] activateAfterTargetReachedNames = new string[0];
		[Tooltip("Each index represents a target")]
		public GameObject[] activateAfterTargetReached = new GameObject[0];
		[Tooltip("Each index represents a target")]
		public MonoBehaviour[] enableAfterTargetReached = new MonoBehaviour[0];
		[Tooltip("If true, the specified enableAfterTargetReached components will be enable in this object instead " +
			"(if this object contains a script of the respective type)")]
		public bool enableInThisObj;
		public string[] activateAfterLastTargetReachedNames = new string[0];
		public GameObject[] activateAfterLastTargetReached = new GameObject[0];
		public MonoBehaviour[] enableAfterLastTargetReached = new MonoBehaviour[0];
		[Tooltip("Execute the OnTargetReached() event when this is destroyed")]
		public bool executeOnTargetReachedOnDestroy = false;
		public bool destroyWhenTargetReached;
		public bool repeatMovementWhenLastTargetReached;
			public bool playSfxEvenIfRepeat;

		public bool rotateTowardsInstead;
			//FALSE
			[Tooltip("If true, the targets will be searched by their names (searching also for clones)")]
			public bool targetNamesInstead = true;
			public string[] targetNames = new string[0];
		    public GameObject[] targets = new GameObject[0];
			public bool overrideSpeeds = true;
			[Tooltip("Each index represents a target")]
			public float[] movementSpeeds = new float[0];
			//TRUE
			public Vector3 targetRot = new Vector3(0f, 0f, 360f);
		[Tooltip("If true, the object will keep moving towards its last target")]
		public bool keepMovingTowardsLast;
		public bool resetPositionAtEnd;

		[System.Serializable]
		public class When{
			public Animator animator;
			/// <summary>
			/// Animator parameter name.
			/// </summary>
			[Tooltip("Animator parameter name.")]
			[IndentAttribute(1)]
			public string play;
			/// <summary>
			/// When the specified play bool parameter is set to.
			/// </summary>
			[Tooltip("When the specified play bool parameter is set to.")]
			[IndentAttribute(2)]
			public bool isSetTo = true;
			[Tooltip("If true, the condition is fulfilled")]
			public bool thisIsTrue;
		}
		public When when = new When();

		internal Vector3 iniPos;
		protected float _enabledTime;
		/// <summary>
		/// Current target index.
		/// </summary>
		protected int i = 0;
		protected bool _lastTargetReached;
		protected bool _quitting;

		void OnEnable () {
			if( fromNameInstead )
				from = fromName.Find();
			else if( fromTagInstead )
				from = fromTag.FindWithTag();
			if( targetNamesInstead && targetNames.Length > 0 )
			{
				targets = targetNames.Find(true).ToArray();
			}
			if( activateByNamesInstead )
			{
				activateAfterTargetReached = activateAfterTargetReachedNames.Find().ToArray();
				activateAfterLastTargetReached = activateAfterLastTargetReachedNames.Find().ToArray();
			}
			_enabledTime = Time.time;
			CheckEnableInThisObj();
			StartCoroutine( SetPos() );
		}

		IEnumerator SetPos()
		{
			yield return new WaitForSeconds(0.001f);
			if( from )
				gameObject.SetPos( from.Position( false, useRectTransform ), false, useRectTransform );
			iniPos = transform.position;
		}
		
		// Update is called once per frame
		void Update () {

			if( Time.frameCount % frameRate == 0 )
			{
				#region CONDITIONS
				if( Time.time < _enabledTime + delay )//DELAY CONDITION
					return;
				if( when.animator != null )//ANIMATOR CONDITION
				{
					bool play = when.animator.GetBool( when.play );
					if( when.isSetTo != play )
						return;
				}
				else if( !when.thisIsTrue )
				{
					return;
				}
				#endregion

				//EXTRA VALIDATIONS
				if( targets.Length > 0 )
				{
					if( i >= targets.Length )//WHEN ITS A MOVEMENT
					{
						OnLastTargetReached();
						return;
					}
					else if( targets[i] == null )
					{
						i++;
						return;
					}
					if( !overrideSpeeds )
					{
						if( movementSpeeds.Length < targets.Length ) 
							Debug.LogWarning( "Override Movement Speeds is false, but Movement Speeds array is smaller" +
								"than targets array!" );
						else if( movementSpeeds.Length > i )
						{
							speed = movementSpeeds[i];
						}
					}
				}
				else if ( !rotateTowardsInstead )
				{
					Debug.LogWarning( "GameObject: "+name+"\t Message: There are no targets!" );
					return;
				}

				if( rotateTowardsInstead )//ROTATE
				{
					if( transform.rotation != Quaternion.Euler( targetRot ) )
					{
						transform.rotation = Quaternion.Euler ( Vector3.MoveTowards( transform.rotation.eulerAngles, targetRot, speed * Time.deltaTime * 100 ) );
						if( transform.rotation.eulerAngles.Distance( targetRot.Abs() ) <= speed * Time.deltaTime * 100 )
						{
							transform.rotation = Quaternion.Euler( targetRot );
						}
					}
					else if( !keepMovingTowardsLast ) OnLastTargetReached();
				}
				else if( transform.position != targets[i].Position( false, useRectTransform ) )//MOVE
				{
					transform.position = Vector3.MoveTowards( transform.position, targets[i].Position( false, useRectTransform ), speed * Time.deltaTime * 100 );
				}
				else  if( !keepMovingTowardsLast )
				{
					if( activateAfterTargetReached.Length > i )
						activateAfterTargetReached[i].SetActiveInHierarchy();
					if( enableAfterTargetReached.Length > i )
						enableAfterTargetReached[i].SetEnabled();
					i++;
				}
			}
		}

		void OnDestroy()
		{
			if( executeOnTargetReachedOnDestroy && !_lastTargetReached && !_quitting )
			{
				OnLastTargetReached();
			}
		}

		void OnApplicationQuit()
		{
			_quitting = true;
		}

		public void SetSingleTarget (GameObject target)
		{
			i = 0;
			targets = new GameObject[] { target };
		}

		public void OnLastTargetReached()
		{
			_lastTargetReached = true;
			activateAfterLastTargetReached.SetActiveInHierarchy();
			enableAfterLastTargetReached.SetEnabled();
			if( repeatMovementWhenLastTargetReached )
			{
				if( playSfxEvenIfRepeat )
				{
					SfxManager.PlayQueue( sfx, gameObject );
				}
				i = 0;
				transform.position = iniPos;
			}
			else
			{
				SfxManager.PlayQueue( sfx, gameObject );
				if( destroyWhenTargetReached )
				{
					Destroy( gameObject );
				}
				enabled = false;
			}
			//frameRate = int.MaxValue;//Prevent unnecessary Updates
		}

		protected void CheckEnableInThisObj()
		{
			if( !enableInThisObj )
			{
				return;
			}
			for( int i=0; i<enableAfterTargetReached.Length; i++ )
			{
				var comp = gameObject.GetComponent( enableAfterTargetReached[i].GetType() );
				if( comp != null && comp is MonoBehaviour )
				{
					enableAfterTargetReached[i] = (MonoBehaviour)comp;
				}
			}
		}

	}

}