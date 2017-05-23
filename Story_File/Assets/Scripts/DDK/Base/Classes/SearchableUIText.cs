//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine.UI;


namespace DDK.Base.Classes 
{
    /// <summary>
    /// This class represents a Text, and it can be used to reference a Text without losing its reference since
    /// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
    /// </summary>
	[System.Serializable]
	public class SearchableUIText : SearchableBehaviour<Text> 
	{
		public SearchableUIText(){}
		public SearchableUIText( string objName )
		{
			if( !string.IsNullOrEmpty( objName ) )
				base.objName = objName;
        }
		public SearchableUIText( Text obj )
		{
			if( obj )
				base.m_object = obj;
		}


		internal string text {
			get{
				if( !m_object )
					return null;
				return m_object.text;
			}
			set{
				m_object.text = value;
			}
		}
    }
}