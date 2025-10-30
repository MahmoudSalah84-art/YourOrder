using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yourOrder.APIs.Errors;
using yourOrder.Core.Entity.BasketAggregate;
using yourOrder.Core.Services;

namespace yourOrder.APIs.Controllers
{
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        public readonly IPaymentService _paymentService;
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {

            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            return (basket == null) ? BadRequest(new ApiResponse(400, "Problem with your basket")) : Ok(basket);
        }
    }
}
