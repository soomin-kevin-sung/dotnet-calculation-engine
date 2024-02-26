using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public class Function : Operation
	{
		public Function(DataType dataType, string functionName, IList<Operation> arguments, bool isIdempotent)
			: base(dataType, arguments.FirstOrDefault(o => o.DependsOnVariables) != null, isIdempotent && arguments.All(o => o.IsIdempotent))
		{
			FunctionName = functionName;
			_arguments = arguments;
			_isIdempotentOverride = isIdempotent;
		}

		IList<Operation> _arguments;
		bool _isIdempotentOverride;

		public string FunctionName { get; }

		public override IList<Operation> Arguments => _arguments;

		internal void SetArguments(IList<Operation> arguments)
		{
			_arguments = arguments;
			DependsOnVariables = arguments.FirstOrDefault(o => o.DependsOnVariables) != null;
			IsIdempotent = _isIdempotentOverride && arguments.All(o => o.IsIdempotent);
		}
	}
}
