//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace DDK.Base.Classes 
{
    /// <summary>
    /// This class represents a Button, and it can be used to reference a Button without losing its reference since
    /// this keeps the name of the object so it can be searched in case there is no reference. NOTE: The name must be unique
    /// </summary>
    [System.Serializable]
    public class SearchableButton : SearchableBehaviour<Button> 
    {
        public SearchableButton(){}
        public SearchableButton( string objName )
        {
            if( !string.IsNullOrEmpty( objName ) )
                base.objName = objName;
        }
    }
}