//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEngine.UI;
using DDK.Base.Extensions;


namespace DDK.Base.UI
{
    /// <summary>
    /// This will Fit the cells depending on the grid's size, instead of keeping the cells size they will be automatically 
    /// resized to fit inside the grid.
    /// </summary>
    [RequireComponent( typeof( GridLayoutGroup ) )]
    [ExecuteInEditMode]
    public class GridLayoutCellsFitter : MonoBehaviour 
    {
        public bool tryKeepSquare = true;
        [NotLessThan( 2 )]
        public int maxColumns = 4;
        [NotLessThan( 2 )]
        public int maxRows = 4;
        [Tooltip("If true, the cell count will be dynamic, meaning that cells can be added or removed and the grid should readjust automatically in runtime")]
        public bool updateCellCount;


        protected GridLayoutGroup _grid;
        protected RectTransform _rt;
        protected int _cellCount;
        protected int _PreferredColumns 
        {
            get
            {
                return _GetPreferredSide( maxColumns );
            }
        }
        protected int _PreferredRows 
        {
            get
            {
                return _GetPreferredSide( maxRows );
            }
        }


        void Awake()
        {
            _grid = GetComponent<GridLayoutGroup>();
            _rt = GetComponent<RectTransform>();
        }
        // Use this for initialization
        void Start () {

            _CalculateCellCount();
        }
        // Update is called once per frame
        void Update () {

            #if UNITY_EDITOR
            if( !Application.isPlaying )
            {
                _UpdateEditor();
                return;
            }
            #endif

            if( updateCellCount )
                _CalculateCellCount();
            _grid.cellSize = _GetCellSize();
        }


        /// <summary>
        /// Updates the cells count and the grid's size.
        /// </summary>
        public void UpdateCellsCountAndSize()
        {
            _CalculateCellCount();
            _grid.cellSize = _GetCellSize();
        }


        /// <summary>
        /// Gets the preferred side (side refers to columns or rows).
        /// </summary>
        /// <param name="clampTo">This should be the max number of -side- (columns or rows).</param>
        private int _GetPreferredSide( int clampTo )
        {
            if( tryKeepSquare && _cellCount > 1 )
            {
                int i = 2;
                while( true )
                {
                    if( i * i >= _cellCount )
                    {
                        return i.Clamp( 2, clampTo );
                    }
                    i++;
                }
            }
            return clampTo;
        }

        protected void _UpdateEditor()
        {
            if( !enabled )
                return;
            if( !_grid || !_rt )
            {
                Awake();
            }
            //FIX ALL VALUES
            _rt.anchorMax = 1f.GetVector2();
            _rt.anchorMin = 0f.GetVector2();
            _rt.pivot = 0.5f.GetVector2();
            _grid.constraint = GridLayoutGroup.Constraint.Flexible;

            _CalculateCellCount();
            _grid.cellSize = _GetCellSize();
        }
        protected void _CalculateCellCount()
        {
            _cellCount = gameObject.ChildCountActiveOnly();
            var ignoredChildren = GetComponentsInChildren<LayoutElement>();
            for( int i=0; i<ignoredChildren.Length; i++ )
            {
                if( ignoredChildren[ i ].ignoreLayout )
                    _cellCount--;
            }
        }
        protected Vector2 _GetCellSize()
        {
            float totalHorizontalSpace = _grid.padding.horizontal + _grid.spacing.x * ( _PreferredColumns - 1 );
            float totalCellsSpaceX = _rt.rect.width - totalHorizontalSpace;
            float x = totalCellsSpaceX / _PreferredColumns;
            float totalVerticalSpace = _grid.padding.vertical + _grid.spacing.y * ( _PreferredRows - 1 );
            float totalCellsSpaceY = _rt.rect.height - totalVerticalSpace;
            float y = totalCellsSpaceY / _PreferredRows;
            if( tryKeepSquare )
            {
                if( x < y )
                {
                    y = x;
                }
                else if( y < x )
                {
                    x = y;
                }
            }
            //TODO
            /*else
            {
                if( x < y )
                {
                   x = y;
                }
                else if( y < x )
                {
                    y = x;
                }
            }*/
            return new Vector2( x, y );
        }
    }
}
