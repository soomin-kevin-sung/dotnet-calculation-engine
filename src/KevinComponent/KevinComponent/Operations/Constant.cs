using KevinComponent.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Operations
{
	public abstract class Constant : Operation
	{
		protected Constant(DataType dataType, bool dependsOnVariables, bool isIdempotent)
			: base(dataType, dependsOnVariables, isIdempotent)
		{
		}

		#region Static Members

		#region Public Methods

		public static Constant ValueToConstant(object value)
		{
			if (EngineUtil.IsStringValue(value))
				return new StringConstant((string)value);
			else if (EngineUtil.IsIntegerValue(value))
				return new FloatingConstant((int)value);
			else if (EngineUtil.IsFloatingValue(value))
				return new FloatingConstant((double)value);
			else
				return new ObjectConstant(value);
		}

		#endregion

		#endregion

		public override void SetArguments(IList<Operation> arguments)
		{
			throw new NotSupportedException();
		}
	}

	public abstract class Constant<T> : Constant
	{
		public Constant(DataType dataType, T value)
			: base(dataType, false, true)
		{
			Value = value;
		}

		#region Public Properties

		public T Value { get; private set; }

		public override IList<Operation> Arguments => new Operation[0];

		#endregion

		#region Public Override Methods

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

		#endregion
	}
}
