using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using yourOrder.APIs.DTOs;
using yourOrder.Core.Entity;
using yourOrder.Core.Interfaces;
using yourOrder.Core.Specifications;

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
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts([FromQuery] ProductParams productParams)
        {
            var spec = new ProductWithBrandAndTypeSpecification(productParams); // Create specification instance

            var products = await _productRepo.GetAllWithSpec(spec); // Use the new method

            var data = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductToReturnDto>>(products); // Map to DTOs

            return Ok(data);

        }

        [HttpGet("{id}")] // GET: api/products/1
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecification(id);
            var product = await _productRepo.GetByIdWithSpec(spec); // Use the new method
            
            var data = _mapper.Map<Product,ProductToReturnDto>(product);
            return Ok(data);
        }
        

        


    }
}
