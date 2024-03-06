using KevinComponent.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Operations
{
	public class Exponentiation : Operation
	{
		public Exponentiation(Operation arg, Operation exponent)
			: base(DataType.Float, arg.DependsOnVariables || exponent.DependsOnVariables, arg.IsIdempotent && exponent.IsIdempotent)
		{
			Base = arg;
			Exponent = exponent;
		}

		public Operation Base { get; internal set; }
		public Operation Exponent { get; internal set; }

		public override IList<Operation> Arguments => new Operation[] { Base, Exponent };

		public override void SetArguments(IList<Operation> arguments)
		{
			ExceptionUtil.ThrowIfGreaterThan(arguments.Count, 2);

			Base = arguments[0];
			Exponent = arguments[1];
		}
	}
}
