//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.Extensions;


namespace DDK.Base.UI
{
    /// <summary>
    /// This will add the immediate active children's height to calculate this gameObject's height.
    /// </summary>
	[ExecuteInEditMode]
	[RequireComponent( typeof( RectTransform ) )]
	public class SizeFitter : MonoBehaviour 
	{
		#if UNITY_EDITOR
		[HelpBoxAttribute]
		public string msg = "Fit() is called OnEnable(). This will add the immediate active children's height to calculate this gameObject's height";
		#endif
		public bool executeInEditMode;
		public bool height;
		[Tooltip("This will be added to the calculated height")]
		public float extra;


		private RectTransform _rectTransform;


		void Awake()
		{
			_rectTransform = transform as RectTransform;
		}
		// Use this for initialization
		void Start () { }//Allows to enable/disable this component
		void OnEnable()
		{
			if( !executeInEditMode )
				return;
			Fit();
		}


		#if UNITY_EDITOR
		// Update is called once per frame
		void Update () 
        {
			if( Application.isPlaying || !enabled || !executeInEditMode )
				return;
			Fit();
		}
		#endif


		public void Fit()
		{
			Transform[] children = _rectTransform.GetChildren();
			float childrenH = extra;
			for( int i=0; i<children.Length; i++ )
			{
				if( height )
				{
                    SizeFitterElement element = children[i].GetComponent<SizeFitterElement>();
                    if( element && element.enabled && element.ignoreParentSizeFitter )
                    {
                        continue;
                    }
					childrenH += children[i].GetComponent<RectTransform>().sizeDelta.y;
				}
			}
			_rectTransform.sizeDelta = new Vector2( _rectTransform.sizeDelta.x, childrenH );
		}
	}
}
