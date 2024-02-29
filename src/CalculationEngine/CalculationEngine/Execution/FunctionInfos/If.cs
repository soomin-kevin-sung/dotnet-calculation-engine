using CalculationEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution.FunctionInfos
{
	public class If : FunctionInfo
	{
		public If()
			: base("if", typeof(object), false, 3, true, false)
		{
		}

		protected override object Execute(object[] args)
		{
			if (EngineUtil.ConvertToDouble(args[0], out var v))
				return v != 0.0 ? args[1] : args[2];

			throw new ArgumentException("The parameter must be numeric.", nameof(args));
		}
	}
}
