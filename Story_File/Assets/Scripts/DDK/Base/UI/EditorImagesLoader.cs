//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;
using UnityEngine.UI;


namespace DDK.Base.UI 
{
	/// <summary>
	/// Editor Only. Attach to an object with children containing images, to assign the sprites on the specified path to them. NOTE: 
	/// The sprites count in the specified path must be equal to the children count.
	/// </summary>
	[ExecuteInEditMode]
	public class EditorImagesLoader : MonoBehaviour 
	{		
		public bool loadOnChildren = true;
		public string childrenSpritesPath = "Sprites";
        [Tooltip("If true, the images will be reassigned to the children if /loadOnChildren/ is true")]
		public bool reset;


		protected bool _assigned;				
		
		
		// Use this for initialization
		void Start () { }//Alows enabling/disabling this component
		
		// Update is called once per frame
		void Update () 
		{			
#if UNITY_EDITOR
			if( enabled && !Application.isPlaying )
			{
				if( reset )
				{
					reset = false;
					_assigned = false;
					Debug.Log ( "Images Reassigned!" );
				}
				AssignImages();
			}
#endif
		}				
		
		
        /// <summary>
        /// Assigns the images in the specified paths to this gameObjects Image children.
        /// </summary>
		public void AssignImages()
		{
			if( loadOnChildren && !_assigned )
			{
                Sprite[] sprites = Resources.LoadAll<Sprite>( childrenSpritesPath );
                GameObject[] children = gameObject.GetChildren();
				if( sprites.Length == children.Length )
				{
                    Image img;//caching
					for( int i=0; i<children.Length; i++ )
					{
						img = children[i].GetComponent<Image>();
						img.sprite = sprites[i];
						img.name = sprites[i].name;
					}
					_assigned = true;
				}
			}
		}		
	}
}
