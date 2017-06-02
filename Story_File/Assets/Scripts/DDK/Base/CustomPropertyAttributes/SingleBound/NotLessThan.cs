using UnityEngine;
using System;

/*

will stop the the user entering a value below this bound

*/
public class NotLessThan : SingleBound
{
    public NotLessThan( int lowerBound ) { IntBound = lowerBound; }
    public NotLessThan( float lowerBound ) { FloatBound = lowerBound; }

	#region DANIEL
	//DANIEL. Added to prevent Unity bug that doesn't allow using multiple attributes
	public NotLessThan( int lowerBound, string tooltip ) 
	{ 
		IntBound = lowerBound;
		this.tooltip = tooltip;
	}
	public NotLessThan( float lowerBound, string tooltip ) 
	{ 
		FloatBound = lowerBound;
		this.tooltip = tooltip;
	}
    public NotLessThan( int lowerBound, string tooltip, string validationFunction, bool reverseCondition = false ) : this( lowerBound, tooltip )
    { 
        this._method = validationFunction;
        this._reverseCondition = reverseCondition;
    }
    public NotLessThan( float lowerBound, string tooltip, string validationFunction, bool reverseCondition = false ) : this( lowerBound, tooltip )
    { 
        this._method = validationFunction;
        this._reverseCondition = reverseCondition;
    }
    public NotLessThan( int lowerBound, string tooltip, int extraIndentLevel, string validationFunction, 
        bool reverseCondition = false ) : this( lowerBound, tooltip, validationFunction, reverseCondition )
    { 
        this._extraIndentLevel = extraIndentLevel;
    }
    public NotLessThan( float lowerBound, string tooltip, int extraIndentLevel, string validationFunction, 
        bool reverseCondition = false ) : this( lowerBound, tooltip, validationFunction, reverseCondition )
    { 
        this._extraIndentLevel = extraIndentLevel;
    }
	#endregion
}