using System;
using System.Runtime.Caching;

namespace SPO.Services
{
	public class CacheService : ICacheService
	{
		public CacheService() { }

		public bool Add(string cacheKey, object value, int cacheExpirationHours = 24)
		{
			var absExpiration = new DateTimeOffset(DateTime.Now.AddHours(cacheExpirationHours));
			var memoryCache = MemoryCache.Default;

			return memoryCache.Add(cacheKey.ToString(), value, absExpiration);
		}

		public void Delete(string cacheKey)
		{
			var memoryCache = MemoryCache.Default;

			if (memoryCache.Contains(cacheKey))
			{
				memoryCache.Remove(cacheKey);
			}
		}

		public object GetValue(string cacheKey)
		{
			var memoryCache = MemoryCache.Default;

			return memoryCache.Get(cacheKey);
		}
	}
}
