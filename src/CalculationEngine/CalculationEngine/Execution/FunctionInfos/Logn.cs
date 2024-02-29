using CalculationEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution.FunctionInfos
{
	public class Logn : FunctionInfo
	{
		public Logn()
			: base("logn", typeof(double), false, 2, true, false)
		{
		}

		protected override object Execute(object[] args)
		{
			if (EngineUtil.ConvertToDouble(args[0], out var v1) &&
				EngineUtil.ConvertToDouble(args[1], out var v2))
				return Math.Log(v1, v2);

			throw new ArgumentException("The parameter must be numeric.", nameof(args));
		}
	}
}
