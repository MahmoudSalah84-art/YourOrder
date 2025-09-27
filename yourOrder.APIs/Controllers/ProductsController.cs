using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using yourOrder.APIs.DTOs;
using yourOrder.Core.Entity;
using yourOrder.Core.Interfaces;

namespace yourOrder.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IMapper _mapper; // Inject IMapper

        public ProductsController(IGenericRepository<Product> productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
        }

        [HttpGet] // GET: api/products
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
        {
            var products = await _productRepo.ListAllAsync();
            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
            return Ok(data);

        }

        [HttpGet("{id}")] // GET: api/products/1
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            var data = _mapper.Map<Product,ProductToReturnDto>(product);
            return Ok(data);
        }

        

        
    }
}
