using KevinComponent.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Execution
{
	public partial class DynamicCompiler
	{
		private static class PrecompiledMethods
		{
			public static object? GetVariableValueOrThrow(string variableName, FormulaContext context)
			{
				if (context.Variables.TryGetValue(variableName, out var result))
					return result;
				else if (context.ConstantRegistry.GetConstantInfo(variableName) is ConstantInfo constantInfo)
					return constantInfo.Value;
				else
					throw new VariableNotDefinedException($"The variable \"{variableName}\" used is not defined.");
			}
		}
	}
}
