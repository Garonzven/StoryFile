//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if USE_SVG_IMPORTER
using SVGImporter;
#endif


namespace DDK.Base.Classes 
{
    /// <summary>
    /// This class represents a Graphic, and it can be used to reference a Graphic without losing its reference since
    /// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
    /// </summary>
	[System.Serializable]
	public class SearchableGraphic : SearchableBehaviour<Graphic> 
	{
		public SearchableGraphic(){}
		public SearchableGraphic( string objName )
		{
			if( !string.IsNullOrEmpty( objName ) )
				base.objName = objName;
		}


        #if USE_SVG_IMPORTER
        private SVGAsset _vectorGraphics;
        /// <summary>
        /// Do not call this in a loop if the /m_object/'s reference might get lost.
        /// </summary>
        public SVGAsset vectorGraphics {
            get{
                if( !m_object )
                    return default( SVGAsset );
                if( !(m_object is SVGImage) )
                {
                    Debug.LogError( "The specified graphic is not a SVGImage", m_object );
                }
                if( _vectorGraphics == null )
                    _vectorGraphics = (m_object as SVGImage).vectorGraphics;
                return _vectorGraphics;
            }
            set{
                if( !m_object )
                    return;
                if( !(m_object is SVGImage) )
                {
                    Debug.LogError( "The specified graphic is not a SVGImage", m_object );
                    return;
                }
                (m_object as SVGImage).vectorGraphics = value;
            }
        }
        private SVGFrameAnimator _svgFrameAnimator;
        /// <summary>
        /// Do not call this in a loop if the /m_object/'s reference might get lost.
        /// </summary>
        public SVGFrameAnimator svgFrameAnimator {
            get{
                if( !m_object )
                    return null;
                if( _svgFrameAnimator == null )
                {
                    _svgFrameAnimator = m_transform.GetComponent<SVGFrameAnimator>();
                    if( _svgFrameAnimator == null )
                    {
                        Debug.LogError( "The specified graphic has no a SVGFrameAnimator component attached", m_object );
                    }
                }
                return _svgFrameAnimator;
            }
            set{
                _svgFrameAnimator = value;
            }
        }
        #endif
	}
}