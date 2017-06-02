using DDK.Base.Extensions;
using UnityEngine;
using DDK.Base.Classes;


namespace DDK.ObjManagement 
{
    /// <summary>
    /// Apply random scale (at runtime) to an object, or its children.
    /// </summary>
    public class ApplyRandomScale : MonoBehaviour 
    {
        [Tooltip("If false, to this object instead.")]
        public bool toChildren = true;
        [ShowIfAttribute( "toChildren, 1" )]
        public bool includeSubChildren;
        [ShowIfAttribute( "toChildren, 1" )]
        public bool includeInactive;
        [Tooltip("If an axis's value is higher than 0 the random scale will be applied to that axis.")]
        public Vector3 axis = new Vector3( 1f, 1f, 0f );
        public RandomRangeFloat scaleRange = new RandomRangeFloat( 0.5f, 1.5f );
        public bool onAwake;


        void Awake()
        {
            if( onAwake )
            {
                Apply();
            }
        }

        // Use this for initialization
        void Start ()
        {
            if( !onAwake )
            {
                Apply();
            }
        }


        protected void Apply()
        {
            if( toChildren )
            {
                CalculateApplyToChildren();
            }
            else CalculateApplyToThis();
        }
        protected void CalculateApplyToThis()
        {
            gameObject.ApplyRandomScale( axis, scaleRange.from, scaleRange.to );
        }
        protected void CalculateApplyToChildren()
        {
            var children = gameObject.GetChildren( includeSubChildren, includeInactive );
            for( int i=0; i<children.Length; i++ )
            {
                children[i].ApplyRandomScale( axis, scaleRange.from, scaleRange.to );
            }
        }
    }
}