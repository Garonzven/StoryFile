using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.ObjManagement
{
    /// <summary>
    /// Moves a gameObject in hierachy swaping its parent
    /// to another gameObject. The swaping happens on
    /// Disable and Enable events.
    /// </summary>
    public class SwapChildren : MonoBehaviour
    {
        public bool swapOnStart = true;
        public bool swapOrigin = false;
        public Transform origin;
        public Transform destination;
        [Tooltip("If enabled will swap the parent " +
            "of origin instead of its children")]

        private GameObject[] _originChildren;
        // Use this for initialization
        void Start ()
        {
            if ( !origin )
            {
                enabled = false;
                return;
            }

            if ( swapOrigin || origin.childCount == 0 )
            {
                _originChildren = new[] { origin.gameObject };
            }
            else
            {
                _originChildren = new GameObject[origin.childCount];

                for ( int index = 0; index < origin.childCount; index++ )
                {
                    _originChildren[index] = origin.GetChild ( index ).gameObject;
                }
            }

            if ( swapOnStart )
            {
                OnEnable ();
            }
        }

        /// <summary>
        /// Moves children from origin to destination
        /// </summary>
        void OnEnable()
        {
            if ( _originChildren == null )
            { return; }

            for ( int index = 0; index < _originChildren.Length; index++ )
            {
                _originChildren[index].transform.SetParent ( destination );
            }
        }

        /// <summary>
        /// Moves children from destination to origin
        /// </summary>
        void OnDisable()
        {
            if ( _originChildren == null )
            { return; }

            //TO PREVENT THIS ERROR: Cannot change GameObject hierarchy while activating or deactivating the parent.
            //Add delay..
            _SetParents().Start();
        }



        protected IEnumerator _SetParents()
        {
            yield return null;
            for ( int index = 0; index < _originChildren.Length; index++ )
            {
                _originChildren[index].transform.SetParent ( origin );
            }
        }
    }

}