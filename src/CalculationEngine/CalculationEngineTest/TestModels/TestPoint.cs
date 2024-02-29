using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngineTest.TestModels
{
	public class TestPoint
	{
		public TestPoint(string name,  double x, double y)
		{
			Name = name;
			X = x;
			Y = y;
		}

		public string Name { get; set; }

		public double X { get; set; }

		public double Y { get; set; }
	}
}
