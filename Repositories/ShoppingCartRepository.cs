
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dreaming.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ShoppingCart;
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
        public async Task<bool> DeleteCartAsync(string CartId)
        {
            return await _data.KeyDeleteAsync(CartId);
        }

        public async Task<ShoppingCart> GetCartAsync(string CartId)
        {
            var CartData = await _data.StringGetAsync(CartId);
            return CartData.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart>(CartData);
        }

        public async Task<ShoppingCart> UpdateCartAsync(ShoppingCart Cart)
        {
            var UpdateCart = await _data.StringSetAsync(Cart.ShopCartId, JsonSerializer.Serialize(Cart));
            if (!UpdateCart) return null;
            return await GetCartAsync(Cart.ShopCartId);
        }
    }
}