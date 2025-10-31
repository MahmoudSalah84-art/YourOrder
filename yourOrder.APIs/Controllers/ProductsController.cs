using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yourOrder.APIs.DTOs.ProductDto;
using yourOrder.APIs.Errors;
using yourOrder.APIs.Helpers;
using yourOrder.Core.Entity.ProductAggregate;
using yourOrder.Core.Interfaces;
using yourOrder.Core.Specifications.ProductSpecification;

namespace yourOrder.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;


        private readonly IMapper _mapper;
        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper , IWebHostEnvironment env) => (_unitOfWork,_mapper , _env) = (unitOfWork, mapper , env);


        [HttpGet] // GET: api/products
        [Cached(600)]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductParams productParams)
        {
            // Spec for getting the count
            var countSpec = new ProductWithFilterForCountSpecification(productParams);
            var totalItems = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);
            // Spec for getting the products with pagination
            var spec = new ProductWithBrandAndTypeSpecification(productParams); 
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpec(spec); 
            // Map products to ProductToReturnDto
            var data = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductToReturnDto>>(products); // Map to DTOs

            var pagination = new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data);
            return Ok( pagination);

        }


        [HttpGet("{id}")] // GET: api/products/1
        [Cached(600)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecification(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec); 
            if (product == null)
            {
                // If not found, return our custom 404 response
                return NotFound(new ApiResponse(404));
            }
            var data = _mapper.Map<Product,ProductToReturnDto>(product);
            return Ok(data);
        }





        [HttpPost("{id}/picture")]
        [Authorize("admin")]
        public async Task<ActionResult<ProductToReturnDto>> UploadProductImage(int id, IFormFile file)
        {
            var spec = new ProductWithBrandAndTypeSpecification(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);
            if (product == null) return NotFound(new ApiResponse(404));

            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiResponse(400, "Please select an image file."));
            }

            var staticFilesPath = _env.WebRootPath;
            var imagesPath = Path.Combine(staticFilesPath/*wwwroot*/, "images", "products");

            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var fullFilePath = Path.Combine(imagesPath, uniqueFileName);


            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //save file path in database
            product.PictureUrl = $"images/products/{uniqueFileName}";

            await _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();

            var data = _mapper.Map<Product, ProductToReturnDto>(product);
            return Ok(data);
        }
    }
}



