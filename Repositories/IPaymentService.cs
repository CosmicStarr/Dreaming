using System.Threading.Tasks;
using Models.Order;
using Models.ShoppingCart;

namespace Dreaming.Repositories
{
    public interface IPaymentService
    {
        Task<ShoppingCart> CreateOrUpdate(string CartId);
        Task<Orders> UpdatePaymentSucceeded(string PaymentId);
        Task<Orders> UpdatePaymentFailed(string PaymentId);
        
    }
}