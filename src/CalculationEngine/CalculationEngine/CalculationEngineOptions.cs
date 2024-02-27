﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine
{
	public class CalculationEngineOptions
	{
		public CalculationEngineOptions()
		{
			CultureInfo = CultureInfo.CurrentCulture;
			CacheEnabled = true;
			OptimizerEnabled = true;
			CaseSensitive = false;
			DefaultFunctions = true;
			DefaultConstants = true;
			CacheMaximumSize = DefaultCacheMaximumSize;
			CacheReductionSize = DefaultCacheReductionSize;
		}

		internal const int DefaultCacheMaximumSize = 500;
		internal const int DefaultCacheReductionSize = 50;

		/// <summary>
		/// The <see cref="CultureInfo"/> required for correctly reading floating poin numbers.
		/// </summary>
		public CultureInfo CultureInfo { get; set; }

		/// <summary>
		/// Enable or disable caching of mathematical formulas.
		/// </summary>
		public bool CacheEnabled { get; set; }

		/// <summary>
		/// Configure the maximum cache size for mathematical formulas.
		/// </summary>
		public int CacheMaximumSize { get; set; }

		/// <summary>
		/// Configure the cache reduction size for mathematical formulas.
		/// </summary>
		public int CacheReductionSize { get; set; }

		/// <summary>
		/// Enable or disable optimizing of formulas.
		/// </summary>
		public bool OptimizerEnabled { get; set; }

		/// <summary>
		/// Enable case sensitive or case insensitive processing mode.
		/// </summary>
		public bool CaseSensitive { get; set; }

		/// <summary>
		/// Enable or disable the default functions.
		/// </summary>
		public bool DefaultFunctions { get; set; }

		/// <summary>
		/// Enable or disable the default constants.
		/// </summary>
		public bool DefaultConstants { get; set; }
	}
}
