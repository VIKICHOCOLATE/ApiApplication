using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace ApiApplication.Shared.Services
{
	public interface ICacheService
	{
		Task<T> GetCachedDataAsync<T>(string key);
		Task SetCacheDataAsync<T>(string key, T data, TimeSpan? expirationTime = null);
	}

	public class RedisCacheService : ICacheService
	{
		private readonly IDistributedCache _cache;

		public RedisCacheService(IDistributedCache cache)
		{
			_cache = cache;
		}

		public async Task<T> GetCachedDataAsync<T>(string key)
		{
			var jsonData = await _cache.GetStringAsync(key);
			if (string.IsNullOrEmpty(jsonData))
				return default(T);
			return JsonSerializer.Deserialize<T>(jsonData);
		}

		public async Task SetCacheDataAsync<T>(string key, T data, TimeSpan? expirationTime = null)
		{
			var jsonData = JsonSerializer.Serialize(data);
			if (expirationTime.HasValue)
			{
				await _cache.SetStringAsync(key, jsonData, new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = expirationTime
				});
			}
			else
			{
				await _cache.SetStringAsync(key, jsonData);
			}
		}
	}
}