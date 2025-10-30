using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using yourOrder.Core.Interfaces;

namespace yourOrder.Services
{
    public class CachingService : ICachingService
    {
        private readonly IDatabase _database;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public CachingService(IConnectionMultiplexer redis)
        {
            if (redis == null)
                throw new ArgumentNullException(nameof(redis));

            _database = redis.GetDatabase();
        }



        public async Task<T?> GetCachedResponseAsync<T>(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(cacheKey));

            var cachedData = await _database.StringGetAsync(cacheKey);
            if (cachedData.IsNullOrEmpty)
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(cachedData!, _jsonOptions);
            }
            catch
            {
                await _database.KeyDeleteAsync(cacheKey);
                return default;
            }
        }

        public async Task<bool> SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(cacheKey));

            if (response is null)
                return false;

            var serialized = JsonSerializer.Serialize(response, _jsonOptions);
            bool result = await _database.StringSetAsync(cacheKey, serialized, timeToLive);

            return result; 
        }


        public async Task<bool> RemoveCacheAsync(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(cacheKey));

            return await _database.KeyDeleteAsync(cacheKey);
        }

        public async Task<bool> ExistsAsync(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(cacheKey));

            return await _database.KeyExistsAsync(cacheKey);
        }
    }
}
