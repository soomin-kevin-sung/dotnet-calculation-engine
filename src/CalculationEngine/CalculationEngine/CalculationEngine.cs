﻿using CalculationEngine.Execution;
using CalculationEngine.Execution.FunctionInfos;
using CalculationEngine.Execution.Interfaces;
using CalculationEngine.Operations;
using CalculationEngine.Tokens;
using CalculationEngine.Util;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Random = CalculationEngine.Execution.FunctionInfos.Random;
using SystemRandom = System.Random;

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
			_random = new SystemRandom();

			// set executor
#warning For Test
			options.ExecutionMode = ExecutionMode.Interpreted;
			if (options.ExecutionMode == ExecutionMode.Interpreted)
				_executor = new Interpreter(_caseSensitive);
			else
				_executor = new Interpreter(_caseSensitive);

			// set optimizer
			_optimizer = new Optimizer(new Interpreter());

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
		readonly SystemRandom _random;
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
			FunctionRegistry.RegisterFunction(new Sin());
			FunctionRegistry.RegisterFunction(new Cos());
			FunctionRegistry.RegisterFunction(new Csc());
			FunctionRegistry.RegisterFunction(new Sec());
			FunctionRegistry.RegisterFunction(new Asin());
			FunctionRegistry.RegisterFunction(new Acos());
			FunctionRegistry.RegisterFunction(new Tan());
			FunctionRegistry.RegisterFunction(new Cot());
			FunctionRegistry.RegisterFunction(new Atan());
			FunctionRegistry.RegisterFunction(new Acot());
			FunctionRegistry.RegisterFunction(new Loge());
			FunctionRegistry.RegisterFunction(new Log10());
			FunctionRegistry.RegisterFunction(new Logn());
			FunctionRegistry.RegisterFunction(new Sqrt());
			FunctionRegistry.RegisterFunction(new Abs());
			FunctionRegistry.RegisterFunction(new If());
			FunctionRegistry.RegisterFunction(new Ceiling());
			FunctionRegistry.RegisterFunction(new Floor());
			FunctionRegistry.RegisterFunction(new Truncate());
			FunctionRegistry.RegisterFunction(new Round());

			// Dynamic based arguments Functions
			FunctionRegistry.RegisterFunction(new Max());
			FunctionRegistry.RegisterFunction(new Min());
			FunctionRegistry.RegisterFunction(new Average());

			// Non Idempotent Functions
			FunctionRegistry.RegisterFunction(new Random(_random));
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
