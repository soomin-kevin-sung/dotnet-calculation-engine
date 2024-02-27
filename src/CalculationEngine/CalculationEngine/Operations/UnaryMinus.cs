using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public class UnaryMinus : Operation
	{
		public UnaryMinus(Operation arg)
			: base(DataType.Float, arg.DependsOnVariables, arg.IsIdempotent)
		{
			Argument = arg;
		}

		public Operation Argument { get; internal set; }

		public override IList<Operation> Arguments => new Operation[] { Argument };

		public override void SetArguments(IList<Operation> arguments)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan(arguments.Count, 1);

			Argument = arguments[0];
		}
	}
}
