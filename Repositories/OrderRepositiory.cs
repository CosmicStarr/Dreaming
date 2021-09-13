using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamMore.Repositories;
using Models;
using Models.Order;

namespace Dreaming.Repositories
{
    public class OrderRepositiory : IOrderRepository
    {
        private readonly IShoppingCart _cartRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IPaymentService _paymentService;

        public OrderRepositiory(IShoppingCart cartRepository, IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _paymentService = paymentService;
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Orders> CreateOrder(string BuyerEmail, int DeliveryId, string CartID, Address orderAddress)
        {
            //Retrieve the shopping cart
            var Cart = await _cartRepository.GetCartAsync(CartID);
            //Creating a list of Orders!
            var Items = new List<OrderItems>();
            foreach (var item in Cart.CartItems)
            {
                //the Shopping Cart
                var ProductItems = await _unitOfWork.Repository<Products>().Get(item.CartItemsId);
                //The actually products in the Shopping Cart
                var ItemsOrdered = new ProductsOrdered(ProductItems.Id, ProductItems.ProductName, ProductItems.ImageUrl);
                //The information about the Shopping Cart the Amount of products, the price, the Quantity of each product
                var orderItems = new OrderItems(ItemsOrdered, ProductItems.Price, item.Amount);
                Items.Add(orderItems);
            }
            var Delivery = await _unitOfWork.Repository<Delievery>().Get(DeliveryId);
            var SubTotal = Items.Sum(item => item.Price * item.Amount);
            //Create an Actually Order
            var ActualOrder = new Orders(Items, BuyerEmail, orderAddress, Delivery, SubTotal, Cart.PaymentID);
            if (ActualOrder == null)
            {
                var existingOrder = await _unitOfWork.Repository<Orders>().GetFirstOrDefault(x => x.PaymentID == Cart.PaymentID);
                if (existingOrder != null) _unitOfWork.Repository<Orders>().Remove(existingOrder);
                await _paymentService.CreateOrUpdate(Cart.PaymentID);
            }

            _unitOfWork.Repository<Orders>().Add(ActualOrder);
            //Saving the Actually Order to the database!
            var results = await _unitOfWork.Complete();
            if (results <= 0) return null;
            //Deleting the Shopping Cart
            // await _cartRepository.DeleteCartAsync(CartID);
            return ActualOrder;

        }

        public async Task<IEnumerable<Delievery>> GetSpecialDeliveries()
        {
            return await _unitOfWork.Repository<Delievery>().GetAllAsync(null, orderby: x => x.OrderBy(x => x.Price), null);
        }
    }
}