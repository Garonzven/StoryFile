using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Object = UnityEngine.Object;

namespace DDK.Base.Classes
{
    /// <summary>
    /// Allows filtering gameObjects by type or tag.
    /// </summary>
    [Serializable]
    public class FilteredGameObjectList
    {
        [SerializeField]
        protected List<GameObject> _filteredList;
        public string byType;
        public string byTag;

        public List<GameObject> FilteredList
        {
            get
            {
                if ( _filteredList != null && _filteredList.Count > 0 )
                {
                    return _filteredList;
                }

                _filteredList = new List<GameObject>();

                if ( !string.IsNullOrEmpty ( byType ) )
                {
                    var typeArray = Object.FindObjectsOfType
                                    (
                                        System.Reflection.Assembly.
                                        GetExecutingAssembly()
                                        .GetType ( byType, true )
                                    ) as GameObject[];

                    if ( typeArray != null )
                    {
                        _filteredList.AddRange ( typeArray );
                    }
                }

                if ( !string.IsNullOrEmpty ( byTag ) )
                {
                    var tagArray = GameObject.FindGameObjectsWithTag ( byTag );

                    if ( tagArray != null )
                    {
                        if ( _filteredList.Count > 0 )
                        {
                            _filteredList = _filteredList.Intersect ( tagArray ).ToList();
                        }
                        else
                        {
                            _filteredList = tagArray.ToList();
                        }
                    }
                }

                return _filteredList;
            }
        }

        public FilteredGameObjectList()
        {
            _filteredList = null;
        }
    }
}
