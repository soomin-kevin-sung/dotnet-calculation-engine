using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Tokens
{
	public enum TokenType
	{		
		Integer,
		FloatingPoint,
		Text,
		Quote,
		DoubleQuote,
		Operation,
		LeftSquareBracket,
		RightSquareBracket,
		LeftRoundBracket,
		RightRoundBracket,
		ArgumentSeparator
	}
}
