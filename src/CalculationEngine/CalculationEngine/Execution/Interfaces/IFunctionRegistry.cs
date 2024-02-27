using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution.Interfaces
{
	public interface IFunctionRegistry : IEnumerable<FunctionInfo>
	{
		int Count { get; }

		FunctionInfo? GetFunctionInfo(string functionName);
		bool IsFunctionName(string functionName);
		void RegisterFunction(string functionName, Delegate function);
		void RegisterFunction(string functionName, Delegate function, bool isIdempotent, bool isOverWritable);
	}
}
