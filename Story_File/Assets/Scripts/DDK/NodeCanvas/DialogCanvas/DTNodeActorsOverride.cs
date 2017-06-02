#if USE_NODE_CANVAS
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using DDK.Base.Extensions;


namespace DDK.NodeCanvas.DialogCanvas
{
    /// <summary>
    /// Base class for DialogueTree nodes that can live within a DialogueTree Graph.
    /// </summary>
    abstract public class DTNodeActorsOverride : DTNode {

        public override string name{
            get
            {
                if( dialogueActors.Contains( actorName ) )
                {
                    return string.Format( "#{0} {1}", ID, actorName );
                }
                return string.Format( "#{0} <color="+ ColorUtility.ToHtmlStringRGB( new Color( 1f, 0.5f, 0f ) ) +">* {1} *</color>", ID, actorName );//#d63e3e
            }
        }

        protected static DialogueActor[] _dialogueActors;
        private static DialogueTree _lastDialogueTree;

		protected void FindActors()
		{
			if( _lastDialogueTree == DialogueTree.currentDialogue && _lastDialogueTree != null)
                return;
			_dialogueActors = GameObject.FindObjectsOfType<DialogueActor> ();
            _lastDialogueTree = DialogueTree.currentDialogue;
		}

		protected IDialogueActor overridenActor{
			get
			{
				for (int i = 0; i < _dialogueActors.Length; i++)
					if (_dialogueActors [i].name == actorName)
						return _dialogueActors [i];
				return finalActor;
			}
		}

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////

        List<string> dialogueActors = new List<string>(){ "INSTIGATOR" };

#if UNITY_EDITOR        
        double _lastOnGUI;
        

        protected override void OnNodeInspectorGUI()
		{             
            //BASE OVERRIDE
            if( _lastOnGUI + 5 < UnityEditor.EditorApplication.timeSinceStartup )
            {
                _lastOnGUI = UnityEditor.EditorApplication.timeSinceStartup;
				FindActors ();
				for( int i=0; i<_dialogueActors.Length; i++ )
                {
					if( dialogueActors.Contains( _dialogueActors[i].name ) )
                        continue;
					dialogueActors.Add( _dialogueActors[i].name );
                }
            }
            GUI.backgroundColor = EditorUtils.lightBlue;
            actorName = EditorUtils.StringPopup(actorName, dialogueActors, false, false);
            GUI.backgroundColor = Color.white;
        }
#endif
    }
}
#endif
