using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yourOrder.Core.Interfaces
{
    public interface ICachingService
    {
        Task<T?> GetCachedResponseAsync<T>(string cacheKey);
        Task<bool> SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
        Task<bool> RemoveCacheAsync(string cacheKey);
        Task<bool> ExistsAsync(string cacheKey);
    }
}
