using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Execution.Interfaces
{
	public interface IPropertyConnector
	{
		object? GetPropertyValue(object target, string propertyName);
	}
}
