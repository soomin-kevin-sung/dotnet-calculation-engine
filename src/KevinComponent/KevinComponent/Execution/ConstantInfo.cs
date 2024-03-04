using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Execution
{
	public class ConstantInfo
	{
		public ConstantInfo(string constantName, object value, bool isOverWritable)
		{
			ConstantName = constantName;
			Value = value;
			ValueType = value.GetType();
			IsOverWritable = isOverWritable;
		}

		public string ConstantName { get; }

		public object Value { get; }

		public Type ValueType { get; }

		public bool IsOverWritable { get; set; }
	}
}
