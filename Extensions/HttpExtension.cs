using System.Text.Json;
using Dreaming.Helper;
using Microsoft.AspNetCore.Http;


namespace Dreaming.Extensions
{
    public static class HttpExtension
    {
        public static void AddPaginationHeader(this HttpResponse response,int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var pagingHeader = new PagerHeader(currentPage,itemsPerPage,totalItems,totalPages);
            // var options = new JsonSerializerOptions
            // {
            //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            // };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(pagingHeader));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
        
    }
}