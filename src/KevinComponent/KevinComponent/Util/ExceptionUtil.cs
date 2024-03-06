using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Util
{
	internal static class ExceptionUtil
	{
		internal static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
		{
			if (argument is null)
				throw new ArgumentNullException(paramName);
		}

		internal static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
		{
			ThrowIfNull(argument, paramName);

			if (string.IsNullOrEmpty(argument))
				throw new ArgumentException("The value cannot be an empty string.", paramName);
		}

		internal static void ThrowIfGreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(other) > 0)
				throw new ArgumentOutOfRangeException(paramName, $"{paramName} ('{value}') must be less than or equal to '{other}'");
		}

		internal static void ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(other) < 0)
				throw new ArgumentOutOfRangeException(paramName, $"{paramName} ('{value}') must be greater than or equal to '{other}'");
		}
	}
}
