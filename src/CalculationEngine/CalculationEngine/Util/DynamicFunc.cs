using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Util
{
	public delegate TResult DynamicFunc<T, TResult>(params T[] values);
}
