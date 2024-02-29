using CalculationEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution.FunctionInfos
{
	public class Min : FunctionInfo
	{
		public Min()
			: base("min", typeof(double), true, -1, true, false)
		{
		}

		protected override object Execute(object[] args)
		{
			if (args.Length == 0)
				return 0;

			var min = double.MaxValue;
			for (int i = 0; i < args.Length; i++)
			{
				if (!EngineUtil.ConvertToDouble(args[i], out var v))
					throw new ArgumentException("The parameter must be numeric.", nameof(args));

				min = Math.Min(min, v);
			}

			return min;
		}
	}
}
