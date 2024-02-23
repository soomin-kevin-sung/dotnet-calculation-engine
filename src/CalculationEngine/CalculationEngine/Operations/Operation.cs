using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Operations
{
	public abstract class Operation
	{
		protected Operation(DataType dataType, bool dependsOnVariables, bool isIdempotent)
		{
			DataType = dataType;
			DependsOnVariables = dependsOnVariables;
			IsIdempotent = isIdempotent;
		}

		public DataType DataType { get; }

		public bool DependsOnVariables { get; }

		public bool IsIdempotent { get; }

		public abstract IList<Operation> Arguments { get; }
	}
}
