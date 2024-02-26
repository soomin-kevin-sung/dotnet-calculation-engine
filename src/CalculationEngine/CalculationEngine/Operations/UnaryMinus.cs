using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public class UnaryMinus : Operation
	{
		public UnaryMinus(DataType dataType, Operation arg)
			: base(dataType, arg.DependsOnVariables, arg.IsIdempotent)
		{
			Argument = arg;
		}

		public Operation Argument { get; internal set; }

		public override IList<Operation> Arguments => new Operation[] { Argument };
	}
}
