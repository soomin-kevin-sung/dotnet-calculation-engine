using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public class Exponentiation : Operation
	{
		public Exponentiation(DataType dataType, Operation arg, Operation exponent)
			: base(dataType, arg.DependsOnVariables || exponent.DependsOnVariables, arg.IsIdempotent && exponent.IsIdempotent)
		{
			Base = arg;
			Exponent = exponent;
		}

		public Operation Base { get; internal set; }
		public Operation Exponent { get; internal set; }

		public override IList<Operation> Arguments => new Operation[] { Base, Exponent };
	}
}
