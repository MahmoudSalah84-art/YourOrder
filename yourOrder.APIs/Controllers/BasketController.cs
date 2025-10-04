using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using yourOrder.APIs.DTOs;
using yourOrder.APIs.Errors;
using yourOrder.Core.Entity;
using yourOrder.Core.Interfaces;

namespace yourOrder.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        [HttpGet] // Get: api/basket/basket1
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            return (basket is null) ? NotFound(new ApiResponse(404)) : Ok(basket);
        }
        
        [HttpPost] // Post: api/basket with body
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);
            return (CreatedOrUpdatedBasket is null) ? BadRequest(new ApiResponse(400)) : Ok(CreatedOrUpdatedBasket);//return id or badrequest
        }
        [HttpDelete("{id}")] // Delete: api/basket/basket1
        public async Task<ActionResult> DeleteBasket(string id)
        {
            bool result = await _basketRepository.DeleteBasketAsync(id);
            return result is true ? Ok() : BadRequest(new ApiResponse(400));
        }

        // Alternative Delete method without route parameter and returning boolean
        //[HttpDelete] // Delete: api/basket/basket1
        //public async Task<ActionResult<bool>> DeleteBasket(string BasketId)
        //{
        //    return await _basketRepository.DeleteBasketAsync(BasketId);
        //}
    }
}

