using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using yourOrder.Core.Interfaces;

namespace yourOrder.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;
        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var _cacheService = context.HttpContext.RequestServices.GetRequiredService<ICachingService>();

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var cachedResponse = await _cacheService.GetCachedResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            var executedContext = await next(); //after

            // chach it
            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                await _cacheService.SetCacheResponseAsync(cacheKey, okObjectResult.Value!, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }

            // 7. Return the response to the user
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            // Create a unique key based on URL and Query parameters
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(request.Path);
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
