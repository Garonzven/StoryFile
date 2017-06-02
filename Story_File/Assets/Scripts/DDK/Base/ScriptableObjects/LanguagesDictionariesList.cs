//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.UI;


namespace DDK.Base.ScriptableObjects {

    /// <summary>
    /// This should be used to ease the process of creating multi-language apps.
    /// </summary>
	[CreateAssetMenu( fileName = "Languages Dictionaries List", menuName = "Scriptable Objects/Languages Dictionaries List")] 
    public class LanguagesDictionariesList : ScriptableObject {

        public LanguagesDictionaries[] dictionaries = new LanguagesDictionaries[0];      


        public LanguagesDictionaries this[ int i ]
        {
            get{
                return dictionaries[ i ];
            }
            set{
                dictionaries[i] = value;
            }
        }

        public int Length {
            get{
                return dictionaries.Length;
            }
        }        
    }

}
