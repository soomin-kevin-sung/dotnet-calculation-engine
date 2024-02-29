using CalculationEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution.FunctionInfos
{
	public class Acos : FunctionInfo
	{
		public Acos()
			: base("acos", typeof(double), false, 1, true, false)
		{
		}

		protected override object Execute(object[] args)
		{
			if (EngineUtil.ConvertToDouble(args[0], out var v))
				return Math.Acos(v);

			throw new ArgumentException("The parameter must be numeric.", nameof(args));
		}
	}
}
