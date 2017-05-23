//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Statics;
using UnityEngine.UI;
using System.Text;
using DDK.Base.Components;
using DDK.Base.ScriptableObjects;
using System.Globalization;



namespace DDK.Base.Extensions 
{
    /// <summary>
    /// String class extension.
    /// </summary>
	public static class StringExt 
    {		
		#region LINE BREAKS
		/// <summary>
		/// The last count of added line breaks.
		/// </summary>
		public static int addedLineBreaks = 0;
		
		/// <summary>
		/// Gets the last count of added line breaks.
		/// </summary>
		/// <returns>
		/// The added line breaks.
		/// </returns>
		/// <param name='txt'>
		/// Text.
		/// </param>
		public static int GetAddedLineBreaks( this string txt )
		{
			return addedLineBreaks;
		}
		
		/// <summary>
		/// Add a line break to the string every time: string index % brk == 0. Example:
		/// string x = "this is an add line breaks test";
		/// x.addLineBreaks(5);
		/// //x = "this /nis an/n add l/nine b/nreaks/n test"
		/// </summary>
		/// <param name='txt'>
		/// Text.
		/// </param>
		/// <param name='brk'>
		/// Where to insert the line breaks.
		/// </param>
		public static string AddLineBreaks( this string txt, int brk )
		{
			if( brk > 0 )
			{
				int len = txt.Length;
				int breaks = len / brk;
				addedLineBreaks = breaks;
				for ( int i=1; i <= breaks; i++ ) txt = txt.Insert(brk * i, "\n");
			}
			return txt;
		}
		
		/// <summary>
		/// Add a line break to the string where the next space " ", after index, is found. 
		/// This is repeated every time: string index % index == 0.
		/// </summary>
		/// <param name='txt'>
		/// Text.
		/// </param>
		/// <param name='index'>
		/// Where to insert the line breaks.
		/// </param>
		public static string AddLineBreaksInSpace( this string txt, int index )
		{
			int len = txt.Length;
			int breaks = len / index;
			addedLineBreaks = breaks;
			for ( int i=1; i <= breaks; i++ ) {
				int insertIndex = txt.IndexOf(" ", index * i);
				if( insertIndex < index * (i+1) && insertIndex != -1 ){
					txt = txt.Remove(insertIndex, 1);
					txt = txt.Insert(insertIndex, "\n");
				}
			}
			return txt;
		}
		
		/// <summary>
		/// Add a line break to the string where the last space " " is found.
		/// </summary>
		/// <param name='txt'>
		/// Text.
		/// </param>
		public static string AddLineBreakInLastSpace( this string txt )
		{
			int insertIndex = txt.LastIndexOf(" ");
			if( insertIndex != -1 ){
				addedLineBreaks = 1;
				txt = txt.Remove(insertIndex, 1);
				txt = txt.Insert(insertIndex, "\n");
			}
			return txt;
		}
		
		/// <summary>
		/// Add a line break to the string where the last space, before 'before',  " " is found.
		/// </summary>
		/// <param name='txt'>
		/// Text.
		/// </param>
		/// <param name='before'>
		/// The start index for the space search (in backwards).
		/// </param>
		public static string AddLineBreakInLastSpace( this string txt, int before )
		{
			if( before > 0 )
			{
				int insertIndex = txt.LastIndexOf(" ", before);
				if( insertIndex != -1 ){
					addedLineBreaks = 1;
					txt = txt.Remove(insertIndex, 1);
					txt = txt.Insert(insertIndex, "\n");
				}
			}
			return txt;
		}
		
		/// <summary>
		/// Add multiple line breaks to the string where the last space, before 'before',  " " is found.
		/// 'Before' is multiplied by an increment of 1 inside a loop, to evaluate if a new line break has to be added again.
		/// </summary>
		/// <param name='txt'>
		/// Text.
		/// </param>
		/// <param name='before'>
		/// The start index for the space search (in backwards).
		/// </param>
		public static string AddLineBreaksInLastSpaces( this string txt, int before )
		{
			if( before > 0 )
			{
				int len = txt.Length;
				int breaks = len / before;//number of line breaks
				addedLineBreaks = breaks;
				int insertIndex = 0;
				while(true){
					if( insertIndex + before >= len ) break;
					insertIndex = txt.LastIndexOf(" ", insertIndex + before );
					if( insertIndex != -1 ){
						txt = txt.Remove(insertIndex, 1);
						txt = txt.Insert(insertIndex, "\n");
					}
					else break;
				}
			}
			return txt;
		}
		#endregion LINE BREAKS

		#region GET
        /// <summary>
        /// Returns a resource in the specified /path/ that contains the specified /substring/. This function might be 
        /// heavy on performance.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="substring">Substring that must be contained inside the resource's name.</param>
        /// <typeparam name="T">The type parameter.</typeparam>
		public static T GetResourceWithSubstring<T>( this string path, string substring ) where T : Object
		{
			T[] resources = Resources.LoadAll<T>( path );
			for( int i=0; i<resources.Length; i++ )
			{
				if( resources[i].name.Contains( substring ) )
				{
					return resources[i];
				}
			}
			return default(T);
		}
		
		/// <summary>
		/// Gets the texture from resources.
		/// </summary>
		/// <returns>The texture from resources.</returns>
		/// <param name="path">The texture's path.</param>
		/// <param name="asClone">If true, the returned texture will be a clone from the one in the specified path.</param>
		public static Texture2D GetTextureFromResources( this string path, bool asClone = false )
		{
			path = path.RemoveFileExtension();
			if( asClone ) {
				return Resources.Load<Texture2D> ( path ).Clone();
			}
			return Resources.Load<Texture2D> ( path );
		}
		/// <summary>
		/// Gets the sprite's texture from resources in a colors array.
		/// </summary>
		/// <returns>The texture from resources.</returns>
		/// <param name="filename">The sprite's filename inside the packed texture. Example: image.png</param>
		/// <param name="manager">the UIToolkit containing the sprite's texture.</param>
		public static Color32[] GetTextureFromResources32( this string filename )
		{
			Texture2D texture = filename.GetTextureFromResources();
			return texture.GetPixels32();//Get the sprite's texture in a color pixels array
		}
		/// <summary>
		/// Gets the substring after the last "."
		/// </summary>
		/// <returns>A new string with the file extension.</returns>
		/// <param name="filename">Filename.</param>
		public static string GetFileExtension( this string filename )
		{
			return filename.GetLastEndPoint( false, "." );
		}
		/// <summary>
		/// Gets the substring after the last /separator/
		/// </summary>
		/// <returns>A new string with the last end point.</returns>
		/// <param name="path">Path.</param>
		/// <param name="includeSeparator">If true, the /separator/ is included in the returned string.</param>
		public static string GetLastEndPoint( this string path, bool includeSeparator = false, string separator = "/" )
		{
			int index = path.LastIndexOf( separator );
			if( index < 0 )
			{
				return path;
			}
			if( includeSeparator )
			{
				return path.Substring( index );
			}
			return path.Substring( ++index );
		}
		/// <summary>
		/// Gets from the suffixed strings the appropiate one (depending on the system language) with no suffix. NOTE: 
		/// Supporting: English, Spanish.
		/// </summary>
		/// <returns>The appropiate string.</returns>
		/// <param name="texts">An array of strings with texts from multiple languages. NOTE: Each language is identified
		/// by the string ending in: _EN, _SP, and so on...</param>
		/// <see cref="LanguageDictionary.cs"/>
		/// <see cref="IosShare.cs"/>
		[System.Obsolete( "Use the LanguageDictionary instead, as in IosShare.cs" )]
		public static string GetFromLanguage( this string[] texts )
		{
			byte verified = 0;
			for( byte i=0; i<texts.Length; i++ )
			{
				if( texts[i].Length >= 3 )
				{
					string suffix = texts[i].Substring( texts[i].Length - 3, 3 );
					switch( suffix )
					{
					case "_EN": 
						verified++;
						if( SystemLanguage.English == Application.systemLanguage ) 
							return texts[i].Substring(0, texts[i].Length - 3 );
						break;
					case "_SP":
						verified++;
						if( SystemLanguage.Spanish == Application.systemLanguage ) 
							return texts[i].Substring(0, texts[i].Length - 3 );
						break;
					}
				}
			}
			if( verified == texts.Length )//DEFAULT TO ENGLISH
			{
				for( byte i=0; i<texts.Length; i++ )
				{
					if( texts[i].Length >= 3 )
					{
						string suffix = texts[i].Substring( texts[i].Length - 3, 3 );
						if( suffix.Equals( "_EN" ) ){
							Debug.Log ( "Default language = English" );
							return texts[i].Substring(0, texts[i].Length - 3 );
						}
					}
				}
			}
			else if( texts.Length != 0 ) {
				string txt = texts[0].Substring(0);//default
				Debug.Log ( "Language not supported or wrong suffix. Showing same text: "+txt );
				return txt;//default
			}
			return "null";
		}
		public static byte GetLinesCount( this string text )
		{
			if( text != null )
			{
				string[] lines = text.Split('\n');
				return (byte)lines.Length;
			}
			return (byte)0;
		}		
		public static int GetSpacesCount( this string text )
		{
			if( text != null )
			{
				string[] words = text.Split(' ');
				return words.Length - 1;
			}
			return 0;
		}		
		public static int GetWordsCount( this string text )
		{
			return text.GetSpacesCount() + 1;
		}		
		/// <summary>
		/// The First word's index is 0.
		/// </summary>
		public static string GetWord( this string text, int index )
		{
			if( text != null )
			{
				string[] words = text.Split(' ');
				return words[ index ];
			}
			return text;
		}		
		/// <summary>
		/// The First word's index is 0.
		/// </summary>
		public static string[] GetWords( this string text, bool splitOnLineBreak = true )
		{
			if( string.IsNullOrEmpty( text ) )
			{
				return null;
			}
			var splitter = new char[]{ ' ', '\n', '\t', '\r' };
			if( !splitOnLineBreak )
			{
				splitter = new char[]{ ' ', '\t', '\r' };
			}
			string[] words = text.Split( splitter );
			return words;
		}
        /// <summary>
        /// Returns the total characters count.
        /// </summary>
        public static int GetTotalCharsCount( this string[] text )
        {
            int count = 0;
            for( int i=0; i<text.Length; i++ )
            {
                if( string.IsNullOrEmpty( text[ i ] ) )
                    continue;
                count += text[ i ].Length;
            }
            return count;
        }
        public static string GetRandomCharAsString ( this string chars )
        {
            return chars[ Random.Range (0, chars.Length) ].ToString ();
        }
		#endregion GET
				
		#region SET
		/// <summary>
		/// Sets the sprite's texture inside the packed texture from resources.
		/// </summary>
		/// <param name="filename">The sprite's filename inside the packed texture. Example: image.png</param>
		/// <param name="sprite">The UIToolkit containing the sprite's texture.</param>
		/// <param name="colors">The new pixel colors of the texture.</param>
		public static void SetTextureFromResources32( this string filename, Color32[] colors )
		{
			Texture2D texture = filename.GetTextureFromResources();
			texture.SetPixels32(colors);
			texture.Apply();
		}
		public static void SetSpriteRendererOpacity( this string objName, float opacity = 1f )
		{
			var obj = GameObject.Find( objName );
			if( obj )
			{
				if( obj.GetComponent<SpriteRenderer>() )
				{
					obj.SetSpriteAlpha( opacity );
				}
			}
		}		
		public static void SetSpriteRenderersOpacity( this string[] objNames, float opacity = 1f )
		{
			for( int i=0; i<objNames.Length; i++ )
			{
				objNames[i].SetSpriteRendererOpacity( opacity );
			}
		}		
		public static IEnumerator SetSpriteRendererOpacityAfter( this string objName, float after = 1f, float opacity = 1f )
		{
			yield return new WaitForSeconds( after );
			var obj = GameObject.Find( objName );
			if( obj )
			{
				if( obj.GetComponent<SpriteRenderer>() )
				{
					obj.SetSpriteAlpha( opacity );
				}
			}
		}		
		public static IEnumerator SetSpriteRenderersOpacityAfter( this string[] objNames, float after = 1f, float opacity = 1f )
		{
			for( int i=0; i<objNames.Length; i++ )
			{
				yield return objNames[i].SetSpriteRendererOpacityAfter( after, opacity );
			}
		}
		[System.Obsolete("Use SetText( Text.. )  instead")]
		/// <summary>
		/// Sets the text checking if the GUIText is null.
		/// </summary>
		public static void SetText( this GUIText txt, string text )
		{
			if( txt != null )
			{
				txt.text = text;
			}
		}		
		/// <summary>
		/// Sets the text checking if the GUIText is null.
		/// </summary>
		public static void SetText( this Text txt, string text )
		{
			if( txt != null )
			{
				txt.text = text;
			}
		}		
		/// <summary>
		/// Sets the text checking if the GUIText is null.
		/// </summary>
		public static void SetText( this Text txt, int text )
		{
			txt.SetText( text.ToString() );
		}		
		/// <summary>
		/// Sets the text property of the UI Text component in the specified objects. Objects will be searched, including clones (instances).
		/// </summary>
		/// <param name="objNames">Object names.</param>
		/// <param name="text">Text.</param>
		public static void SetTexts( this string[] objNames, string text )
		{
			var objs = GameObject.FindObjectsOfType<Text>();
			for( int i=0; i<objs.Length; i++ )
			{
				for( int j=0; j<objNames.Length; j++ )
				{
					if( objs[i].name.Contains( objNames[j] ) )
					{
						objs[i].GetComponent<Text>().text = text;
						break;
					}
				}
			}
		}
		/// <summary>
		/// Sets the interactable button property.
		/// </summary>
		/// <param name="obj">Object containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		public static IEnumerator SetInteractableAfter( this string obj, bool interactable, float delay = 0f )
		{
			yield return new WaitForSeconds( delay );
			obj.Find<Button>().interactable = interactable;
		}		
		/// <summary>
		/// Checks if there is a Button component attached, if so, sets its interactable property.
		/// </summary>
		/// <param name="obj">Object containing a Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		public static void SetInteractable( this string obj, bool interactable )
		{
			obj.Find().SetInteractable( interactable );
		}		
		#endregion SET

		#region REMOVE
		/// <summary>
		/// Removes the substring from the last /separator/
		/// </summary>
		/// <returns>A string with the last end point removed.</returns>
		/// <param name="path">Path.</param>
		/// <param name="includeSeparator">If true, the separator will also be removed.</param>
		/// <param name="modifySrcPath">If set to <c>true</c> the source path will be the same as the returned value.</param>
		public static string RemoveLastEndPoint( this string path, bool includeSeparator = true,
		                                        bool modifySrcPath = false, string separator = "/" )
		{
			int index = path.LastIndexOf( separator );
			if( index < 0 )
			{
				return path;
			}
			if( !includeSeparator )
			{
				index++;
			}
			string newPath = path.Substring( 0, index );
			if( modifySrcPath )
			{
				path = newPath;
			}
			return newPath;
		}
		/// <summary>
		/// removes the substring located after the last "."
		/// </summary>
		/// <returns>The file extension.</returns>
		/// <param name="filename">Filename.</param>
		/// <param name="modifySrcString">If set to <c>true</c> the source string will be the same as the returned value.</param>
		public static string RemoveFileExtension( this string filename, bool modifySrcString = false )
		{
			return filename.RemoveLastEndPoint( true, modifySrcString, "." );
			/*int extIndex = filename.LastIndexOf(".");
			if( extIndex < 0 )
			{
				return filename;
			}
			string filenameNoExt = filename.Substring( 0, extIndex );
			if( modifySrcString )
			{
				filename = filenameNoExt;
			}
			return filenameNoExt;*/
		}
		/// <summary>
		/// Removes the duplicated chars and returns a new string.
		/// </summary>
		public static string RemoveDuplicatedChars( this string text )
		{
			// --- Removes duplicate chars using string concats. ---
			// Store encountered letters in this string.
			//StringBuilder table = new StringBuilder( text.Length );
			char[] table = new char[ text.Length ];
			
			// Store the result in this string.
			StringBuilder result = new StringBuilder( text.Length );
			
			// Loop over each character.
			for ( int i=0; i<text.Length; i++ )
			{
				char value = text[i];
				// See if character is in the table.
				if ( table.IndexOf(value) == -1 )
				{
					// Append to the table and the result.
					table[i] = value;
					result.Append( value );
				}
			}
			return result.ToString();
		}
		public static string RemoveBeforeMatch( this string text, int startIndex, string match, bool includeMatch = false )
		{
			int index = text.IndexOf( match, startIndex );
			int extra = ( includeMatch ) ? 0 : -1;
			return text.Remove( startIndex, index + match.Length - startIndex + extra );
		}
		public static string RemoveAfterMatch( this string text, int startIndex, string match, bool includeMatch = false )
		{
			int index = text.IndexOf( match, startIndex );
			int extra = ( includeMatch ) ? 0 : 1;
			return text.Remove( index + match.Length + extra, text.Length - index + match.Length - extra );
		}
		public static string ReplaceBetweenMatches( this string text, string match1, string match2, string replacement, bool includeMatches = false )
		{
			int index1 = text.IndexOf( match1, 0 );
			if( index1 < 0 )
				return text;
			int index2 = text.IndexOf( match2, index1 );
			if( index2 < 0 )
				return text;
			//FIX INDEXES
			index1 += match1.Length;
			index2 += match2.Length - 1;
			if( includeMatches )
			{
				if( index1 > 0 )
					index1 -= 1;
				if( index2 < text.Length - 1 )
					index2 += 1;
			}

			string old = text.Substring( index1, index2 - index1 );
			return text.Replace( old, replacement );
		}
		/// <summary>
		/// Strips diacritics from this string.
		/// </summary>
		/// <returns>A new string without the diacritics.</returns>
		public static string RemoveDiacritics( this string text )
		{
			string formD = text.Normalize(NormalizationForm.FormD);
			StringBuilder sb = new StringBuilder();
			char[] chars = formD.ToCharArray();

			char current; //caching
			for( int i=0; i<chars.Length; i++ )
			{
				current = chars[i];
				UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory( current );
				if (uc != UnicodeCategory.NonSpacingMark)
				{
					sb.Append( current );
				}
			}
			return sb.ToString().Normalize(NormalizationForm.FormC);
		}
		#endregion REMOVE

		#region ADD
		/// <summary>
		/// Adds a substring before the last "."
		/// </summary>
		/// <returns>The filename with the substring added.</returns>
		/// <param name="filename">Filename.</param>
		/// <param name="modifySrcString">If set to <c>true</c> the source string will be the same as the returned value.</param>
		public static string AddBeforeFileExtension( this string filename, string txt, bool modifySrcString = false )
		{
			string ext = filename.GetFileExtension();
			string newFilename = filename.RemoveFileExtension() + txt + ext;
			if( modifySrcString )
			{
				filename = newFilename;
			}
			return newFilename;
		}
		/// <summary>
		/// Adds the specified /prefix/ to all strings in this IList.
		/// </summary>
		public static void AddPrefix( this IList<string> texts, string prefix )
		{
			for( int i=0; i<texts.Count; i++ )
			{
				texts[i] = prefix + texts[i];
			}
		}
		/// <summary>
		/// Adds the specified /prefix/ to all strings in this IList and returns a new list.
		/// </summary>
		public static List<string> AddGetPrefix( this IList<string> texts, string prefix )
		{
			List<string> _texts = new List<string>( texts );
			_texts.AddPrefix( prefix );
			return _texts;
		}
		#endregion ADD

		#region MISC
		public static string LimitMaxChars( this string text, int max )
		{
			if( string.IsNullOrEmpty( text ) )
				return text;
			if( text.Length > max )
			{
				return text = text.Substring( 0, max-1 );
			}
			else return text;
		}		
		public static void Call( this string phoneNumber )
		{
			Application.OpenURL("tel://"+phoneNumber);
		}		
		public static void SendMail( this string email, string subject = "Subject", string body = "Message" )
		{
			Application.OpenURL( "mailto:" + email + "?subject=" + subject + "&body=" + body );
		}
		/// <summary>
		/// Returns a new string with the last space clamped.
		/// </summary>
		/// <param name="before">If set to <c>true</c> the clamp will be done before the last space; otherwise, after.</param>
		public static string ClampLastSpace( this string txt, int maxLength = 20, bool before = true )
		{
			if( string.IsNullOrEmpty( txt ) )
				return txt;
			if( txt.Length <= maxLength )
				return txt;
			int lastBlank = txt.LastIndexOf( " ", maxLength );
			char cap = txt[ lastBlank + 1 ];
			if( !before )
			{
				lastBlank = txt.IndexOf( " ", maxLength );
				cap = txt[ lastBlank + 1 ];
			}
			return txt.Substring( 0, lastBlank ) + " " + cap.ToString().ToUpper() + ".";
		}
		/// <summary>
		/// Returns a new string clamped to the specified /maxLength/.
		/// </summary>
		public static string Clamp( this string txt, int maxLength = 20 )
		{
			if( string.IsNullOrEmpty( txt ) )
				return txt;
			if( txt.Length <= maxLength )
				return txt;
			return txt.Substring( 0, maxLength );
		}
		/// <summary>
		/// Returns a new string clamped to the specified /maxLength/.
		/// </summary>
		/// <param name="extra"> This will be added to the returned string only if this is clamped, in other words, only
		/// if /maxLength/ is exceeded. The returned string with the added /extra/ will not exceed the specified /maxLength/ </param>
		public static string Clamp( this string txt, int maxLength, string extra )
		{
			if( string.IsNullOrEmpty( txt ) || string.IsNullOrEmpty( extra ) || maxLength <= extra.Length )
				return txt;
			if( txt.Length <= maxLength )
				return txt;			
			return txt.Substring( 0, maxLength - extra.Length ) + extra;
		}
		/*/// <summary>
		/// Returns a string array with the splitted items. The split is done everytime the maxLength is exceeded.
		/// </summary>
		/// <param name="before">If set to <c>true</c> the clamp will be done before the last space; otherwise, after.</param>
		/// <param name="capitalizeNext">If set to <c>true</c> the next line's first letter will be capitalized.</param>
		public static string[] Split( this string txt, int maxLength = 20, bool before = true, bool capitalizeNext = true )
		{
			if( string.IsNullOrEmpty( txt ) )
				return new string[] { txt };
			if( txt.Length <= maxLength )
				return new string[] { txt };

			string[] splittedText = new string[ Mathf.CeilToInt( txt.Length / maxLength ) ];

			string _txt = string.Copy( txt );
			string _cap;
			for( int i=0; i<splittedText.Length; i++ )
			{
				if( capitalizeNext )
				{
					_txt[ ]
				}
				int lastBlank = _txt.LastIndexOf( " ", maxLength );
				char cap = _txt[ lastBlank + 1 ];
				if( !before )
				{
					lastBlank = _txt.IndexOf( " ", maxLength );
					cap = _txt[ lastBlank + 1 ];
				}
				_cap = cap.ToString().ToUpper();
				splittedText[ i ] = _txt.Substring( 0, lastBlank );
				_txt = _txt.Substring( maxLength + 1, _txt.Length - maxLength );
			}
		}*/
		/// <summary>
		/// Determines if there is an active object with the specified names.
		/// </summary>
		public static bool IsThereAnActiveObj( this string[] names )
		{
			for( int i=0; i<names.Length; i++ )
			{
				string name = names[i];
				if( GameObject.Find( name ) ){
					return true;
				}
				else if( GameObject.Find( name+"(Clone)" ) ){
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Split each specified string each time its separator is found. The returning list can be read
		/// as follows: list[text index][splitted string index]
		/// </summary>
		/// <param name="txts">The texts strings.</param>
		/// <param name="separator">Separator.</param>
		public static List<string[]> Split( this string[] txts, string[] separator )
		{
			List<string[]> split = new List<string[]>();
			
			for( int i=0; i<txts.Length; i++ )
			{
				split.Add( txts[i].Split( separator, System.StringSplitOptions.None ) );
			}
			return split;
		}
		public static void Print( this IList<string> txt )
		{
			if( txt == null )
				return;
			for( int i=0; i<txt.Count; i++ )
			{
				Debug.Log ( txt[i]+"\n" );
			}
		}
		public static string Capitalize( this string txt, bool modifyThis = false )
		{
			if( modifyThis )
			{
				return txt = txt.Capitalize();
			}
			string text = txt;
			text = text.ToUpper();
			char cap = text[0];
			text = text.ToLower();
			return cap + text.Substring(1);
		}	
		public static char GetLastChar( this string txt )
		{
			return txt[ txt.Length - 1 ];
		}
		/// <summary>
		/// Removes the substring from the last "/"
		/// </summary>
		/// <returns>A string with the last end point removed.</returns>
		/// <param name="path">Path.</param>
		/// <param name="skipLastEndPoint">If true, the last end point won't be lowered.</param>
		/// <param name="modifySrcPath">If set to <c>true</c> the source path will be the same as the returned value.</param>
		public static string ToLower( this string text, bool skipLastEndPoint, bool modifySourceString = false )
		{
			int index = text.LastIndexOf("/");
			if( index < 0 )
			{
				return text.ToLower();
			}
			string _text = text.Substring( 0, index ).ToLower();
			if( skipLastEndPoint )
			{
				_text += text.GetLastEndPoint( true );
			}
			else _text += text.GetLastEndPoint( true ).ToLower();
			if( modifySourceString )
			{
				text = _text;
			}
			return _text;
		}
		/// <summary>
		/// Determines if this UI element (will be searched by name) is inside its canvas.
		/// </summary>
		public static bool IsInsideItsCanvas( this string uiObj )
		{
			return uiObj.Find().IsInsideItsCanvas();
		}
		/// <summary>
		/// Parses the specified string into an Integer. If it can't be parsed, a warning message will be shown.
		/// </summary>
		/// <param name="defaultReturn">The value to return if the string can't be parsed.</param>
		public static int ToInt( this string value, int defaultReturn = -1 )
		{
			int val = defaultReturn;
			if( !int.TryParse( value, out val ) )
			{
				Debug.LogWarning("The string that you want to parse to int can't be parsed: " + value);
			}
			return val;
		}
		/// <summary>
		/// Parses the specified string into a Float. If it can't be parsed, a warning message will be shown.
		/// </summary>
		/// <param name="defaultReturn">The value to return if the string can't be parsed.</param>
		public static float ToFloat( this string value, float defaultReturn = -1f )
		{
			float val = defaultReturn;
			value = value.Replace( "f", "" );
			if( !float.TryParse( value, out val ) )
			{
				Debug.LogWarning("The string that you want to parse to float can't be parsed: " + value);
			}
			return val;
		}
		/// <summary>
		/// Parses the specified string into a Boolean. If it can't be parsed, a warning message will be shown.
		/// </summary>
		/// <param name="value">True, False, T or F.</param>
		/// <param name="defaultReturn">The value to return if the string can't be parsed.</param>
		public static bool ToBool( this string value, bool defaultReturn = false )
		{
			string _val = value.ToLower();
			switch( _val )
			{
			case "true": return true;
			case "t": return true;
			case "1": return true;
			case "false": return false;
			case "f": return false;
			case "0": return false;
			}
			Debug.LogWarning("The string that you want to parse to bool can't be parsed: " + value);
			return defaultReturn;
		}
		/// <summary>
		/// This ensures the path ends with "/" and returns a new string with the correct ending.
		/// </summary>
		public static string CheckSeparator( this string path )
		{
			if( !path.EndsWith( "/" ) )
			{
				return path + "/";
			}
			return path;
		}
		public static bool ContainsIgnoreCase( this string text, string value )
		{
			CultureInfo culture = CultureInfo.CurrentCulture;
			if( culture.CompareInfo.IndexOf( text, value, CompareOptions.IgnoreCase ) >= 0 )
			{
				return true;
			}
			 return false;
		}
		public static StringBuilder Append( this StringBuilder builder, params string[] values )
		{
			if( values == null )
				return builder;
			for( int i=0; i<values.Length; i++ )
			{
				builder = builder.Append( values[i] );
			}
			return builder;
		}
		#endregion MISC

		#region VALIDATIONS
		/// <summary>
		/// Ensures this string is between the specified values. Returns a new string with the validation done.
		/// </summary>
		public static string ValidateNumber( this string text, float from, float to )
		{
			float input = float.Parse( text );
			if( input < from ) {
				text = from.ToString();
			}
			else if( input > to ) {
				text = to.ToString();
			}
			return text;
		}
		#endregion

		#region EXISTS CHECKING
		public static bool ClassExists( this string name )
		{
			return ( System.Type.GetType( name ) != null );
		}
		public static bool ObjectContainingSubstringExists( this string substring, bool searchForClones = true )
		{
			var objs = GameObject.FindObjectsOfType<Transform>();
			for( int i=0; i<objs.Length; i++ )
			{
				if( objs[i].name.Contains( substring ) )
				{
					return true;
				}
				else if ( objs[i].name.Contains( substring+"(Clone)" ) )
				{
					return true;
				}
			}
			return false;
		}
		
		public static bool AnyObjectContainingSubstringExists( this string[] substrings )
		{
			for( int i=0; i<substrings.Length; i++ )
			{
				if( substrings[i].ObjectContainingSubstringExists() ) 
					return true;
			}
			return false;
		}
		#endregion EXISTS CHECKING
					
		#region FIND
		/// <summary>
		/// Find the specified gameobjects by name. This also searches inside the inactive gameobjects array of the 
		/// InactivesHolder (InactivesHolder.cs) gameobject.
		/// </summary>
		/// <seealso cref="InactivesHolder.cs"/>
		/// <param name="names">Names.</param>
		/// <param name="searchForClones">If set to <c>true</c> search for clones.</param>
		public static List<GameObject> Find( this IList<string> names, bool searchForClones = true )
		{
			List<GameObject> objs = new List<GameObject>();
			
			for( int i=0; i<names.Count; i++ )
			{
				objs.Add( names[i].Find(searchForClones) );
			}
			return objs;
		}
		/// <summary>
		/// Find the specified components in the specified objs that will be searched by their names. If an object or component
		/// is null it will not be taken into account. This also searches inside the inactive gameobjects array of the 
		/// InactivesHolder (InactivesHolder.cs) gameobject.
		/// </summary>
		/// <seealso cref="InactivesHolder.cs"/>
		/// <returns> The found components. </returns>
		/// <param name="objsNames">Objects names.</param>
		/// <param name="searchInClone">If set to <c>true</c> search in clones.</param>
		public static List<T> Find<T>( this IList<string> objsNames, bool searchInClones = true ) where T : Component
		{
			List<T> components = new List<T>();
			
			var objs = objsNames.Find();
			for( int i=0; i<objs.Count; i++ )
			{
				if( objs[i] != null )//if object and component to search are valid, search for its component
				{
					var comp = objs[i].GetComponentInChildren<T>();
					if( !comp )
						comp = objs[i].GetComponentInParent<T>();
					components.Add( comp );
				}
			}
			return components;
		}
        /// <summary>
        /// Find the specified components in the specified objs that will be searched by their names. If an object or component
        /// is null it will not be taken into account. This also searches inside the inactive gameobjects array of the 
        /// InactivesHolder (InactivesHolder.cs) gameobject.
        /// </summary>
        /// <seealso cref="InactivesHolder.cs"/>
        /// <returns> The found components. </returns>
        /// <param name="objsNames">Objects names.</param>
        /// <param name="searchInClone">If set to <c>true</c> search in clones.</param>
        public static List<T> FindWithTags<T>( this IList<string> objsTags, bool searchInClones = true ) where T : Component
        {
            List<GameObject> objs = new List<GameObject>();
            int i=0;
            for( i=0; i<objsTags.Count; i++ )
            {
                objs.AddRange( GameObject.FindGameObjectsWithTag( objsTags[i] ) );
            }
            List<T> components = new List<T>( objs.Count );
            for( i=0; i<objs.Count; i++ )
            {
                var comp = objs[i].GetComponentInChildren<T>();
                if( !comp )
                    comp = objs[i].GetComponentInParent<T>();
                components.Add( comp );
            }
            return components;
        }
		/// <summary>
		/// Find the specified object by name. This also searches inside the inactive gameobjects array of the 
		/// InactivesHolder (InactivesHolder.cs) gameobject. Returns /null/ if no object is found.
		/// </summary>
		/// <seealso cref="InactivesHolder.cs"/>
		/// <param name="objName">Object name.</param>
		/// <param name="searchForClone">If set to <c>true</c> search for clone.</param>
		public static GameObject Find( this string objName, bool searchForClone = true )
		{
			if (!string.IsNullOrEmpty(objName))
			{
				var obj = GameObject.Find( objName );
				if( searchForClone && obj == null )
				{
					obj = GameObject.Find( objName+"(Clone)" );
				}
				if( obj == null )//Search inactives
				{
					obj = InactivesHolder.GetInactiveObjs().Find(objName);
				}
				return obj;
			}
			return null;
		}		
		/// <summary>
		/// Find the specified object's component by the object's name, the component can be in any children. If no 
		/// object with this name is found, a search will be done inside the inactive gameobjects array of the 
		/// InactivesHolder (InactivesHolder.cs) gameobject. Returns the default(T) if no /T/ is found.
		/// </summary>
		/// <seealso cref="InactivesHolder.cs"/>
		/// <param name="objName">Object name.</param>
		/// <param name="searchForClone">If set to <c>true</c> search for clone.</param>
		public static T Find<T>( this string objName, bool searchForClone = true ) where T : Component
		{
			GameObject obj = objName.Find( searchForClone );
			if( !obj )
				return default( T );
			T comp = obj.GetComponentInChildren<T>();
			if( comp != default( T ) )
				return comp;
			List<GameObject> inactiveObjs = InactivesHolder.GetInactiveObjs();
			if( inactiveObjs.Count == 0 )
				return default( T );
			var comps = GameObject.FindObjectsOfType<T>();
			comps = inactiveObjs.GetComponents<T>();
			for( int i=0; i<comps.Length; i++ )
			{
				if( comps[i].IsAnyAncestorNamed( objName ) || ( searchForClone && comps[i].IsAnyAncestorNamed( objName + "(Clone)" ) ) )
				{
					return comps[i];
				}
			}
			return default(T);

			/*var obj = objName.Find( searchForClone );
			if( obj )
			{
				return obj.GetComponent<T>();
			}
			return default(T);*/
		}	
		/// <summary>
		/// Find and object by tag. This also searches inside the inactive gameobjects array of the 
		/// InactivesHolder (InactivesHolder.cs) gameobject.
		/// </summary>
		/// <seealso cref="InactivesHolder.cs"/>
		/// <param name="objName">Object tag.</param>
		public static GameObject FindWithTag( this string tag )
		{
			GameObject obj;
			try{
				obj = GameObject.FindWithTag( tag );
			} catch( UnityException )
			{
				return null;
			}
			if( obj == null )//Search inactives
			{
				obj = InactivesHolder.GetWithTag(tag);
			}
			return obj;
		}	
		/// <summary>
		/// Find and object by tag. This also searches inside the inactive gameobjects array of the 
		/// InactivesHolder (InactivesHolder.cs) gameobject.
		/// </summary>
		/// <seealso cref="InactivesHolder.cs"/>
		/// <param name="objName">Object tag.</param>
		public static T FindWithTag<T>( this string tag ) where T : Object
		{
			GameObject obj = tag.FindWithTag();
			if( obj )
			{
				return obj.GetComponentInChildren<T>();
			}
			return null;
		}	
		/// <summary>
		/// Find an object by tag. This also searches inside the inactive gameobjects array of the 
		/// InactivesHolder (InactivesHolder.cs) gameobject. Returns an empty array if no objects were found.
		/// </summary>
		/// <seealso cref="InactivesHolder.cs"/>
		/// <param name="objName">Object tag.</param>
		public static GameObject[] FindByTag( this string tag )
		{
			var objs = GameObject.FindGameObjectsWithTag( tag );
			if( objs.Length == 0 )//Search inactives
			{
				objs = InactivesHolder.GetByTag(tag).ToArray();
			}
			return objs;
		}		
		/// <summary>
		/// Find behaviours by tag. This also searches inside the inactive gameobjects array of the 
		/// InactivesHolder (InactivesHolder.cs) gameobject. Returns an empty array if no /T/ were found.
		/// </summary>
		/// <seealso cref="InactivesHolder.cs"/>
		/// <param name="objName">Object tag.</param>
		public static T[] FindBehavioursByTag<T>( this string tag ) where T : Behaviour
		{
			var objs = tag.FindByTag();
			T[] comps = new T[objs.Length];
			for( int i=0; i<comps.Length; i++ )
			{
				comps[i] = objs[i].GetBehaviour<T>() as T;
			}
			return comps;
		}		
		/// <summary>
		/// Find components by tag. This also searches inside the inactive gameobjects array of the 
		/// InactivesHolder (InactivesHolder.cs) gameobject. Returns an empty array if no objects were found.
		/// </summary>
		/// <seealso cref="InactivesHolder.cs"/>
		/// <param name="objName">Object tag.</param>
		public static T[] FindComponentsByTag<T>( this string tag ) where T : Component
		{
			var objs = tag.FindByTag();
			T[] comps = new T[objs.Length];
			for( int i=0; i<comps.Length; i++ )
			{
				comps[i] = objs[i].GetComponent<T>();
			}
			return comps;
		}
		[System.Obsolete("Use FindUIText instead")]
		public static GUIText FindGuiText( this string objName, bool searchForClone = true )
		{
			GameObject obj = objName.Find( searchForClone );
			if( obj )
			{
				return obj.GetComponent<GUIText>();
			}
			else return null;
		}		
		public static Text FindUIText( this string objName, bool searchForClone = true )
		{
			GameObject obj = objName.Find( searchForClone );
			if( obj )
			{
				return obj.GetComponent<Text>();
			}
			else return null;
		}
		#endregion FIND

		#region OBJS MANAGEMENT
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy. This also checks if the object is null.
		/// The object will be searched by name in the hierarchy and inside the inactive objects array of the InactivesHolder (InactivesHolder.cs) gameobject. 
		/// Clones will also be searched.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static GameObject SetActiveInHierarchy( this string obj, bool active = true, bool destroyIfClone = true, float after = 0f, bool immediate = false )
		{
			var _obj = obj.Find();
			return _obj.SetActiveInHierarchy( active, destroyIfClone, after, immediate );
		}		
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy. This also checks if the object is null.
		/// The object will be searched by name in the hierarchy and inside the inactive objects array of the InactivesHolder (InactivesHolder.cs) gameobject. 
		/// Clones will also be searched.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static GameObject SetActiveInHierarchyIncludeChildren( this string obj, bool active = true, bool childrenActive = true, bool destroyIfClone = true, float after = 0f, bool immediate = false )
		{
			var _obj = obj.Find();
			return _obj.SetActiveInHierarchyIncludeChildren( active, childrenActive, destroyIfClone, after, immediate );
		}
		/// <summary>
		/// Sets the specified objects active/inactive. The object will be searched by name in the hierarchy and inside 
		/// the inactive objects array of the InactivesHolder (InactivesHolder.cs) gameobject. Clones will also be searched.
		/// </summary>
		/// <param name="objs">Objects.</param>
		/// <param name="active">If set to <c>true</c> active.</param>
		public static void SetActive( this IList<string> objs, bool active = true )
		{
			var _objs = objs.Find();
			_objs.SetActive();
		}		
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy. 
		/// The object will be searched by name in the hierarchy and inside the inactive objects array of the InactivesHolder
		/// (InactivesHolder.cs) gameobject. Clones will also be searched.
		/// </summary>
		/// <returns>The objects.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static IEnumerable SetGetActiveInHierarchy( this IList<string> objs, bool active = true, bool destroyIfClone = true, float after = 0f, bool immediate = false )
		{
			var _objs = objs.Find();
			foreach( var obj in _objs.SetGetActiveInHierarchy( active, destroyIfClone, after, immediate ) )
			{
				yield return obj;
			}
		}
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy. 
		/// The object will be searched by name in the hierarchy and inside the inactive objects array of the InactivesHolder
		/// (InactivesHolder.cs) gameobject. Clones will also be searched.
		/// </summary>
		/// <returns>The objects.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static IEnumerable SetGetActiveInHierarchyAfter( this IList<string> objs, float time, bool active = true )
		{
			var _objs = objs.Find();
			foreach( var obj in _objs.SetGetActiveInHierarchyAfter( time, active ) )
			{
				yield return obj;
			}
		}		
		/// <summary>
		/// Sets the specified object -active- in the hierarchy, if the object is a prefab it will be instantiated if active = true, 
		/// if it is a clone it will be destroyed if specified. Note: It will be considered as a clone if the 
		/// reference points to a prefab that has an instantiated clone in the hierarchy.
		/// </summary>
		/// <returns>The objects.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="active">If set to <c>true</c> the object is set to active.</param>
		/// <param name="destroyIfClone">If set to <c>true</c> the clone will be destroyed.</param>
		/// <param name="after">Destroy after.</param>
		/// <param name="immediate">If set to <c>true</c> the object will be immediately destroyed.</param>
		public static void SetActiveInHierarchy( this IList<string> objs, bool active = true, bool destroyIfClone = true, float after = 0f, bool immediate = false )
		{
			objs.Find().SetActiveInHierarchy( active, destroyIfClone, after, immediate );
		}		
		public static IEnumerator SetActiveInHierarchyAfter( this IList<string> objs, float time, bool active = true )
		{
			yield return new WaitForSeconds(time);
			objs.SetActiveInHierarchy( active );
		}		
		/// <summary>
		/// Also checks for clones (instances). If obj is null, false is returned.
		/// </summary>
		/// <returns><c>true</c> if the specified obj is active in hierarchy; otherwise, <c>false</c>.</returns>
		/// <param name="obj">Object.</param>
		public static bool IsActiveInHierarchy( this string obj )
		{
			var _obj = obj.Find();
			return _obj.IsActiveInHierarchy();
		}		
		public static void SetChildrenActive( this string obj, bool active = true )
		{
			var _obj = obj.Find();
			_obj.SetChildrenActive( active );
		}		
		public static void Destroy( this IList<string> objs, float after = 0f )
		{
			var _objs = objs.Find();
			_objs.Destroy( after );
		}
		/// <summary>
		/// Determines if the specified object is 2D (has a sprite renderer).
		/// </summary>
		public static bool Is2D( this string obj )
		{
			var _obj = obj.Find();
			return _obj.Is2D();
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Animates the string's color (using rich text). This is not optimized for performance, specially if used on 
        /// mobile devices.
		/// </summary>
		/// <returns> The same /txt/ string, each time with a color closer to the target. </returns>
		/// <param name="sizeIndex"> If higher than 0 and there is more than a size tag, then the specified tag will be the one modified </param>
		public static IEnumerable<string> AnimColorCo( this string txt, Color from, Color target, float duration, 
		                                      bool avoidAlpha = false, int sizeIndex = 0 )
		{
			if( string.IsNullOrEmpty( txt ) )
			{
				Debug.LogWarning ("No string to animate");
				yield break;
			}
			foreach( Color color in from.ChangeTowardsCo( target, duration, avoidAlpha ) )
			{
				int _sizeIndex = txt.IndexOf( "<color=#" );
				for( int i=0; i<sizeIndex; i++ )
				{
					int tempIndex = txt.IndexOf( "<color=#", _sizeIndex + 1 );
					if( tempIndex == -1 )
						break;
					_sizeIndex = tempIndex;
				}
				if( _sizeIndex != -1 )
				{
					yield return txt.ReplaceBetweenMatches( "<color=#", ">", ColorUtility.ToHtmlStringRGBA( color ) );
				}
				else yield return "<color=#" + ColorUtility.ToHtmlStringRGBA( color ) + ">" + txt + "</color>";
			}
		}
        /// <summary>
        /// Lerps the string's color (using rich text).
        /// </summary>
        /// <returns> The same /txt/ string, with a color closer to the target. </returns>
        public static string LerpColor( this string txt, Color from, Color target, float t, bool avoidAlpha = false )
        {
            if( string.IsNullOrEmpty( txt ) )
            {
                Debug.LogWarning ("No string to lerp. Returning null");
                return null;
            }
            Color color = from.Lerp( target, t, avoidAlpha );
            if( txt.IndexOf( "<color=#" ) != -1 )
            {
                return txt.ReplaceBetweenMatches( "<color=#", ">", ColorUtility.ToHtmlStringRGBA( color ) );
            }
            else return "<color=#" + ColorUtility.ToHtmlStringRGBA( color ) + ">" + txt + "</color>";
        }
		/// <summary>
		/// Animates the string's color (using rich text).
		/// </summary>
		/// <returns> The same /txt/ string, each time with a color closer to the target. </returns>
		/// <param name="sizeIndex"> If higher than 0 and there is more than a size tag, then the specified tag will be the one modified </param>
		public static IEnumerable<string> AnimSizeCo( this string txt, int from, int target, float duration, int sizeIndex = 0 )
		{
			if( string.IsNullOrEmpty( txt ) )
			{
				Debug.LogWarning ("No string to animate");
				yield break;
			}
			foreach( int size in from.MoveTowardsCo( target, duration ) )
			{
				int _sizeIndex = txt.IndexOf( "<size=" );
				for( int i=0; i<sizeIndex; i++ )
				{
					int tempIndex = txt.IndexOf( "<size=", _sizeIndex + 1 );
					if( tempIndex == -1 )
						break;
					_sizeIndex = tempIndex;
				}
				if( _sizeIndex != -1 )
				{
					yield return txt.ReplaceBetweenMatches( "<size=", ">", size.ToString() );
				}
				else yield return "<size=" + size.ToString() + ">" + txt + "</size>";
			}
		}
		/// <summary>
		/// Animates the string's color (using rich text).
		/// </summary>
		/// <returns> The same /txt/ string, each time with a color closer to the target. </returns>
		/// <param name="sizeIndex"> If higher than 0 and there is more than a size tag, then the specified tag will be the one modified </param>
		public static IEnumerable<string> AnimSizeAndColorCo( this string txt, int fromSize, int toSize, Color fromColor,
		                                             Color toColor, float duration, bool avoidAlpha = false )
		{
			if( string.IsNullOrEmpty( txt ) )
			{
				Debug.LogWarning ("No string to animate");
				yield break;
			}
			List<int> sizes = new List<int>( fromSize.MoveTowardsCo( toSize, duration ) );
			List<Color> colors = new List<Color>( fromColor.ChangeTowardsCo( toColor, duration, avoidAlpha ) );
			for( int i=0; i<sizes.Count; i++ ) //foreach( int size in fromSize.MoveTowardsCo( toSize, duration ) )
			{
				string text = "";
				if( txt.IndexOf( "<size=" ) != -1 )
				{
					text = txt.ReplaceBetweenMatches( "<size=", ">", sizes[i].ToString() );
					yield return text.ReplaceBetweenMatches( "<color=#", ">", ColorUtility.ToHtmlStringRGBA( colors[i] ) );
				}
				else 
				{
					text = "<size=" + sizes[i].ToString() + ">" + txt + "</size>";
					yield return "<color=#" + ColorUtility.ToHtmlStringRGBA( colors[i] ) + ">" + text + "</color>";
				}
			}
		}
		#endregion
	}


}