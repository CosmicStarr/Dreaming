using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dreaming.Errors;
using Dreaming.Extensions;
using Dreaming.Repositories;
using DreamMore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.CreateOrder;
using Models.DTOs;

namespace Dreaming.Controllers
{
    // [Authorize]
    public class OrderController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IOrderRepository repository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repository = repository;
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<OrderForClientToSeeDTO>> GetOrderByIdAsync(int id, string Email = null)
        {
           
            Email = HttpContext.User.RetrieveEmail();
             var orderObj = await _unitOfWork.Repository<ActualOrder>().Get(id);
            if(orderObj == null) return NotFound(new ApiErrorResponse(404,"Your order does not exist!"));
            var mapNew = _mapper.Map<ActualOrder,OrderForClientToSeeDTO>(orderObj);
            return Ok(mapNew);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderForClientToSeeDTO>>> GetOrdersAsync()
        {
            var Email = HttpContext.User.RetrieveEmail();
            var orders = await _unitOfWork.Repository<ActualOrder>().GetAllAsync(x => x.Email == Email,null,"Delivery,ShippingAddress,OrderedItems");
            var mapAgain = _mapper.Map<IEnumerable<ActualOrder>,IEnumerable<OrderForClientToSeeDTO>>(orders);
            return Ok(mapAgain);
        }

        [HttpPost]
        public async Task<ActionResult<ActualOrder>> CreateOrder(ActualOrderDTO actualOrderDTO)
        {
            var CartItems = new CartItems();
            var Email = HttpContext.User.RetrieveEmail();
            var Address = _mapper.Map<AddressDTO, Address>(actualOrderDTO.ShippingAddressDTO);
            var Order = await _repository.CreateOrder(Email, Address, actualOrderDTO.DeliveryId,actualOrderDTO.CartId);
            if (Order == null) return BadRequest();
            return Ok(Order);

        }
    }
}