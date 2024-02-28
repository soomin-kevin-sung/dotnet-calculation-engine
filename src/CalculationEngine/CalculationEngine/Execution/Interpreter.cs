using CalculationEngine.Exceptions;
using CalculationEngine.Execution.Interfaces;
using CalculationEngine.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
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
				{ typeof(FloatingConstant), OnFloatingConstantExecute },
				{ typeof(FloatingConstant), OnFloatingConstantExecute },
				{ typeof(FloatingConstant), OnFloatingConstantExecute },
				{ typeof(FloatingConstant), OnFloatingConstantExecute },
				{ typeof(FloatingConstant), OnFloatingConstantExecute },
				{ typeof(FloatingConstant), OnFloatingConstantExecute },
			};
		}

		#region Private Variables

		readonly Dictionary<Type, Func<Operation, IFunctionRegistry, IConstantRegistry, IDictionary<string, object>, object>> _executeDict;
		readonly bool _caseSensitive;

		#endregion

		#region Public Methods

		public Func<IDictionary<string, object>, object> BuildFormula(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry)
		{
			throw new NotImplementedException();
		}

		public object Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry)
		{
			throw new NotImplementedException();
		}

		public object Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IDictionary<string, object> variables)
		{
			ArgumentNullException.ThrowIfNull(operation);

			if (operation.GetType() == typeof(FloatingConstant))
			{

			}
			else if (operation.GetType() == typeof(StringConstant))
			{

			}
			else if (operation.GetType() == typeof(ObjectConstant))
			{

			}
			else if (operation.GetType() == typeof(Variable))
			{

			}
			else if (operation.GetType() == typeof(Addition))
			{

			}
			else if (operation.GetType() == typeof(Subtraction))
			{

			}
			else if (operation.GetType() == typeof(Multiplication))
			{

			}
			else if (operation.GetType() == typeof(Division))
			{

			}
			else if (operation.GetType() == typeof(Modulo))
			{

			}
			else if (operation.GetType() == typeof(Exponentiation))
			{

			}
			else if (operation.GetType() == typeof(UnaryMinus))
			{

			}
			else if (operation.GetType() == typeof(And))
			{

			}
			else if (operation.GetType() == typeof(Or))
			{

			}
			else if (operation.GetType() == typeof(LessThan))
			{

			}
			else if (operation.GetType() == typeof(LessOrEqualThan))
			{

			}
			else if (operation.GetType() == typeof(GreaterThan))
			{

			}
			else if (operation.GetType() == typeof(GreaterOrEqualThan))
			{

			}
			else if (operation.GetType() == typeof(Equal))
			{

			}
			else if (operation.GetType() == typeof(NotEqual))
			{

			}
			else if (operation.GetType() == typeof(Function))
			{

			}
			else
			{

			}

#warning working.
			return null;
		}

		#endregion

		#region Private Methods

		private object OnFloatingConstantExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IDictionary<string, object> variables)
		{
			return ((FloatingConstant)operation).Value;
		}

		private object OnStringConstantExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IDictionary<string, object> variables)
		{
			return ((StringConstant)operation).Value;
		}

		private object OnObjectConstantExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IDictionary<string, object> variables)
		{
			return ((ObjectConstant)operation).Value;
		}

		private object OnVariableExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IDictionary<string, object> variables)
		{
			var variable = (Variable)operation;
			if (variables.TryGetValue(variable.Name, out var value))
				return value;
			else
				throw new VariableNotDefinedException($"The variable \"{variable.Name}\" used is not defined.");
		}

		private object OnAdditionExecute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IDictionary<string, object> variables)
		{
			var addition = (Addition)operation;
			var arg1 = Execute(addition.Argument1, functionRegistry, constantRegistry, variables);
			var arg2 = Execute(addition.Argument1, functionRegistry, constantRegistry, variables);

#warning working.
			return null;
		}

		#endregion
	}
}
