//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace DDK.Base.UI
{
    /// <summary>
    /// This will automatically set a layout's size so it fits its content. This can be useful if you need a dynamic 
    /// layout to fit properly inside a Scroll Rect.
    /// </summary>
    [ExecuteInEditMode]
    public class LayoutSizeFitter : MonoBehaviour 
    {       
        public bool executeInEditMode;
        public LayoutGroup layoutGroup;
        public bool height;
        public bool width;


        RectTransform _rTransform;
        VerticalLayoutGroup _vLayoutGroup
        {
            get
            {
                return (VerticalLayoutGroup) layoutGroup;
            }
        }
        HorizontalLayoutGroup _hLayoutGroup
        {
            get
            {
                return (HorizontalLayoutGroup) layoutGroup;
            }
        }


        // Use this for initialization
        IEnumerator _OnEnable () 
        {
            if( !layoutGroup )
            {
                layoutGroup = GetComponentInChildren<LayoutGroup>();
                if( !layoutGroup )
                {
                    Debug.LogWarning ("No Layout was found", gameObject );
                    enabled = false;
                    yield break;
                }
            }
            _rTransform = (RectTransform) layoutGroup.transform;
            yield return new WaitForSeconds( 0.1f );
            Fit();
        }
        void OnEnable()
        {
            StartCoroutine( _OnEnable() );
        }
        #if UNITY_EDITOR
        void Update()
        {
            if( Application.isPlaying || !executeInEditMode || !enabled || 
                ( _rTransform && _rTransform.childCount == 0 ) || !layoutGroup )
                return;
            _rTransform = (RectTransform) layoutGroup.transform;
            Fit();
        }
        #endif


        public void Fit()
        {
            if( !enabled )
            {
                Debug.LogWarning ("This has been disabled", gameObject );
                return;
            }
            float x = _rTransform.sizeDelta.x;
            //float y = _rTransform.sizeDelta.y; TODO
            if( height )
            {
                Vector2 temp = _rTransform.anchorMin;
                temp.y = 1f;
                _rTransform.anchorMin = temp;
                temp = _rTransform.anchorMax;
                temp.y = 1f;
                _rTransform.anchorMax = temp;
                temp = _rTransform.pivot;
                temp.y = 1f;
                _rTransform.pivot = temp;
                _rTransform.sizeDelta = new Vector2( x, GetProperHeight() );
            }
            //TODO
            /*if( width )
            {
                Vector2 temp = _rTransform.anchorMin;
                temp.x = 0f;
                _rTransform.anchorMin = temp;
                temp = _rTransform.anchorMax;
                temp.x = 0f;
                _rTransform.anchorMax = temp;
                temp = _rTransform.pivot;
                temp.x = 0f;
                _rTransform.pivot = temp;
                _rTransform.sizeDelta = new Vector2( GetProperWidth(), y );
            }*/
        }
        public float GetProperHeight()
        {
            int rows = 1;
            if( _rTransform.childCount > 0 )
            {
                rows = _rTransform.childCount;
            }
            float totalSpacing = rows - 1;
            if( _vLayoutGroup )
            {
                totalSpacing *= _vLayoutGroup.spacing;
            }
            float h = layoutGroup.padding.top + layoutGroup.padding.bottom + totalSpacing;
            RectTransform child;//caching
            LayoutElement layoutElement;//caching
            for( int i=0; i<rows; i++ )
            {
                child = (RectTransform) _rTransform.GetChild( i );
                if( child == null )
                {
                    Debug.LogWarning( string.Format( "Index ({0}): Transform has no more children, breaking for-loop", i ), gameObject );
                    break;
                }
                layoutElement = child.GetComponent<LayoutElement>();
                if( layoutElement && layoutElement.enabled )
                {
                    if( layoutElement.minHeight > 0 )
                    {
                        h += layoutElement.minHeight;
                    }
                    else if( layoutElement.preferredHeight > 0 )
                    {
                        h += layoutElement.preferredHeight;
                    }
                    if( layoutElement.flexibleHeight > 0 )
                    {
                        h += layoutElement.flexibleHeight;
                    }
                }
                else h += child.sizeDelta.y;
            }
            return h;
        }
        //TODO
        /*public float GetProperWidth()
        {
            float totalSpacing = 1f;
            int columns = 1;
            if( layoutGroup.constraint == LayoutGroup.Constraint.FixedRowCount )
            {
                columns = _childrenCount / layoutGroup.constraintCount;
            }
            else if( layoutGroup.constraint == LayoutGroup.Constraint.FixedColumnCount )
            {
                columns = layoutGroup.constraintCount;
            }
            totalSpacing = ( columns - 1 ) * layoutGroup.spacing.x;
            return layoutGroup.padding.left + layoutGroup.padding.right + ( layoutGroup.cellSize.x * columns ) + totalSpacing;
        }*/
    }
}