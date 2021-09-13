using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Models.Order;

namespace Dreaming.Repositories
{
  
    public interface IOrderRepository
    {
         Task<Orders> CreateOrder(string BuyerEmail, int DeliveryID, string CartID, Address orderAddress);
         Task<IEnumerable<Delievery>> GetSpecialDeliveries();
    }
}