using CalculationEngine.Exceptions;
using CalculationEngine.Execution;
using CalculationEngine.Execution.Interfaces;
using CalculationEngine.Operations;
using CalculationEngine.Tokens;
using CalculationEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine
{
	public class AstBuilder
	{
		public AstBuilder(IFunctionRegistry functionRegistry, bool caseSensitive, IConstantRegistry? compliedConstants = null)
		{
			ArgumentNullException.ThrowIfNull(functionRegistry);
			_functionRegistry = functionRegistry;
			_localConstantRegistry = compliedConstants ?? new ConstantRegistry(caseSensitive);
			_caseSensitive = caseSensitive;

			_resultStack = new Stack<Operation>();
			_operatorStack = new Stack<Token>();
			_parameterCount = new Stack<int>();
			_operationPriority = new Dictionary<char, int>()
			{
				{ '(', 0 },
				{ '&', 1 },
				{ '|', 1 },
				{ '<', 2 },
				{ '>', 2 },
				{ '≤', 2 },
				{ '≥', 2 },
				{ '≠', 2 },
				{ '=', 2 },
				{ '+', 3 },
				{ '-', 3 },
				{ '*', 4 },
				{ '/', 4 },
				{ '%', 4 },
				{ '_', 6 },
				{ '^', 5 },
			};

			_operationCtors = new Dictionary<char, Func<Operation>>()
			{
				{ '+', () => new Addition(_resultStack.Pop(), _resultStack.Pop()) },
				{ '-', () => new Subtraction(_resultStack.Pop(), _resultStack.Pop()) },
				{ '*', () => new Multiplication(_resultStack.Pop(), _resultStack.Pop()) },
				{ '/', () => new Division(_resultStack.Pop(), _resultStack.Pop()) },
				{ '%', () => new Modulo(_resultStack.Pop(), _resultStack.Pop()) },
				{ '_', () => new UnaryMinus(_resultStack.Pop()) },
				{ '^', () => new Exponentiation(_resultStack.Pop(), _resultStack.Pop()) },
				{ '&', () => new And(_resultStack.Pop(), _resultStack.Pop()) },
				{ '|', () => new Or(_resultStack.Pop(), _resultStack.Pop()) },
				{ '<', () => new GreaterThan(_resultStack.Pop(), _resultStack.Pop()) },
				{ '≤', () => new GreaterOrEqualThan(_resultStack.Pop(), _resultStack.Pop()) },
				{ '>', () => new LessThan(_resultStack.Pop(), _resultStack.Pop()) },
				{ '≥', () => new LessOrEqualThan(_resultStack.Pop(), _resultStack.Pop()) },
				{ '=', () => new Equal(_resultStack.Pop(), _resultStack.Pop()) },
				{ '≠', () => new NotEqual(_resultStack.Pop(), _resultStack.Pop()) },
			};
		}

		#region Private Variables

		readonly IFunctionRegistry _functionRegistry;
		readonly IConstantRegistry _localConstantRegistry;
		readonly bool _caseSensitive;
		readonly Dictionary<char, int> _operationPriority;
		readonly Stack<Operation> _resultStack;
		readonly Stack<Token> _operatorStack;
		readonly Stack<int> _parameterCount;
		readonly Dictionary<char, Func<Operation>> _operationCtors;

		#endregion

		#region Public Methods

		public Operation Build(IEnumerable<Token> tokens)
		{
			_resultStack.Clear();
			_operatorStack.Clear();
			_parameterCount.Clear();

			foreach (var token in tokens)
			{
				var value = token.Value;
				switch (token.TokenType)
				{
					case TokenType.Integer:
						_resultStack.Push(new FloatingConstant((int)token.Value));
						break;
					case TokenType.FloatingPoint:
						_resultStack.Push(new FloatingConstant((double)token.Value));
						break;

					case TokenType.MemberPoint:
						_resultStack.Push(new ReferenceMember(_resultStack.Pop(), (string)token.Value));
						break;

					case TokenType.Text:
						// if is function
						if (_functionRegistry.IsFunctionName((string)token.Value))
						{
							// push to operator stack and paramaetercount stack.
							_operatorStack.Push(token);
							_parameterCount.Push(1);
						}
						else
						{
							// if local constant
							string tokenValue = (string)token.Value;
							if (_localConstantRegistry.IsConstantName(tokenValue))
							{
								_resultStack.Push(ConvertConstant(token));
							}
							// if variable
							else
							{
								if (_caseSensitive)
									tokenValue = tokenValue.ToLowerFast();
								
								_resultStack.Push(new Variable(tokenValue));
							}
						}
						break;

					case TokenType.String:
						_resultStack.Push(new StringConstant((string)token.Value));
						break;

					case TokenType.LeftBracket:
						_operatorStack.Push(token);
						break;

					case TokenType.RightBracket:
						PopOperations(true, token);
						break;

					case TokenType.ArgumentSeparator:
						PopOperations(false, token);
						_parameterCount.Push(_parameterCount.Pop() + 1);
						break;

					case TokenType.Operation:
						var op1Token = token;
						var op1 = (char)op1Token.Value;

						while (_operatorStack.Count > 0 &&
							(_operatorStack.Peek().TokenType == TokenType.Operation || _operatorStack.Peek().TokenType == TokenType.Text))
						{
							var op2Token = _operatorStack.Peek();
							var op2 = (char)op2Token.Value;
							bool isFunctionOnTopOfStack = op2Token.TokenType == TokenType.Text;

							if (isFunctionOnTopOfStack)
							{
								_operatorStack.Pop();
								_resultStack.Push(ConvertFunction(op2Token));
							}
							else
							{
								if ((IsLeftAssociativeOperation(op1) && _operationPriority[op1] <= _operationPriority[op2]) ||
									_operationPriority[op1] < _operationPriority[op2])
								{
									_operatorStack.Pop();
									_resultStack.Push(ConvertOperation(op2Token));
								}
								else
								{
									break;
								}
							}
						}

						_operatorStack.Push(op1Token);
						break;
				}
			}

			PopOperations(false, null);

			CheckResultStack();

			return _resultStack.Pop();
		}

		#endregion

		#region Private Methods

		private Operation ConvertOperation(Token operationToken)
		{
			try
			{
				if (!_operationCtors.TryGetValue((char)operationToken.Value, out var ctor))
					throw new ArgumentException($"Unknown operation \"{operationToken}\".", "operation");

				return ctor();
			}
			// stack empty issue
			catch (InvalidOperationException)
			{
				// If we encounter a Stack empty issue this means there is a syntax issue in 
				// the mathematical formula
				throw new ParseException(
					$"There is a syntax issue for the operation \"{operationToken.Value}\" at position {operationToken.StartPosition}. " +
					$"The number of arguments does not match with what is expected.");
			}
		}

		private Operation ConvertConstant(Token constantToken)
		{
			try
			{
				var tokenValue = (string)constantToken.Value;
				if (!_localConstantRegistry.IsConstantName(tokenValue))
					throw new ArgumentException($"Unknown operation \"{tokenValue}\".", "operation");

				var constantInfo = _localConstantRegistry.GetConstantInfo(tokenValue) ?? throw new ArgumentException($"Not exist ConstantInfo : {tokenValue}");

				if (constantInfo.ValueType == typeof(string))
					return new StringConstant((string)constantInfo.Value);
				else if (constantInfo.ValueType == typeof(int) ||
					constantInfo.ValueType == typeof(long) ||
					constantInfo.ValueType == typeof(float) ||
					constantInfo.ValueType == typeof(double) ||
					constantInfo.ValueType == typeof(bool))
					return new FloatingConstant((double)constantInfo.Value);
				else
					return new ObjectConstant(constantInfo.Value);
			}
			// stack empty issue
			catch (InvalidOperationException)
			{
				// If we encounter a Stack empty issue this means there is a syntax issue in 
				// the mathematical formula
				throw new ParseException(
					$"There is a syntax issue for the operation \"{constantToken.Value}\" at position {constantToken.StartPosition}. " +
					$"The number of arguments does not match with what is expected.");
			}
		}

		private Operation ConvertFunction(Token functionToken)
		{
			try
			{
				var functionName = ((string)functionToken.Value).ToLowerInvariant();
				if (!_functionRegistry.IsFunctionName(functionName))
					throw new ArgumentException($"Unknown function \"{functionToken.Value}\".", "function");

				var functionInfo = _functionRegistry.GetFunctionInfo(functionName) ?? throw new ArgumentNullException($"Not exist FunctionInfo : {functionName}");
				int numOfParams = _parameterCount.Pop();
				if (!functionInfo.IsDynamicFunc)
					numOfParams = functionInfo.NumberOfParameters;

				var operations = new List<Operation>();
				for (int i = 0; i < numOfParams; i++)
					operations.Add(_resultStack.Pop());

				operations.Reverse();

				return new Function(GetDataTypeFromType(functionInfo.ReturnType), functionName, operations, functionInfo.IsIdempotent);
			}
			// stack empty issue
			catch (InvalidOperationException)
			{
				// If we encounter a Stack empty issue this means there is a syntax issue in 
				// the mathematical formula
				throw new ParseException(
					$"There is a syntax issue for the operation \"{functionToken.Value}\" at position {functionToken.StartPosition}. " +
					$"The number of arguments does not match with what is expected.");
			}
		}

		private void PopOperations(bool untilLeftBracket, Token? currentToken)
		{
			if (untilLeftBracket && currentToken == null)
				throw new ArgumentNullException(nameof(currentToken), "If the parameter \"untillLeftBracket\" is set to true, " +
					"the parameter \"currentToken\" cannot be null.");

			while (_operatorStack.Count > 0 &&
				_operatorStack.Peek().TokenType != TokenType.LeftBracket)
			{
				var token = _operatorStack.Pop();
				switch (token.TokenType)
				{
					// operation
					case TokenType.Operation:
						_resultStack.Push(ConvertOperation(token));
						break;

					// function name
					case TokenType.Text:
						_resultStack.Push(ConvertFunction(token));
						break;
				}
			}

			if (untilLeftBracket)
			{
				// pop left bracket
				if (_operatorStack.Count > 0 &&
					_operatorStack.Peek().TokenType == TokenType.LeftBracket)
					_operatorStack.Pop();
				else if (currentToken != null)
					throw new ParseException($"No matching left bracket found for the right bracket at position {currentToken.Value.StartPosition}.");
				else
					throw new ArgumentNullException(nameof(currentToken),
						"If the parameter \"untillLeftBracket\" is set to true, the parameter \"currentToken\" cannot be null.");
			}
			else
			{
				if (_operatorStack.Count > 0 &&
					_operatorStack.Peek().TokenType == TokenType.LeftBracket &&
					currentToken != null && currentToken.Value.TokenType != TokenType.ArgumentSeparator)
					throw new ParseException($"No matching right bracket found for the left bracket at position {_operatorStack.Peek().StartPosition}.");
			}
		}

		private void CheckResultStack()
		{
			if (_resultStack.Count > 1)
			{
				var operations = _resultStack.ToArray();

				for (int i = 1; i < operations.Length; i++)
				{
					Operation operation = operations[i];

					if (operation.GetType() == typeof(FloatingConstant))
					{
						var constant = (FloatingConstant)operation;
						throw new ParseException($"Unexpected constant \"{constant.Value}\" found.");
					}
					else if (operation.GetType() == typeof(Variable))
					{
						var variable = (Variable)operation;
						throw new ParseException($"Unexpected variable \"{variable.Name}\" found.");
					}
				}

				throw new ParseException("The syntax of the provided formula is not valid.");
			}
		}

		private bool IsLeftAssociativeOperation(char character)
		{
			return character == '*' || character == '+' || character == '-' || character == '/';
		}

		private DataType GetDataTypeFromType(Type type)
		{
			if (type == typeof(string))
				return DataType.String;
			else if (type == typeof(int) ||
				type == typeof(long) ||
				type == typeof(float) ||
				type == typeof(double) ||
				type == typeof(bool))
				return DataType.Float;
			else
				return DataType.Object;
		}

		#endregion
	}
}
