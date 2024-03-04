using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Operations
{
	public class Variable : Operation
	{
		public Variable(string name)
			: base(DataType.Object, true, false)
		{
			Name = name;
		}

		public string Name { get; private set; }

		public override IList<Operation> Arguments => new Operation[0];

		public override bool Equals(object? obj)
		{
			if (obj is Variable other)
				return Name.Equals(other.Name);
			else
				return false;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override void SetArguments(IList<Operation> arguments)
		{
			throw new NotSupportedException();
		}
	}
}
