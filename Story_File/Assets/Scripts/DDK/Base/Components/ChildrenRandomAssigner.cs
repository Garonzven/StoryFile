//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections.Generic;
using DDK.Base.Extensions;
using DDK.Base.Misc;
using DDK.Base.Statics;


namespace DDK.Base.Components 
{	
	/// <summary>
	/// Attach to a gameobject with children to randomly assign values to their specified type behaviour (without 
    /// repeating the assigned values); T2 should enclose T. This is done in order, E.g: Imagine an array of 2 assigners, 
    /// the first with 2 elements { "A", "B" } and the second with 1 element { "C" }, the first assigner has an assign 
    /// limit of 2, and the gameObject holding this component has 3 children. The first 2 children (Text) will always 
    /// have "A" or "B" assigned randomly, unless the assign limit is set to 1 in which case only one of the first 2 
    /// children will have either and the other will stay with its current value, but the last (3rd) child will always "C".
    /// </para>
    /// <para>
    /// </para>
    /// What is this useful for? You might create a game were you need to have multiple choices for every 3 children in 
    /// a gameObject. You can create multiple assigners with 3 elements (options/choices) each, and this will shuffle the 
    /// answers.
    /// </summary>
	/// </summary>
	[ExecuteInEditMode]
	public abstract class ChildrenRandomAssigner<T, T2> : MonoBehaviour where T2 : Behaviour 
    {		
		[System.Serializable]
		public abstract class RandomAssigner
        {			
			[Tooltip("If not empty, the below path won't be used")]
			public List<T> elements = new List<T>();
            [ShowIfAttribute( "_IsSpritesEmpty" )]
			public string elementsPath = "Elements/1";
			[SerializeField]
			[Tooltip("The amount of elements to assign. This will be clamped to the /elements/ count")]
			private int assignLimit = 2;
			public bool nameObjsAsElement;


            #if UNITY_EDITOR
            protected bool _IsSpritesEmpty()
            {
                return elements.Count == 0;
            }
            #endif


			public int _AssignLimit 
            {
                get //PREVENT for FROM LOOPING FOREVER CAUSING UNITY TO CRASH
                {
					return assignLimit = assignLimit.Clamp( 0, elements.Count );
				}
			}

			
			/// <summary>
			/// Randomly assignes elements from the /pathPrefix/ + /elementsPath/ to the specified behaviour without repeating assigned elements.
			/// Returns the assigned elements.
			/// </summary>
			/// <param name="pathsPrefix">The elements path prefix.</param>
			/// <param name="behaviours">The behaviours in which the elements will be assigned.</param>
			public abstract List<T> Assign( PathHolder.Index pathsPrefix, List<T2> behaviours );
			protected abstract void _Assign( T2 behaviour, T element )	;		
		}
		
		
		
		public bool executeOnceInEditMode;
		[Tooltip("If true, this will be done in the Awake() instead of the Start()")]
		public bool onAwake;
		public bool includeSubChildren;
		public bool includeInactive;
		[Tooltip("This is added to the elementsPath")]
		public PathHolder.Index pathsPrefix;
		
		
		
		/// <summary>
		/// A list of all the elements that were assigned.
		/// </summary>
		internal List<T> m_assigned = new List<T>();
		/// <summary>
		/// All Behaviour components in children.
		/// </summary>
		internal List<T2> m_children = new List<T2>();

		public static List<T> m_excludeFromAssigners = new List<T>();
		
		

		void Awake()
		{
			if( onAwake && Application.isPlaying )
			{
				_Start();
			}
		}		
		// Use this for initialization
		void Start () 
        {			
			if( !onAwake && Application.isPlaying )
			{
				_Start();
			}
		}
		protected virtual void _Start()
		{
			m_children.AddRange( gameObject.GetCompsInChildren<T2>( includeSubChildren, includeInactive ) );
			PreAssignment();
		}		
		// Update is called once per frame
		void Update () 
        {			
			#if UNITY_EDITOR
			if( !Application.isPlaying && executeOnceInEditMode )
			{
				_Start();
				executeOnceInEditMode = false;
				enabled = false;
				Utilities.Log ( "Randoms Assigned!" );
			}
			#endif			
		}

		/// <summary>
		/// This gets called before assigning the random elements.
		/// </summary>
		protected abstract void PreAssignment();
        		
		
		/// <summary>
		/// Returns a random unassigned element.
		/// </summary>
        /// <param name="assigners">The Random Assigners.</param>
		/// <param name="assignerIndex">Assigner index.</param>
		/// <param name="addToAssigned">If true, the element will be added to the /assigned/ list.</param>
		public T GetUnassigned( RandomAssigner[] assigners, int assignerIndex = 0, bool addToAssigned = false )
		{
            var assiger = assigners[ assignerIndex ];
            var assignerElementsCount = assiger.elements.Count;
            T element = default( T );
            List<int> indexes = new List<int>( assignerElementsCount );
            for( int i=0; i<assignerElementsCount; i++ )//Fill indexes
            {
                indexes.Add( i );
            }
            for( int i=0; i<assignerElementsCount; i++ )
            {
                element = assiger.elements[ indexes.PopRandom() ];
                if( !m_assigned.Contains( element ) )
                {
                    if( addToAssigned )
                    {
                        m_assigned.Add( element );
                    }
                    return element;
                }
            }
            return element;
		}
	}	
}