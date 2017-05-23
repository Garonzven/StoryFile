//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System;
using DDK.Base.Classes;
using UnityEngine.UI;
using System.Globalization;
using DDK.Base.Statics;
using DDK.Base.Extensions;


namespace DDK.Platforms.iOS
{
    /// <summary>
    /// Date time picker for iOS devices, Android is pending implementation.
    /// </summary>
	public class DateTimePicker : MonoBehaviour 
	{
		[System.Serializable]
		public class TimeTextTargets
		{
			public Text hour;
			public Text minutes;
			public Text seconds;
			[Tooltip( "AM/PM" )]
			public Text daytime;
		}


		[HelpBoxAttribute]
		public string msg = "If this has a button component, the Show() action is automatically added. If disabled, actions won't be invoked";
#if UNITY_IOS && USE_IOS_NATIVE
		public IOSDateTimePickerMode mode = IOSDateTimePickerMode.Time;
#endif
		public TimeTextTargets time = new TimeTextTargets();
		[Header("Events")]
		public ComposedEvent onDateChanged = new ComposedEvent();
		public ComposedEvent onPickerClosed = new ComposedEvent();


		public DateTime PickedDateTime { get; private set; }
		/// <summary>
		///Holds a reference if this object has a Button component.
		/// </summary>
		protected Button _bt;
		protected bool _isShowingPicker;


		// Use this for initialization
		void Awake () 
		{
#pragma warning disable 0162
			//Subscribing to actions
#if UNITY_IOS && USE_IOS_NATIVE
			IOSDateTimePicker.Instance.OnDateChanged += OnDateChanged;
			IOSDateTimePicker.Instance.OnPickerClosed += OnPickerClosed;
#else
			enabled = false;
			return;
#endif
			_bt = GetComponentInChildren<Button>();
			if( _bt )
			{
				_bt.onClick.AddListener( () => Show () );
			}
#pragma warning restore 0162
		}
		void Start() {}//Allows enabling/disabling this component
		void OnEnable()
		{
			PickedDateTime = DateTime.Now;
		}


		public void Show()
		{
#pragma warning disable 0162
#if UNITY_EDITOR
			Utilities.Log ( "This only works on the device", gameObject );
			return;
#endif
#if UNITY_IOS && USE_IOS_NATIVE
			if( !enabled || _isShowingPicker )
				return;
			IOSDateTimePicker.Instance.Show( mode, PickedDateTime );
			_isShowingPicker = true;
#endif
#pragma warning restore 0162
		}
		public void SetHour( string value )
		{
			if( !time.hour || !enabled )
				return;
			time.hour.text = value.PadLeft( 2, '0');
		}
		public void SetMinutes( string value )
		{
			if( !time.minutes || !enabled )
				return;
			time.minutes.text = value.PadLeft( 2, '0');
        }
		public void SetSeconds( string value )
		{
			if( !time.seconds || !enabled )
				return;
			time.seconds.text = value.PadLeft( 2, '0');
        }
		public void SetDaytime( string value = "AM" )
		{
			if( !time.daytime || !enabled )
				return;
			if( !value.Equals( "AM" ) || !value.Equals( "PM" ) )
			{
				value = PickedDateTime.ToString( "tt", CultureInfo.InvariantCulture );
			}
			time.daytime.text = value;
        }
		public void SetDaytime( DateTime tt )
		{
			if( tt == default( DateTime ) || !time.daytime || !enabled )
				return;
			time.daytime.text = tt.ToString( "tt", CultureInfo.InvariantCulture );
        }
        public void SetPickedDateTime( DateTime value )
		{
			if( !enabled )
				return;
			PickedDateTime = value;
			SetHour( value.Hour.ToString() );
			SetMinutes( value.Minute.ToString() );
			SetSeconds( value.Second.ToString() );
			SetDaytime( value );
		}
		public void SetPickedDateTimeFromCurrentTimeTargets()
		{
			DateTime dateTime = DateTime.Today;
			dateTime = dateTime.AddHours( (double)time.hour.text.ToInt() );
			dateTime = dateTime.AddMinutes( (double)time.minutes.text.ToInt() );
			if( time.daytime.text.Equals( "PM" ) )
			{
				dateTime = dateTime.AddHours( 12d );
			}
			PickedDateTime = dateTime;
		}


		#region EVENTS
		void OnDateChanged( DateTime dateTime ) 
		{
			if( !enabled )
				return;
			Utilities.Log( "OnDateChanged: " + dateTime.ToString(), gameObject );
			onDateChanged.Invoke();
		}		
		void OnPickerClosed( DateTime dateTime ) 
		{
			if( !enabled )
				return;
			Utilities.Log( "OnPickerClosed: " + dateTime.ToString(), gameObject );
			SetPickedDateTime( dateTime );
			onPickerClosed.Invoke();
			_isShowingPicker = false;
		}
		#endregion
	}
}
