//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DDK.Base.Extensions;
using DDK.Base.Misc;
using DDK.Base.Statics;


namespace DDK.Base.UI 
{
	/// <summary>
	/// Attach to an object with children image objects to randomly assign sprites to them (without repeating the 
    /// assigned sprites). This is done in order, E.g: Imagine an array of 2 assigners, the first with 2 elements 
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
	/// </summary>
	[ExecuteInEditMode]
	public class ChildrenRandomSpritesAssigner : MonoBehaviour 
    {
		[System.Serializable]
		public class RandomAssigner
        {
			[Tooltip("If not empty, the below path won't be used")]
			public List<Sprite> sprites = new List<Sprite>();
			[ShowIfAttribute( "_IsSpritesEmpty" )]
			public string spritePath = "Sprites/1";
			[SerializeField]
			[Tooltip("The amount of sprites to assign. This will be clamped to the /sprites/ count")]
			private int assignLimit = 2;
			public bool nameObjsAsSprite;


            #if UNITY_EDITOR
			protected bool _IsSpritesEmpty()
			{
				return sprites.Count == 0;
			}
            #endif


			public int AssignLimit 
            {
                get //PREVENT for FROM LOOPING FOREVER CAUSING UNITY TO CRASH
                {					
                    return assignLimit = assignLimit.Clamp( 0, sprites.Count );
				}
			}


			/// <summary>
			/// Randomly assignes sprites from the /pathPrefix/ + /spritesPath/ to the specified imgs without repeating assigned sprites.
			/// Returns the assigned sprites.
			/// </summary>
			/// <param name="pathsPrefix">The sprites path prefix, used just if the sprites list is empty.</param>
			/// <param name="imgs">Imgs.</param>
			public List<Sprite> Assign( PathHolder.Index pathsPrefix, List<Image> imgs )
			{
				List<Sprite> assigned = new List<Sprite>();
				if( sprites.Count == 0 )
				{
					string prefix = "";
					if( pathsPrefix.isValid )
					{
						prefix = pathsPrefix.path;
					}
					sprites = Resources.LoadAll<Sprite>( prefix + spritePath ).ToList();
				}
                //Create shuffled list of images and assign the sprites to them.
                List<Image> shuffledImgs = imgs.GetRandoms( imgs.Count );

                for( int i=0, j=0; i<shuffledImgs.Count; i++ )
                {
                    if( shuffledImgs[i].sprite != null )
                    {
                        continue;
                    }
                    _Assign( shuffledImgs[i], sprites[j] );
                    assigned.Add( sprites[j] );
                    j++;
                    if( assigned.Count == AssignLimit )
                    {
                        break;
                    }
                }
				return assigned;
			}

			protected void _Assign( Image img, Sprite sprite )
			{
				img.sprite = sprite;
				if( nameObjsAsSprite )
				{
					img.name = sprite.name;
				}
			}
		}


		public bool executeOnceInEditMode;
		[Tooltip("If true, this will be done in the Awake() instead of the Start()")]
		public bool onAwake;
		[Tooltip("If true, the children of the children will be used instead")]
		public bool useSubChildrenInstead;
		[Tooltip("This is added to the spritesPath")]
        public PathHolder.Index pathsPrefix = new PathHolder.Index();
		public RandomAssigner[] assigners = new RandomAssigner[2];


		/// <summary>
		/// A list of all the sprites that were assigned.
		/// </summary>
		internal List<Sprite> m_assigned = new List<Sprite>();
		/// <summary>
		/// All Image components in children.
		/// </summary>
		internal List<Image> m_children = new List<Image>();
		public static List<Sprite> m_excludeFromAssigners = new List<Sprite>();


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
		protected void _Start()
		{
			if( useSubChildrenInstead )
			{
				List<Image> children = gameObject.GetCompsInChildren<Image>();
				for( int i=0; i<children.Count; i++ )
				{
					m_children.AddRange( children[i].gameObject.GetCompsInChildren<Image>() );
				}
			}
			else m_children.AddRange( gameObject.GetCompsInChildren<Image>() );
			for( int i=0; i<m_children.Count; i++ )
			{
				m_children[i].sprite = null;
			}
			for( int i=0; i<assigners.Length; i++ )
			{
                m_assigned.AddRange( assigners[i].Assign( pathsPrefix, m_children.GetRange( m_assigned.Count, assigners[i].sprites.Count ) ) );
			}
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
				Utilities.Log ( "Random Sprites Assigned!" );
			}
#endif
		}
	}
}
