using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Tokens
{
	public enum TokenType
	{		
		Integer,
		FloatingPoint,
		Text,
		MemberPoint,
		String,
		Operation,
		LeftBracket,
		RightBracket,
		ArgumentSeparator
	}
}
