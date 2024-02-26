using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Util
{
	internal static class EngineUtil
	{
		internal static string ToLowerFast(this string text)
		{
			StringBuilder buffer = new StringBuilder(text.Length);

			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];

				if (c >= 'A' && c <= 'Z')
				{
					buffer.Append((char)(c + 32));
				}
				else
				{
					buffer.Append(c);
				}
			}

			return buffer.ToString();
		}
	}
}
