//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;

using UnityEngine.UI;

using DDK.GamesDevelopment.Characters;
using DDK.Base.Managers;

namespace DDK.GamesDevelopment.Characters._3D {

	public class PathClimberController 
        : PathController<CharacterAutoClimbController, Graphic> {

		public float loadNextSceneDelay;

		public override GameObject currentCorrectItem
		{
			get
			{
				return _waypoints[_path[_currentRow]];
			}
		}

		private void GoalReachedHandler()
		{
			onGoalReached.InvokeAll();
			_eventSystem.enabled = true;
		}
                
		// Use this for initialization
		void Start()
		{
			//INIT
			Movement.onGoalReached += GoalReachedHandler;
			_InitPaths ();
		}

		/// <summary>
		/// This should be called on the onClick event of a path way point (UI button).
		/// </summary>
		public override void OnPointClick( Graphic source )
		{
			//Debug.Log ( character.RealPosition().Distance( img.GetWorldPos() ) );
			if( !enabled )
				return;
			clickedWaypoint = _waypoints.IndexOf( source.gameObject );
			//IF WAYPOINT VALID, MOVE TO IT
			if( clickedWaypoint == _path[_currentRow] )
			{
				_eventSystem.enabled = false;
				this.source = source;
				onCorrect.InvokeAll();
				StartCoroutine(CorrectClick());
            }   
			else
			{
				SfxManager.PlayClip( "Effects", incorrects.GetRandom<AudioClip>(), true, gameObject );
                onIncorrect.InvokeAll();
            }
		}

		protected override IEnumerator CorrectClick()
		{
			SfxManager.PlayClip( "Effects", correct, true, gameObject );
			//MOVE CHARACTER TO THE PATH'S WAYPOINT
			Movement._Goal = source.transform;
			//CHECK IF MOVING TO NEXT ROW
			yield return new WaitForSeconds( DelayToFadeOut );
			for( int i=_nextRowMarker; i<_nextRowMarker + columns; i++ )
			{
				//IS THE CLICKED WAYPOINT IN NEXT ROW ?
				if( clickedWaypoint != i )//NO, CHARACTER ISN'T MOVING TO NEXT ROW..
					continue;
				
				//FADE OUT CURRENT WAYPOINT
				if( fadeOutNewRow )
				{
					for( int j=0; j<columns; j++ )
					{
						_waypoints[ _nextRowMarker + j ].AnimAlpha( 0f, 2f, true, true );
					}
				}
				
				//NEW ROW EVENT
				if ( _currentPath < onMovedToNewRow.Length )
					onMovedToNewRow[_currentPath].InvokeCo().Start();
				

				_currentRow++;
				_nextRowMarker += columns;
				
				if( !HasGameEnded )
					_CheckIfNewPath();
				else
					Movement.onGoalReached -= GoalReachedHandler;
				break;
			}
		}
    }
    
}
