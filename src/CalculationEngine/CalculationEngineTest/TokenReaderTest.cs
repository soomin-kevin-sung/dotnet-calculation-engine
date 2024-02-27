using CalculationEngine.Tokens;

namespace CalculationEngineTest
{
	public class TokenReaderTest
	{
		[Fact]
		public void NumericTokensTest()
		{
			// arrange
			var reader = new TokenReader();
			var formula = "1+2+(3*4+5.12)-1e-9";

			// act
			var tokens = reader.Read(formula);

			// assert
			var expectedTokens = new Token[] {
				new() { TokenType = TokenType.Integer, Value = 1, Length = 1, StartPosition = 0 },
				new() { TokenType = TokenType.Operation, Value = '+', Length = 1, StartPosition = 1 },
				new() { TokenType = TokenType.Integer, Value = 2, Length = 1, StartPosition = 2 },
				new() { TokenType = TokenType.Operation, Value = '+', Length = 1, StartPosition = 3 },
				new() { TokenType = TokenType.LeftBracket, Value = '(', Length = 1, StartPosition = 4 },
				new() { TokenType = TokenType.Integer, Value = 3, Length = 1, StartPosition = 5 },
				new() { TokenType = TokenType.Operation, Value = '*', Length = 1, StartPosition = 6 },
				new() { TokenType = TokenType.Integer, Value = 4, Length = 1, StartPosition = 7 },
				new() { TokenType = TokenType.Operation, Value = '+', Length = 1, StartPosition = 8 },
				new() { TokenType = TokenType.FloatingPoint, Value = 5.12, Length = 4, StartPosition = 9 },
				new() { TokenType = TokenType.RightBracket, Value = ')', Length = 1, StartPosition = 13 },
				new() { TokenType = TokenType.Operation, Value = '-', Length = 1, StartPosition = 14 },
				new() { TokenType = TokenType.FloatingPoint, Value = 1E-9, Length = 4, StartPosition = 15 },
			};

			for (int i = 0; i < tokens.Count; i++)
				Assert.Equal(expectedTokens[i], tokens[i]);
		}

		[Fact]
		public void StringTokensTest()
		{
			// arrange
			var reader = new TokenReader();
			var formula = "\"dsadfe dsafed12312+04095_)()(\"";

			// act
			var tokens = reader.Read(formula);

			// assert
			var expectedTokens = new List<Token>() {
				new() { TokenType = TokenType.String, Value = "dsadfe dsafed12312+04095_)()(", Length = 29, StartPosition = 1 },
			};

			for (int i = 0; i < tokens.Count; i++)
				Assert.Equal(expectedTokens[i], tokens[i]);
		}

		[Fact]
		public void StringVariableTokensTest()
		{
			// arrange
			var reader = new TokenReader();
			var formula = "var1==\"dsadfe dsafed12312+04095_)()(\"";

			// act
			var tokens = reader.Read(formula);

			// assert
			var expectedTokens = new List<Token>() {
				new() { TokenType = TokenType.Text, Value = "var1", Length = 4, StartPosition = 0 },
				new() { TokenType = TokenType.Operation, Value = '=', Length = 2, StartPosition = 4 },
				new() { TokenType = TokenType.String, Value = "dsadfe dsafed12312+04095_)()(", Length = 29, StartPosition = 7 }
			};

			for (int i = 0; i < tokens.Count; i++)
				Assert.Equal(expectedTokens[i], tokens[i]);
		}

		[Fact]
		public void FunctionTokensTest()
		{
			// arrange
			var reader = new TokenReader();
			var formula = "func(\"ABCD\")==\"dsadfe dsafed12312+04095_)()(\"";

			// act
			var tokens = reader.Read(formula);

			// assert
			var expectedTokens = new List<Token>() {
				new() { TokenType = TokenType.Text, Value = "func", Length = 4, StartPosition = 0 },
				new() { TokenType = TokenType.LeftBracket, Value = '(', Length = 1, StartPosition = 4 },
				new() { TokenType = TokenType.String, Value = "ABCD", Length = 4, StartPosition = 6 },
				new() { TokenType = TokenType.RightBracket, Value = ')', Length = 1, StartPosition = 11 },
				new() { TokenType = TokenType.Operation, Value = '=', Length = 2, StartPosition = 12 },
				new() { TokenType = TokenType.String, Value = "dsadfe dsafed12312+04095_)()(", Length = 29, StartPosition = 15 },
			};

			for (int i = 0; i < tokens.Count; i++)
				Assert.Equal(expectedTokens[i], tokens[i]);
		}

		[Fact]
		public void ReferenceMemberTest()
		{
			// arrange
			var reader = new TokenReader();
			var formula = "1 - P1.Offset";

			// act
			var tokens = reader.Read(formula);

			// assert
			var expectedTokens = new List<Token>() {
				new() { TokenType = TokenType.Integer, Value = 1, Length = 1, StartPosition = 0 },
				new() { TokenType = TokenType.Operation, Value = '-', Length = 1, StartPosition = 2 },
				new() { TokenType = TokenType.Text, Value = "P1", Length = 2, StartPosition = 4 },
				new() { TokenType = TokenType.MemberPoint, Value = ".Offset", Length = 7, StartPosition = 6 },
			};

			for (int i = 0; i < tokens.Count; i++)
				Assert.Equal(expectedTokens[i], tokens[i]);
		}
	}
}