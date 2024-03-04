using CalculationEngine.Exceptions;
using CalculationEngine.Execution.Interfaces;
using CalculationEngine.Operations;
using CalculationEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution
{
	public class Interpreter : IExecutor
	{
		public Interpreter()
			: this(false)
		{

		}

		public Interpreter(bool caseSensitive)
		{
			_caseSensitive = caseSensitive;
			_executeDict = new() {
				{ typeof(FloatingConstant), OnFloatingConstantExecute },
				{ typeof(StringConstant), OnStringConstantExecute },
				{ typeof(ObjectConstant), OnObjectConstantExecute },
				{ typeof(Variable), OnVariableExecute },
				{ typeof(Addition), OnAdditionExecute },
				{ typeof(Subtraction), OnSubtractionExecute },
				{ typeof(Multiplication), OnMultiplicationExecute },
				{ typeof(Division), OnDivisionExecute },
				{ typeof(Modulo), OnModuloExecute },
				{ typeof(Exponentiation), OnExponentiationExecute },
				{ typeof(UnaryMinus), OnUnaryMinusExecute },
				{ typeof(And), OnAndExecute },
				{ typeof(Or), OnOrExecute },
				{ typeof(LessThan), OnLessThanExecute },
				{ typeof(LessOrEqualThan), OnLessOrEqualThanExecute },
				{ typeof(GreaterThan), OnGreaterThanExecute },
				{ typeof(GreaterOrEqualThan), OnGreaterOrEqualThanExecute },
				{ typeof(Equal), OnEqualThanExecute },
				{ typeof(NotEqual), OnNotEqualThanExecute },
				{ typeof(Function), OnFunctionExecute },
				{ typeof(ReferenceObjectProperty), OnReferenceObjectPropertyExecute }
			};
		}

		#region Private Variables

		readonly Dictionary<Type, Func<Operation, IFunctionRegistry, IConstantRegistry, IPropertyConnector?, IDictionary<string, object>, object>> _executeDict;
		readonly bool _caseSensitive;

		#endregion

		#region Public Methods

		public Func<IDictionary<string, object>, object> BuildFormula(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector)
		{
			if (_caseSensitive)
				return variables => Execute(operation, functionRegistry, constantRegistry, propertyConnector, variables);
			else
				return variables =>
				{
					variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
					return Execute(operation, functionRegistry, constantRegistry, propertyConnector, variables);
				};
		}

		public object Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector)
		{
			return Execute(operation, functionRegistry, constantRegistry, propertyConnector, new Dictionary<string, object>());
		}

		public object Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			ArgumentNullException.ThrowIfNull(operation);
			var operationType = operation.GetType();
			if (_executeDict.TryGetValue(operationType, out var func))
				return func(operation, functionRegistry, constantRegistry, propertyConnector, variables);

			throw new ArgumentException($"Unsupported operation \"{operation.GetType().FullName}\".", nameof(operation));
		}

		#endregion

		#region Private Methods

		private object OnFloatingConstantExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			return ((FloatingConstant)operation).Value;
		}

		private object OnStringConstantExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			return ((StringConstant)operation).Value;
		}

		private object OnObjectConstantExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			return ((ObjectConstant)operation).Value;
		}

		private object OnVariableExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var variable = (Variable)operation;
			if (variables.TryGetValue(variable.Name, out var value))
				return value;
			else
				throw new VariableNotDefinedException($"The variable \"{variable.Name}\" used is not defined.");
		}

		private object OnAdditionExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var addition = (Addition)operation;
			var arg1 = Execute(addition.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(addition.Argument2, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
				return v1 + v2;
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnSubtractionExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var subtraction = (Subtraction)operation;
			var arg1 = Execute(subtraction.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(subtraction.Argument2, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
				return v1 - v2;
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnMultiplicationExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var multiplication = (Multiplication)operation;
			var arg1 = Execute(multiplication.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(multiplication.Argument2, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
				return v1 * v2;
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnDivisionExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var division = (Division)operation;
			var arg1 = Execute(division.Dividend, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(division.Divisor, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
				return v1 / v2;
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnModuloExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var modulo = (Modulo)operation;
			var arg1 = Execute(modulo.Dividend, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(modulo.Divisor, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
				return v1 % v2;
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnExponentiationExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var exponentiation = (Exponentiation)operation;
			var arg1 = Execute(exponentiation.Base, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(exponentiation.Exponent, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
				return Math.Pow(v1, v2);
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnUnaryMinusExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var unaryMinus = (UnaryMinus)operation;
			var arg1 = Execute(unaryMinus.Argument, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1))
				return -v1;
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnAndExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var and = (And)operation;
			var arg1 = Execute(and.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			if (EngineUtil.ConvertToDouble(arg1, out var v1) && v1 == 1)
			{
				var arg2 = Execute(and.Argument2, functionRegistry,  constantRegistry, propertyConnector);
				if (EngineUtil.ConvertToDouble(arg2, out var v2) && v2 == 1)
					return 1.0;
			}

			return 0.0;
		}

		private object OnOrExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var or = (Or)operation;
			var arg1 = Execute(or.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			if (EngineUtil.ConvertToDouble(arg1, out var v1) && v1 == 1)
				return 1.0;

			var arg2 = Execute(or.Argument2, functionRegistry, constantRegistry, propertyConnector, variables);
			if (EngineUtil.ConvertToDouble(arg2, out var v2) && v2 == 1)
				return 1.0;

			return 0.0;
		}

		private object OnLessThanExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var lessThan = (LessThan)operation;
			var arg1 = Execute(lessThan.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(lessThan.Argument2, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
				return v1 < v2 ? 1.0 : 0.0;
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnLessOrEqualThanExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var lessOrEqualThan = (LessOrEqualThan)operation;
			var arg1 = Execute(lessOrEqualThan.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(lessOrEqualThan.Argument2, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
				return v1 <= v2 ? 1.0 : 0.0;
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnGreaterThanExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var greaterThan = (GreaterThan)operation;
			var arg1 = Execute(greaterThan.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(greaterThan.Argument2, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
				return v1 > v2 ? 1.0 : 0.0;
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnGreaterOrEqualThanExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var greaterOrEqualThan = (GreaterOrEqualThan)operation;
			var arg1 = Execute(greaterOrEqualThan.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(greaterOrEqualThan.Argument2, functionRegistry, constantRegistry, propertyConnector, variables);

			if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
				return v1 >= v2 ? 1.0 : 0.0;
			else
				throw new ArgumentException($"Wrong argument type on {operation.GetType()} operation.", nameof(operation));
		}

		private object OnEqualThanExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var equal = (Equal)operation;
			var arg1 = Execute(equal.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(equal.Argument2, functionRegistry, constantRegistry, propertyConnector, variables);

			return Equals(arg1, arg2) ? 1.0 : 0.0;
		}

		private object OnNotEqualThanExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var notEqual = (NotEqual)operation;
			var arg1 = Execute(notEqual.Argument1, functionRegistry, constantRegistry, propertyConnector, variables);
			var arg2 = Execute(notEqual.Argument2, functionRegistry, constantRegistry, propertyConnector, variables);

			return Equals(arg1, arg2) ? 0.0 : 1.0;
		}

		private object OnFunctionExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			var function = (Function)operation;
			var functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);
			ArgumentNullException.ThrowIfNull(functionInfo);

			var args = function.Arguments;
			var ops = new object[args.Count];
			for (int i = 0; i < args.Count; i++)
				ops[i] = Execute(args[i], functionRegistry, constantRegistry, propertyConnector, variables);
			
			return functionInfo.Invoke(ops);
		}

		private object OnReferenceObjectPropertyExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables)
		{
			if (propertyConnector == null)
				return 0.0;

			var referenceObjectProperty = (ReferenceObjectProperty)operation;
			var arg1 = Execute(referenceObjectProperty.Argument, functionRegistry, constantRegistry, propertyConnector, variables) ??
				throw new ArgumentException($"Argument is null on {operation.GetType()} operation.", nameof(operation));

			return propertyConnector.GetPropertyValue(arg1, referenceObjectProperty.MemberName) ?? 0.0;
		}

		#endregion
	}
}
