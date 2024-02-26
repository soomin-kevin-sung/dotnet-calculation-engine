using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution.Interfaces
{
	public interface IConstantRegistry : IEnumerable<ConstantInfo>
	{
		ConstantInfo? GetConstantInfo(string constantName);
		bool IsConstantName(string constantName);
		void RegisterConstant(string constantName, object value);
		void RegisterConstant(string constantName, object value, bool isOverWritable);
	}
}
