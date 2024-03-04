using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Operations
{
	public class ReferenceObjectProperty : Operation
	{
		public ReferenceObjectProperty(Operation arg, string memberName)
			: base(DataType.Object, arg.DependsOnVariables, arg.IsIdempotent)
		{
			Argument = arg;
			MemberName = memberName;
		}

		public Operation Argument { get; internal set; }
		public string MemberName { get; }

		public override IList<Operation> Arguments => new Operation[] { Argument };

		public override void SetArguments(IList<Operation> arguments)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan(arguments.Count, 1);

			Argument = arguments[0];
		}
	}
}
