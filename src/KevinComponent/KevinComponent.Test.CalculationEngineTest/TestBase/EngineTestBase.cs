using KevinComponent.Execution;
using KevinComponent.Test.CalculationEngineTest.TestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Test.CalculationEngineTest.TestBase
{
    public abstract class EngineTestBase
    {
		protected EngineTestBase(ExecutionMode mode)
		{
			_mode = mode;
		}

		readonly ExecutionMode _mode;

		[Fact]
        public void NumericFormulaTest()
        {
			// arrange
			var engine = new CalculationEngine(new CalculationEngineOptions() { ExecutionMode = _mode });
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
			var engine = new CalculationEngine(new CalculationEngineOptions() { ExecutionMode = _mode });
			var variables = new Dictionary<string, object>()
			{
				{ "ss", "TESTSTRING" },
				{ "cOdE", "PRESENT" }
			};

			// act
			var answer = engine.Calculate("if(SS == \"TESTSTRING\", \"SUCCESS\", \"FAIL\")", variables);

			// assert
			Assert.True(answer is string);
			Assert.Equal("SUCCESS", (string)answer);
		}

		[Fact]
		public void ReferenceObjectPropertyFormulaTest()
        {
			// arrange
			var options = new CalculationEngineOptions()
			{
				ExecutionMode = _mode,
				PropertyConnectorFactory = caseSensitive => new TestPropertyConnector(caseSensitive)
			};
			var engine = new CalculationEngine(options);
			var variables = new Dictionary<string, object>()
			{
				{ "p1", new TestPoint("A", 0, 1) },
				{ "p2", new TestPoint("B", 1.25, -2.56) },
				{ "Code", "A" }
			};

			// act
			var answer = engine.Calculate("p1.x + p2.y", variables);
			var answer2 = engine.Calculate("if ( Code == p1.Name, \"True\", \"False\")", variables);

			// assert
			Assert.Equal(-2.56, answer);
			Assert.Equal("True", answer2);
		}

		[Fact]
		public void AndOrFormulaTest()
		{
			// arrange
			var engine = new CalculationEngine(new CalculationEngineOptions() { ExecutionMode = _mode });
			var variables = new Dictionary<string, object>()
			{
				{ "a", 5 },
				{ "b", 10 },
				{ "c", 15 },
				{ "d", 0 },
			};

			// act
			var answer = engine.Calculate("if(a < b || (a > d && c > d), \"True\", \"False\")", variables);

			// assert
			Assert.Equal("True", answer);
		}
    }
}
