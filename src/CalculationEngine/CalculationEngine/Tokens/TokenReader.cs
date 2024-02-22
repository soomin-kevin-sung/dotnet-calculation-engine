using CalculationEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Tokens
{
	public class TokenReader
	{
		public TokenReader()
			: this(CultureInfo.CurrentCulture)
		{

		}

		public TokenReader(CultureInfo cultureInfo)
		{
			CultureInfo = cultureInfo;
			DecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator[0];
			ArgumentSeparator = cultureInfo.TextInfo.ListSeparator[0];

		}

		#region Static Members

		static TokenReader()
		{
			Operators = new HashSet<char>()
			{
				' ', '+', '-', '*', '/',
				'^', '%', '!', '&', '|',
				'=', ',', '(', ')', '<',
				'>', '[', ']', '≤', '≥',
				'≠'
			};
		}

		#region Public Properties

		/// <summary>
		/// Operators
		/// </summary>
		public static IReadOnlySet<char> Operators { get; }

		#endregion



		#endregion

		#region Public Properties

		/// <summary>
		/// Current CultureInfo
		/// </summary>
		public CultureInfo CultureInfo { get; }
		/// <summary>
		/// DecimalSeparator from CultureInfo
		/// </summary>
		public char DecimalSeparator { get; }
		/// <summary>
		/// ArgumentSeparator from CultureInfo
		/// </summary>
		public char ArgumentSeparator { get; }

		#endregion

		#region Public Methods

		public IReadOnlyList<Token> Read(string formula)
		{
			if (string.IsNullOrEmpty(formula))
				throw new ArgumentNullException("formula");

			var tokens = new List<Token>();
			var chars = formula.ToCharArray();

			bool isFormulaSubPart = true;
			bool isScientific = false;

			for (int i = 0; i < chars.Length; i++)
			{
				if (IsPartOfNumeric(chars[i], true, isFormulaSubPart))
				{
					StringBuilder buffer = new StringBuilder();
					buffer.Append(chars[i]);

					int startPosition = i;
					while (++i < chars.Length && IsPartOfNumeric(chars[i], false, isFormulaSubPart))
					{
						if (isScientific && IsScientificNotation(chars[i]))
							throw new ParseException(string.Format("Invalid token \"{0}\" detected at position {1}.", chars[i], i));

						if (IsScientificNotation(chars[i]))
						{
							isScientific = IsScientificNotation(chars[i]);
							if (chars[i + 1] == '-')
								buffer.Append(chars[i++]);
						}

						buffer.Append(chars[i]);
					}

					var s = buffer.ToString();

					if (int.TryParse(buffer.ToString(), out int intValue))
					{
						tokens.Add(new Token() { TokenType = TokenType.Integer, Value = intValue, StartPosition = startPosition, Length = i - startPosition });
						isFormulaSubPart = false;
					}
					else
					{
						if (double.TryParse(
							buffer.ToString(),
							NumberStyles.Float | NumberStyles.AllowThousands,
							CultureInfo, out double doubleValue))
						{
							tokens.Add(new Token() { TokenType = TokenType.FloatingPoint, Value = doubleValue, StartPosition = startPosition, Length = i - startPosition });
							isScientific = false;
							isFormulaSubPart = false;
						}
						else if (buffer.ToString() == "-")
						{
							// Verify if we have a unary minus, we use the token '_' for a unary minus in the AST builder
							tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '_', StartPosition = startPosition, Length = 1 });
						}
						// Else we skip
					}

					// Last character read
					if (i == chars.Length)
						continue;
				}

				if (IsPartOfText(chars[i], true))
				{
					string buffer = $"{chars[i]}";
					int startPosition = i;

					while (++i < chars.Length && IsPartOfText(chars[i], false))
						buffer += chars[i];

					tokens.Add(new Token() { TokenType = TokenType.Text, Value = buffer, StartPosition = startPosition, Length = i - startPosition });
					isFormulaSubPart = false;

					// Last character read
					if (i == chars.Length)					
						continue;
				}

				if (chars[i] == ArgumentSeparator)
				{
					tokens.Add(new Token() { TokenType = TokenType.ArgumentSeparator, Value = chars[i], StartPosition = i, Length = 1 });
					isFormulaSubPart = false;
				}
				else
				{
					switch (chars[i])
					{
						case ' ':
							continue;

						case '+':
						case '-':
						case '*':
						case '/':
						case '^':
						case '%':
						case '≤':
						case '≥':
						case '≠':
							if (IsUnaryMinus(chars[i], tokens))
							{
								// We use the token '_' for a unary minus in the AST builder
								tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '_', StartPosition = i, Length = 1 });
							}
							else
							{
								tokens.Add(new Token() { TokenType = TokenType.Operation, Value = chars[i], StartPosition = i, Length = 1 });
							}
							isFormulaSubPart = true;
							break;

						case '(':
							tokens.Add(new Token() { TokenType = TokenType.LeftRoundBracket, Value = chars[i], StartPosition = i, Length = 1 });
							isFormulaSubPart = true;
							break;

						case ')':
							tokens.Add(new Token() { TokenType = TokenType.RightRoundBracket, Value = chars[i], StartPosition = i, Length = 1 });
							isFormulaSubPart = false;
							break;

						case '[':
							tokens.Add(new Token() { TokenType = TokenType.LeftRoundBracket, Value = chars[i], StartPosition = i, Length = 1 });
							isFormulaSubPart = true;
							break;

						case ']':
							tokens.Add(new Token() { TokenType = TokenType.LeftRoundBracket, Value = chars[i], StartPosition = i, Length = 1 });
							isFormulaSubPart = false;
							break;

						case '<':
							if (i + 1 < chars.Length && chars[i + 1] == '=')
								tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '≤', StartPosition = i++, Length = 2 });
							else
								tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '<', StartPosition = i, Length = 1 });
							isFormulaSubPart = false;
							break;

						case '>':
							if (i + 1 < chars.Length && chars[i + 1] == '=')
								tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '≥', StartPosition = i++, Length = 2 });
							else
								tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '>', StartPosition = i, Length = 1 });
							isFormulaSubPart = false;
							break;

						case '!':
							if (i + 1 < chars.Length && chars[i + 1] == '=')
							{
								tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '≠', StartPosition = i++, Length = 2 });
								isFormulaSubPart = false;
							}
							else
								throw new ParseException(string.Format("Invalid token \"{0}\" detected at position {1}.", chars[i], i));
							break;

						case '&':
							if (i + 1 < chars.Length && chars[i + 1] == '&')
							{
								tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '&', StartPosition = i++, Length = 2 });
								isFormulaSubPart = false;
							}
							else
								throw new ParseException(string.Format("Invalid token \"{0}\" detected at position {1}.", chars[i], i));
							break;

						case '|':
							if (i + 1 < chars.Length && chars[i + 1] == '|')
							{
								tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '|', StartPosition = i++, Length = 2 });
								isFormulaSubPart = false;
							}
							else
								throw new ParseException(string.Format("Invalid token \"{0}\" detected at position {1}.", chars[i], i));
							break;

						case '=':
							if (i + 1 < chars.Length && chars[i + 1] == '=')
							{
								tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '=', StartPosition = i++, Length = 2 });
								isFormulaSubPart = false;
							}
							else
								throw new ParseException(string.Format("Invalid token \"{0}\" detected at position {1}.", chars[i], i));
							break;

						default:
							throw new ParseException(string.Format("Invalid token \"{0}\" detected at position {1}.", chars[i], i));
					}
				}
			}
		}

		#endregion

		#region Private Methods

		private bool IsPartOfNumeric(char character, bool isFirstCharacter, bool isFormulaSubPart)
		{
			return character == DecimalSeparator ||
				(character >= '0' && character <= '9') ||
				(isFormulaSubPart && isFirstCharacter && character == '-') ||
				(!isFirstCharacter && character == 'e') ||
				(!isFirstCharacter && character == 'E');
		}

		private bool IsPartOfText(char character, bool isFirstCharacter)
		{
			if (isFirstCharacter && IsNumericCharacter(character))
				return false;

			return !Operators.Contains(character);
		}

		private bool IsNumericCharacter(char item)
		{
			return char.IsDigit(item);
		}

		private bool IsUnaryMinus(char currentToken, List<Token> tokens)
		{
			if (currentToken == '-')
			{
				Token previousToken = tokens[^1];

				return !(previousToken.TokenType == TokenType.FloatingPoint ||
						 previousToken.TokenType == TokenType.Integer ||
						 previousToken.TokenType == TokenType.Text ||
						 previousToken.TokenType == TokenType.RightRoundBracket ||
						 previousToken.TokenType == TokenType.RightSquareBracket);
			}
			else
				return false;
		}

		private bool IsScientificNotation(char currentToken)
		{
			return currentToken == 'e' || currentToken == 'E';
		}

		#endregion
	}
}
