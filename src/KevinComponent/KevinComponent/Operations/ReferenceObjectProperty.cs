using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Operations
{
	public class ReferenceObjectProperty : Operation
	{
		public ReferenceObjectProperty(Operation arg, string propertyName)
			: base(DataType.Object, arg.DependsOnVariables, arg.IsIdempotent)
		{
			Argument = arg;
			PropertyName = propertyName;
		}

		public Operation Argument { get; internal set; }
		public string PropertyName { get; }

		public override IList<Operation> Arguments => new Operation[] { Argument };

		public override void SetArguments(IList<Operation> arguments)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan(arguments.Count, 1);

			Argument = arguments[0];
		}
	}
}
