using KevinComponent.Exceptions;
using KevinComponent.Execution.Interfaces;
using KevinComponent.Operations;
using KevinComponent.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Execution
{
	public partial class DynamicCompiler
	{
		private static class PrecompiledMethods
		{
			public static object GetVariableValueOrThrow(FormulaContext context, string variableName)
			{
				if (context.Variables.TryGetValue(variableName, out var result))
					return result;
				else if (context.ConstantRegistry.GetConstantInfo(variableName) is ConstantInfo constantInfo)
					return constantInfo.Value;
				else
					throw new VariableNotDefinedException($"The variable \"{variableName}\" used is not defined.");
			}

			public static object InvokeFunction(FormulaContext context, string functionName, object[] args)
			{
				var functionRegistry = context.FunctionRegistry;
				var functionInfo = functionRegistry.GetFunctionInfo(functionName);
				ExceptionUtil.ThrowIfNull(functionInfo);

				return functionInfo.Invoke(args);
			}

			public static object Add(object arg1, object arg2)
			{
				if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
					return v1 + v2;
				else
					throw new ArgumentException($"Wrong argument type on {nameof(Add)} method.");
			}

			public static object Subtract(object arg1, object arg2)
			{
				if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
					return v1 - v2;
				else
					throw new ArgumentException($"Wrong argument type on {nameof(Subtract)} method.");
			}

			public static object Multiply(object arg1, object arg2)
			{
				if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
					return v1 * v2;
				else
					throw new ArgumentException($"Wrong argument type on {nameof(Multiply)} method.");
			}

			public static object Divied(object dividend, object divisor)
			{
				if (EngineUtil.ConvertToDouble(dividend, out var v1) && EngineUtil.ConvertToDouble(divisor, out var v2))
					return v1 / v2;
				else
					throw new ArgumentException($"Wrong argument type on {nameof(Divied)} method.");
			}

			public static object Modulo(object dividend, object divisor)
			{
				if (EngineUtil.ConvertToDouble(dividend, out var v1) && EngineUtil.ConvertToDouble(divisor, out var v2))
					return v1 % v2;
				else
					throw new ArgumentException($"Wrong argument type on {nameof(Divied)} method.");
			}

			public static object Pow(object arg1, object arg2)
			{
				if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
					return Math.Pow(v1, v2);
				else
					throw new ArgumentException($"Wrong argument type on {nameof(Pow)} method.");
			}

			public static object Negate(object arg1)
			{
				if (EngineUtil.ConvertToDouble(arg1, out var v1))
					return -v1;
				else
					throw new ArgumentException($"Wrong argument type on {nameof(Negate)} method.");
			}

			public static bool Equal(object arg1, object arg2)
			{
				if (EngineUtil.IsStringValue(arg1) && EngineUtil.IsStringValue(arg2))
					return string.Equals((string)arg1, (string)arg2);
				else if (EngineUtil.ConvertToDouble(arg1, out var v1) && EngineUtil.ConvertToDouble(arg2, out var v2))
					return v1 == v2;
				else
					return arg1 == arg2;
			}

			public static bool NotEqual(object arg1, object arg2)
			{
				return !Equal(arg1, arg2);
			}
		}
	}
}
