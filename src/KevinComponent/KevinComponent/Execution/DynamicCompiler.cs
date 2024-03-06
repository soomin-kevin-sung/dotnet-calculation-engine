using KevinComponent.Execution.FunctionInfos;
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
				{ typeof(Function), OnFunctionGenerateMethodBody },
				{ typeof(ReferenceObjectProperty), OnReferenceObjectPropertyGenerateMethodBody }
			};
		}

		#region Private Variables

		readonly bool _caseSensitive;
		readonly Dictionary<Type, Func<Operation, ParameterExpression, Expression>> _generateMethodBodyDict;

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
			var func = BuildFormulaInternal(operation);
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

		private Func<FormulaContext, object> BuildFormulaInternal(Operation operation)
		{
			var contextParameter = Expression.Parameter(typeof(FormulaContext), "context");
			var lambda = Expression.Lambda<Func<FormulaContext, object>>(
				GenerateMethodBody(operation, contextParameter),
				contextParameter
			);

			return lambda.Compile();
		}

		private Expression GenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			ExceptionUtil.ThrowIfNull(operation);

			if (_generateMethodBodyDict.TryGetValue(operation.GetType(), out var generateMethodBody))
				return generateMethodBody(operation, contextParameter);

			throw new ArgumentException($"Unsupported operation \"{operation.GetType().FullName}\".", nameof(operation));
		}

		private Expression OnFloatingConstantGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			return CreateObjectExpression(((FloatingConstant)operation).Value);
		}

		private Expression OnStringConstantGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			return CreateObjectExpression(((StringConstant)operation).Value);
		}

		private Expression OnObjectConstantGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			return CreateObjectExpression(((ObjectConstant)operation).Value);
		}

		private Expression OnVariableGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var variable = (Variable)operation;
			var func = PrecompiledMethods.GetVariableValueOrThrow;
			return Expression.Call(null, func.GetMethodInfo(), contextParameter, Expression.Constant(variable.Name));
		}

		private Expression OnAdditionGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var addition = (Addition)operation;
			var arg1 = GenerateMethodBody(addition.Argument1, contextParameter);
			var arg2 = GenerateMethodBody(addition.Argument2, contextParameter);

			var func = PrecompiledMethods.Add;
			return Expression.Call(null, func.GetMethodInfo(), arg1, arg2);
		}

		private Expression OnSubtractionGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var subtraction = (Subtraction)operation;
			var arg1 = GenerateMethodBody(subtraction.Argument1, contextParameter);
			var arg2 = GenerateMethodBody(subtraction.Argument2, contextParameter);

			return Expression.Subtract(arg1, arg2);
		}

		private Expression OnMultiplicationGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var multiplication = (Multiplication)operation;
			var arg1 = GenerateMethodBody(multiplication.Argument1, contextParameter);
			var arg2 = GenerateMethodBody(multiplication.Argument2, contextParameter);

			var func = PrecompiledMethods.Multiply;
			return Expression.Call(null, func.GetMethodInfo(), arg1, arg2);
		}

		private Expression OnDivisionGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var division = (Division)operation;
			var dividend = GenerateMethodBody(division.Dividend, contextParameter);
			var divisor = GenerateMethodBody(division.Divisor, contextParameter);

			var func = PrecompiledMethods.Multiply;
			return Expression.Call(null, func.GetMethodInfo(), dividend, divisor);
		}

		private Expression OnModuloGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var modulo = (Modulo)operation;
			var dividend = GenerateMethodBody(modulo.Dividend, contextParameter);
			var divisor = GenerateMethodBody(modulo.Divisor, contextParameter);

			var func = PrecompiledMethods.Modulo;
			return Expression.Call(null, func.GetMethodInfo(), dividend, divisor);
		}

		private Expression OnExponentiationGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var exponentiation = (Exponentiation)operation;
			var arg1 = GenerateMethodBody(exponentiation.Base, contextParameter);
			var arg2 = GenerateMethodBody(exponentiation.Exponent, contextParameter);

			var func = PrecompiledMethods.Pow;
			return Expression.Call(null, func.GetMethodInfo(), arg1, arg2);
		}

		private Expression OnUnaryMinusGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var unaryMinus = (UnaryMinus)operation;
			var arg = GenerateMethodBody(unaryMinus.Argument, contextParameter);

			var func = PrecompiledMethods.Negate;
			return Expression.Call(null, func.GetMethodInfo(), arg);
		}

		private Expression OnAndGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var and = (And)operation;
			var func = PrecompiledMethods.NotEqual;
			var arg1 = Expression.Call(null, func.GetMethodInfo(), GenerateMethodBody(and.Argument1, contextParameter), CreateObjectExpression(0.0));
			var arg2 = Expression.Call(null, func.GetMethodInfo(), GenerateMethodBody(and.Argument2, contextParameter), CreateObjectExpression(0.0));

			return Expression.Condition(Expression.And(arg1, arg2), CreateObjectExpression(1.0), CreateObjectExpression(0.0));
		}

		private Expression OnOrGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var or = (Or)operation;
			var func = PrecompiledMethods.NotEqual;
			var arg1 = Expression.Call(null, func.GetMethodInfo(), GenerateMethodBody(or.Argument1, contextParameter), CreateObjectExpression(0.0));
			var arg2 = Expression.Call(null, func.GetMethodInfo(), GenerateMethodBody(or.Argument2, contextParameter), CreateObjectExpression(0.0));

			return Expression.Condition(Expression.Or(arg1, arg2), CreateObjectExpression(1.0), CreateObjectExpression(0.0));
		}

		private Expression OnLessThanGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var lessThan = (LessThan)operation;
			var arg1 = GenerateMethodBody(lessThan.Argument1, contextParameter);
			var arg2 = GenerateMethodBody(lessThan.Argument2, contextParameter);

			return Expression.Condition(Expression.LessThan(arg1, arg2), CreateObjectExpression(1.0), CreateObjectExpression(0.0));
		}

		private Expression OnLessThanOrEqualGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var lessThanOrEqual = (LessThanOrEqual)operation;
			var arg1 = GenerateMethodBody(lessThanOrEqual.Argument1, contextParameter);
			var arg2 = GenerateMethodBody(lessThanOrEqual.Argument2, contextParameter);

			return Expression.Condition(Expression.LessThanOrEqual(arg1, arg2), CreateObjectExpression(1.0), CreateObjectExpression(0.0));
		}

		private Expression OnGreaterThanGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var greaterThan = (GreaterThan)operation;
			var arg1 = GenerateMethodBody(greaterThan.Argument1, contextParameter);
			var arg2 = GenerateMethodBody(greaterThan.Argument2, contextParameter);

			return Expression.Condition(Expression.GreaterThan(arg1, arg2), CreateObjectExpression(1.0), CreateObjectExpression(0.0));
		}

		private Expression OnGreaterThanOrEqualGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var greaterThanOrEqual = (GreaterThanOrEqual)operation;
			var arg1 = GenerateMethodBody(greaterThanOrEqual.Argument1, contextParameter);
			var arg2 = GenerateMethodBody(greaterThanOrEqual.Argument2, contextParameter);

			return Expression.Condition(Expression.GreaterThanOrEqual(arg1, arg2), CreateObjectExpression(1.0), CreateObjectExpression(0.0));
		}

		private Expression OnEqualGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var equal = (Equal)operation;
			var arg1 = GenerateMethodBody(equal.Argument1, contextParameter);
			var arg2 = GenerateMethodBody(equal.Argument2, contextParameter);

			var func = PrecompiledMethods.Equal;
			return Expression.Condition(Expression.Call(null, func.GetMethodInfo(), arg1, arg2), CreateObjectExpression(1.0), CreateObjectExpression(0.0));
		}

		private Expression OnNotEqualGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var notEqual = (NotEqual)operation;
			var arg1 = GenerateMethodBody(notEqual.Argument1, contextParameter);
			var arg2 = GenerateMethodBody(notEqual.Argument2, contextParameter);

			var func = PrecompiledMethods.NotEqual;
			return Expression.Condition(Expression.Call(null, func.GetMethodInfo(), arg1, arg2), CreateObjectExpression(1.0), CreateObjectExpression(0.0));
		}

		private Expression OnFunctionGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var function = (Function)operation;
			var invokeFunc = PrecompiledMethods.InvokeFunction;

			// convert arguments to expressions
			var args = function.Arguments;
			var exps = new Expression[args.Count];
			for (int i = 0; i < args.Count; i++)
				exps[i] = GenerateMethodBody(args[i], contextParameter);

			return Expression.Call(
				null,
				invokeFunc.GetMethodInfo(),
				contextParameter,
				Expression.Constant(function.FunctionName),
				Expression.NewArrayInit(typeof(object), exps));
		}

		private Expression OnReferenceObjectPropertyGenerateMethodBody(Operation operation, ParameterExpression contextParameter)
		{
			var referenceObjectProperty = (ReferenceObjectProperty)operation;
			var expPropertyConnector = Expression.Property(contextParameter, "PropertyConnector");
			var getPropertyValueMI = typeof(IPropertyConnector).GetMethod("GetPropertyValue");
			ExceptionUtil.ThrowIfNull(getPropertyValueMI);

			var arg = GenerateMethodBody(referenceObjectProperty.Argument, contextParameter);
			return Expression.Call(expPropertyConnector, getPropertyValueMI, arg, Expression.Constant(referenceObjectProperty.PropertyName));
		}

		private Expression CreateObjectExpression(object value)
		{
			return Expression.Constant(value, typeof(object));
		}

		#endregion
	}
}
