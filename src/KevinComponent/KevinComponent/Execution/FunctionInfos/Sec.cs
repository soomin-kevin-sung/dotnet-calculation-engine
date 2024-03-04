using KevinComponent.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Execution.FunctionInfos
{
	public class Sec : FunctionInfo
	{
		public Sec()
			: base("sec", typeof(double), false, 1, true, false)
		{
		}

		protected override object Execute(object[] args)
		{
			if (EngineUtil.ConvertToDouble(args[0], out var v))
				return MathUtil.Sec(v);

			throw new ArgumentException("The parameter must be numeric.", nameof(args));
		}
	}
}
