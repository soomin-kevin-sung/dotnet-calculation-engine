using KevinComponent.Execution.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent
{
	public class FormulaContext
	{
		public FormulaContext(IDictionary<string, object> variables, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IPropertyConnector? propertyConnector)
		{
			Variables = variables;
			FunctionRegistry = functionRegistry;
			ConstantRegistry = constantRegistry;
			PropertyConnector = propertyConnector;
		}

		#region Public Properties

		public IDictionary<string, object> Variables { get; }

		public IFunctionRegistry FunctionRegistry { get; }

		public IConstantRegistry ConstantRegistry { get; }

		public IPropertyConnector? PropertyConnector { get; }

		#endregion
	}
}
