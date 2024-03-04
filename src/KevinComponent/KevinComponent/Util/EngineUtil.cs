using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Util
{
	internal static class EngineUtil
	{
		internal static IDictionary<string, object> ConvertVariableNamesToLowerCase(IDictionary<string, object> variables)
		{
			var temp = new Dictionary<string, object>();
			foreach (var keyValuePair in variables)
				temp.Add(keyValuePair.Key.ToLowerFast(), keyValuePair.Value);

			return temp;
		}

		internal static string ToLowerFast(this string text)
		{
			var buffer = new StringBuilder(text.Length);

			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];

				if (c >= 'A' && c <= 'Z')
					buffer.Append((char)(c + 32));
				else
					buffer.Append(c);
			}

			return buffer.ToString();
		}

		internal static  bool ConvertToDouble(object value, out double result)
		{
			if (IsFloatingValue(value))
			{
				result = (double)value;
				return true;
			}
			else if (IsIntegerValue(value))
			{
				result = (int)value;
				return true;
			}

			result = 0;
			return false;
		}

		internal static bool IsNumericValue(object value)
		{
			return IsIntegerValue(value) || IsFloatingValue(value);
		}

		internal static bool IsIntegerValue(object value)
		{
			return value is int ||
				value is long ||
				value is short ||
				value is bool;
		}

		internal static bool IsFloatingValue(object value)
		{
			return value is float || value is double;
		}

		internal static bool IsStringValue(object value)
		{
			return value is string;
		}
	}
}
