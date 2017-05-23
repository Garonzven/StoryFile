using System;
using System.Collections;
using System.Collections.Generic;

using DDK.Base.Extensions;
using DDK.Base.Managers;
using DDK.Base.Classes;
using DDK.Base.Statics;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Random = UnityEngine.Random;
using DDK;

namespace DDK.GamesDevelopment
{
	public abstract class PathController<T, U> : MonoBehaviour where T : MonoBehaviour where U : Component
	{
		[Tooltip ( "The parent of the way points that will be used to create a path for the player to move" )]
		[DisplayName ( "Way points Parent" )]
		public GameObject pointsParent;
		
		public GameObject character;
		public Action<U> moveAction;
		
		//List, so the collection is not read only)
		[Header ( "Per Path Assets (Order Matters)" )]
		public List<Sprite> sprites = new List<Sprite>();
		//List, so the collection is not read only
		public List<string> texts = new List<string>();
		public AudioClip[] instructions = new AudioClip[0];
		
		[Space ( 10f )]
		public int columns = 3;
		public int rowsPerPath = 3;
		
		[Space ( 10f )]
		[Tooltip ( "If true, when the character climbs to a new row, it will fade out." )]
		public bool fadeOutNewRow;
		[ShowIf( "_FadeOutNewRow", 1 )]
		public float fadeDuration = 2f;
		[ShowIf( 0f, 1f, "_FadeOutNewRow", 1 )]
		public float fadeTarget = 0f;
		
		[Header ( "Events" )]
		[Tooltip ( "Each index represents a row" )]
		public DelayedAction[] onMovedToNewRow = new DelayedAction[0];
		[Tooltip ( "Each index represents a row" )]
		public DelayedAction[] onIncorrectPerRow = new DelayedAction[0];
		public DelayedAction[] onCorrect;
		public DelayedAction[] onGoalReached;
		public DelayedAction[] onIncorrect;
		public DelayedAction[] onGameEnd;
		public bool setGameEndOnLastPoint = true;
		public bool disableOnLastPoint = true;
		public bool pauseInstructionOnLastPoint = true;
		
		[Header ( "Audio Clips" )]
		public AudioClip correct;
		
		[Tooltip ( "A random clip from this array will be played" )]
		public AudioClip[] incorrects;
		
		[Tooltip ( "All clips that must play on game end" )]
		public AudioClip[] endGameClips;
		
		
		protected bool _FadeOutNewRow()
		{
			return fadeOutNewRow;
		}
		
		
		/// <summary>
		///     The row in which the character is located right now.
		///     The first row is 1.
		/// </summary>
		protected int _currentRow;
		/// <summary>
		///     The index of the current path, starts at 0.
		/// </summary>
		protected int _currentPath;
		/// <summary>
		///     All the points to which the character can try to move by clicking.
		/// </summary>
		protected List<GameObject> _waypoints;
		/// <summary>
		///     Represents each path's correct sprite.
		/// </summary>
		protected List<Sprite> _pathsSprites = new List<Sprite>();
		/// <summary>
		///     Represents the current path's sprite which is the correct one.
		/// </summary>
		protected Sprite _currentSprite;
		/// <summary>
		///     Represents each path's correct text.
		/// </summary>
		protected List<string> _pathsTexts = new List<string>();
		/// <summary>
		///     Represents the current path's text which is the correct one.
		/// </summary>
		protected string _currentText;
		/// <summary>
		///     Represents the first index of the next row.
		/// </summary>
		protected int _nextRowMarker;
		/// <summary>
		///     The main path indexes. This contains each of the main elements
		///     (indexes) per row that conform the whole path.
		/// </summary>
		protected List<int> _path = new List<int>();
		protected EventSystem _eventSystem;
		
		private T _movementComponent;
		private bool _gameEnded = false;
        /// <summary>
        /// true if the character is moving to the next position.
        /// </summary>
        private bool _isMoving;
		
		protected int _TotalRows
		{
			get { return _waypoints.Count / columns; }
		}

		protected U source;
		protected int clickedWaypoint;
		
		public virtual GameObject currentCorrectItem
		{
			get
			{
				return _waypoints[_path[_currentRow]];
			}
		}

		public virtual Button currentCorrectButton
		{
			get
			{
				return currentCorrectItem.GetComponent<Button>();
			}
		}

		public virtual int CurrentPath
		{
			get { return _currentPath; }
		}

		public virtual int CurrentRow
		{
			get { return _currentRow; }
		}

		public virtual List<GameObject> CurrentRowWaypoints
		{
			get
			{ 
				return _waypoints.GetElements<GameObject, GameObject>(_currentRow * columns, columns * (_currentRow + 1) - 1);
			}
		}

		public float DelayToFadeOut;
		
		public T Movement
		{
			get
			{
				if ( _movementComponent == null && character != null )
				{
					_movementComponent = character.GetComponent<T>();
				}
				
				if ( _movementComponent == null )
				{
					_movementComponent = FindObjectOfType<T>();
					
					if ( _movementComponent )
					{
						character = _movementComponent.gameObject;
					}
				}
				
				if ( _movementComponent == null )
				{
                    Utilities.LogWarning ( "Character's " + typeof ( T ) + " not specified.." );
					enabled = false;
				}
				
				return _movementComponent;
			}
		}
		
		
		
		protected virtual void Awake()
		{
			if ( !pointsParent )
			{
                Utilities.LogWarning ( "Points Parent not specified.." );
				enabled = false;
				return;
			}
			_eventSystem = GameObject.Find ( "EventSystem" )
				.GetComponent<EventSystem>();
		}
		
		public bool HasGameEnded
		{
			get
			{
				if ( _gameEnded )
				{
					return _gameEnded;
				}
				
				if ( _currentRow == _TotalRows )
				{
					_gameEnded = true;
					_eventSystem.enabled = true;  //Added by Germain, event system is disabled at the end of scene if this isnt here
					
					if ( onGameEnd != null )
					{
						onGameEnd.InvokeAll();
					}
					
					if ( endGameClips != null && endGameClips.Length > 0 )
					{
						SfxManager.GetSource ( "Effects" ).Play ( endGameClips ).Start();
					}
					
					if ( setGameEndOnLastPoint )
					{
						Game.ended = true;
					}
					
					if ( disableOnLastPoint )
					{
						enabled = false;
					}
					
					enabled = !disableOnLastPoint;
				}
				
				if ( _gameEnded )
				{
                    Utilities.Log ( "game ended!", this );
				}
				
				return _gameEnded;
			}
		}
		
		public virtual void OnPointClick ( U source )
		{
            if ( !enabled || HasGameEnded || _isMoving )
			{
				return;
			}
			this.source = source;
			int _clickedWaypoint = _waypoints.IndexOf ( source.gameObject );
			
			// check if way point is valid, then move to that point
			if ( _clickedWaypoint == _path[_currentRow] )
			{
                clickedWaypoint = _clickedWaypoint;
				if ( onCorrect != null )
				{
					onCorrect.InvokeAll();
				}
				StartCoroutine( CorrectClick() );
			}
			else
			{
				SfxManager.PlayClip ( "Effects", incorrects.GetRandom<AudioClip>() );

				if ( onIncorrect != null )
				{
					onIncorrect.InvokeAll();
				}
				
				if ( _currentPath < onIncorrectPerRow.Length && onIncorrectPerRow != null )
				{
					onIncorrectPerRow[_currentPath].InvokeCo().Start();
				}
				
				if ( incorrects == null || incorrects.Length <= 0 )
				{
					return;
				}
			}
		}

		protected virtual IEnumerator CorrectClick()
		{
            // move character to the position
            moveAction ( source );
            _isMoving = true;

			if ( correct != null )
			{
				SfxManager.PlayClip ( "Effects", correct );
			}			
			yield return new WaitForSeconds (DelayToFadeOut);
			// check movement to next row
			for ( int i = _nextRowMarker; i < _nextRowMarker + columns; i++ )
			{
				if ( clickedWaypoint != i )
				{
					continue;
				}
				
				if ( fadeOutNewRow )
				{
					for ( int j = 0; j < columns; j++ )
					{
						_waypoints[_nextRowMarker + j].AnimAlpha( fadeTarget, fadeDuration, true, true );
					}
				}
				
				if ( _currentPath < onMovedToNewRow.Length && onMovedToNewRow != null )
				{
					onMovedToNewRow[_currentPath].InvokeCo().Start();
				}
				
				_currentRow++;
				_nextRowMarker += columns;
				
				if ( !HasGameEnded )
				{
					_CheckIfNewPath();
				}
				
				break;
			}
            _isMoving = false;
		}
		
		/// <summary>
		///     Initializes all the paths. Each path is conformed
		///     by -rowsPerPath- public variable.
		/// </summary>
		protected virtual void _InitPaths()
		{
			_waypoints = pointsParent.GetChildren().ToList();
			_waypoints.Reverse();
			float currentRow = 0;
			for ( int i = 0; i < _waypoints.Count; i += columns ) //from row to row
			{
				//SET A VALID PATH WAYPOINT'S SPRITE / TEXT AS THE CURRENT SPRITE / TEXT
				int ranIndex = Random.Range ( i, i + columns );
				
				//IS NEXT ROW A NEW PATH ?
				if ( currentRow % rowsPerPath == 0 && currentRow < _TotalRows )
				{
					_SetCurrentSprite();
					_SetCurrentText();
				}
				
				if ( _currentSprite != null )
				{
					_waypoints[ranIndex].SetImageSprite ( _currentSprite );
				}
				
				if ( _currentText != null )
				{
					_waypoints[ranIndex].SetText ( _currentText );
				}
				
				//previousIndex = ranIndex;
				_path.Add(ranIndex);				
				currentRow++;
			}

			if ( sprites.Count > 0 )
			{
				_currentSprite = _pathsSprites[0];
			}
			
			if ( texts.Count > 0 )
			{
				_currentText = _pathsTexts[0];
			}
		}
		
		/// <summary>
		///     Sets a new Text as the current Text, removes it from
		///     the texts list, and adds it to the paths texts list.
		/// </summary
		protected virtual void _SetCurrentText()
		{
			if ( texts.Count == 0 )
			{
				_currentText = null;
				return;
			}
			
			//SET CURRENT TEXT
            _currentText = texts.PopRandom();
			_pathsTexts.Add ( _currentText );
		}
		
		/// <summary>
		///     Sets a new sprite as the current sprite, removes it
		///     from the sprites list, and adds it to the paths sprites list.
		/// </summary
		protected virtual void _SetCurrentSprite()
		{
			if ( sprites.Count == 0 )
			{
				_currentSprite = null;
				return;
			}
			
			//SET CURRENT SPRITE
            _currentSprite = sprites.PopRandom();
			_pathsSprites.Add ( _currentSprite );
		}
		
		protected virtual void _NextPath()
		{
			_currentPath++;
			
			//SET CURRENT SPRITE / TEXT
			if ( sprites.Count > 0 )
			{
				_currentSprite = _pathsSprites[_currentPath];
			}
			
			if ( texts.Count > 0 )
			{
				_currentText = _pathsTexts[_currentPath];
			}
		}
		
		protected virtual void _CheckIfNewPath()
		{
			//IS NEXT ROW A NEW PATH ?
			if ( _currentRow % rowsPerPath == 0 )
			{
				_NextPath();
			}
		}
	}
}