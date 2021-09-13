using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Dreaming.Repositories
{
    public class ResponseCache : IResponseCache
    {
        private readonly IDatabase _data;
        public ResponseCache(IConnectionMultiplexer redis)
        {
            _data = redis.GetDatabase();
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
           if(response == null)
           {
               return;
           }
           var options = new JsonSerializerOptions
           {
               PropertyNamingPolicy = JsonNamingPolicy.CamelCase
           };

           var newresponse = JsonSerializer.Serialize(response,options);
           await _data.StringSetAsync(cacheKey,newresponse,timeToLive);
        }

        public async Task<string> GetCachedResponse(string cacheKey)
        {
            var cacheResponse = await _data.StringGetAsync(cacheKey);
            if(cacheResponse.IsNullOrEmpty)
            {
                return null;
            }
            return cacheResponse;
        }
    }
}