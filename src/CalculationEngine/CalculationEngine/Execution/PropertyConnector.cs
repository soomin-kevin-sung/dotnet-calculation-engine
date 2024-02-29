﻿using CalculationEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Execution
{
	public abstract class PropertyConnector
	{
		protected PropertyConnector(bool caseSensitive)
		{
			CaseSensitive = caseSensitive;
		}

		#region Protected Properties

		protected bool CaseSensitive { get; }

		#endregion

		#region Protected Abstract Methods

		protected abstract object? GetPropertyValueInternal(object target, string propertyName);

		#endregion

		#region Public Methods

		public object? GetPropertyValue(object target, string propertyName)
		{
			return GetPropertyValueInternal(target, ConvertPropertyName(propertyName));
		}

		#endregion

		#region Protected Methods

		protected string ConvertPropertyName(string propertyName)
		{
			if (!CaseSensitive)
				return propertyName.ToLowerFast();

			return propertyName;
		}

		#endregion
	}
}
