using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DDK.Base.Extensions;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;
using DDK.Base.UI.Managers;

namespace DDK
{
    public class KaraokeManagerCreator : EditorWindow
    {
        /// <summary>
        ///     The list of talking characters of the lesson
        /// </summary>
        private readonly List<string> _lessonCharacter = new List<string>
        {
            "Arturo" // Arturo by default
        };

        /// <summary>
        ///     The karaoke base holds the template for
        ///     all the cloned <see cref="CharacterKaraokeManager" />
        ///     that will be created during text extractiong.
        ///     See: <see cref="_ProcessRawText" />
        /// </summary>
        private GameObject _karaokeBase;

        /// <summary>
        ///     All the cloned gameObjects will be put
        ///     as children of <see cref="_karaokeParent" />
        /// </summary>
        private GameObject _karaokeParent;

        // foldouts
        private bool _viewCharacters = true;
        private bool _viewOptions = true;
        private bool _viewTextarea = true;
        private bool _viewExtractor = true;

        // toggles
        private bool _extractText = true;
        private bool _autoClean = true;
        private bool _extractKaraoke;
        private bool _fineTuning;
        private bool _viewDetails;
        private bool _viewResults;

        // scrolling positions
        private Vector2 _textAreaPos = Vector2.right;
        private Vector2 _lessonCharPos = Vector2.right;
        private Vector2 _wholeWindowPos = Vector2.right;
        private Vector2 _editingPos = Vector2.right;

        // string fields 
        private string _charName = "";
        private string _rawText = "";
        private string _nameFormat = "${INDEX} ${ACTOR} - ${LINE,20}";

        private string _NameFormatted(CharacterKaraokeManager ckm, int index)
        {
            var cnf = _nameFormat;

            cnf = cnf.Replace( "${INDEX}", index.ToString() );
            cnf = cnf.Replace( "${ACTOR}", ckm.activeName );

            var text = Regex.Replace(ckm.text, @" <p= [0-9]*\.?[0-9]+> ", " ");
            text = text.Replace(" <f> ", " ").Replace(Environment.NewLine, " ");
            var match = Regex.Match( cnf, @"\$\{LINE,(\d+)\}" );
            var count = text.Length;

            if ( match.Success && match.Groups.Count > 1 )
            {
                var len = int.Parse( match.Groups[1].Value );

                return Regex.Replace( cnf, @"\$\{LINE,\d+\}",
                    text.Substring( 0, Math.Min( count, len ) ) );
            }

            return cnf.Replace( "${LINE}", text );
        }

        private string _NameFormatted(KaraokeFromSplit kfs)
        {
            var cnf = _nameFormat;

            cnf = cnf.Replace( "${INDEX}", kfs.parentIndex );
            cnf = cnf.Replace( "${ACTOR}", kfs.actor );

            var text = Regex.Replace(kfs.text, @" <p= [0-9]*\.?[0-9]+> ", " ");
            text = text.Replace(" <f> ", " ").Replace(Environment.NewLine, " ");
            var match = Regex.Match( cnf, @"\$\{LINE,(\d+)\}" );
            var count = text.Length;

            if ( match.Success && match.Groups.Count > 1 )
            {
                var len = int.Parse( match.Groups[1].Value );

                return Regex.Replace( cnf, @"\$\{LINE,\d+\}",
                    text.Substring( 0, Math.Min( count, len ) ) );
            }

            return cnf.Replace( "${LINE}", text );
        }

        /// <summary>
        ///     Lists of <see cref="CharacterKaraokeManager" /> extracted from
        ///     parent <see cref="GameObject" /> in
        ///     <see cref="_ExtractKaraokesInfo" />
        /// </summary>
        private CharacterKaraokeManager[] _karaokesFromParent;

        /// <summary>
        ///     n-tree containing all dialog lines and further splits
        ///     of these dialog lines
        /// </summary>
        private readonly List<KaraokeFromSplit> _splitKaraokes =
            new List<KaraokeFromSplit>();

        /// <summary>
        ///     Character limit <c>int</c>
        ///     for component <see cref="KaraokeManager" />
        /// </summary>
        private int _karaokeCharacterLimit = 40;

        /// <summary>
        ///     Disable when done <c>bool</c> for component
        ///     <see cref="KaraokeManager" />
        /// </summary>
        private bool _karaokeDisableWhenDone = true;

        /// <summary>
        ///     Overflow <c>bool</c>  for component <see cref="KaraokeManager" />
        /// </summary>
        private bool _karaokeManagerOverflow;

        /// <summary>
        ///     <see cref="Text" /> UI component
        ///     for <see cref="CharacterKaraokeManager" />
        /// </summary>
        private Text _characterNameText;

        /// <summary>
        ///     If <c>true</c> will insert a pause after every
        ///     <c>"!"</c> symbol
        /// </summary>
        private bool _pauseAfterExclamation = true;

        /// <summary>
        ///     If <c>true</c> will insert a pause after every
        ///     <c>"."</c> symbol
        /// </summary>
        private bool _pauseAfterPeriod = true;

        /// <summary>
        ///     The pause time after every <c>"."</c> symbol.
        /// </summary>
        private float _periodPauseTime = 1f;

        /// <summary>
        ///     If <c>true</c> will insert a pause after every
        ///     <c>"?"</c> symbol
        /// </summary>
        private bool _pauseAfterQuestion = true;

        /// <summary>
        ///     If <c>true</c> will replace <c>"-"</c> with <c>" - "</c>
        /// </summary>
        private bool _separateSpellingWords = true;

        /// <summary>
    	/// If <c>true</c> will delete anything inside text parentheses
    	/// including the parenthesis
        /// </summary>
        private bool _ignoreInParenthesis = true;

        /// <summary>
        ///     If <c>true</c> will insert a pause after every
        ///     <c>-</c> separated word.
        /// </summary>
        private bool _pauseAfterSpelling;

        /// <summary>
        ///     The pause time of every spelled word will be
        ///     <c>number of syllables</c> * <see cref="_syllableTime" />
        /// </summary>
        private float _syllableTime;

        [MenuItem("Custom/Create/ Kareokes From Raw Text")]
        private static void Init()
        {
            var window = GetWindow<KaraokeManagerCreator>
                ( typeof(KaraokeManagerCreator) );
            window.Show();
            var titleHeader = new GUIContent
            {
                text = "Karaoke Extractor"
            };
            window.titleContent = titleHeader;
        }

        /// <summary>
        ///     <see cref="KaraokeFromSplit" /> defines a dialog line
        ///     and its further splits. The split lines are stored in
        ///     <see cref="nodes" />
        /// </summary>
        public class KaraokeFromSplit
        {
            public int depth;
            public string parentIndex;
            public bool foldout;
            // can be nested
            public List<KaraokeFromSplit> nodes;
            public bool pauseFoldout;
            // result string from the karaoke splitting operation
            public string text;
            // character karaoke manager options
            public string actor;
            public int characterLimit;
            public bool disableWhenDone;
            public bool managerOverflow;
            public Text nameText;
        }

        private void OnGUI()
        {
            _wholeWindowPos = EditorGUILayout.BeginScrollView( _wholeWindowPos );

            EditorGUILayout.BeginHorizontal();
            _extractText = GUILayout.Toggle( _extractText && !_fineTuning,
                "Extract Karaokes From Text", "Button" );
            _fineTuning = GUILayout.Toggle( _fineTuning && !_extractText,
                "Fine Tune Karaoke Managers", "Button" );
            EditorGUILayout.EndHorizontal();

            if ( _extractText )
            {
                EditorGUIUtility.labelWidth = 200;

                _DrawBaseInput();

                _DrawLessonCharactersList();

                _DrawLessonTextInput();

                _DrawTextExtractorOptions();

                _DrawKaraokeManagerOptions();

                EditorGUILayout.Space();
                if ( GUILayout.Button( "Process Raw Text" ) )
                {
                    _ProcessRawText();
                }
            }

            if ( _fineTuning )
            {
                _DrawBaseInput();

                _ExtractKaraokesInfo();

                _DrawDetailsAndResults();

                var confirmed = GUILayout.Button( "Process Results" );

                if ( confirmed )
                {
                    _ProcessFineTuning( _splitKaraokes );
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        ///     this will create clones of <see cref="_karaokeBase" />
        ///     with properly modified <see cref="CharacterKaraokeManager" />
        ///     from the <see cref="_splitKaraokes" /> tree nodes.
        /// </summary>
        private void _ProcessFineTuning(IList<KaraokeFromSplit> source)
        {
            for ( var index = 0;
                source != null && index < source.Count;
                index++ )
            {
                // only the leafs of the n-tree
                if ( source[index].nodes == null ||
                     source[index].nodes.Count == 0 )
                {
                    // add game object
                    var karaoke = Instantiate( _karaokeBase );
                    karaoke.SetParent( _karaokeParent.transform );
                    // name new karaoke
                    karaoke.name = _NameFormatted( source[index] );
                    // try getting a named karaoke manager
                    var kmManager =
                        karaoke.GetComponent<CharacterKaraokeManager>();
                    // copy rect transform values
                    _karaokeBase.GetComponents<RectTransform>()
                        .CopyTo( karaoke );
                    // if still no karaoke manager just add it
                    if ( !kmManager )
                    {
                        kmManager =
                            karaoke.AddComponent<CharacterKaraokeManager>();
                    }
                    // set text for karaoke
                    kmManager.text = source[index].text;
                    kmManager.charactersLimit = source[index].characterLimit;
                    kmManager.overflow = source[index].managerOverflow;
                    kmManager.disableWhenDone = source[index].disableWhenDone;
                    kmManager.activeName = source[index].actor;
                    kmManager.nameTextHolder = source[index].nameText;
                }

                // draw split karaokes childs
                _ProcessFineTuning( source[index].nodes );
            }
        }

        /// <summary>
        ///     This will draw the dialogue splits <see cref="_splitKaraokes" />
        ///     and the  result from deleting and splitting the dialogue lines
        /// </summary>
        private void _DrawDetailsAndResults()
        {
            _editingPos = GUILayout.BeginScrollView( _editingPos );

            _viewDetails = GUILayout.Toggle( _viewDetails,
                "View Karaoke Details", "Button" );
            if ( _viewDetails )
            {
                _DrawAllKaraokesInfo( _splitKaraokes );
            }

            _viewResults = GUILayout.Toggle( _viewResults, "View Result",
                "Button" );
            if ( _viewResults )
            {
                _DrawKaraokesResult( _splitKaraokes );
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        ///     Draws the result from splitting, deleting and editing
        ///     <see cref="KaraokeFromSplit" /> nodes / dialogue lines.
        /// </summary>
        /// <param name="source">The source.</param>
        private void _DrawKaraokesResult(IList<KaraokeFromSplit> source)
        {
            for ( var index = 0;
                source != null && index < source.Count;
                index++ )
            {
                if ( source[index].nodes == null ||
                     source[index].nodes.Count == 0 )
                {
                    EditorGUILayout.LabelField( source[index].text );
                }
                // draw split karaokes childs
                _DrawKaraokesResult( source[index].nodes );
            }
        }

        /// <summary>
        ///     Extracts the <see cref="CharacterKaraokeManager" /> info
        ///     from the parent <see cref="GameObject" />
        /// </summary>
        private void _ExtractKaraokesInfo()
        {
            _extractKaraoke ^= GUILayout.Toggle( _extractKaraoke,
                "Extract Karaokes Info", "Button" );

            if ( _extractKaraoke && !_karaokeParent )
            {
                Debug.LogError(
                    "You need to set Karaoke Parent for this to work " );
                return;
            }

            if ( !_extractKaraoke || !_karaokeParent )
            {
                return;
            }

            // extract CharacterKaraokeManagers from parent gameObject
            _karaokesFromParent = _karaokeParent
                .GetComponentsInChildren<CharacterKaraokeManager>();
            _splitKaraokes.Clear();

            // backup original text strings into list of n-tree roots
            for ( var index = 0; index < _karaokesFromParent.Length; index++ )
            {
                var t = _karaokesFromParent[index];
                if ( _karaokeBase == null )
                {
                    _karaokeBase = t.gameObject;
                }

                _splitKaraokes.Add( new KaraokeFromSplit
                {
                    text = t.text,
                    depth = 0,
                    actor = t.activeName,
                    characterLimit = t.charactersLimit,
                    disableWhenDone = t.disableWhenDone,
                    managerOverflow = t.overflow,
                    nameText = t.nameTextHolder,
                    parentIndex = index.ToString()
                } );
            }

            _viewDetails = true;
        }

        /// <summary>
        ///     Will draw all the info <see cref="KaraokeFromSplit" /> and also
        ///     give options to split, delete, clean, modify dialogue lines
        /// </summary>
        /// <param name="source">The source.</param>
        private void _DrawAllKaraokesInfo(IList<KaraokeFromSplit> source)
        {
            for ( var index = 0;
                source != null && index < source.Count;
                index++ )
            {
                // karaoke info foldout
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space( 18 * source[index].depth );
                    // karaoke manager foldout with pause info & source text
                    source[index].foldout = EditorGUILayout.Foldout
                        ( source[index].foldout, _NameFormatted( source[index]) );
                }
                GUILayout.EndHorizontal();

                if ( !source[index].foldout )
                {
                    continue;
                }

                // draw the gameobject original source text
                if ( source[index].depth == 0 )
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField( "Original Source Text" );
                    EditorGUILayout.TextField( _karaokesFromParent[index].text );
                    EditorGUI.indentLevel--;
                }

                // draw the string resulting from editing
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space( 18 * (source[index].depth + 1) );
                    EditorGUILayout.LabelField( "Live Editing Text" );
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space( 18 * (source[index].depth + 1) );
                    source[index].text = EditorGUILayout.TextField(
                        source[index].text );

                    // clean option to delete unnecesary spaces
                    if ( GUILayout.Button( "Clean", GUILayout.Height( 14 ),
                        GUILayout.Width( 45 ) ) )
                    {
                        _CleanString(ref source[index].text);
                    }

                    // delete will delete this KaraokeSplit node
                    if ( GUILayout.Button( "Delete", GUILayout.Height( 14 ),
                        GUILayout.Width( 50 ) ) )
                    {
                        source.RemoveAt( index );
                        return;
                    }
                }
                GUILayout.EndHorizontal();

                // pause options will draw all the dialogue pause and options
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space( 18 * (source[index].depth + 1) );
                    source[index].pauseFoldout = EditorGUILayout.Foldout
                        ( source[index].pauseFoldout, "Pauses Options" );
                }
                GUILayout.EndHorizontal();

                if ( source[index].pauseFoldout )
                {
                    _DrawPauses( source[index] );
                }

                // draw current karaoke split nodes - recursive call
                _DrawAllKaraokesInfo( source[index].nodes );
            }
        }

        private static void _CleanString(ref string text)
        {
            // clean multiple spaces
            text = Regex.Replace
                (text, @"\s{2,}", " ");
            // delete spaces at start and end
            text = text.Trim();
            // if there is a pause at the end it add a space char
            text += text.Last() == '>' ? " " : "";
        }

        /// <summary>
        /// Draws information of the source dialog line pauses.
        /// Also lets set the time, split or delete this pause
        /// </summary>
        /// <param name="source">The source.</param>
        private void _DrawPauses(KaraokeFromSplit source)
        {
            var karaokeString = source.text;
            var splitPerPause = Regex
                .Split( karaokeString, @" <p= [0-9]*\.?[0-9]+> " );
            var pausesTime = Regex
                .Match( karaokeString, @" <p= [0-9]*\.?[0-9]+> " );

            karaokeString = "";

            if ( !splitPerPause.Any() )
            {
                return;
            }

            var pausesCount = 0;
            EditorGUILayout.BeginHorizontal();
            {
                while ( pausesTime.Success )
                {
                    EditorGUILayout.BeginVertical();
                    {
                        // build final string
                        karaokeString += splitPerPause[pausesCount];

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space( 18 * (source.depth + 1) );
                            EditorGUILayout.TextField(
                                splitPerPause[pausesCount++] );
                        }
                        GUILayout.EndHorizontal();

                        // draw pause indicator
                        var t = float.Parse( Regex.Match
                            ( pausesTime.Value, @"[0-9]*\.?[0-9]+" ).Value );

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space( 18 * (source.depth + 1) );
                            EditorGUILayout.TextField( "<p= " + t + ">" );
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space( 18 * (source.depth + 1) );
                            t = EditorGUILayout.FloatField( Math.Abs( t ) );
                        }
                        GUILayout.EndHorizontal();

                        bool deletedPause, splitAtPause;

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space( 18 * (source.depth + 1) );
                            deletedPause = GUILayout.Button( "Delete",
                                GUILayout.Height( 16 ) );
                            splitAtPause = GUILayout.Button( "Split",
                                GUILayout.Height( 16 ) );
                        }
                        GUILayout.EndHorizontal();

                        // build final string
                        var pauseMark = " <p= " + t + "> ";
                        karaokeString += deletedPause || splitAtPause
                            ? ""
                            : pauseMark;
                        // split karaoke manager into two
                        if ( splitAtPause )
                        {
                            if ( source.nodes == null )
                            {
                                source.nodes = new List<KaraokeFromSplit>();
                            }
                            var id = source.nodes.Count;
                            var text1 = karaokeString;
                            if(_autoClean) _CleanString(ref text1);
                            var first = new KaraokeFromSplit
                            {
                                text = text1,
                                depth = source.depth + 1,
                                actor = source.actor,
                                characterLimit = source.characterLimit,
                                disableWhenDone = source.disableWhenDone,
                                managerOverflow = source.managerOverflow,
                                nameText = source.nameText,
                                parentIndex = source.parentIndex + "." + id
                            };
                            var text2 = source.text.Substring
                                (first.text.Length + pauseMark.Length - 1);
                            if (_autoClean) _CleanString(ref text2);
                            var second = new KaraokeFromSplit
                            {
                                text = text2,
                                depth = source.depth + 1,
                                actor = source.actor,
                                characterLimit = source.characterLimit,
                                disableWhenDone = source.disableWhenDone,
                                managerOverflow = source.managerOverflow,
                                nameText = source.nameText,
                                parentIndex = source.parentIndex + "." + (id + 1)
                            };

                            source.nodes.Add( first );
                            source.nodes.Add( second );
                        }

                        pausesTime = pausesTime.NextMatch();
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();

            // save build string
            source.text = karaokeString + splitPerPause[pausesCount];
        }

        /// <summary>
        ///     Window entry point for both modes, shows the
        ///     <see cref="_karaokeParent" /> gameObject field
        ///     and the <see cref="_karaokeBase" /> template
        ///     for extraction mode, and the gameObject output
        ///     name format <see cref="_nameFormat" />
        /// </summary>
        private void _DrawBaseInput()
        {
            EditorGUILayout.Space();
            _karaokeParent = (GameObject)EditorGUILayout.ObjectField
                ( "Kareoke Parent", _karaokeParent, typeof(GameObject), true );

            _karaokeBase = (GameObject)EditorGUILayout.ObjectField
                ( "Kareoke Base", _karaokeBase, typeof(GameObject), true );

            GUILayout.BeginHorizontal();
            _nameFormat = EditorGUILayout.TextField( "Output Name Format",
                _nameFormat );
            _autoClean = GUILayout.Toggle(_autoClean, "Automatic String Trim");
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws the list of provided lesson characters
        /// </summary>
        private void _DrawLessonCharactersList()
        {
            _viewCharacters = EditorGUILayout.Foldout( _viewCharacters,
                "Lesson Characters (" + _lessonCharacter.Count + ")" );

            if ( !_viewCharacters )
            {
                return;
            }

            EditorGUI.indentLevel++;

            _lessonCharPos = EditorGUILayout.BeginScrollView( _lessonCharPos );
            for ( var index = 0; index < _lessonCharacter.Count; index++ )
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.TextField( _lessonCharacter[index] );
                    if ( GUILayout.Button( "Delete", GUILayout.Width( 72 ),
                        GUILayout.Height( 14 ) ) )
                    {
                        _lessonCharacter.RemoveAt( index );
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            // insert new lesson character identifier
            EditorGUILayout.BeginHorizontal();
            {
                _charName = EditorGUILayout.TextField( _charName );
                if ( GUILayout.Button( "New Character", GUILayout.Width( 100 ),
                    GUILayout.Height( 14 ) ) && _charName != "" )
                {
                    _lessonCharacter.Add( _charName );
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        /// <summary>
        ///     Draws the text area to put the lesson text script
        /// </summary>
        private void _DrawLessonTextInput()
        {
            EditorGUILayout.Space();
            _viewTextarea = EditorGUILayout.Foldout( _viewTextarea,
                "Lesson Raw Text" );
            if ( !_viewTextarea )
            {
                return;
            }

            _textAreaPos = EditorGUILayout.BeginScrollView( _textAreaPos,
                GUILayout.Height( 200 ) );
            _rawText = EditorGUILayout.TextArea( _rawText,
                GUILayout.ExpandHeight( true ) );
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        ///     Text extraction mode options for processing raw lesson text
        ///     <see cref="_rawText" />.
        /// </summary>
        private void _DrawTextExtractorOptions()
        {
            _viewExtractor = EditorGUILayout.Foldout( _viewExtractor,
                "Text Extractor Options" );

            if ( !_viewExtractor )
            {
                return;
            }

            EditorGUI.indentLevel++;

            _pauseAfterPeriod = EditorGUILayout.Toggle
                ( "Pause After Period", _pauseAfterPeriod );

            if ( _pauseAfterPeriod )
            {
                EditorGUI.indentLevel++;
                _periodPauseTime = Math.Abs( EditorGUILayout.FloatField
                    ( "Period Pause Time", _periodPauseTime ) );
                EditorGUI.indentLevel--;
            }
            _pauseAfterQuestion = EditorGUILayout.Toggle
                ( "Pause After Question", _pauseAfterQuestion );
            _pauseAfterExclamation = EditorGUILayout.Toggle
                ( "Pause After Exclamation", _pauseAfterExclamation );

            EditorGUILayout.Space();

            _separateSpellingWords = EditorGUILayout.Toggle
                ( "Separate Spelling", _separateSpellingWords );
            _pauseAfterSpelling = EditorGUILayout.Toggle
                ( "Syllables Pause Time", _pauseAfterSpelling );
            if ( _pauseAfterSpelling )
            {
                EditorGUI.indentLevel++;
                _syllableTime = EditorGUILayout.FloatField
                    ( "Syllable Time Duration", _syllableTime );
                EditorGUI.indentLevel--;
            }

        	EditorGUILayout.Space();
        	
        	_ignoreInParenthesis = EditorGUILayout.Toggle
        		( "Ignore In Parentheses", _ignoreInParenthesis );

            EditorGUI.indentLevel--;
        }

        /// <summary>
        ///     Sets the options for every generated
        ///     <see cref="CharacterKaraokeManager" /> during
        ///     extraction mode.
        /// </summary>
        private void _DrawKaraokeManagerOptions()
        {
            _viewOptions = EditorGUILayout.Foldout(
                _viewOptions, "Karaoke Manager Options" );
            if ( _viewOptions )
            {
                EditorGUI.indentLevel++;
                _karaokeCharacterLimit = EditorGUILayout.IntField
                    ( "Character Limit", _karaokeCharacterLimit );
                _karaokeManagerOverflow = EditorGUILayout.Toggle
                    ( "Enable Overflow", _karaokeManagerOverflow );
                _karaokeDisableWhenDone = EditorGUILayout.Toggle
                    ( "Disable When Done", _karaokeDisableWhenDone );
                _characterNameText = (Text)EditorGUILayout.ObjectField
                    ( "Name Text Holder", _characterNameText, typeof(Text),
                        true );
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        ///     This function will process / interpret <see cref="_rawText" />
        ///     input to produce a series of <see cref="_karaokeBase" /> clones
        ///     with a properly modified <see cref="CharacterKaraokeManager" />
        ///     component.
        /// </summary>
        private void _ProcessRawText()
        {
            if ( !_karaokeBase || !_karaokeParent || !_characterNameText )
            {
                Debug.LogError( "You need to set Karaoke Base " +
                                ", Karaoke Parent and Name Text Holder for " +
                                "this to work " );
                return;
            }

            var numDialogLines = 0;
            // initially split the text input per newline to extract
            // per dialogue line
            foreach ( var line in _rawText
                .Split( new[] { Environment.NewLine, "\n", "\r\n" },
                    StringSplitOptions.RemoveEmptyEntries ) )
            {
                // look for lines that match <ActorName>: <ANYTHING> 
                foreach ( var actor in _lessonCharacter )
                {
                    if ( !line.Contains( actor + ": " ) )
                    {
                        continue;
                    }

                    // extract any text content after the ": " string
                    var dialogLine = line.Substring( line.IndexOf( actor +
                                                                   ": ",
                        StringComparison.Ordinal ) + (actor + ": ")
                            .Length );

                    // separate spelled words he-llo -> he - llo
                    if ( _separateSpellingWords )
                    {
                        if ( _pauseAfterSpelling )
                        {
                            // add a token at the end of spelling words
                            dialogLine = Regex.Replace( dialogLine,
                                @"[\-{1}]\w+[\.{1}]", "$0#spelling~end#" );
                        }
                        // look for "-" characters and replace to add space
                        dialogLine = dialogLine.Replace( "-", " - " );
                    }
                    // insert pause indicator
                    if ( _pauseAfterPeriod )
                    {
                        // add the pause token after every "." character
                        dialogLine = dialogLine.Replace( ". ",
                            ". <p= " + _periodPauseTime + "> " );

                        if ( _pauseAfterSpelling )
                        {
                            // if pauseAfterSpelling token if found, replace
                            // with pause token
                            dialogLine = dialogLine.Replace
                                ( "#spelling~end#", " <p= " + (Regex
                                    .Matches( dialogLine, "-" ).Count + 1)
                                                    * _syllableTime + "> " );
                        }
                    }
                    // adds pause token after "?" characters
                    if ( _pauseAfterQuestion )
                    {
                        dialogLine = dialogLine.Replace( "? ",
                            "? <p= " + _periodPauseTime + "> " );
                    }
                    // adds pause token after "!" characters
                    if ( _pauseAfterExclamation )
                    {
                        dialogLine = dialogLine.Replace( "! ",
                            "! <p= " + _periodPauseTime + "> " );
                    }
                    // replace parentheses
                    if ( _ignoreInParenthesis )
                    {
                        dialogLine = Regex.Replace( dialogLine, @" ?\(.*?\)", string.Empty );
                    }

                    // add game object
                    var karaoke = Instantiate( _karaokeBase );
                    karaoke.SetParent( _karaokeParent.transform );
                    // try getting a named karaoke manager
                    var kmManager =
                        karaoke.GetComponent<CharacterKaraokeManager>();
                    // copy rect transform values
                    _karaokeBase.GetComponents<RectTransform>()
                        .CopyTo( karaoke );
                    // if still no karaoke manager just add it
                    if ( !kmManager )
                    {
                        kmManager =
                            karaoke.AddComponent<CharacterKaraokeManager>();
                    }

                    if(_autoClean) _CleanString(ref dialogLine);
                    // set text for karaoke
                    kmManager.text = dialogLine;
                    kmManager.charactersLimit = _karaokeCharacterLimit;
                    kmManager.overflow = _karaokeManagerOverflow;
                    kmManager.disableWhenDone = _karaokeDisableWhenDone;
                    kmManager.activeName = actor;
                    kmManager.nameTextHolder = _characterNameText;
                    // name new karaoke
                    karaoke.name = _NameFormatted( kmManager, numDialogLines );
                    // successful dialog line
                    numDialogLines++;
                }
            }
        }
    }
}