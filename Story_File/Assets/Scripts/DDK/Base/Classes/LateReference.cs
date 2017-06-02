using System;
using System.Linq;

using DDK.Base.Extensions;

using UnityEngine;

using Object = UnityEngine.Object;

namespace DDK.Base.Classes
{
    /// <summary>
    /// Obtains a transform reference once <see cref="LateReference.Reference"/>
    /// is called. This is useful to obtain objects during runtime that cannot
    /// be directly reference through the editor.
    /// </summary>
    [Serializable]
    public class LateReference
    {
        /// <summary>
        /// The lookup objects can be filtered by the a <see cref="Component"/>
        /// the <see cref="_reference"/> has.
        /// </summary>
        protected Type _discriminant = typeof ( Transform );

        /// <summary>
        /// The direct reference
        /// </summary>
        [Header ( "Late References" ), SerializeField]
        private Transform _reference;

        /// <summary>
        /// A reference to a component to be filtered as one
        /// of the <see cref="_reference"/> components
        /// </summary>
        public Component componentReference;

        /// <summary>
        /// Filter by object name
        /// </summary>
        [Header ( "Query Parameters" )]
        public string gameObjectName;

        /// <summary>
        /// Filter by tag
        /// </summary>
        public string findWithTag;

        /// <summary>
        /// Filter within layer
        /// </summary>
        public LayerMask findInLayer = ~0; // default = Everything

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>
        /// The reference.
        /// </value>
        public Transform Reference
        {
            get
            {
                if ( _reference != null )
                {
                    return _reference;
                }

                if ( componentReference != null )
                {
                    _discriminant = componentReference.GetType();
                }

                if ( string.IsNullOrEmpty ( gameObjectName ) )
                {
                    return _reference;
                }

                Component[] gArray = null;
                Component[] t1 = null, t2 = null;

                if ( !string.IsNullOrEmpty ( findWithTag ) )
                {
                    t1 = GameObject.FindGameObjectsWithTag ( findWithTag )
                         .Select ( t => t.transform as Component ).ToArray();
                }

                if ( componentReference != null )
                {
                    t2 = Object.FindObjectsOfType ( _discriminant ) as
                         Component[];
                }

                if ( t1 != null && t2 != null )
                {
                    gArray = t1.Join ( t2,
                                       j => j.gameObject,
                                       k => k.gameObject,
                                       ( j, k ) => j.GetComponent ( _discriminant )
                                     ).ToArray();
                }
                else if ( t1 == null && t2 != null )
                {
                    gArray = t2;
                }
                else if ( t1 != null )
                {
                    gArray = t1;
                }
                else
                {
                    gArray = Object.FindObjectsOfType ( _discriminant ) as
                             Component[];
                }

                componentReference = gArray.ToList().Find ( g =>
                {
                    if ( g.gameObject.IsInLayerMask ( findInLayer )
                            && g.name == gameObjectName )
                    {
                        return true;
                    }

                    return false;
                } );
                findInLayer = 1 << componentReference.gameObject.layer;
                findWithTag = componentReference.tag;
                return _reference = componentReference.transform;
            }
            set
            {
                if ( componentReference == null )
                { componentReference = value.transform; }

                if ( string.IsNullOrEmpty ( findWithTag ) )
                { findWithTag = value.tag; }

                if ( string.IsNullOrEmpty ( gameObjectName ) )
                { gameObjectName = value.name; }

                findInLayer = 1 << value.gameObject.layer;
                _reference = value;
            }
        }

        public bool IsReferenceSet
        {
            get { return _reference != null; }
        }

        public void ComponentFilter<T>() where T : Behaviour
        {
            _discriminant = typeof ( T );
        }
    }
}