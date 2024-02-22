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
	}
}
