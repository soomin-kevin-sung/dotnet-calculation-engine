using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public class StringConstant : Constant<string>
	{
		public StringConstant(string value)
			: base(DataType.String, value)
		{
		}
	}
}
