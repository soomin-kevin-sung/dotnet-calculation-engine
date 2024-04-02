using KevinComponent.Execution.FunctionInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Test.CalculationEngineTest.TestModels
{
	public class TestFunction : FunctionInfo
	{
		public TestFunction()
			: base("Test", typeof(double), false, 2, true, false)
		{
		}

		protected override object Execute(object[] args)
		{
			if (args.Length < 2)
				return double.NaN;

			if (args[0] is double a &&
				args[1] is double b)
				return a * b;

			return double.NaN;
		}
	}
}
