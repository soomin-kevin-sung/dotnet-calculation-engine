using KevinComponent.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Test.CalculationEngineTest.TestModels
{
    public class TestPropertyConnector : PropertyConnector
    {
		public TestPropertyConnector(bool caseSensitive) : base(caseSensitive)
		{
		}

		protected override object? OnGetPropertyValue(object target, string propertyName)
        {
            if (target is TestPoint testPoint)
            {
                if (propertyName.Equals(ConvertPropertyName(".Name")))
                    return testPoint.Name;
                else if (propertyName.Equals(ConvertPropertyName(".X")))
                    return testPoint.X;
				else if (propertyName.Equals(ConvertPropertyName(".Y")))
					return testPoint.Y;
			}

            return null;
        }
    }
}
