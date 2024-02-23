using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Tokens
{
	public struct Token
	{
		/// <summary>
		/// Start Position of Token
		/// </summary>
		public int StartPosition;

		/// <summary>
		/// Length of Token
		/// </summary>
		public int Length;

		/// <summary>
		/// TokenType
		/// </summary>
		public TokenType TokenType;

		/// <summary>
		/// Value of Token
		/// </summary>
		public object Value;

		#region Operator Overloads

		public static bool operator ==(Token a, Token b)
		{
			var result = a.TokenType == b.TokenType &&
				a.Value.Equals(b.Value) &&
				a.StartPosition == b.StartPosition &&
				a.Length == b.Length;
			return result;
		}

		public static bool operator !=(Token a, Token b)
		{
			return !(a == b);
		}

		#endregion

		#region Public Override Methods

		public override string ToString()
		{
			return $"{TokenType} | {Value} | {StartPosition} | {Length}";
		}

		public override bool Equals(object? obj)
		{
			if (obj is not Token)
				return false;

			return this == (Token)obj;
		}

		#endregion
	}
}
