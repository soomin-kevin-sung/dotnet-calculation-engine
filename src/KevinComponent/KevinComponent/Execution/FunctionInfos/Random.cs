using KevinComponent.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemRandom = System.Random;

namespace KevinComponent.Execution.FunctionInfos
{
	public class Random : FunctionInfo
	{
		public Random(SystemRandom random)
			: base("random", typeof(double), false, 0, false, false)
		{
			_random = random;
		}

		readonly SystemRandom _random;

		protected override object Execute(object[] args)
		{
			return _random.NextDouble();
		}
	}
}
