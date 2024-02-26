using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution
{
	public class FunctionInfo
	{
		public FunctionInfo(string functionName, int numberOfParameters, bool isIdempotent, bool isOverWritable, bool isDynamicFunc, Delegate function)
		{
			FunctionName = functionName;
			NumberOfParameters = numberOfParameters;
			IsIdempotent = isIdempotent;
			IsOverWritable = isOverWritable;
			IsDynamicFunc = isDynamicFunc;
			Function = function;
			ReturnType = function.Method.ReturnType;
		}

		public string FunctionName { get; }

		public int NumberOfParameters { get; }

		public bool IsOverWritable { get; set; }

		public bool IsIdempotent { get; set; }

		public bool IsDynamicFunc { get; }

		public Delegate Function { get; }

		public Type ReturnType { get; }
	}
}
