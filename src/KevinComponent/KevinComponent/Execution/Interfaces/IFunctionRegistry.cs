using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KevinComponent.Execution.FunctionInfos;

namespace KevinComponent.Execution.Interfaces
{
    public interface IFunctionRegistry : IEnumerable<FunctionInfo>
	{
		int Count { get; }

		FunctionInfo? GetFunctionInfo(string functionName);
		bool IsFunctionName(string functionName);
		void RegisterFunction(FunctionInfo functionInfo);
	}
}
