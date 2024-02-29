using CalculationEngine.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngineTest.TestModels
{
    public class TestPropertyConnector : PropertyConnector
    {
		public TestPropertyConnector(bool caseSensitive) : base(caseSensitive)
		{
		}

		protected override object? GetPropertyValueInternal(object target, string memberName)
        {
            if (target is TestPoint testPoint)
            {
                if (memberName == ConvertPropertyName(".Name"))
                    return testPoint.Name;
                else if (memberName == ConvertPropertyName(".X"))
                    return testPoint.X;
				else if (memberName == ConvertPropertyName(".X"))
					return testPoint.Y;
			}

            return null;
        }
    }
}
