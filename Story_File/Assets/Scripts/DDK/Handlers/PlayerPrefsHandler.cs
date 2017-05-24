//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.UI;
using DDK.Base.Statics;


namespace DDK.Handlers
{
	public class PlayerPrefsHandler : MonoBehaviour 
    {	
		[HelpBoxAttribute]
		public string msg = "If disabled, the actions won't be executed";
		public string key;
		[Header("Defaults")]
		public bool defaultBool = false;
		public int defaultInt = 0;
		public float defaultFloat = 0f;
		public string defaultString;


        protected string NULL_TEXT_MSG 
        {
            get{ return Constants.NULL_TEXT_MSG; }
        }


		void Start() {} //Alows enabling/disabling this component


		#region SET
		public void SetKeyToHandle( string key )
		{
			if( !enabled )
				return;
			this.key = key;
		}
		public void SetInt( int value )
		{
			if( !enabled )
				return;
			if( string.IsNullOrEmpty( key ) )
				return;
			PlayerPrefs.SetInt( key, value );
		}
		public void SetFloat( int value )
		{
			if( !enabled )
				return;
			if( string.IsNullOrEmpty( key ) )
				return;
			PlayerPrefs.SetFloat( key, value );
		}
		public void SetBool( bool value )
		{
			if( !enabled )
				return;
			if( string.IsNullOrEmpty( key ) )
				return;
			PlayerPrefs.SetInt( key, value ? 1 : 0 );
		}
		public void SetString( string value )
		{
			if( !enabled )
				return;
			if( string.IsNullOrEmpty( key ) )
				return;
			PlayerPrefs.SetString( key, value );
		}
		#endregion

		#region ADD
		public void AddToInt( int value )
		{
			if( !enabled )
				return;
			if( string.IsNullOrEmpty( key ) )
				return;
			int current = PlayerPrefs.GetInt( key, defaultInt );
			PlayerPrefs.SetInt( key, current + value );
		}
		public void AddToFloat( int value )
		{
			if( !enabled )
				return;
			if( string.IsNullOrEmpty( key ) )
				return;
			float current = PlayerPrefs.GetFloat( key, defaultFloat );
			PlayerPrefs.SetFloat( key, current + value );
		}
		public void AddCurrentIntValueToTextValue( Text text )
		{
			if( text.IsNull( NULL_TEXT_MSG, gameObject ) )
				return;
			text.text += GetInt( key ).ToString();
		}
		public void AddCurrentFloatValueToTextValue( Text text )
		{
			if( text.IsNull( NULL_TEXT_MSG, gameObject ) )
				return;
			text.text += GetFloat( key ).ToString();
		}
		public void AddCurrentStringValueToTextValue( Text text )
		{
			if( text.IsNull( NULL_TEXT_MSG, gameObject ) )
				return;
			text.text += GetString( key );
		}
		#endregion

		#region MISC
		public void Save()
		{
			if( !enabled )
				return;
			PlayerPrefs.Save();
		}
		public void DeleteKey( string key )
		{
			if( !enabled )
				return;
			if( PlayerPrefs.HasKey( key ) )
			{
				PlayerPrefs.DeleteKey( key );
			}
		}
		/// <summary>
		/// Deletes all keys and their values from the PlayerPrefs.
		/// </summary>
		public void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}
		#endregion

		#region CURRENT KEY'S VALUE TO SOME OBJECT'S VALUE
		/// <summary>
		/// Sets the specified behaviours enabled state as the current key's bool value.
		/// </summary>
		/// <param name="comp">Comp.</param>
		public void CurrentKeyValueToEnabledState( Behaviour comp )
		{
			if( comp.IsNull( "The specified Behaviour is null", gameObject ) )
				return;
			comp.enabled = GetBool( key );
		}
		/// <summary>
		/// Sets the AudioListener paused state as the current key's bool value.
		/// </summary>
		public void CurrentKeyValueToAudioListenerPausedState()
		{
			AudioListener.pause = GetBool( key );
		}
		/// <summary>
		/// Sets the AudioListener paused state as the current key's bool value.
		/// </summary>
		public void CurrentKeyValueToAudioListenerResumedState()
		{
			AudioListener.pause = !GetBool( key );
		}
		/// <summary>
		/// Sets the AudioListener paused state as the current key's bool value.
		/// </summary>
		public void CurrentKeyValueToToggleState( Toggle toggle )
		{
			if( toggle.IsNull( "The specified Toggle is null", gameObject ) )
				return;
			toggle.isOn = GetBool( key );
		}
		public void CurrentIntValueToTextValue( Text text )
		{
			if( text.IsNull( NULL_TEXT_MSG, gameObject ) )
				return;
			text.text = GetInt( key ).ToString();
		}
		public void CurrentFloatValueToTextValue( Text text )
		{
			if( text.IsNull( NULL_TEXT_MSG, gameObject ) )
				return;
			text.text = GetFloat( key ).ToString();
		}
		public void CurrentStringValueToTextValue( Text text )
		{
			if( text.IsNull( NULL_TEXT_MSG, gameObject ) )
				return;
			text.text = GetString( key );
		}
		#endregion



		#region GET
		public bool GetBool( string key )
		{
			if( string.IsNullOrEmpty( key ) )
				return defaultBool;
			return PlayerPrefs.GetInt( key, defaultBool ? 1 : 0 ) == 1 ? true : false;
		}
		public int GetInt( string key )
		{
			if( string.IsNullOrEmpty( key ) )
				return defaultInt;
			return PlayerPrefs.GetInt( key, defaultInt );
		}
		public float GetFloat( string key )
		{
			if( string.IsNullOrEmpty( key ) )
				return defaultFloat;
			return PlayerPrefs.GetFloat( key, defaultFloat );
		}
		public string GetString( string key )
		{
			if( string.IsNullOrEmpty( key ) )
				return defaultString;
			return PlayerPrefs.GetString( key, defaultString );
		}
		#endregion
	}
}
