using System;
using System.Threading.Tasks;

namespace Dreaming.Repositories
{
    public interface IResponseCache
    {
        Task CacheResponseAsync(string cacheKey,object response,TimeSpan timeToLive);
        
        Task<string> GetCachedResponse(string cacheKey);
    }
}