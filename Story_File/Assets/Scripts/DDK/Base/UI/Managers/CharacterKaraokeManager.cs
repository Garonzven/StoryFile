using UnityEngine;
using UnityEngine.UI;

namespace DDK.Base.UI.Managers
{
    /// <summary>
    ///     It associates a character name string to the KareokeManager
    ///     set that name in the nameTextHolder.
    ///     Also see <see cref="KaraokeManager" />
    /// </summary>
    public class CharacterKaraokeManager : KaraokeManager
    {
        /// <summary>
        ///     Character name string to set into characterNameHolder
        /// </summary>
        [Header ( "Kareoke From - Character Name" )]
        public string activeName = "";
        /// <summary>
        ///     UI element that holds the current character name
        /// </summary>
        public Text nameTextHolder;


        private new void OnEnable()
        {
            // execute kareoke manager OnEnable
            base.OnEnable();

            // associate character name
            if ( nameTextHolder != null )
            {
                nameTextHolder.text = activeName;
            }
        }
    }
}