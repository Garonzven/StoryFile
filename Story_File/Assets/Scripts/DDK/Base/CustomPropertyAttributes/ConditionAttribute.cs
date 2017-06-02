//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DDK {
	    
    /// <summary>
	/// Apply to a field to show it only if a condition is fulfilled.
	/// </summary>
	public class ConditionAttribute : PropertyAttribute {
		
		/// <summary>
		/// The validation function, field or property. Any case must return bool.
		/// </summary>
		public string _methodFieldOrProperty;
		public int _extraIndentLevel;
		/// <summary>
		/// If true, the condition's result will be reversed.
		/// </summary>
		public bool _reverseCondition;
		
		//RANGE
		public float fMin;
		public float fMax;
		public int iMin;
		public int iMax;

        //public string header;



        //VALIDATIONS
#if UNITY_EDITOR
        public bool IsItRange(SerializedProperty prop)
        {
            if (fMin != 0f || fMax != 0f)
            {
                if (prop.propertyType == SerializedPropertyType.Float)
                    return true;
                else {
                    Debug.LogError("The property /" + prop.name + "/ must be of type Float");
                    return false;
                }
            }
            else if (iMin != 0 || iMax != 0)
            {
                if (prop.propertyType == SerializedPropertyType.Integer)
                    return true;
                else {
                    Debug.LogError("The property /" + prop.name + "/ must be of type Integer");
                    return false;
                }
            }
            return false;
        }
#endif
        /// <summary>
        /// Returns true if the attribute is a range attribute with values higher than 100 or lower than -100.
        /// </summary>
        public bool ShouldBeIntSlider {
			get{
				if( (iMin >= -100 && iMax <= 100) && ( iMin != 0 || iMax != 0 ) )
					return true;
				return false;
			}
		}
		/// <summary>
		/// Returns true if the attribute is a range attribute with values higher than 100 or lower than -100.
		/// </summary>
		public bool ShouldBeFloatSlider {
			get{
				if( (fMin >= -100f && fMax <= 100f) && ( fMin != 0f || fMax != 0f ) )
					return true;
				return false;
			}
		}
		
		
		
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ConditionAttribute ( string validationFunctionFieldOrProperty, int extraIndentLevel = 0 ) 
		{
			_methodFieldOrProperty = validationFunctionFieldOrProperty;
			_extraIndentLevel = extraIndentLevel;
		}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="hideInstead">If true, the condition's result will be reversed.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ConditionAttribute ( string validationFunctionFieldOrProperty, bool reverseCondition, int extraIndentLevel = 0 ) 
			: this( validationFunctionFieldOrProperty, extraIndentLevel )
		{			
			_reverseCondition = reverseCondition;
		}
		
		#region RANGE		
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ConditionAttribute ( float min, float max, string validationFunctionFieldOrProperty, int extraIndentLevel = 0 ) 
			: this( validationFunctionFieldOrProperty, extraIndentLevel )
		{
			fMin = min;
			fMax = max;
		}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="hideInstead">If true, the condition's result will be reversed.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ConditionAttribute ( float min, float max, string validationFunctionFieldOrProperty, bool reverseCondition, 
			int extraIndentLevel = 0 ) : this( min, max, validationFunctionFieldOrProperty, extraIndentLevel )
		{			
			_reverseCondition = reverseCondition;
		}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ConditionAttribute ( int min, int max, string validationFunctionFieldOrProperty, int extraIndentLevel = 0 ) 
			: this( validationFunctionFieldOrProperty, extraIndentLevel )
		{
			iMin = min;
			iMax = max;
		}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="hideInstead">If true, the condition's result will be reversed.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ConditionAttribute ( int min, int max, string validationFunctionFieldOrProperty, bool reverseCondition, 
			int extraIndentLevel = 0 ) : this( min, max, validationFunctionFieldOrProperty, extraIndentLevel )
		{			
			_reverseCondition = reverseCondition;
		}
		#endregion

        
        /*
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunction">The name of the function/method that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="hideInstead">If true, the condition's result will be reversed.</param>
		public ShowIfAttribute ( string header, string validationFunction, bool hideInstead ) : this( validationFunction, hideInstead )
		{			
			this.header = header;
		}*/
    }



	/// <summary>
	/// Apply to a field to show it only if a condition is fulfilled.
	/// </summary>
	public class ShowIfAttribute : ConditionAttribute {

		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ShowIfAttribute ( string validationFunctionFieldOrProperty, int extraIndentLevel = 0 )
		: base( validationFunctionFieldOrProperty, extraIndentLevel ){}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="hideInstead">If true, the condition's result will be reversed.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ShowIfAttribute ( string validationFunctionFieldOrProperty, bool reverseCondition, int extraIndentLevel = 0 )
		: base( validationFunctionFieldOrProperty, reverseCondition, extraIndentLevel ){}
		
		#region RANGE		
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ShowIfAttribute ( float min, float max, string validationFunctionFieldOrProperty, int extraIndentLevel )
		: base( min, max, validationFunctionFieldOrProperty, extraIndentLevel ){}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="hideInstead">If true, the condition's result will be reversed.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ShowIfAttribute ( float min, float max, string validationFunctionFieldOrProperty, bool reverseCondition, int extraIndentLevel = 0 )
		: base( min, max, validationFunctionFieldOrProperty, reverseCondition, extraIndentLevel ){}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public ShowIfAttribute ( int min, int max, string validationFunctionFieldOrProperty, int extraIndentLevel )
		: base( min, max, validationFunctionFieldOrProperty, extraIndentLevel ){}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="hideInstead">If true, the condition's result will be reversed.</param>
        /// <param name="extraIndentLevel">Extra indentation level.</param>
		public ShowIfAttribute ( int min, int max, string validationFunctionFieldOrProperty, bool reverseCondition, int extraIndentLevel = 0 )
		: base( min, max, validationFunctionFieldOrProperty, reverseCondition, extraIndentLevel ){}
        #endregion
	}	
	/// <summary>
	/// Apply to a field to show it only if a condition is fulfilled.
	/// </summary>
	public class DisableIfAttribute : ConditionAttribute {

		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public DisableIfAttribute ( string validationFunctionFieldOrProperty, int extraIndentLevel = 0 )
		: base( validationFunctionFieldOrProperty, extraIndentLevel ){}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="hideInstead">If true, the condition's result will be reversed.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public DisableIfAttribute ( string validationFunctionFieldOrProperty, bool reverseCondition, int extraIndentLevel = 0 )
		: base( validationFunctionFieldOrProperty, reverseCondition, extraIndentLevel ){}
		
		#region RANGE		
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public DisableIfAttribute ( float min, float max, string validationFunctionFieldOrProperty, int extraIndentLevel = 0 )
		: base( min, max, validationFunctionFieldOrProperty, extraIndentLevel ){}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="hideInstead">If true, the condition's result will be reversed.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public DisableIfAttribute ( float min, float max, string validationFunctionFieldOrProperty, bool reverseCondition, int extraIndentLevel = 0 )
		: base( min, max, validationFunctionFieldOrProperty, reverseCondition, extraIndentLevel ){}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="extraIndentLevel">Extra indentation level.</param>
		public DisableIfAttribute ( int min, int max, string validationFunctionFieldOrProperty, int extraIndentLevel = 0 )
		: base( min, max, validationFunctionFieldOrProperty, extraIndentLevel ){}
		/// <summary>
		/// Allows showing the property only if a condition is fulfilled.
		/// </summary>
		/// <param name="validationFunctionFieldOrProperty">The name of the function/field/property that will validate the field. NOTE: 
		/// It must return a bool value, if the value is true the field is shown.</param>
		/// <param name="hideInstead">If true, the condition's result will be reversed.</param>
        /// <param name="extraIndentLevel">Extra indentation level.</param>
		public DisableIfAttribute ( int min, int max, string validationFunctionFieldOrProperty, bool reverseCondition, int extraIndentLevel = 0 )
        : base( min, max, validationFunctionFieldOrProperty, reverseCondition, extraIndentLevel ){}
        #endregion
    }	
}
