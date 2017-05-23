using UnityEngine;
using System;

/*

will stop the the user entering a value above this bound

*/
public class NotMoreThan : SingleBound
{
    public NotMoreThan(int upperBound) { IntBound = upperBound; }
    public NotMoreThan(float upperBound) { FloatBound = upperBound; }

	#region DANIEL
	//DANIEL. Added to prevent Unity bug that doesn't allow using multiple attributes
	public NotMoreThan( int upperBound, string tooltip ) 
	{ 
		IntBound = upperBound;
		this.tooltip = tooltip;
	}
	public NotMoreThan( float upperBound, string tooltip ) 
	{ 
		FloatBound = upperBound;
		this.tooltip = tooltip;
	}
    public NotMoreThan( int lowerBound, string tooltip, string validationFunction, bool reverseCondition = false ) : this( lowerBound, tooltip )
    { 
        this._method = validationFunction;
        this._reverseCondition = reverseCondition;
    }
    public NotMoreThan( float lowerBound, string tooltip, string validationFunction, bool reverseCondition = false ) : this( lowerBound, tooltip )
    { 
        this._method = validationFunction;
        this._reverseCondition = reverseCondition;
    }
    public NotMoreThan( int lowerBound, string tooltip, int extraIndentLevel, string validationFunction, 
        bool reverseCondition = false ) : this( lowerBound, tooltip, validationFunction, reverseCondition )
    { 
        this._extraIndentLevel = extraIndentLevel;
    }
    public NotMoreThan( float lowerBound, string tooltip, int extraIndentLevel, string validationFunction, 
        bool reverseCondition = false ) : this( lowerBound, tooltip, validationFunction, reverseCondition )
    { 
        this._extraIndentLevel = extraIndentLevel;
    }
	#endregion
}