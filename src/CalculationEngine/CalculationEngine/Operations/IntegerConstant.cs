using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public class IntegerConstant : Constant<int>
	{
		public IntegerConstant(int value)
			: base(DataType.Integer, value)
		{
		}
	}
}
