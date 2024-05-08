using KevinComponent;
using KevinComponent.Execution;
using KevinComponent.Execution.FunctionInfos;
using KevinComponent.Operations;
using KevinComponent.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Test.CalculationEngineTest
{
	public class AstBuilderTest
	{
		[Fact]
		public void NumericOperationTest()
		{
			// arrange
			var caseSensitive = false;
			var functions = new FunctionRegistry(caseSensitive);
			var astBuilder = new AstBuilder(functions, caseSensitive);
			var tokenReader = new TokenReader();
			var formula = "1+2+(3*4+5.12)-1e-9";

			// act
			var tokens = tokenReader.Read(formula);
			var op = astBuilder.Build(tokens);

			// assert
			var expectedOperatorTypes = new Type[]
			{
				typeof(Subtraction), // 1+2+(3*4+5.12)-1e-9
				typeof(Addition), // 1+2+(3*4+5.12)
				typeof(FloatingConstant), // 1e-9
				typeof(Addition), // 1+2
				typeof(Addition), // 3*4+5.12
				typeof(FloatingConstant), // 1
				typeof(FloatingConstant), // 2
				typeof(Multiplication), // 3*4
				typeof(FloatingConstant), // 5.12
				typeof(FloatingConstant), // 3
				typeof(FloatingConstant), // 4
			};

			var q = new Queue<Operation>();
			q.Enqueue(op);

			int expIndex = 0;
			while (q.Count > 0)
			{
				var node = q.Dequeue();
				Assert.True(node.GetType() == expectedOperatorTypes[expIndex++]);

				var args = node.Arguments;
				for (int i = 0; i < args.Count; i++)
					q.Enqueue(args[i]);
			}
		}

		[Fact]
		public void StringOperationTest()
		{
			// arrange
			var caseSensitive = false;
			var functions = new FunctionRegistry(caseSensitive);
			var astBuilder = new AstBuilder(functions, caseSensitive);
			var tokenReader = new TokenReader();
			var formula = "\"dsadfe dsafed12312+04095_)()(\"";

			// act
			var tokens = tokenReader.Read(formula);
			var op = astBuilder.Build(tokens);

			// assert
			Assert.True(op.GetType() == typeof(StringConstant));
		}

		[Fact]
		public void StringVariableOperationTest()
		{
			// arrange
			var caseSensitive = false;
			var functions = new FunctionRegistry(caseSensitive);
			var astBuilder = new AstBuilder(functions, caseSensitive);
			var tokenReader = new TokenReader();
			var formula = "var1==\"dsadfe dsafed12312+04095_)()(\"";

			// act
			var tokens = tokenReader.Read(formula);
			var op = astBuilder.Build(tokens);

			// assert
			var expectedOperatorTypes = new Type[]
			{
				typeof(Equal), // var1=="dsadfe dsafed12312+04095_)()("
				typeof(Variable), // var1
				typeof(StringConstant), // "dsadfe dsafed12312+04095_)()("
			};

			var q = new Queue<Operation>();
			q.Enqueue(op);

			int expIndex = 0;
			while (q.Count > 0)
			{
				var node = q.Dequeue();
				Assert.True(node.GetType() == expectedOperatorTypes[expIndex++]);

				var args = node.Arguments;
				for (int i = 0; i < args.Count; i++)
					q.Enqueue(args[i]);
			}
		}

		[Fact]
		public void FunctionOpertionTest()
		{
			// arrange
			var caseSensitive = false;
			var functions = new FunctionRegistry(caseSensitive);
			var astBuilder = new AstBuilder(functions, caseSensitive);
			var tokenReader = new TokenReader();
			var formula = "if (var1 == 1, \"dsadfe dsafed12312+04095_)()(\", \"NO\")";

			// act
			functions.RegisterFunction(new If());
			var tokens = tokenReader.Read(formula);
			var op = astBuilder.Build(tokens);

			// assert
			var expectedOperatorTypes = new Type[]
			{
				typeof(Function), // if (var1 == 1, "dsadfe dsafed12312+04095_)()(", "NO")
				typeof(Equal), // "NO"
				typeof(StringConstant), // "dsadfe dsafed12312+04095_)()("
				typeof(StringConstant), // "NO"
				typeof(Variable), // var1
				typeof(FloatingConstant) // 1
			};

			var q = new Queue<Operation>();
			q.Enqueue(op);

			int expIndex = 0;
			while (q.Count > 0)
			{
				var node = q.Dequeue();
				Assert.True(node.GetType() == expectedOperatorTypes[expIndex++]);

				var args = node.Arguments;
				for (int i = 0; i < args.Count; i++)
					q.Enqueue(args[i]);
			}
		}

		[Fact]
		public void ReferenceMemeberOpertionTest()
		{
			// arrange
			var caseSensitive = false;
			var functions = new FunctionRegistry(caseSensitive);
			var astBuilder = new AstBuilder(functions, caseSensitive);
			var tokenReader = new TokenReader();
			var formula = "1 - P1.Offset";

			// act
			var tokens = tokenReader.Read(formula);
			var op = astBuilder.Build(tokens);

			// assert
			var expectedOperatorTypes = new Type[]
			{
				typeof(Subtraction), // 1 - P1.Offset
				typeof(FloatingConstant), // 1
				typeof(ReferenceObjectProperty), // P1.Offset
				typeof(Variable) // P1
			};

			var q = new Queue<Operation>();
			q.Enqueue(op);

			int expIndex = 0;
			while (q.Count > 0)
			{
				var node = q.Dequeue();
				Assert.True(node.GetType() == expectedOperatorTypes[expIndex++]);

				var args = node.Arguments;
				for (int i = 0; i < args.Count; i++)
					q.Enqueue(args[i]);
			}
		}
	}
}
