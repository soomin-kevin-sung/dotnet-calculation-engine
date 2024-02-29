using CalculationEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine = CalculationEngine.CalculationEngine;

namespace CalculationEngineTest
{
	public class InterpreterTest
	{
		[Fact]
		public void NumericFormulaTest()
		{
			// arrange
			var engine = new Engine();
			var variables = new Dictionary<string, object>()
			{
				{ "X", 1 },
				{ "y", 3 },
			};

			// act
			var answer = engine.Calculate("x + y * 3.12", variables);

			// assert
			Assert.True(answer is double);
			Assert.Equal(1 + 3 * 3.12, (double)answer);
		}

		[Fact]
		public void StringAndFunctionFormulaTest()
		{
			// arrange
			var engine = new Engine();
			var variables = new Dictionary<string, object>()
			{
				{ "ss", "TESTSTRING" },
			};

			// act
			var answer = engine.Calculate("if(SS == \"TESTSTRING\", \"SUCCESS\", \"FAIL\")", variables);

			// assert
			Assert.True(answer is string);
			Assert.Equal("SUCCESS", (string)answer);
		}
	}
}
