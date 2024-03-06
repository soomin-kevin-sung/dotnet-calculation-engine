using KevinComponent.Execution.FunctionInfos;
using KevinComponent.Execution.Interfaces;
using KevinComponent.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Execution
{
    public class FunctionRegistry : IFunctionRegistry
	{
		public FunctionRegistry(bool caseSensitive)
		{
			_caseSensitive = caseSensitive;
			_functions = new Dictionary<string, FunctionInfo>();
		}

		#region Constants

		const string DynamicFuncTypeName = "Jace.DynamicFunc";

		#endregion

		#region Private Variables

		readonly bool _caseSensitive;
		readonly Dictionary<string, FunctionInfo> _functions;

		#endregion

		#region Public Properties

		public int Count
			=> _functions.Count;

		#endregion

		#region Public Methods

		public IEnumerator<FunctionInfo> GetEnumerator()
		{
			return _functions.Values.GetEnumerator();
		}

		public FunctionInfo? GetFunctionInfo(string functionName)
		{
			if (string.IsNullOrEmpty(functionName))
				throw new ArgumentNullException(nameof(functionName));

			return _functions.TryGetValue(ConvertFunctionName(functionName), out var functionInfo) ? functionInfo : null;
		}

		public bool IsFunctionName(string functionName)
		{
			if (string.IsNullOrEmpty(functionName))
				throw new ArgumentNullException(nameof(functionName));

			return _functions.ContainsKey(ConvertFunctionName(functionName));
		}

		public void RegisterFunction(FunctionInfo functionInfo)
		{
			ExceptionUtil.ThrowIfNull(functionInfo);

			// convert name by caseSensitive
			var functionName = ConvertFunctionName(functionInfo.FunctionName);

			// check the function is valid.
			if (_functions.ContainsKey(functionName))
			{
				if (!_functions[functionName].IsOverWritable)
					throw new Exception($"The function \"{functionName}\" cannot be overwriten.");
				else if (_functions[functionName].NumberOfParameters != functionInfo.NumberOfParameters)
					throw new Exception("The number of parameters cannot be changed when overwriting a method.");
				else if (_functions[functionName].IsDynamicFunc != functionInfo.IsDynamicFunc)
					throw new Exception("A Func can only be overwritten by another Func and a DynamicFunc can only be overwritten by another DynamicFunc.");

				_functions[functionName] = functionInfo;
			}
			else
			{
				_functions.Add(functionName, functionInfo);
			}
		}

		#endregion

		#region Private Methods

		private string ConvertFunctionName(string functionName)
		{
			return _caseSensitive ? functionName : functionName.ToLowerFast();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
