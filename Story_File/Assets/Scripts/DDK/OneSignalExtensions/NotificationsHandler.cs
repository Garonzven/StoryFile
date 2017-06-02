using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Classes;
using System.Globalization;
using DDK.Base.Extensions;


#if USE_ONE_SIGNAL
using OneSignalPush.MiniJSON;
#endif


namespace DDK.OneSignalExtensions
{
	public class NotificationsHandler : MonoBehaviourExt 
	{
#if USE_ONE_SIGNAL
		#region ENUMS / CLASSES
		public enum Relations { EqualTo, GreaterThan, LowerThan }
		public enum Operator { OR }
		
		[System.Serializable]
		public class Content
		{
			[ShowIfAttribute( "_UpdateName" )]
			[SerializeField]
			protected string name;
			public SystemLanguage language = SystemLanguage.English;
			public string headings = "Title";
			public string message = "Test Message";
			
			protected bool _UpdateName()
			{
				name = language.ToString();
				return false;
			}
		}
		[System.Serializable]
		public class Dic
		{
			[ShowIfAttribute( "_UpdateName" )]
			[SerializeField]
			protected string name;
			[Tooltip("True if this element is an OR operator")]
			[SerializeField]
			private bool OR;
			[ShowIfAttribute( "OR", true )]
			public string key = "key";
			[ShowIfAttribute( "OR", true )]
			public Relations relation = Relations.EqualTo;
			[ShowIfAttribute( "OR", true )]
			public string value = "value";
			
			
			protected bool _UpdateName()
			{
				if( OR )
					name = "OR";
				else name = key + " " + relation.ToString() + " " + value;
				return false;
			}
			
			
			protected const string KEY = "key";
			protected const string RELATION = "relation";
			protected const string VALUE = "value";
			protected const string OPERATOR = "operator";
			public string Relation 
			{
				get
				{
					switch( relation )
					{
					case Relations.EqualTo: return "=";
					case Relations.GreaterThan: return ">";
					default: return "<";
					}
				}
			}
			public Dictionary<string, string> Get()
			{
				Dictionary<string, string> dic = new Dictionary<string, string>();
				if( OR )
				{
					dic.Add( OPERATOR, "OR" );
				}
				else
				{
					dic.Add( KEY, key );
					dic.Add( RELATION, Relation );
					dic.Add( VALUE, value );
				}
				return dic;
			}
		}
		#endregion
		
		
		public Content[] contents = new Content[]{ new Content() };
		[Header("Targeting Parameters")]
		[Tooltip("The first valid will take priority over the following Targeting Parameters")]
		public List<string> playerIds = new List<string>();
		[Tooltip("The first valid will take priority over the following Targeting Parameters")]
		public Dic[] tags = new Dic[0];
		[Header( "Events" )]
		[Tooltip("This is used on some multiparam functions to separate the parameters")]
		public char argsSeparator = ':';
		public ComposedEvent onPostNotificationSuccess = new ComposedEvent();
		public ComposedEvent onPostNotificationFailure = new ComposedEvent();
		
		
		private static string extraMessage;
		private static Dictionary<string, string> _Contents { get; set; }
		private static Dictionary<string, string> _Headings { get; set; }
		
		
		void Start ()
		{
			_UpdateContentsAndHeadings();
		}
		
		protected virtual void OnPostNotificationSuccess( Dictionary<string, object> response )
		{
			onPostNotificationSuccess.Invoke();
			extraMessage = "Notification posted successful! \n" + Json.Serialize ( response );
			Debug.Log ( extraMessage );

		}
		protected virtual void OnPostNotificationFailure( Dictionary<string, object> response )
		{
			onPostNotificationFailure.Invoke();
			extraMessage = "Notification failed to post:\n" + Json.Serialize ( response );
			Debug.LogError ( extraMessage );
		}
		
		/// <summary>
		/// Updates the _Contents and _Headings dictionaries.
		/// </summary>
		protected void _UpdateContentsAndHeadings()
		{
			if( _Contents != null )
			{
				_Contents.Clear();
				_Contents = null;
			}
			if( _Headings != null )
			{
				_Headings.Clear();
				_Headings = null;
			}
			_Contents = new Dictionary<string, string>();
			_Headings = new Dictionary<string, string>();
			// Get all available cultures on the current system.
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			
			for( int i=0; i<contents.Length; i++ )
			{
				for( int j=0; j<cultures.Length; j++ ) 
				{				
					string code =  cultures[j].Name;
					// Only all two-letter codes.
					if( code.Length != 2 || !cultures[j].DisplayName.Contains( contents[i].language.ToString() ) )
						continue;
					_Contents.Add( code, contents[i].message );
					_Headings.Add( code, contents[i].headings );
					break;
				}   
			}
		}
		protected Dictionary<string, string>[] _GetTags()
		{
			Dictionary<string, string>[] _tags = new Dictionary<string, string>[ tags.Length ];
			for( int i=0; i<tags.Length; i++ )
			{
				_tags[i] = tags[i].Get();
			}
			return _tags;
		}
		
		
		public virtual void PostNotification()
		{
			//ASSEMBLY DICTIONARY
			Dictionary<string, object> notification = new Dictionary<string, object>();
			notification.Add( OneSignalKeys.contents.ToString(), _Contents );
			notification.Add( OneSignalKeys.headings.ToString(), _Headings );
			//TARGETING PARAMETER
			if( playerIds.Count > 0 )
			{
				notification.Add( OneSignalKeys.include_player_ids.ToString(), playerIds );
			}
			else if( tags.Length > 0 )
			{
				notification.Add( OneSignalKeys.tags.ToString(), _GetTags() );
			}
			
			OneSignal.PostNotification( notification, OnPostNotificationSuccess, OnPostNotificationFailure );
		}
		public virtual void SendTag( string key_value )
		{ 
			string[] args = key_value.Split( argsSeparator );
			if( args.Length != 2 )
			{
				Debug.LogWarning ("Wrong param format, it must match: key" + argsSeparator + "value" );
				return;
			}
			OneSignal.SendTag( args[0], args[1] );
		}
		public virtual void DeleteTag( string key )
		{ 
            OneSignal.DeleteTag( key );
        }
#endif
    }
}
