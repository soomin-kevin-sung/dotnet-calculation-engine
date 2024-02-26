using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public class Modulo : Operation
	{
		public Modulo(Operation dividend, Operation divisor)
			: base(DataType.Float, dividend.DependsOnVariables || divisor.DependsOnVariables, dividend.IsIdempotent && divisor.IsIdempotent)
		{
			Dividend = dividend;
			Divisor = divisor;
		}

		public Operation Dividend { get; internal set; }
		public Operation Divisor { get; internal set; }

		public override IList<Operation> Arguments => new Operation[] { Dividend, Divisor };
	}
}
