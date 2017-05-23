using UnityEngine;
using System.Collections;
using DDK.Base.Fx.Transitions;
using DDK.Base.Classes;


namespace DDK.Base.Extensions
{
    /// <summary>
    /// StateMachine class extension.
    /// </summary>
    public class StateMachineBehaviourExt : StateMachineBehaviour 
    {   
        public virtual void LoadLevelAsync( string levelName )
        {
            AutoFade.LoadLevelAsync( levelName );
        }
        /// <summary>
        /// Show the level that was loaded using this class's LoadLevelAsync().
        /// </summary>
        public void ShowAsyncLoadedLevel()
        {
            if( string.IsNullOrEmpty( AutoFade._lastLoadedLevel ) )
                return;
            AutoFade.ShowLastAsyncLoadedLevel();
        }
        /// <summary>
        /// Show the level that was loaded using this class's LoadLevelAsync().
        /// </summary>
        public void ShowAsyncLoadedLevel( float delay )
        {
            if( string.IsNullOrEmpty( AutoFade._lastLoadedLevel ) )
                return;
            AutoFade.ShowLastAsyncLoadedLevel( delay );
        }
    }
}
