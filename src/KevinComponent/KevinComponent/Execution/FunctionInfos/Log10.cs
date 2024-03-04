using KevinComponent.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Execution.FunctionInfos
{
	public class Log10 : FunctionInfo
	{
		public Log10()
			: base("log10", typeof(double), false, 1, true, false)
		{
		}

		protected override object Execute(object[] args)
		{
			if (EngineUtil.ConvertToDouble(args[0], out var v))
				return Math.Log10(v);

			throw new ArgumentException("The parameter must be numeric.", nameof(args));
		}
	}
}
