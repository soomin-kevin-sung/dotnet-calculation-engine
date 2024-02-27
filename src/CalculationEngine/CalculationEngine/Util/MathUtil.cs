using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Util
{
	public static class MathUtil
	{
		public static double Cot(double a)
			=> 1 / Math.Tan(a);

		public static double Acot(double d)
			=> Math.Atan(1 / d);

		public static double Csc(double a)
			=> 1 / Math.Sin(a);

		public static double Sec(double d)
			=> 1 / Math.Cos(d);
	}
}
