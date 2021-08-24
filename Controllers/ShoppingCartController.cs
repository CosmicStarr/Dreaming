using System.Threading.Tasks;
using Dreaming.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Dreaming.Controllers
{
    public class ShoppingCartController : BaseController
    {
        private readonly IShoppingCart _cart;
       
        public ShoppingCartController(IShoppingCart cart)
        {
            _cart = cart;
        }

        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetShoppingCartById(string Id)
        {
            var obj = await _cart.GetCartAsync(Id);
            if(obj == null)
            {
                return null;
            }
            return Ok(obj ?? new ShoppingCart(Id));
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateCartAsync(ShoppingCart shoppingCart)
        {
            var obj = await _cart.UpdateCartAsync(shoppingCart);
            return Ok(obj);
        }

        [HttpDelete]
        public async Task DeleteCartAsync(string Id)
        {
            var obj = await _cart.DeleteCarteAsync(Id);
        }
    }
}