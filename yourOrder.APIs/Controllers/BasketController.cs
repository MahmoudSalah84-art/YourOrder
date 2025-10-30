using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using yourOrder.APIs.DTOs.BasketDto;
using yourOrder.APIs.Errors;
using yourOrder.Core.Entity.BasketAggregate;
using yourOrder.Core.Interfaces;

namespace yourOrder.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly ICachingService _cachingService;
        private readonly IMapper _mapper;
        public BasketController(ICachingService cachingService, IMapper mapper) => (_cachingService, _mapper) = (cachingService, mapper);
        

        [HttpGet] // Get: api/basket/basket1
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _cachingService.RemoveCacheAsync(id);
            return (basket is false) ? NotFound(new ApiResponse(404)) : Ok(basket);
        }
        

        [HttpPost] // Post: api/basket       with body
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basketDto)
        {
            var Basket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basketDto);

            var CreatedOrUpdatedBasket = await _cachingService.SetCacheResponseAsync(Basket.Id, Basket, TimeSpan.FromMinutes(30));

            return (CreatedOrUpdatedBasket is false) ? BadRequest(new ApiResponse(400)) : Ok(CreatedOrUpdatedBasket);//return id or badrequest
        }


        [HttpDelete("{id}")] // Delete: api/basket/basket1
        public async Task<ActionResult> DeleteBasket(string id)
        {
            bool result = await _cachingService.RemoveCacheAsync(id);
            return result is true ? Ok() : BadRequest(new ApiResponse(400));
        }

        
    }
}

