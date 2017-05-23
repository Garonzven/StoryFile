//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using DDK.Base.Statics;


namespace DDK.Base.Components 
{
	/// <summary>
	/// DO NOT ATTACH DIRECTLY. This is used my some extension methods to check if a gameObject is a prefab or not.
	/// </summary>
	[ExecuteInEditMode]
    public class IsPrefab : MonoBehaviour //THIS HAS AN EDITOR CLASS
    {
		public int id;
		public bool idWasSet;
        /// <summary>
        /// true if this gameObject is a prefab's instance.
        /// </summary>
        public bool isInstance;


		public static int nextValidId;


        void Awake () 
        {
            CheckPrefab();
        }   
		// Use this for initialization
		void Start () 
        {
			CheckPrefab();
		}		
		// Update is called once per frame
		void Update () 
        {
			CheckPrefab();
		}
        void OnEnable()
        {
            CheckPrefab();
        }



        /// <summary>
        /// Checks if this instance is a prefab's instance, if not, this component is destroyed.
        /// </summary>
		void _Validate()
		{
            if( gameObject.AnyAncestorContains("(Clone)") )
			{
				isInstance = true;
			}
			else DestroyImmediate( this );//IS NOT A PREFAB NOR PREFAB INSTANCE, SINCE THIS EVALUATES IN PLAY MODE
		}
        /// <summary>
        /// Verifies if this gameObject is a prefab.
        /// </summary>
        public void CheckPrefab()
		{
			if( Application.isPlaying )
			{
				_Validate();
				if( this )
					enabled = false;//PREVENT FUTURE EXECUTIONS
			}
			else //IS EXECUTING IN EDITOR
			{
				Utilities.LogWarning("For IsPrefab component to work properly the prefab must be instantiated in play mode.");
				DestroyImmediate( this );
			}
		}
	}
}
