using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dreaming.Extensions;
using Dreaming.Repositories;
using DreamMore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models.Order;

namespace Dreaming.Controllers
{
     
    public class OrdersController:BaseController
    {
        private readonly IOrderRepository _order;
        private readonly IMapper _mapper; 
        private readonly IUnitOfWork _unit;
        public OrdersController(IUnitOfWork unitOfWork, IMapper mapper, IOrderRepository Order)
        {
            _mapper = mapper;
            _order = Order;
            _unit = unitOfWork;
        }

        [HttpGet("delivery")]
        public async Task<IEnumerable<Delievery>> GetDelieveries()
        {
            return await _order.GetSpecialDeliveries();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActualOrderDTO>>> GetOrdersForUser()
        {
            var Email = HttpContext.User.RetrieveEmail();
            var Order = await _unit.Repository<Orders>().GetAllAsync(x => x.Email == Email
            ,x =>x.OrderBy(x =>x.Deliveries),"ShippingAddress,Deliveries,ItemsOrders");
            return Ok(_mapper.Map<IEnumerable<Orders>, IEnumerable<ActualOrderDTO>>(Order));
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ActualOrderDTO>> GetSingleOrderbyID(int Id)
        {
            var Email = HttpContext.User.RetrieveEmail();
            var SingleOrder = await _unit.Repository<Orders>().Get(Id);
            if (SingleOrder == null) return NotFound();
            return _mapper.Map<Orders, ActualOrderDTO>(SingleOrder);
        }

        [HttpPost]
        public async Task<ActionResult<Orders>> CreateOrder(OrderDTO orderDTO)
        {
            var Email = HttpContext.User.RetrieveEmail();
            var Address = _mapper.Map<AddressDTO, Address>(orderDTO.ShiptoAddress);
            var NewOrder = await _order.CreateOrder(Email,orderDTO.SpecialDeliveryID,orderDTO.CartId, Address);
            if (NewOrder == null) return BadRequest();
            return Ok(NewOrder);
        }
    }
}