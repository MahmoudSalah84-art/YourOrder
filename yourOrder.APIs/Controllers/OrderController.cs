using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using yourOrder.APIs.DTOs.Account;
using yourOrder.APIs.DTOs.OrderAggregateDto;
using yourOrder.APIs.Errors;
using yourOrder.Core.Entity.OrderAggregate;
using yourOrder.Core.Services;
using yourOrder.Services;

namespace yourOrder.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        public OrderController(IOrderService orderService, IMapper mapper) => (_orderService, _mapper) = (orderService, mapper);

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orderAddress = _mapper.Map<AddressDto, OrderAddress>(orderDto.ShipToAddress);

            var order = await _orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, orderAddress);

            return (order == null) ? BadRequest(new ApiResponse(400, "An Error Occured During Creating the order")) : Ok(order);
        }

        [HttpGet("userOrders")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var orders = await _orderService.GetOrdersForUserAsync(buyerEmail);

            return (orders != null) ? Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders))
              : BadRequest(new ApiResponse(400, "The Current user hasn't confimed any orders yet!"));
               
        }

        [HttpGet("userOrders/{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdForUser(id, buyerEmail);

            return (order != null) ? Ok(_mapper.Map<Order, OrderToReturnDto>(order))
                : NotFound(new ApiResponse(404, "Order not found"));
        }

        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliverMethods()
        {
            return Ok(await _orderService.GetDeliveryMethodAsync());
        }
    }
}
