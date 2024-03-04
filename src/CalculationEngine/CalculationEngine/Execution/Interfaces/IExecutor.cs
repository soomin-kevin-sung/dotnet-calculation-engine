using CalculationEngine.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution.Interfaces
{
	public interface IExecutor
	{
		object Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector);
		object Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector, IDictionary<string, object> variables);
		Func<IDictionary<string, object>, object> BuildFormula(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector);
	}
}
