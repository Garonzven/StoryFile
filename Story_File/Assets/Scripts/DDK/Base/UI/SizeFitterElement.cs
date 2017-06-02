//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;


namespace DDK.Base.UI
{
    /// <summary>
    /// Allows extra setup in SizeFitter components.
    /// </summary>
    /// <see cref="SizeFitter"/>
    [RequireComponent( typeof( RectTransform ) )]
    public class SizeFitterElement : MonoBehaviour 
    {
        public bool ignoreParentSizeFitter;


        void Start () //Allows enabling/disabling this component
        {

        }
    }

}