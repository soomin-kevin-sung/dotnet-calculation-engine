using KevinComponent.Execution.Interfaces;
using KevinComponent.Operations;
using KevinComponent.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Execution
{
	public partial class DynamicCompiler : IExecutor
	{
		public DynamicCompiler()
			: this(false)
		{
		}

		public DynamicCompiler(bool caseSensitive)
		{
			_caseSensitive = caseSensitive;
			_generateMethodBodyDict = new() {
				{ typeof(FloatingConstant), OnFloatingConstantGenerateMethodBody },
				{ typeof(StringConstant), OnStringConstantGenerateMethodBody },
				{ typeof(ObjectConstant), OnObjectConstantGenerateMethodBody },
				{ typeof(Variable), OnVariableGenerateMethodBody },
				{ typeof(Addition), OnAdditionGenerateMethodBody },
				{ typeof(Subtraction), OnSubtractionGenerateMethodBody },
				{ typeof(Multiplication), OnMultiplicationGenerateMethodBody },
				{ typeof(Division), OnDivisionGenerateMethodBody },
				{ typeof(Modulo), OnModuloGenerateMethodBody },
				{ typeof(Exponentiation), OnExponentiationGenerateMethodBody },
				{ typeof(UnaryMinus), OnUnaryMinusGenerateMethodBody },
				{ typeof(And), OnAndGenerateMethodBody },
				{ typeof(Or), OnOrGenerateMethodBody },
				{ typeof(LessThan), OnLessThanGenerateMethodBody },
				{ typeof(LessThanOrEqual), OnLessThanOrEqualGenerateMethodBody },
				{ typeof(GreaterThan), OnGreaterThanGenerateMethodBody },
				{ typeof(GreaterThanOrEqual), OnGreaterThanOrEqualGenerateMethodBody},
				{ typeof(Equal), OnEqualGenerateMethodBody },
				{ typeof(NotEqual), OnNotEqualGenerateMethodBody },
				//{ typeof(Function),  },
				//{ typeof(ReferenceObjectProperty), OnReferenceObjectPropertyExecute }
			};
		}

		#region Private Variables

		readonly bool _caseSensitive;
		readonly Dictionary<Type, Func<Operation, ParameterExpression, IFunctionRegistry, Expression>> _generateMethodBodyDict;

		#endregion

		#region Public Methods

		public object Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector)
		{
			return Execute(operation, functionRegistry, constantRegistry, propertyConnector, new Dictionary<string, object>());
		}

		public object Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			return BuildFormula(operation, functionRegistry, constantRegistry, propertyConnector)(variables);
		}

		public Func<IDictionary<string, object>, object> BuildFormula(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector)
		{
			var func = BuildFormulaInternal(operation, functionRegistry);
			if (_caseSensitive)
				return variables => func(new FormulaContext(variables, functionRegistry, constantRegistry, propertyConnector));
			else
				return variables =>
				{
					variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
					return func(new FormulaContext(variables, functionRegistry, constantRegistry, propertyConnector));
				};
		}

		#endregion

		#region Private Methods

		private Func<FormulaContext, object> BuildFormulaInternal(Operation operation, IFunctionRegistry functionRegistry)
		{
			var contextParameter = Expression.Parameter(typeof(FormulaContext), "context");
			var lambda = Expression.Lambda<Func<FormulaContext, object>>(
				GenerateMethodBody(operation, contextParameter, functionRegistry),
				contextParameter
			);

			return lambda.Compile();
		}

		private Expression GenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			ArgumentNullException.ThrowIfNull(operation);

			if (_generateMethodBodyDict.TryGetValue(operation.GetType(), out var generateMethodBody))
				return generateMethodBody(operation, contextParameter, functionRegistry);

			throw new ArgumentException($"Unsupported operation \"{operation.GetType().FullName}\".", nameof(operation));
		}

		//{ typeof(Addition), OnAdditionExecute },
		//{ typeof(Subtraction), OnSubtractionExecute },
		//{ typeof(Multiplication), OnMultiplicationExecute },
		//{ typeof(Division), OnDivisionExecute },
		//{ typeof(Modulo), OnModuloExecute },
		//{ typeof(Exponentiation), OnExponentiationExecute },
		//{ typeof(UnaryMinus), OnUnaryMinusExecute },
		//{ typeof(And), OnAndExecute },
		//{ typeof(Or), OnOrExecute },
		//{ typeof(LessThan), OnLessThanExecute },
		//{ typeof(LessOrEqualThan), OnLessOrEqualThanExecute },
		//{ typeof(GreaterThan), OnGreaterThanExecute },
		//{ typeof(GreaterOrEqualThan), OnGreaterOrEqualThanExecute },
		//{ typeof(Equal), OnEqualThanExecute },
		//{ typeof(NotEqual), OnNotEqualThanExecute },
		//{ typeof(Function), OnFunctionExecute },
		//{ typeof(ReferenceObjectProperty), OnReferenceObjectPropertyExecute }

		private Expression OnFloatingConstantGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			return Expression.Constant(((FloatingConstant)operation).Value, typeof(double));
		}

		private Expression OnStringConstantGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			return Expression.Constant(((StringConstant)operation).Value, typeof(string));
		}

		private Expression OnObjectConstantGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			return Expression.Constant(((ObjectConstant)operation).Value, typeof(object));
		}

		private Expression OnVariableGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var variable = (Variable)operation;
			var func = PrecompiledMethods.GetVariableValueOrThrow;
			return Expression.Call(null, func.GetMethodInfo(), Expression.Constant(variable.Name), contextParameter);
		}

		private Expression OnAdditionGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var addition = (Addition)operation;
			var arg1 = GenerateMethodBody(addition.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(addition.Argument2, contextParameter, functionRegistry);

			return Expression.Add(arg1, arg2);
		}

		private Expression OnSubtractionGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var subtraction = (Subtraction)operation;
			var arg1 = GenerateMethodBody(subtraction.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(subtraction.Argument2, contextParameter, functionRegistry);

			return Expression.Subtract(arg1, arg2);
		}

		private Expression OnMultiplicationGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var multiplication = (Multiplication)operation;
			var arg1 = GenerateMethodBody(multiplication.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(multiplication.Argument2, contextParameter, functionRegistry);

			return Expression.Multiply(arg1, arg2);
		}

		private Expression OnDivisionGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var division = (Division)operation;
			var dividend = GenerateMethodBody(division.Dividend, contextParameter, functionRegistry);
			var divisor = GenerateMethodBody(division.Divisor, contextParameter, functionRegistry);

			return Expression.Multiply(dividend, divisor);
		}

		private Expression OnModuloGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var modulo = (Modulo)operation;
			var dividend = GenerateMethodBody(modulo.Dividend, contextParameter, functionRegistry);
			var divisor = GenerateMethodBody(modulo.Divisor, contextParameter, functionRegistry);

			return Expression.Modulo(dividend, divisor);
		}

		private Expression OnExponentiationGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var exponentiation = (Exponentiation)operation;
			var arg1 = GenerateMethodBody(exponentiation.Base, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(exponentiation.Exponent, contextParameter, functionRegistry);

			var powMethod = typeof(Math).GetRuntimeMethod("Pow", [typeof(double), typeof(double)])
				?? throw new ArgumentNullException("cannot find \"Math.Pow\" Method");
			return Expression.Call(null, powMethod, arg1, arg2);
		}

		private Expression OnUnaryMinusGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var unaryMinus = (UnaryMinus)operation;
			var arg = GenerateMethodBody(unaryMinus.Argument, contextParameter, functionRegistry);

			return Expression.Negate(arg);
		}

		private Expression OnAndGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var and = (And)operation;
			var arg1 = GenerateMethodBody(and.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(and.Argument2, contextParameter, functionRegistry);

			return Expression.Condition(Expression.And(arg1, arg2), Expression.Constant(1.0), Expression.Constant(0.0));
		}

		private Expression OnOrGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var or = (Or)operation;
			var arg1 = GenerateMethodBody(or.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(or.Argument2, contextParameter, functionRegistry);

			return Expression.Condition(Expression.Or(arg1, arg2), Expression.Constant(1.0), Expression.Constant(0.0));
		}

		private Expression OnLessThanGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var lessThan = (LessThan)operation;
			var arg1 = GenerateMethodBody(lessThan.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(lessThan.Argument2, contextParameter, functionRegistry);

			return Expression.Condition(Expression.LessThan(arg1, arg2), Expression.Constant(1.0), Expression.Constant(0.0));
		}

		private Expression OnLessThanOrEqualGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var lessThanOrEqual = (LessThanOrEqual)operation;
			var arg1 = GenerateMethodBody(lessThanOrEqual.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(lessThanOrEqual.Argument2, contextParameter, functionRegistry);

			return Expression.Condition(Expression.LessThanOrEqual(arg1, arg2), Expression.Constant(1.0), Expression.Constant(0.0));
		}

		private Expression OnGreaterThanGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var greaterThan = (GreaterThan)operation;
			var arg1 = GenerateMethodBody(greaterThan.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(greaterThan.Argument2, contextParameter, functionRegistry);

			return Expression.Condition(Expression.GreaterThan(arg1, arg2), Expression.Constant(1.0), Expression.Constant(0.0));
		}

		private Expression OnGreaterThanOrEqualGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var greaterThanOrEqual = (GreaterThanOrEqual)operation;
			var arg1 = GenerateMethodBody(greaterThanOrEqual.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(greaterThanOrEqual.Argument2, contextParameter, functionRegistry);

			return Expression.Condition(Expression.GreaterThanOrEqual(arg1, arg2), Expression.Constant(1.0), Expression.Constant(0.0));
		}

		private Expression OnEqualGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var equal = (Equal)operation;
			var arg1 = GenerateMethodBody(equal.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(equal.Argument2, contextParameter, functionRegistry);

			return Expression.Condition(Expression.Equal(arg1, arg2), Expression.Constant(1.0), Expression.Constant(0.0));
		}

		private Expression OnNotEqualGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			var notEqual = (NotEqual)operation;
			var arg1 = GenerateMethodBody(notEqual.Argument1, contextParameter, functionRegistry);
			var arg2 = GenerateMethodBody(notEqual.Argument2, contextParameter, functionRegistry);

			return Expression.Condition(Expression.NotEqual(arg1, arg2), Expression.Constant(1.0), Expression.Constant(0.0));
		}

		private Expression OnFunctionGenerateMethodBody(Operation operation, ParameterExpression contextParameter, IFunctionRegistry functionRegistry)
		{
			//var function = (Function)operation;
			//var functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);
			//ArgumentNullException.ThrowIfNull(functionInfo);

			//var args = function.Arguments;
			//var ops = new object[args.Count];
			//for (int i = 0; i < args.Count; i++)
			//	ops[i] = Execute(args[i], functionRegistry, constantRegistry, propertyConnector, variables);

			//return Expression.Condition(Expression.NotEqual(arg1, arg2), Expression.Constant(1.0), Expression.Constant(0.0));
			return null;
		}

		#endregion
	}
}
