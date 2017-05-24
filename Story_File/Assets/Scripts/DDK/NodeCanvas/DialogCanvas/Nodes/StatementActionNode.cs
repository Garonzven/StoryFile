//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Extensions;
using DDK.Base.SoundFX;
using DDK.Base.Managers;


#if USE_NODE_CANVAS
using NodeCanvas.DialogueTrees;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using NodeCanvas;
#endif


namespace DDK.NodeCanvas.DialogCanvas.Nodes
{
#if USE_NODE_CANVAS
	[Name("✫ Say and Action")]
	[Description("Make the selected Dialogue Actor to talk and, afterwards, execute an Action Task for the Dialogue Actor selected." +
	             "You can make the text more dynamic by using variable names in square brackets\ne.g. [myVarName]")]
#endif
	public class StatementActionNode 
#if USE_NODE_CANVAS
	: DTNodeActorsOverride, ITaskAssignable<ActionTask> 
#endif
	{
#if USE_NODE_CANVAS
		public bool actionInParallel;
		public bool actionGoesFirst;
		//SUBS OPTIONS
		public bool showSubsBeforeExecute = true;
		public bool hideSubsOnStatementEnd;
		public bool hideSubsOnActionEnd;
		public bool showSubsOnStatementEnd;
		public bool showSubsOnActionEnd;

		public MultiAudioStatement statement = new MultiAudioStatement("This is a dialogue text");
		[SerializeField]
		private ActionTask _action;
		public float nextNodeDelay;
		
		public ActionTask action {
			get {return _action;}
			set	{_action = value;}
		}		
		public Task task {
			get {return action;}
            set {action = (ActionTask)value;}
        }
		private IBlackboard _bb;
		private byte _parallelCount;
		private bool _IsParallelDone {
			get{
				return ( _parallelCount == 2 ) ?  true : false;
			}
		}
		//private AudioSource dialogueActorAudioSource;

		protected override Status OnExecute( Component agent, IBlackboard bb )
		{
			/*if ( action == null )
				return Error("Action is null on Dialogue Action Node");*/
			FindActors ();
			if ( overridenActor == null || overridenActor.Equals(null) ){ //Added by Germain to avoid errors when changing scenes while executing node
				status = Status.Failure;
				return status;
			}

			_parallelCount = 0;//Reset in case the node gets repeated
			status = Status.Running;

			if( showSubsBeforeExecute )
				ShowSubs( true );

			_bb = bb;
			if( actionGoesFirst )
			{
				StartCoroutine( UpdateAction( overridenActor.transform ) );
			}
			else
			{
				ExecuteDialogs();

                if( actionInParallel )
				{
					StartCoroutine( UpdateAction( overridenActor.transform ) );
				}
			}
						
			return status;
		}
		
		void OnStatementFinish()
		{
			if( actionInParallel )
			{
				_parallelCount++;
			}
            else if( !actionGoesFirst )
			{
				StartCoroutine( UpdateAction( overridenActor.transform ) );
			}
			else 
			{
				Continue( nextNodeDelay );
			}

			if( _IsParallelDone )
			{
				if( hideSubsOnStatementEnd || hideSubsOnActionEnd )
				{
					ShowSubs( false );
				}
				else if( showSubsOnStatementEnd || showSubsOnActionEnd )
				{
					ShowSubs( true );
				}
				Continue( nextNodeDelay );
			}
			else if( hideSubsOnStatementEnd )
			{
				ShowSubs( false );
			}
			else if( showSubsOnStatementEnd )
			{
				ShowSubs( true );
			}
		}

		IEnumerator UpdateAction( Component actionAgent )
		{			
			if( action == null )
			{
				if( actionInParallel )
					_parallelCount++;
				else 
				{
					Continue( nextNodeDelay );
				}
				yield break;
			}
            Status actionStatus = Status.Running;//caching
            while( status == Status.Running )
			{
                try{
                    actionStatus = action.ExecuteAction( actionAgent, graphBlackboard );
                }catch( MissingReferenceException ) 
                {
                    actionStatus = Status.Failure;
                    yield break;
                }
				if( actionStatus != Status.Running )
				{
					OnActionEnd( actionStatus == Status.Success ? true : false );
                    yield break;
                }
                
                yield return null;
            }
        }
		void OnActionEnd(bool success){
			
			if( success )
			{
				if( actionInParallel )
				{
					_parallelCount++;
				}
				else if( actionGoesFirst )
				{
					ExecuteDialogs();
				}
				else Continue( nextNodeDelay );

				if( _IsParallelDone )
				{
					if( hideSubsOnStatementEnd || hideSubsOnActionEnd )
					{
						ShowSubs( false );
					}
					else if( showSubsOnStatementEnd || showSubsOnActionEnd )
					{
						ShowSubs( true );
					}
					Continue( nextNodeDelay );
				}
				else if( hideSubsOnActionEnd )
				{
					ShowSubs( false );
				}
				else if( showSubsOnActionEnd )
				{
					ShowSubs( true );
				}
				return;
			}
			
			status = Status.Failure;
			DLGTree.Stop( false );
		}
		void ShowSubs( bool show )
		{
			if( KaraokeDialogueUI.Instance )
			{
				KaraokeDialogueUI.Instance.ShowSubtitles( show );
			}
		}
		void ExecuteDialogs()
		{
//			if ( this == null || overrActor == null || overrActor.Equals(null) ){ //Added by Germain to avoid getting error if scene changes while executing
//				return;
//			}
			if( string.IsNullOrEmpty( statement.text ) )//DON'T SHOW SUBTITLES IF THERE IS NO TEXT
			{
				float delay = 0f;
				if( statement.audio )
				{
					/*if( !dialogueActorAudioSource )
					{
						dialogueActorAudioSource = overrActor.transform.GetComponent<AudioSource>();
					}
					if( !dialogueActorAudioSource )
					{
						SfxManager.PlayClip (SfxManager.GetCurrentSource().name, statement.audio, false, null);
					}*/
                    SfxManager.PlayClip (SfxManager.GetCurrentSource().name, statement.audio, false, null);
					/*else
					{
						dialogueActorAudioSource.clip = statement.audio;
						dialogueActorAudioSource.Play();
					}*/
					delay = statement.audio.length;
					( overridenActor as DialogueActor ).TalkWithRate( true, statement.talkingRate, delay );
					statement.secondaryAudios.PlayAll( overridenActor as DialogueActor ).Start( delay );
				}
				StartCoroutine( _OnStatementFinish( delay + statement.secondaryAudios.GetTotalDuration() ) );
				return;
			}
			var tempStatement = statement.BlackboardReplace( _bb );
			DialogueTree.RequestSubtitles( new SubtitlesRequestInfo( overridenActor, tempStatement, OnStatementFinish ) );
		}
		IEnumerator _OnStatementFinish( float delay )
		{
			yield return new WaitForSeconds( delay );
			OnStatementFinish();
		}
		public void Continue( float delay )
		{
			StartCoroutine( _Continue( delay ) );
        }
		IEnumerator _Continue( float delay )
		{
			yield return new WaitForSeconds( delay );
			status = Status.Success;
			DLGTree.Continue();
		}
		
		protected override void OnReset()
		{
			if ( action != null )
                action.EndAction( null );
        }        
        public override void OnGraphPaused()
		{
            if ( action != null )
                action.PauseAction();
        }


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR		
		protected override void OnNodeGUI()
		{
			var displayText = statement.text.Length > 60? statement.text.Substring(0, 60) + "..." : statement.text;
            GUILayout.Label("\"<i> " + displayText + "</i> \"");
		}

		bool _subsFoldout;
		bool _secondaryClipsFoldout;
        #if UNITY_EDITOR
        double _lastOnGUI;
        #endif
		[SerializeField]
		List<bool> _secondaryClipsFoldouts;
		protected override void OnNodeInspectorGUI()
		{			
			base.OnNodeInspectorGUI();

			bool isStatementValid = (!string.IsNullOrEmpty( statement.text ) && statement.audio) || statement.audio;

			if( !actionGoesFirst )
				actionInParallel = UnityEditor.EditorGUILayout.Toggle( "Action In Parallel", actionInParallel );
			if( !actionInParallel )
				actionGoesFirst = UnityEditor.EditorGUILayout.Toggle( "Action Goes First", actionGoesFirst );

			_subsFoldout = UnityEditor.EditorGUILayout.Foldout( _subsFoldout, "Subtitles Options" );
			if( _subsFoldout )
			{
				UnityEditor.EditorGUI.indentLevel++;
				if( !showSubsOnStatementEnd && !showSubsOnActionEnd )
					showSubsBeforeExecute = UnityEditor.EditorGUILayout.Toggle( "Show Subs Before Execute", showSubsBeforeExecute );
				if( !hideSubsOnStatementEnd && !hideSubsOnActionEnd && !showSubsBeforeExecute )
				{
					if( !showSubsOnActionEnd && isStatementValid )
						showSubsOnStatementEnd = UnityEditor.EditorGUILayout.Toggle( "Show Subs After Statement", showSubsOnStatementEnd );
					if( !showSubsOnStatementEnd )
						showSubsOnActionEnd = UnityEditor.EditorGUILayout.Toggle( "Show Subs After Action", showSubsOnActionEnd );
				}
				if( !hideSubsOnActionEnd && isStatementValid )
					hideSubsOnStatementEnd = UnityEditor.EditorGUILayout.Toggle( "Hide Subs After Statement", hideSubsOnStatementEnd );
				if( !hideSubsOnStatementEnd )
					hideSubsOnActionEnd = UnityEditor.EditorGUILayout.Toggle( "Hide Subs After Action", hideSubsOnActionEnd );
				UnityEditor.EditorGUI.indentLevel--;
			}
			var areaStyle = new GUIStyle(GUI.skin.GetStyle("TextArea"));
			areaStyle.wordWrap = true;
			
			GUILayout.Label("Dialogue Text");
			statement.text = UnityEditor.EditorGUILayout.TextArea(statement.text, areaStyle, GUILayout.Height(100));
            statement.audio = UnityEditor.EditorGUILayout.ObjectField("Audio File", statement.audio, typeof(AudioClip), false)  as AudioClip;
			if( statement.audio )
			{
				statement.talkingRate = UnityEditor.EditorGUILayout.FloatField( "Talking Rate (s)", statement.talkingRate );
				_secondaryClipsFoldout = UnityEditor.EditorGUILayout.Foldout( _secondaryClipsFoldout, "Secondary Audio Files" );
				if( _secondaryClipsFoldout )
				{
					if( _secondaryClipsFoldouts == null || statement.secondaryAudios == null )
					{
						_secondaryClipsFoldouts = new List<bool>();
						statement.secondaryAudios = new List<DelayedAudioClip>();
					}
					bool addElement = GUILayout.Button( "Add Element" );
					if( addElement )
					{
						_secondaryClipsFoldouts.Add( true );
						statement.secondaryAudios.Add( new DelayedAudioClip() );
                    }
                    if( statement.secondaryAudios.Count > 0 ) 
					{
						for( int i=0; i<statement.secondaryAudios.Count; i++ )
						{
							UnityEditor.EditorGUI.indentLevel++;
							if( statement.secondaryAudios[i].clip )
							{
								_secondaryClipsFoldouts[i] = UnityEditor.EditorGUILayout.Foldout(
									_secondaryClipsFoldouts[i], statement.secondaryAudios[i].clip.name );
							}
							else
							{
                                _secondaryClipsFoldouts[i] = UnityEditor.EditorGUILayout.Foldout(
                                    _secondaryClipsFoldouts[i], "Element " + i.ToString() );
                            }
                            if( _secondaryClipsFoldouts[i] )
							{
								UnityEditor.EditorGUI.indentLevel++;
								statement.secondaryAudios[i].delay = UnityEditor.EditorGUILayout.FloatField(
									new GUIContent( "Delay", "If the text's lines match the statement's total audios this delay" +
										"will also affect the dialogue box's speech" ), statement.secondaryAudios[i].delay );
								statement.secondaryAudios[i].clip = UnityEditor.EditorGUILayout.ObjectField(
									"Audio File", statement.secondaryAudios[i].clip, typeof(AudioClip), false)  as AudioClip;
								statement.secondaryAudios[i].talkingRate = UnityEditor.EditorGUILayout.FloatField(
									"Talking Rate (s)", statement.secondaryAudios[i].talkingRate );
								statement.secondaryAudios[i].sfxManagerSource = UnityEditor.EditorGUILayout.TextField(
                                    "SfxManager Source", statement.secondaryAudios[i].sfxManagerSource );
								bool removeElement = GUILayout.Button( "Remove Element " + i.ToString() );
								if( removeElement )
								{
									_secondaryClipsFoldouts.RemoveAt( i );
                                    statement.secondaryAudios.RemoveAt( i );
								}
								UnityEditor.EditorGUI.indentLevel--;
							}
							UnityEditor.EditorGUI.indentLevel--;
                        }
						UnityEditor.EditorGUILayout.Space();
						UnityEditor.EditorGUILayout.Space();
					}
				}
			}
            statement.meta = UnityEditor.EditorGUILayout.TextField("Metadata", statement.meta);

			nextNodeDelay = UnityEditor.EditorGUILayout.FloatField( "Next Node Delay", nextNodeDelay );
        }        
        #endif
#endif
    }
}
