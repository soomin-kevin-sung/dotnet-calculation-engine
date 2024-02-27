using CalculationEngine.Execution.Interfaces;
using CalculationEngine.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine
{
	public class Optimizer
	{
		public Optimizer(IExecutor executor)
		{
			_executor = executor;
		}

		#region Private Variables

		readonly IExecutor _executor;

		#endregion

		#region Public Methods

		public Operation Optimize(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry)
		{
			// can optimize to constant
			if (!operation.DependsOnVariables &&
				operation.IsIdempotent &&
				!typeof(Constant<>).IsAssignableFrom(operation.GetType()))
			{
				var result = _executor.Execute(operation, functionRegistry, constantRegistry);
				return Constant.ValueToConstant(result);
			}
			else
			{
				var arguments = operation.Arguments;
				if (arguments.Count > 0)
				{
					var optimizedArguments = operation.Arguments.Select(a => Optimize(a, functionRegistry, constantRegistry)).ToList();
					operation.SetArguments(optimizedArguments);
				}

				return operation;
			}
		}

		#endregion
	}
}
