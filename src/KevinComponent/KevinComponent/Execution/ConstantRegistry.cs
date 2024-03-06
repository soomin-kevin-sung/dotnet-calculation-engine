using KevinComponent.Execution.Interfaces;
using KevinComponent.Util;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Execution
{
	public class ConstantRegistry : IConstantRegistry
	{
		public ConstantRegistry(bool caseSensitive)
		{
			_caseSensitive = caseSensitive;
			_constants = new Dictionary<string, ConstantInfo>();
		}

		#region Private Variables

		readonly bool _caseSensitive;
		readonly Dictionary<string, ConstantInfo> _constants;

		#endregion

		#region Public Properties

		public int Count
			=> _constants.Count;

		#endregion

		#region Public Methods

		public IEnumerator<ConstantInfo> GetEnumerator()
		{
			return _constants.Values.GetEnumerator();
		}

		public ConstantInfo? GetConstantInfo(string constantName)
		{
			ExceptionUtil.ThrowIfNullOrEmpty(constantName);
			return _constants.TryGetValue(ConvertConstantName(constantName), out var constantInfo) ? constantInfo : null;
		}

		public bool IsConstantName(string constantName)
		{
			ExceptionUtil.ThrowIfNullOrEmpty(constantName);
			return _constants.ContainsKey(ConvertConstantName(constantName));
		}

		public void RegisterConstant(string constantName, object value)
		{
			RegisterConstant(constantName, value, true);
		}

		public void RegisterConstant(string constantName, object value, bool isOverWritable)
		{
			ExceptionUtil.ThrowIfNullOrEmpty(constantName);

			constantName = ConvertConstantName(constantName);
			var constantInfo = new ConstantInfo(constantName, value, isOverWritable);

			if (_constants.ContainsKey(constantName))
			{
				if (_constants[constantName].IsOverWritable)
					throw new Exception($"The constant \"{constantName}\" cannot be overwriten.");

				_constants[constantName] = constantInfo;
			}
			else
			{
				_constants.Add(constantName, constantInfo);
			}	
		}

		#endregion

		#region Private Methods

		private string ConvertConstantName(string constantName)
		{
			return _caseSensitive ? constantName : constantName.ToLowerFast();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
