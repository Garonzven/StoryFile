using UnityEngine;
using UnityEditor;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

namespace DDK
{
    [InitializeOnLoad]
    public class AutosaveOnRun
    {
        private enum AutosaveEnum
        {
            Always,
            Never,
            Ask
        }
        const string AUTOSAVE = "autosave";


        static AutosaveOnRun()
        {
            AutoSavePrefs();
            if( EditorPrefs.GetInt( AUTOSAVE, -1 ) == (int) AutosaveEnum.Never )
                return;
            EditorApplication.playmodeStateChanged = () =>
            {
                if( 
                    #if UNITY_5_3_OR_NEWER
                    EditorSceneManager.GetActiveScene().isDirty && 
                    #else
                    EditorApplication.isSceneDirty &&
                    #endif
                    EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
                {
                    if( EditorPrefs.GetInt( AUTOSAVE, -1 ) == (int) AutosaveEnum.Always || 
                        ( EditorPrefs.GetInt( AUTOSAVE, -1 ) == (int) AutosaveEnum.Ask && EditorUtility.DisplayDialog("Save", "Save Current Modified Scenes ?", "Save", "Don't Save") ) )
                        //if( EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() ) //BUG: WHEN SELECTING DON'T SAVE THIS RETURNS TRUE....
                    {
                        #if UNITY_5_3_OR_NEWER
                        Debug.Log("Auto-Saving scene before entering Play mode: " + EditorSceneManager.GetActiveScene() );
                        EditorSceneManager.SaveOpenScenes();
                        #else
                        Debug.Log("Auto-Saving scene before entering Play mode: " + EditorApplication.currentScene);
                        EditorApplication.SaveScene();
                        #endif
                        AssetDatabase.SaveAssets();
                    }
                }
            };
        }
        static void AutoSavePrefs()
        {
            if( EditorPrefs.GetInt( AUTOSAVE, -1 ) == -1 )
            {
                int autosave = EditorUtility.DisplayDialogComplex("Auto-Save Preferences", "Do you want to auto-save all scenes before entering play mode ?" +
                    " \n\nYou can change the settings anytime in the menu bar Custom/Preferences/Autosave..", "Always", "Never", "Ask");
                EditorPrefs.SetInt( AUTOSAVE, autosave );
            }
        }
        [MenuItem( "Custom/Preferences/Autosave.." )]
        static void AutoSavePreferences()
        {
            int autosave = EditorUtility.DisplayDialogComplex("Auto-Save Preferences", "Do you want to auto-save all scenes before entering play mode ?", "Always", "Never", "Ask");
            EditorPrefs.SetInt( AUTOSAVE, autosave );
        }
    }

}