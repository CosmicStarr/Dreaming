using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamMore.Repositories;
using Microsoft.Extensions.Configuration;
using Models.Order;
using Models.ShoppingCart;
using Stripe;
using Product = Models.Products;

namespace Dreaming.Repositories
{
    public class PaymentService : IPaymentService
    {
        private readonly IShoppingCart _cartService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        public PaymentService(IUnitOfWork unitOfWork, IShoppingCart cartService, IConfiguration config)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _cartService = cartService;

        }
        public async Task<ShoppingCart> CreateOrUpdate(string cartId)
        {
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
            var cart = await _cartService.GetCartAsync(cartId);
            if(cart == null) return null;
            var shippingPrice = 0m;
            if(cart.DeliveryId.HasValue)
            {
                var delieveryId = await _unitOfWork.Repository<Delievery>().GetFirstOrDefault(x => x.DeliveryId == cart.DeliveryId);
                shippingPrice = delieveryId.Price;
            }
            foreach (var item in cart.CartItems)
            {
                var products = await _unitOfWork.Repository<Product>().GetFirstOrDefault(x => x.Id == item.CartItemsId);
                if(item.Price != products.Price)
                {
                    item.Price = products.Price;
                }
            }

            var service = new PaymentIntentService();
            PaymentIntent intent;
            if(string.IsNullOrEmpty(cart.PaymentID))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long) cart.CartItems.Sum(z => z.Amount * (z.Price * 100)) + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>{"card"}   
                };
                intent = await service.CreateAsync(options);
                cart.PaymentID = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long) cart.CartItems.Sum(z => z.Amount * (z.Price * 100)) + (long)shippingPrice * 100,
                };
                await service.UpdateAsync(cart.PaymentID,options);
            }
            await _cartService.UpdateCartAsync(cart);

            return cart;
        }

        public async Task<Orders> UpdatePaymentFailed(string PaymentId)
        {
            var Order = await _unitOfWork.Repository<Orders>().GetFirstOrDefault(x => x.PaymentID == PaymentId);
            if(Order == null) return null;
            Order.Status = OrderStatus.PaymentFailed;
            _unitOfWork.Repository<Orders>().Update(Order);
            return Order;
        }

        public async Task<Orders> UpdatePaymentSucceeded(string PaymentId)
        {
            var Order = await _unitOfWork.Repository<Orders>().GetFirstOrDefault(x => x.PaymentID == PaymentId);
            if(Order == null) return null;
            Order.Status = OrderStatus.PaymentRecevied;
            _unitOfWork.Repository<Orders>().Update(Order);
            await _unitOfWork.Complete();
            return Order;
        }
    }
}