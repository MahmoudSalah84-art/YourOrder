using Microsoft.AspNetCore.Mvc;
using yourOrder.Core.Entity;
using yourOrder.Core.Interfaces;

namespace yourOrder.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productRepo;

        public ProductsController(IGenericRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        [HttpGet] // GET: api/products
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
        {
            var products = await _productRepo.ListAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")] // GET: api/products/1
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            return Ok(product);
        }


    }
}
