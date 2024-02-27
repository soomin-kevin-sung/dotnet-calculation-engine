using CalculationEngine.Execution.Interfaces;
using CalculationEngine.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution
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

		public void RegisterFunction(string functionName, Delegate function)
		{
			RegisterFunction(functionName, function, true, true);
		}

		public void RegisterFunction(string functionName, Delegate function, bool isIdempotent, bool isOverWritable)
		{
			ArgumentException.ThrowIfNullOrEmpty(functionName);
			ArgumentNullException.ThrowIfNull(function);

			var funcType = function.GetType();
			ArgumentException.ThrowIfNullOrEmpty(funcType.FullName);

			bool isDynFunc = false;
			int numOfParams = -1;

			if (funcType.FullName.StartsWith("System.Func"))
				numOfParams = function.GetMethodInfo().GetParameters().Length;
			else if (funcType.FullName.StartsWith(DynamicFuncTypeName))
				isDynFunc = true;
			else
				throw new ArgumentException("Only System.Func and " + DynamicFuncTypeName + " delegates are permitted.", nameof(function));

			// convert name by caseSensitive
			functionName = ConvertFunctionName(functionName);

			// check the function is valid.
			if (_functions.ContainsKey(functionName))
			{
				if (!_functions[functionName].IsOverWritable)
					throw new Exception($"The function \"{functionName}\" cannot be overwriten.");
				if (_functions[functionName].NumberOfParameters != numOfParams)
					throw new Exception("The number of parameters cannot be changed when overwriting a method.");
				if (_functions[functionName].IsDynamicFunc != isDynFunc)
					throw new Exception("A Func can only be overwritten by another Func and a DynamicFunc can only be overwritten by another DynamicFunc.");
			}

			// add function to _functions.
			var functionInfo = new FunctionInfo(functionName, numOfParams, isIdempotent, isOverWritable, isDynFunc, function);
			if (!_functions.TryAdd(functionName, functionInfo))
				_functions[functionName] = functionInfo;
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
