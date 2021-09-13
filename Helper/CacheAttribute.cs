using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dreaming.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Dreaming.Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timetoLive;
        public CacheAttribute(int timetoLive)
        {
            _timetoLive = timetoLive;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IResponseCache>();
            var  cacheKey = GenerateCacheKey(context.HttpContext.Request);
            var cacheResponse = await cache.GetCachedResponse(cacheKey);
            if(!string.IsNullOrEmpty(cacheResponse))
            {
                var content = new ContentResult
                {
                  Content = cacheResponse,  
                  ContentType = "application/json",
                  StatusCode = 200
                };

                context.Result = content;
                return;
            }
            var exeContext = await next();
            if(exeContext.Result is OkObjectResult okObjectResult)
            {
                await cache.CacheResponseAsync(cacheKey,okObjectResult.Value,TimeSpan.FromSeconds(_timetoLive));
            }
        }

        private string GenerateCacheKey(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");

            foreach (var (key, value) in request.Query.OrderBy(x =>x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}