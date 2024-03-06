using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Util
{
	public partial class MemoryCache<TKey, TValue>
		where TKey : notnull
	{
		public MemoryCache(int maximumSize, int reductionSize)
		{
			ExceptionUtil.ThrowIfLessThan(maximumSize, 1);
			ExceptionUtil.ThrowIfLessThan(reductionSize, 1);

			_maximumSize = maximumSize;
			_reductionSize = reductionSize;
			_dict = new ConcurrentDictionary<TKey, CacheItem>();
		}

		#region Private Variables

		readonly ConcurrentDictionary<TKey, CacheItem> _dict;
		readonly int _maximumSize;
		readonly int _reductionSize;
		long _counter;

		#endregion

		#region Public Properties

		public TValue this[TKey key]
		{
			get
			{
				var item = _dict[key];
				item.Accessed();

				return item.Value;
			}
		}

		public int Count => _dict.Count;

		#endregion

		#region Public Methods

		public bool ContainsKey(TKey key)
			=> _dict.ContainsKey(key);

		public bool TryGetValue(TKey key, out TValue? value)
		{
			if (_dict.TryGetValue(key, out var item))
			{
				item.Accessed();
				value = item.Value;
				return true;
			}
			else
			{
				value = default;
				return false;
			}
		}

		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			ExceptionUtil.ThrowIfNull(valueFactory);

			var item = _dict.GetOrAdd(key, k =>
			{
				EnsureCacheStorageAvailable();
				return new CacheItem(this, valueFactory(k));
			});

			return item.Value;
		}

		#endregion

		#region Private Methods

		private void EnsureCacheStorageAvailable()
		{
			if (_dict.Count >= _maximumSize) // >= because we want to add an item after this method
			{
				IList<TKey> keysToDelete = (from p in _dict.ToArray()
											where p.Key != null && p.Value != null
											orderby p.Value.LastAccessed ascending
											select p.Key).Take(_reductionSize).ToList();

				foreach (TKey key in keysToDelete)
					_dict.TryRemove(key, out _);
			}
		}

		#endregion
	}
}
