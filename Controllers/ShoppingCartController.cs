using System.Threading.Tasks;
using AutoMapper;
using Dreaming.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Models.ShoppingCart;

namespace Dreaming.Controllers
{
 
    public class ShoppingCartController : BaseController
    {
      
        private readonly IShoppingCart _cartRepository;
        private readonly IMapper _mapper;

        public ShoppingCartController(IShoppingCart cartRepository,IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetCartById(string Id)
        {
            var Cart = await _cartRepository.GetCartAsync(Id);
            return Ok(Cart ?? new ShoppingCart(Id));
        }
        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart shoppingCart)
        {
            var UpdatedCart = await _cartRepository.UpdateCartAsync(shoppingCart);
            return Ok(UpdatedCart);
        }
        [HttpDelete]
        public async Task DeleteAsync(string Id)
        {
            await _cartRepository.DeleteCartAsync(Id);
        }
    }
}