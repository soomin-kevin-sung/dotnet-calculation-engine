using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Operations
{
	public class FloatingConstant : Constant<double>
	{
		public FloatingConstant(double value)
			: base(DataType.Float, value)
		{
		}
	}
}
