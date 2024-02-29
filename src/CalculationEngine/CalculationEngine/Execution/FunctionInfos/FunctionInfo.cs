using CalculationEngine.Execution.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution.FunctionInfos
{
    public abstract partial class FunctionInfo
	{
		protected FunctionInfo(string functionName, Type returnType, bool isDynamicFunc, int numOfParameter, bool isIdempotent, bool isOverWritable)
		{
			FunctionName = functionName;
			ReturnType = returnType;
			NumberOfParameters = numOfParameter;
			IsIdempotent = isIdempotent;
			IsDynamicFunc = isDynamicFunc;
			IsOverWritable = isOverWritable;
		}

		#region Public Properties

		public string FunctionName { get; }

		public int NumberOfParameters { get; }

		public bool IsOverWritable { get; set; }

		public bool IsIdempotent { get; set; }

		public bool IsDynamicFunc { get; }

		public Type ReturnType { get; }

		#endregion

		#region Protected Abstract Methods

		protected abstract object Execute(object[] args);

		#endregion

		#region Public Methods

		public object Invoke(object[] args)
		{
			ArgumentNullException.ThrowIfNull(nameof(args));
			if (!IsDynamicFunc && args.Length != NumberOfParameters)
				throw new ArgumentException($"There must be {NumberOfParameters} Parameters", nameof(args));

			var result = Execute(args);
			if (!ReturnType.IsAssignableFrom(result.GetType()))
				throw new Exception("Wrong Return Type");

			return result;
		}

		#endregion
	}
}
