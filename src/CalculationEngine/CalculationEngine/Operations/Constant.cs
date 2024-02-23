using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public abstract class Constant<T> : Operation
	{
		public Constant(DataType dataType, T value)
			: base(dataType, false, true)
		{
			Value = value;
		}

		public T Value { get; private set; }

		public override IList<Operation> Arguments => new Operation[0];

		public override bool Equals(object? obj)
		{
			if (obj is Constant<T> other)
				return Value?.Equals(other.Value) ?? false;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}
	}
}
