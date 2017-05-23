//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.UI;


namespace DDK.Base.Classes 
{
	[System.Serializable]
	public class When
    {
		[Tooltip("If true, the condition is fulfulled")]
		public bool thisIsTrue;
		[Space(10f)]
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
		[Tooltip("All must be inactive for this condition to be fulfilled")]
		public GameObject[] whenInactive;
		[Tooltip("All must be active for this condition to be fulfilled")]
		public GameObject[] whenActive;
		[Tooltip("If this object has a sprite (SpriteRenderer or Image) and its alpha is lower than the specified, this condition is fulfilled")]
		public float whenSpriteAlphaLowerThan;


		public When() {}
		/// <summary>
		/// Initializes a new instance of the <see cref="DDK.Base.Classes.When"/> class.
		/// </summary>
		/// <param name="thisIsTrue">If true, the condition is fulfulled</param>
		public When( bool thisIsTrue )
		{
			this.thisIsTrue = thisIsTrue;
		}


		/// <summary>
		/// Determines whether this condition fulfilled.
		/// </summary>
		/// <returns><c>true</c> if the condition is fulfilled in the specified source; otherwise, <c>false</c>.</returns>
		/// <param name="source">The object containing this instance.</param>
		public bool IsConditionFulfilled ( GameObject source )
		{
			if( thisIsTrue )
			{
				return true;
			}
			if( animator != null )//ANIMATOR CONDITION
			{
				bool _play = animator.GetBool( play );
				if( isSetTo != _play )
					return true;
			}
			for( int i=0; i<whenInactive.Length; i++ )//INACTIVES CONDITION
			{
				if( whenInactive[i].IsActiveInHierarchy() )
				{
					break;
				}
				if( i == whenInactive.Length - 1 )
					return true;
			}
			for( int i=0; i<whenActive.Length; i++ )//ACTIVES CONDITION
			{
				if( !whenActive[i].IsActiveInHierarchy() )
				{
					break;
				}
				if( i == whenActive.Length - 1 )
					return true;
			}
			//SPRITE'S ALPHA CONDITION
			var ren =source. GetComponent<SpriteRenderer>();
			var img = source.GetComponent<Image>();
			if( ren )
			{
				if( ren.color.a < whenSpriteAlphaLowerThan )
				{
					return true;
				}
			}
			else if( img )
			{
				if( img.color.a < whenSpriteAlphaLowerThan )
				{
					return true;
				}
			}
			return false;
		}
	}
}
