using CalculationEngine.Execution;
using CalculationEngine.Execution.Interfaces;
using CalculationEngine.Operations;
using CalculationEngine.Tokens;
using CalculationEngine.Util;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace CalculationEngine
{
	public class CalculationEngine
	{
		public CalculationEngine()
			: this(new CalculationEngineOptions())
		{
		}

		public CalculationEngine(CultureInfo cultureInfo)
			: this(new CalculationEngineOptions() { CultureInfo = cultureInfo })
		{
		}

		public CalculationEngine(CalculationEngineOptions options)
		{
			_executionFormulaCache = new MemoryCache<string, Func<IDictionary<string, object>, object>>(options.CacheMaximumSize, options.CacheReductionSize);

			// functions and constants are not always caseSensitive.
			FunctionRegistry = new FunctionRegistry(false);
			ConstantRegistry = new ConstantRegistry(false);

			// options
			_cultureInfo = options.CultureInfo;
			_cacheEnabled = options.CacheEnabled;
			_optimizerEnabled = options.OptimizerEnabled;
			_caseSensitive = options.CaseSensitive;

			// random
			_random = new Random();

			// set executor
			_executor = null;

			// set optimizer
			_optimizer = new Optimizer(null);

			// register defautl constants
			if (options.DefaultConstants)
				RegisterDefaultConstants();

			// register defautl functions
			if (options.DefaultFunctions)
				RegisterDefaultFunctions();

		}

		#region Private Variables

		readonly MemoryCache<string, Func<IDictionary<string, object>, object>> _executionFormulaCache;
		readonly CultureInfo _cultureInfo;
		readonly bool _cacheEnabled;
		readonly bool _optimizerEnabled;
		readonly bool _caseSensitive;
		readonly Random _random;
		readonly IExecutor _executor;
		readonly Optimizer _optimizer;

		#endregion

		#region Internal Properties

		internal FunctionRegistry FunctionRegistry { get; }

		internal ConstantRegistry ConstantRegistry { get; }

		#endregion

		#region Public Properties

		public IEnumerable<FunctionInfo> Functions
			=> FunctionRegistry;

		public IEnumerable<ConstantInfo> Constants
			=> ConstantRegistry;

		#endregion

		#region Public Methods

		public object? Calculate(string formulaText)
		{
			return Calculate(formulaText, new Dictionary<string, object>());
		}

		public object? Calculate(string formulaText, IDictionary<string, object> variables)
		{
			ArgumentException.ThrowIfNullOrEmpty(formulaText);
			ArgumentNullException.ThrowIfNull(variables);

			if (!_caseSensitive)
				variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);

			CheckVariableNames(variables);

			if (!IsInFormulaCache(formulaText, null, out var function))
			{
				var operation = BuildAbstractSyntaxTree(formulaText, new ConstantRegistry(_caseSensitive));
				function = BuildFormula(formulaText, null, operation);
			}

			return function(variables);
		}

		#endregion

		#region Private Methods

		private void RegisterDefaultConstants()
		{
			ConstantRegistry.RegisterConstant("e", Math.E, false);
			ConstantRegistry.RegisterConstant("pi", Math.PI, false);
		}

		private void RegisterDefaultFunctions()
		{
			FunctionRegistry.RegisterFunction("sin", Math.Sin, true, false);
			FunctionRegistry.RegisterFunction("cos", Math.Cos, true, false);
			FunctionRegistry.RegisterFunction("csc", MathUtil.Csc, true, false);
			FunctionRegistry.RegisterFunction("sec", MathUtil.Sec, true, false);
			FunctionRegistry.RegisterFunction("asin", Math.Asin, true, false);
			FunctionRegistry.RegisterFunction("acos", Math.Acos, true, false);
			FunctionRegistry.RegisterFunction("tan", Math.Tan, true, false);
			FunctionRegistry.RegisterFunction("cot", MathUtil.Cot, true, false);
			FunctionRegistry.RegisterFunction("atan", Math.Atan, true, false);
			FunctionRegistry.RegisterFunction("acot", MathUtil.Acot, true, false);
			FunctionRegistry.RegisterFunction("loge", (double a) => Math.Log(a), true, false);
			FunctionRegistry.RegisterFunction("log10", Math.Log10, true, false);
			FunctionRegistry.RegisterFunction("logn", (double a, double b) => Math.Log(a, b), true, false);
			FunctionRegistry.RegisterFunction("sqrt", Math.Sqrt, true, false);
			FunctionRegistry.RegisterFunction("abs", (double a) => Math.Abs(a), true, false);
			FunctionRegistry.RegisterFunction("if", (double a, object b, object c) => a != 0.0 ? b : c, true, false);
			FunctionRegistry.RegisterFunction("ceiling", (double a) => Math.Ceiling(a), true, false);
			FunctionRegistry.RegisterFunction("floor", (double a) => Math.Floor(a), true, false);
			FunctionRegistry.RegisterFunction("truncate", (double a) => Math.Truncate(a), true, false);
			FunctionRegistry.RegisterFunction("round", (double a) => Math.Round(a), true, false);

			// Dynamic based arguments Functions
			FunctionRegistry.RegisterFunction("max", (DynamicFunc<double, double>)((a) => a.Max()), true, false);
			FunctionRegistry.RegisterFunction("min", (DynamicFunc<double, double>)((a) => a.Min()), true, false);
			FunctionRegistry.RegisterFunction("avg", (DynamicFunc<double, double>)((a) => a.Average()), true, false);

			// Non Idempotent Functions
			FunctionRegistry.RegisterFunction("random", _random.NextDouble, false, false);
		}

		private bool IsInFormulaCache(string formulaText, ConstantRegistry? compiledConstants, [NotNullWhen(true)] out Func<IDictionary<string, object>, object>? compiledFunction)
		{
			compiledFunction = null;
			if (_cacheEnabled)
				return _executionFormulaCache.TryGetValue(GenerateFormulaCacheKey(formulaText, compiledConstants), out compiledFunction);

			return false;
		}

		private string GenerateFormulaCacheKey(string formulaText, ConstantRegistry? compiledConstants)
		{
			if (compiledConstants != null && compiledConstants.Count > 0)
				return $"{formulaText}@{string.Join(",", compiledConstants.Select(t => $"{t.ConstantName}:{t.Value}"))}";

			return formulaText;
		}

		private Operation BuildAbstractSyntaxTree(string formulaText, ConstantRegistry compiledConstants)
		{
			// formulaText to Tokens.
			var tokenReader = new TokenReader(_cultureInfo);
			var tokens = tokenReader.Read(formulaText);

			// Tokens to Operation.
			var astBuilder = new AstBuilder(FunctionRegistry, _caseSensitive, compiledConstants);
			var operation = astBuilder.Build(tokens);

			if (_optimizerEnabled)
				return _optimizer.Optimize(operation, FunctionRegistry, ConstantRegistry);
			else
				return operation;
		}

		private Func<IDictionary<string, object>, object> BuildFormula(string formulaText, ConstantRegistry? compiledConstants, Operation operation)
		{
			return _executionFormulaCache.GetOrAdd(
				GenerateFormulaCacheKey(formulaText, compiledConstants),
				v => _executor.BuildFormula(operation, FunctionRegistry, ConstantRegistry));
		}

		private void CheckVariableNames(IDictionary<string, object> variables)
		{
			foreach (string variableName in variables.Keys)
			{
				var constantInfo = ConstantRegistry.GetConstantInfo(variableName);
				if (constantInfo != null && !constantInfo.IsOverWritable)
					throw new ArgumentException($"The name \"{variableName}\" is a reservered variable name that cannot be overwritten.", nameof(variables));

				var functionInfo = FunctionRegistry.GetFunctionInfo(variableName);
				if (functionInfo != null)
					throw new ArgumentException($"The name \"{variableName}\" is a function name. Parameters cannot have this name.", nameof(variables));
			}
		}

		#endregion
	}
}
