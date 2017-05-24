using UnityEngine;
using UnityEngine.UI;
using DDK.Base.Extensions;
using System;
using System.Globalization;
using DDK.Base.Statics;


namespace DDK.Base.UI
{
	[RequireComponent( typeof( InputField ) )]
	public class InputFieldValidator : MonoBehaviour 
	{
        [ShowIfAttribute( "onEndEdit", true )]
		public bool onValueChanged;
        [ShowIfAttribute( "onValueChanged", true )]
		public bool onEndEdit = true;
		//DECIMAL
		[Space( 5f )]
		[ShowIfAttribute( "_IsDecimal" )]
		public float minValue = 0f;
		[ShowIfAttribute( "_IsDecimal" )]
		public float maxValue = float.MaxValue;
		//INT
		[ShowIfAttribute( "_IsInt" )]
		public int minNumber = 0;
		[ShowIfAttribute( "_IsInt" )]
		public int maxNumber = int.MaxValue;
        //CUSTOM
        [ShowIfAttribute( "_IsCustom" )]
        public bool isDate;
        [ShowIfAttribute( "isDate", 1 )]
        public string dateFormat = "MM/dd/yyyy";
        [ShowIfAttribute( "isDate", 1 )]
        public bool maxDateIsToday;


		protected bool _IsDecimal()
		{
			return m_InputField.contentType == InputField.ContentType.DecimalNumber;
		}
		protected bool _IsInt()
		{
			return m_InputField.contentType == InputField.ContentType.IntegerNumber;
		}
        protected bool _IsCustom()
        {
            return m_InputField.contentType == InputField.ContentType.Custom;
        }


		private InputField _inputField;
		public InputField m_InputField
        {
			get
            {
				if( !_inputField )
					_inputField = GetComponent<InputField>();
				return _inputField;
			}
		}


		// Use this for initialization
		void Start () 
        {
			if( !m_InputField )
				return;
			if( onEndEdit )
			{
				m_InputField.onEndEdit.AddListener( (x) => Validate( m_InputField.text ) );
			}
			else if( onValueChanged )
			{
				m_InputField.onValueChanged.AddListener( (x) => Validate( m_InputField.text ) );
			}
		}


		public void Validate( string value )
		{
			if( _IsDecimal() )
			{
				m_InputField.text = Mathf.Clamp( value.ToFloat(), minValue, maxValue ).ToString();
			}
			else if ( _IsInt() )
			{
				m_InputField.text = Mathf.Clamp( value.ToInt(), minNumber, maxNumber ).ToString();
			}
            else if ( _IsCustom() )
            {
                DateTime date;
                bool isValid = DateTime.TryParseExact( m_InputField.text, dateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date);
                if( maxDateIsToday )
                {
                    isValid = date.CompareTo( DateTime.Today ) <= 0;
                }
                if( isValid )
                {
                    isValid = date.Year > 1916;
                }
                if( isValid )
                {
                    m_InputField.text = date.ToString();
                }
                else 
                {
                    Utilities.Log( Color.yellow, string.Format( "Input Date doesn't match format: {0}", dateFormat ) );
                    m_InputField.text = "";
                }
            }
		}
	}
}
