using KevinComponent.Execution;
using KevinComponent.Test.CalculationEngineTest.TestBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Test.CalculationEngineTest
{
	public class InterpreterEngineTest : EngineTestBase
	{
		public InterpreterEngineTest() : base(ExecutionMode.Interpreted)
		{
		}
	}
}
