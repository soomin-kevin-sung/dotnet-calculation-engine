using KevinComponent.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Execution.FunctionInfos
{
	public class Max : FunctionInfo
	{
		public Max()
			: base("max", typeof(double), true, -1, true, false)
		{
		}

		protected override object Execute(object[] args)
		{
			if (args.Length == 0)
				return 0;

			var max = double.MinValue;
			for (int i = 0; i < args.Length; i++)
			{
				if (!EngineUtil.ConvertToDouble(args[i], out var v))
					throw new ArgumentException("The parameter must be numeric.", nameof(args));

				max = Math.Max(max, v);
			}

			return max;
		}
	}
}
