using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Util
{
	public partial class MemoryCache<TKey, TValue>
	{
		private class CacheItem
		{
			public CacheItem(MemoryCache<TKey, TValue> parent, TValue value)
			{
				_cache = parent;
				Value = value;

				Accessed();
			}

			#region Private Variables

			readonly MemoryCache<TKey, TValue> _cache;

			#endregion

			#region Public Properties

			public TValue Value { get; }

			public long LastAccessed { get; private set; }

			#endregion

			#region Public Methods

			public void Accessed()
			{
				LastAccessed = Interlocked.Increment(ref _cache._counter);
			}

			#endregion
		}
	}
}
