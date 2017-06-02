//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;
using System.Collections.Generic;
using DDK.Base.Statics;
using DDK.Base.UI;
using DDK.Base.ScriptableObjects;


namespace DDK.GamesDevelopment {

	public class WordSearchGameController : MonoBehaviour {
		
		[System.Serializable]
		public class AdvancedEnabler{
			public MonoBehaviour[] enable;
			public int whenAllFoundEquals = 36;
		}	

		

        /// <summary>
        /// Each dictionary should contain the alphabet letters
        /// </summary>
        [Tooltip("Each dictionary should contain the alphabet letters")]
		public LanguagesDictionaries languagesDictionaries;
		/// <summary>
		/// The object holding the Texts that will be used to manage the game. Ex: A GridLayout object.
		/// </summary>
		[Tooltip("The object holding the Texts that will be used to manage the game. Ex: A GridLayout object.")]
		public GameObject lettersParent;
		public int columns = 10;
		/// <summary>
		/// The object holding the objects (names...) that represent the words to search.
		/// </summary>
		[Tooltip("The object holding the objects (names...) that represent the words to search.")]
		public GameObject wordsObjsParent;
		/// <summary>
		/// If true, the word might be inverted, for example, instead of just going from left-right or up-down they might go the opposite way.
		/// </summary>
		[Tooltip("If true, the word might be inverted, for example, instead of just going from left-right or up-down they might go the opposite way.")]
		public bool allowInvertedDirectionWords = true;
		public bool allowDiagonals = true;
		public bool allowCrossWords = true;
		[Space(10f)]
		public MonoBehaviour[] enableWhenAllFound;
		public GameObject[] activateWhenAllFound;
		public AdvancedEnabler enableWhen = new AdvancedEnabler();
		public Color foundColor = Color.grey;
		[Space(5f)]
		public bool usingDraggableLineRenderer = true;
		[Space(10f)]
		public bool useDebugColor = false;
		public Color wordsDebugColor = Color.yellow;
		public Color wordsDebugColorDecrement = new Color( 0f, 0.11f, 0f, 0f );
		
						

		public bool noMoreDirs
		{
			get{
				if( _invalidDirs.Count >= 8 )
					return true;
				return false;
			}
		}
		/// <summary>
		/// The total found words since the game started. This is useful is the game is repeated multiple times and some action 
		/// needs to be executed after X executions. This is controlled by the  -enableWhen- public variable.
		/// </summary>
		public static int allFoundCount;

		internal Text[] _letters;
		internal string[] _words;
		internal List<GameObject> _wordsObjs;
		/// <summary>
		/// Stores the positions for the first and last letter of each word: word[i][0] = first letter position. 
		/// word[i][1] = last letter position
		/// </summary>
		internal List<int[]> _wordsReference = new List<int[]>();
		/// <summary>
		/// Stores the Text (letters) objects of every word.
		/// </summary>
		internal List<Text[]> _wordsTexts = new List<Text[]>();
		/// <summary>
		/// The total found words
		/// </summary>
		internal int _foundCount;

		private List<int> _invalidDirs = new List<int>();
		/// <summary>
		/// The cells that are being used to store the words chars positions.
		/// </summary>
		protected List<int> _usedCells = new List<int>();
						
		
		
		void Awake () {
			
			if( lettersParent )
			{
				_letters = lettersParent.GetComponentsInChildren<Text>();
			}
			if( wordsObjsParent )
			{
				_words = wordsObjsParent.GetChildren().GetNames();
				FillCells();
				SetWords();
			}
			_wordsObjs = _words.Find();
		}
		
		void Start()
		{
			StartCoroutine( LateStart() );
		}
		
		IEnumerator LateStart()
		{
			yield return null;
			SetDragDrop();
		}
		
		// Update is called once per frame
		void Update () {
			
			if( usingDraggableLineRenderer )
			{
				if( DraggableLineRenderer.correctDropsCount == 1 )
				{
					_foundCount++;
					allFoundCount++;
					FadeOutLastFound();
					SetLastFoundLettersColor();
					DraggableLineRenderer.correctDropsCount = 0;
				}
				else if( allFoundCount == enableWhen.whenAllFoundEquals )
				{
					enableWhen.enable.SetEnabled();
					allFoundCount = 0;
					_foundCount = 0;
				}
				else if( _foundCount == _words.Length )
				{
					_foundCount = 0;
					enableWhenAllFound.SetEnabled();
					activateWhenAllFound.SetActiveInHierarchy();
				}
			}
		}
		
				
		
		
		public void SetLastFoundLettersColor()
		{
			var foundLetters = GetLastFoundLetters();
			for( int i=0; i<foundLetters.Length; i++ )
			{
				foundLetters[i].color = foundColor;
			}
		}
		
		/// <summary>
		/// Fades out the last found object if it has a FadeImagesComponent
		/// </summary>
		public void FadeOutLastFound()
		{
			var lastFound = GetLastFoundWordObj();
			if( lastFound )
			{
                #pragma warning disable 0618
				var fade = lastFound.GetComponent<FadeImages>();
                #pragma warning restore 0618
				if( fade )
				{
					if( fade.fadeOutOnly )
					{
						fade.enabled = true;
					}
					else Debug.LogError( "Found word has FadeImages component but it is not fade out only" );
				}
			}
		}
		
		public string GetLastFoundWord()
		{
			return GetLastFoundLetters().GetAsString();
		}
		
		public Text[] GetLastFoundLetters()
		{
			if( usingDraggableLineRenderer )
			{
				var lastFoundLetter = DraggableLineRenderer.lastDroppedTarget.GetComponent<Text>();
				for( int i=0; i<_letters.Length; i++ )//Check every letter
				{
					if( _letters[i] == lastFoundLetter )//found
					{
						for( int j=0; j<_wordsTexts.Count; j++ )//Check every word
						{
							if( lastFoundLetter == _wordsTexts[j][0] )//if last found letter equals first letter
							{
								return _wordsTexts[j];
							}
							else if( lastFoundLetter == _wordsTexts[j][_wordsTexts[j].Length-1] )//if last found letter equals last letter
							{
								return _wordsTexts[j];
							}
						}
					}
				}
			}
			return null;
		}
		
		public GameObject GetLastFoundWordObj()
		{
			return GetLastFoundWord().Find();
		}
		
		public void SetWords()
		{
			for( int i=0; i<_words.Length; i++ )
			{
				Text[] wordLetters = new Text[_words[i].Length];
				int[] first_last = new int[2];
				int[] cell_dir = GetCell_DirForWord( _words[i].ToUpper() );
				first_last[0] = cell_dir[0];
				for( int j=0; j<_words[i].Length; j++ )
				{
					_usedCells.Add( cell_dir[0] );
					_letters[cell_dir[0]].text = _words[i][j].ToString().ToUpper();
					if( useDebugColor )
					{
						_letters[cell_dir[0]].color = wordsDebugColor - wordsDebugColorDecrement * i;
					}
					wordLetters[j] = _letters[cell_dir[0]];
					cell_dir[0] += cell_dir[1];
				}
				first_last[1] = cell_dir[0] - cell_dir[1];
				_wordsReference.Add( first_last );
				_wordsTexts.Add( wordLetters );
			}
		}
		
		public void FillCells()
		{
			for( int i=0; i<_letters.Length; i++ )
			{
                _letters[i].text = languagesDictionaries.GetCurrentLanguageDictionary()[0].GetRandomCharAsString().ToUpper();
			}
		}
		
		protected void SetDragDrop()
		{
			if( usingDraggableLineRenderer )
			{
				for( int i=0; i<_letters.Length; i++ )//Check every letter
				{
					var lineRenderer = _letters[i].GetComponentInParent<DraggableLineRenderer>();
					for( int j=0; j<_wordsReference.Count; j++ )//Check every word
					{
						if( i == _wordsReference[j][0] )//if it starts at this cell
						{
							lineRenderer.dropTarget = _letters[ _wordsReference[j][1] ].gameObject;
							break;
						}
						else if( i == _wordsReference[j][1] )//if it ends at this cell
						{
							lineRenderer.dropTarget = _letters[ _wordsReference[j][0] ].gameObject;
							break;
						}
						else lineRenderer.dropTarget = null;
					}
					lineRenderer._allDropTargets = GetDropTargets( i );
				}
			}
		}
		
		/// <summary>
		/// Gets the drop targets for the DraggableLineRenderer of the specified cell.
		/// </summary>
		/// <returns>The drop targets.</returns>
		/// <param name="cell">Cell.</param>
		protected List<GameObject> GetDropTargets( int cell )
		{
			List<GameObject> dropTargets = new List<GameObject>();
			dropTargets.AddRange( GetHorizontalCellsNextTo(cell) );
			dropTargets.AddRange( GetVerticalCellsNextTo(cell) );
			if( allowDiagonals )
			{
				dropTargets.AddRange( GetDiagonalCellsNextTo(cell) );
			}
			return dropTargets;
		}
		
		protected List<GameObject> GetHorizontalCellsNextTo( int cell )
		{
			int row = GetRowIndex( cell );
			List<GameObject> cells = new List<GameObject>();
			bool exit = false;
			for( int i=0; i<_letters.Length; i++ )
			{
				if( GetRowIndex( i ) == row )
				{
					cells.Add( _letters[i].gameObject );
					exit = true;
				}
				else if( exit )
				{
					break;
				}
			}
			return cells;
		}
		
		protected List<GameObject> GetVerticalCellsNextTo( int cell )
		{
			int col = GetColumnIndex( cell );
			List<GameObject> cells = new List<GameObject>();
			for( int i=0; i<_letters.Length; i++ )
			{
				if( GetColumnIndex( i ) == col )
				{
					cells.Add( _letters[i].gameObject );
					i += columns - 1;
				}
			}
			return cells;
		}
		
		protected List<GameObject> GetDiagonalCellsNextTo( int cell )
		{
			int mainCell = cell;
			List<GameObject> cells = new List<GameObject>();
			for( int i=0; i<4; i++ )
			{
				int dir = 0;
				switch( i )
				{
				case 0: dir = columns + 1;//diagonal down-right
					break;
				case 1: dir = columns - 1;//diagonal down-left
					break;
				case 2: dir = -( columns - 1 );//diagonal up-right
					break;
				case 3: dir = -( columns + 1 );//diagonal up-left
					break;
				}
				
				while( true )//Add every diagonal cell in the specified direction
				{
					if( !IsNextCellValid( cell, dir ) )
					{
						break;
					}
					cell += dir;
					if( cell >= _letters.Length || cell < 0 )
					{
						break;
					}
					cells.Add( _letters[cell].gameObject );
				}
				cell = mainCell;
			}
			return cells;
		}
		
		public int GetRowIndex( int cell )
		{
			return (int)( cell / columns );
		}
		
		public int GetColumnIndex( int cell )
		{
			return cell % columns;
		}
		
		/// <summary>
		/// Gets the cell and direction (in that order) for the specified word. Note: While getting the direction of the word 
		/// the cell value may change which is why it is returned.
		/// </summary>
		/// <returns>The cell and direction (in that order) for the specified word.</returns>
		/// <param name="cell">Cell.</param>
		/// <param name="word">Word.</param>
		public int[] GetCell_DirForWord( string word, int cell = -1 )
		{
			if( cell == -1 )
			{
				cell = GetRandomCell();
			}
			var dir = GetDir();
			if( IsDirValid( cell, dir, word ) )
			{
				return new int[]{ cell, dir };
			}
			else
			{
				_invalidDirs.Add( dir );
				if( noMoreDirs )
				{
					cell = GetRandomCell();
				}
				return GetCell_DirForWord( word, cell );
			}
		}
		
		public int GetRandomCell()
		{
			return Random.Range( 0, _letters.Length );
		}
		
		/// <summary>
		/// Gets a random direction in which the word might be written.
		/// </summary>
		/// <returns>The direction.</returns>
		public int GetDir()
		{
			int ran = Random.Range( 0, 8 );
			if( !allowDiagonals )
			{
				ran = Random.Range( 0, 4 );
			}
			if( !allowInvertedDirectionWords && ran % 2 != 0 )
			{
				return GetDir();
			}
			if( _invalidDirs.Contains( ran ) )
			{
				ran = 10;
				if( !noMoreDirs )
				{
					return GetDir();
				}
			}
			switch( ran )
			{
			case 0: return 1;//right
			case 1: return -1;//left
			case 2: return columns;//down
			case 3: return -columns;//up
			case 4: return columns + 1;//diagonal down-right
			case 5: return columns - 1;//diagonal down-left
			case 6: return -( columns - 1 );//diagonal up-right
			case 7: return -( columns + 1 );//diagonal up-left
			}
			return ran;
		}
		
		public bool IsDirValid( int cell, int dir, string word )
		{
			var lastIndexPos = dir * ( word.Length - 1 ) + cell;
			if( lastIndexPos > 0 && lastIndexPos < _letters.Length )//CHECK IF THE WORD FITS IN THE CALCULATED POSITION
			{
				for( int i=0; i<word.Length; i++ )
				{
					//CHECK USED CELLS
					if( _usedCells.Contains( cell ) )
					{
						if( allowCrossWords )
						{
							if( !_letters[cell].text.Equals( word[i] ) )
							{
								return false;
							}
						}
						else
						{
							return false;
						}
					}
					//CHECK IF WORD WILL BE SPLIT BY GRID SIDES
					if( i < word.Length - 1 )
					{
						if( !IsNextCellValid( cell, dir ) )
						{
							return false;
						}
					}
					
					cell += dir;
				}
				
				_invalidDirs.Clear();
				return true;
			}
			return false;
		}
		
		public bool IsNextCellValid( int cell, int dir )
		{
			bool currentCellR = ( (cell + 1) % columns == 0 ) ? true : false;//Current cell is located at the RIGHT side of the grid (last column).
			bool currentCellL = ( cell % columns == 0 || cell == 0 ) ? true : false;//Current cell is located at the LEFT side of the grid (first column).
			if( currentCellL || currentCellR )
			{
				bool nextCellR = ( (cell + dir + 1) % columns == 0 ) ? true : false;//Next cell is located at the RIGHT side of the grid (last column).
				bool nextCellL = (( cell + dir) % columns == 0 || cell == 0 ) ? true : false;//Next cell is located at the LEFT side of the grid (first column).
				if( (nextCellL && currentCellR) || (nextCellR && currentCellL) )
				{
					return false;
				}
			}
			return true;
		}
			
		
	}


}