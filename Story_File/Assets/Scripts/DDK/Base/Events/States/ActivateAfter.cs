//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.Base.Events.States 
{
	/// <summary>
	/// Activates the specified GameObjects and/or enables the specified Behaviours after a specified delay.
	/// </summary>
	public class ActivateAfter : MonoBehaviour 
    {		
		public float delay;
		public GameObject[] objs = new GameObject[0];
		[Tooltip("If true, the objs activated/instantiated will be positioned on this obj (same transform position)")]
		[ShowIfAttribute( "_IsObjsNotEmpty", 1 )]
		public bool positionObjsOnThis;
		[ShowIfAttribute( "positionObjsOnThis", 2 )]
		[Tooltip("If true and the object has a RectTransform, its anchored position will be used instead")]
		public bool useRectTransform;
        public Behaviour[] comps = new Behaviour[0];
		[Header("Events")]
        [ShowIfAttribute( "destroyThisAfterActivationDone", true )]
		public bool disableThisAfterActivationsDone = true;
        [ShowIfAttribute( "disableThisAfterActivationsDone", true )]
		public bool destroyThisAfterActivationDone;


        protected WaitForSeconds _wait;
		

		protected bool _IsObjsNotEmpty()
		{
			return objs.Length > 0;
		}

		
        private void Awake()
        {
            _wait = new WaitForSeconds( delay );
        }
		protected virtual void OnEnable () 
        {			
			StartCoroutine( _StartCoroutines() );
		}
		
        /// <summary>
        /// Activates/Instantiates the specified gameObjects, enables the specified components, and invokes the specified events.
        /// </summary>
		protected IEnumerator _StartCoroutines()
		{
			StartCoroutine( Activate() );
			comps.EnableAfter( true, delay );
            yield return _wait;
			InvokeEvents ();
		}


        /// <summary>
        /// Activates/Instantiates the specified gameObjects, and sets their position on this gameObject if that was specified.
        /// </summary>
		public IEnumerator Activate()
		{
            yield return _wait;
            GameObject obj = null;
            for( int i=0; i<objs.Length; i++ )
            {
                obj = objs[i].SetActiveInHierarchy();
                if( positionObjsOnThis )
                {
                    obj.SetPos( transform.position );
                }
            }
		}
        /// <summary>
        /// Disables/Destroys this component, depending on what was specified.
        /// </summary>
		public void InvokeEvents()
		{
			if( disableThisAfterActivationsDone )
				enabled = false;
			else if( destroyThisAfterActivationDone )
			{
				Destroy( this );
			}
		}

	}

}
