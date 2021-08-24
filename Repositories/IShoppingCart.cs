using System.Threading.Tasks;
using Models;

namespace Dreaming.Repositories
{
    public interface IShoppingCart
    {
        Task<ShoppingCart> GetCartAsync(string Id);
        Task<ShoppingCart> UpdateCartAsync(ShoppingCart shoppingCart);
        Task<bool> DeleteCarteAsync(string Id);
    }
}