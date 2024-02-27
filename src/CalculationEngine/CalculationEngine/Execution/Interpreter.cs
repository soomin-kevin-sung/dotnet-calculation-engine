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
		}

		#region Private Variables

		bool _caseSensitive;

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
				return null;

			return null;
		}

		#endregion
	}
}
