using System.Threading.Tasks;
using Models;
using Models.CreateOrder;

namespace Dreaming.Repositories
{
  
    public interface IOrderRepository
    {
          Task<ActualOrder> CreateOrder(string Email, Address address, int delivery,string cartId);
    }

}