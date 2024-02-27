using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public class And : Operation
	{
		public And(Operation arg1, Operation arg2)
			: base(DataType.Float, arg1.DependsOnVariables || arg2.DependsOnVariables, arg1.IsIdempotent || arg2.IsIdempotent)
		{
			Argument1 = arg1;
			Argument2 = arg2;
		}

		public Operation Argument1 { get; internal set; }
		public Operation Argument2 { get; internal set; }

		public override IList<Operation> Arguments => new Operation[] { Argument1, Argument2 };

		public override void SetArguments(IList<Operation> arguments)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan(arguments.Count, 2);

			Argument1 = arguments[0];
			Argument2 = arguments[1];
		}
	}
}
