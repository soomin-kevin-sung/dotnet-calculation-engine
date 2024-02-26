using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public class ObjectConstant : Constant<object>
	{
		public ObjectConstant(object value)
			: base(DataType.Object, value)
		{
		}
	}
}
