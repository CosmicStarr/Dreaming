using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamMore.Repositories;
using Models;
using Models.CreateOrder;

namespace Dreaming.Repositories
{
    public class OrderRepository:IOrderRepository
    {
        private readonly IShoppingCart _shop; 
        private readonly IUnitOfWork _unit;
        public OrderRepository(IUnitOfWork unitOfWork, IShoppingCart shoppingCart)
        {
            _shop = shoppingCart;
            _unit = unitOfWork;
        }

        public async Task<ActualOrder> CreateOrder(string Email, Address address, int deliveryId, string cartId)
        {
            var cart = await _shop.GetCartAsync(cartId);
            var listOfOrders = new List<OrderedItem>();
            foreach (var item in cart.CartItems)
            {
                var product = await _unit.Repository<Products>().Get(item.CartItemsId); 
                var ProductsOrdered = new ProductsOrdered(product.Id,product.ProductName,product.ImageUrl);
                var OrderedItem = new OrderedItem(ProductsOrdered,product.Price,item.Amount); 
                listOfOrders.Add(OrderedItem);
            }
        
            var getdelivery = await _unit.Repository<Delivery>().Get(deliveryId);
            var subTotal = listOfOrders.Sum(o => o.Price * o.Amount);

            var actualOrder = new ActualOrder(listOfOrders,Email,address,getdelivery,subTotal);
            _unit.Repository<ActualOrder>().Add(actualOrder);
            var results = await _unit.Complete();
            if(results <= 0) return null;
            await _shop.DeleteCarteAsync(cartId);
            return actualOrder;
        }
    }
}