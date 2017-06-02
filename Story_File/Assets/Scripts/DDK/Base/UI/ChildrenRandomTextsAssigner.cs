//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections.Generic;
using DDK.Base.Extensions;
using DDK.Base.Misc;
using UnityEngine.UI;
using DDK.Base.Components;
using DDK.Base.Statics;


namespace DDK.Base.UI 
{	
	/// <summary>
    /// <para>
	/// Attach to an object with children Text components to randomly assign text values to them (without repeating the 
    /// assigned texts). This is done in order, E.g: Imagine an array of 2 assigners, the first with 2 elements 
    /// { "A", "B" } and the second with 1 element { "C" }, the first assigner has an assign limit of 2, and the 
    /// gameObject holding this component has 3 children. The first 2 children (Text) will always have "A" or "B" 
    /// assigned randomly, unless the assign limit is set to 1 in which case only one of the first 2 children will have 
    /// either and the other will stay with its current value, but the last (3rd) child will always "C".
    /// </para>
    /// <para>
    /// </para>
    /// What is this useful for? You might create a game were you need to have multiple choices for every 3 children in 
    /// a gameObject. You can create multiple assigners with 3 elements (options/choices) each, and this will shuffle the 
    /// answers.
	/// </summary>
	[ExecuteInEditMode]
	public class ChildrenRandomTextsAssigner : ChildrenRandomAssigner<string, Text> 
    {		
		[System.Serializable]
		public class RandomTextAssigner : RandomAssigner 
        {
			/// <summary>
			/// Randomly assignes text values to the specified Texts without repeating assigned values.
			/// Returns the assigned text values.
			/// </summary>
			/// <param name="pathsPrefix">The sprites path prefix.</param>
            /// <param name="texts">Texts.</param>
			public override List<string> Assign( PathHolder.Index pathsPrefix, List<Text> texts )
			{
                List<string> assigned = new List<string>();
                //Create shuffled list of texts and set their values
                List<Text> shuffledTexts = texts.GetRandoms( texts.Count );

                for( int i=0, j=0; i<shuffledTexts.Count; i++ )
                {
                    _Assign( shuffledTexts[i], elements[j] );
                    assigned.Add( elements[j] );
                    j++;
                    if( assigned.Count == _AssignLimit )
                    {
                        break;
                    }
                }
				return assigned;
			}
			
			protected override void _Assign( Text behaviour, string element )
			{
				behaviour.text = element;
				if( nameObjsAsElement )
				{
					behaviour.name = element;
				}
			}	
		}
		
		

		public RandomTextAssigner[] assigners = new RandomTextAssigner[2];
		

		
		protected override void _Start()
		{
			base._Start();

			if( m_children.Count == 0 )
			{
                Utilities.LogWarning ("There are no components in the children");
				return;
			}

			for( int i=0; i<assigners.Length; i++ )
			{
                m_assigned.AddRange( assigners[i].Assign( pathsPrefix, m_children.GetRange( m_assigned.Count, assigners[i].elements.Count ) ) );
			}
		}		
		/// <summary>
		/// This gets called before assigning the random elements.
		/// </summary>
		protected override void PreAssignment()
		{
			for( int i=0; i<m_children.Count; i++ )
			{
				m_children[i].text = "";
			}
		}
	}	
}