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

		public bool DependsOnVariables { get; internal set; }

		public bool IsIdempotent { get; internal set; }

		public abstract IList<Operation> Arguments { get; }
	}
}
