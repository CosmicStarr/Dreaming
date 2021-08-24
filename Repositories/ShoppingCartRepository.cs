
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dreaming.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using StackExchange.Redis;

namespace Dreaming.Repositories
{
    public class ShoppingCartRepository : IShoppingCart
    {
        private readonly IDatabase _data;
        public ShoppingCartRepository(IConnectionMultiplexer redis)
        {
           
            _data = redis.GetDatabase();
        }
        public async Task<bool> DeleteCarteAsync(string Id)
        {
            return await _data.KeyDeleteAsync(Id);
        }

        public async Task<ShoppingCart> GetCartAsync(string Id)
        {
            var data = await _data.StringGetAsync(Id);
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart>(data);
        }

        public async Task<ShoppingCart> UpdateCartAsync(ShoppingCart shoppingCart)
        {
            var created = await _data.StringSetAsync(shoppingCart.ShopCartId,JsonSerializer.Serialize<ShoppingCart>(shoppingCart),TimeSpan.FromDays(10));
            if(!created) return null;
            return await GetCartAsync(shoppingCart.ShopCartId);
        }
    }
}