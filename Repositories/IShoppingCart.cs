using System.Threading.Tasks;
using Models;
using Models.ShoppingCart;

namespace Dreaming.Repositories
{
    public interface IShoppingCart
    {
        Task<ShoppingCart> GetCartAsync(string CartId);
        Task<ShoppingCart> UpdateCartAsync(ShoppingCart Cart);
        Task<bool> DeleteCartAsync(string CartId);
    }
}