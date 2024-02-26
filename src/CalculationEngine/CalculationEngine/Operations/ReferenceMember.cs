using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public class ReferenceMember : Operation
	{
		public ReferenceMember(Operation arg, string memberName)
			: base(DataType.Object, arg.DependsOnVariables, arg.IsIdempotent)
		{
			Argument = arg;
			MemberName = memberName;
		}

		public Operation Argument { get; }
		public string MemberName { get; }

		public override IList<Operation> Arguments => new Operation[] { Argument };
	}
}
