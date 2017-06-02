using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.UI;


namespace DDK.Base.Events {
	
	public class OnClickSetParent : MonoBehaviour {

		[HelpBoxAttribute]
		public string msg = "If this object has a Button component, the event is automatically added to the onClick Listener";
		[Space(15f)]
		public Transform parent;
	    public Transform child;
		[Tooltip("If /parent/ is null, an object with the specified name will be used if found")]
		public string parentName;
		public bool worldPosStays = true;



		// Use this for initialization
		void Start () {

			if( !parent )
			{
				parent = parentName.Find<Transform>();
			}
			if( !parent )
			{
				enabled = false;
			}
			if( GetComponent<Button>() )
			{
				GetComponent<Button>().onClick.AddListener( () => SetParent() );
			}
		}

/*
		// Update is called once per frame
		void Update () {
			
		}*/



		public void SetParent()
		{
		    if ( !child )
		    {
                child = transform;
            }

            child.SetParent( parent, worldPosStays );
		}

	}
}
