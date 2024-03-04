﻿using CalculationEngine;
using CalculationEngineTest.TestModels;
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

		[Fact]
		public void ReferenceObjectPropertyFormulaTest()
		{
			// arrange
			var options = new CalculationEngineOptions()
			{
				PropertyConnectorFactory = caseSensitive => new TestPropertyConnector(caseSensitive)
			};
			var engine = new Engine(options);
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
	}
}
