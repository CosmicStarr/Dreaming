
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dreaming.Extensions;
using Dreaming.Helper;
using DreamMore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;

namespace Dreaming.Controllers
{
    // [Authorize]
    public class ProductsController : BaseController
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<ProductsDTO>> GetProduct(int Id)
        {
            var obj = await _unitOfWork.Repository<Products>().GetFirstOrDefault(x => x.Id == Id, "Category,Brand");
            var data = _mapper.Map<Products,ProductsDTO>(obj);
            if (data == null)
            {
                return BadRequest();
            }
            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductsDTO>>> GetAllProducts([FromQuery] Parms Parms)
        {   
            var data = await _unitOfWork.Repository<Products>().GetAll(Parms, null,orderby:x => x.OrderBy(x=>x.ProductName),"Category,Brand");
            Response.AddPaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            var nuData = _mapper.Map<PagerList<Products>,PagerList<ProductsDTO>>(data);
            return Ok(nuData);
        }


        [HttpGet("Category")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories()
        {
            var Catdata = await _unitOfWork.Repository<Category>().GetAllAsync();
            var data = _mapper.Map<IEnumerable<Category>,IEnumerable<CategoryDTO>>(Catdata);
            return Ok(data);
        }

        [HttpGet("Brand")]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetBrands()
        {
            var Branddata = await _unitOfWork.Repository<Brand>().GetAllAsync();
            var data = _mapper.Map<IEnumerable<Brand>,IEnumerable<BrandDTO>>(Branddata);
            return Ok(data);
        }
    }
}