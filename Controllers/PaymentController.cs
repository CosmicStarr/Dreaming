using System.IO;
using System.Threading.Tasks;
using Dreaming.Errors;
using Dreaming.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.Order;
using Models.ShoppingCart;
using Stripe;

namespace Dreaming.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private const string webHookSecret = " whsec_rlyDuVKc05PHlO5yFcYf6Bz1hN63R9AQ";
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [HttpPost("{CartID}")]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePayment(string CartID)
        {
            var Cart = await _paymentService.CreateOrUpdate(CartID);
            if (Cart == null) return BadRequest(new ApiErrorResponse(400, "Your Cart Does Not Exist!"));
            return Ok(Cart);
        }

        [HttpPost("webHook")]
        public async Task<ActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], webHookSecret);
            PaymentIntent intent;
            Orders orders;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeeded:",intent.Id);
                    orders = await _paymentService.UpdatePaymentSucceeded(intent.Id);
                    _logger.LogInformation("Order Updated Recevied Success!", orders.ActualId);
                break;
                case "payment_intent.payment_failed":
                     intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed:",intent.Id);
                    orders = await _paymentService.UpdatePaymentFailed(intent.Id);
                     _logger.LogInformation("Order Updated Recevied Failed!", orders.ActualId);
                break;
            }

            return new EmptyResult();
        }
    }
}