using UnityEngine;
using System.Collections;

public class RegexAttribute : PropertyAttribute {

	public enum MinMaxPatterns {
		minMaxLengthJustLetters,
		minMaxLengthLettersAndNumbers
	}


	public readonly string pattern;
	public readonly string helpMessage;

	public readonly int min;
	public readonly int max;

	public RegexAttribute ( string pattern, string helpMessage ) {

		this.pattern = pattern;
		this.helpMessage = helpMessage;
	}

	public RegexAttribute ( MinMaxPatterns pattern, int min, int max, string helpMessage ) {

		this.min = min;
		this.max = max;
		this.pattern = GetMinMaxPattern( pattern );
		this.helpMessage = helpMessage;
	}



	public string GetMinMaxPattern( MinMaxPatterns pattern )
	{
		switch( pattern )
		{
		case MinMaxPatterns.minMaxLengthJustLetters: return string.Format( "^[a-zA-Z.]{{{0},{1}}}$", min, max );
		case MinMaxPatterns.minMaxLengthLettersAndNumbers: return string.Format( "^[a-zA-Z0-9.]{{{0},{1}}}$", min, max );
		}
		return null;
	}

}
