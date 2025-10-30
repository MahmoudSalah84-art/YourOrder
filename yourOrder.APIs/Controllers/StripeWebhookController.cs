using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using yourOrder.Core.Services;
using yourOrder.Services;

namespace yourOrder.APIs.Controllers
{
    [Route("api/webhook/stripe")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly StripeSettings _stripe;
        private readonly ILogger<StripeWebhookController> _logger;
        public StripeWebhookController(IPaymentService paymentService, IOptions<StripeSettings> stripe, ILogger<StripeWebhookController> logger)
        {
            _paymentService = paymentService;
            _stripe = stripe.Value;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], _stripe.WebhookSecret);

                PaymentIntent intent;

                switch (stripeEvent.Type)
                {
                    case EventTypes.PaymentIntentSucceeded:
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        _logger.LogInformation("Payment Succeeded: ", intent.Id);
                        await _paymentService.UpdateOrderStatusOnSuccess(intent.Id);
                    break;

                    case EventTypes.PaymentIntentPaymentFailed:
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        _logger.LogInformation("Payment Failed: ", intent.Id);
                        await _paymentService.UpdateOrderStatusOnFailure(intent.Id);
                    break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
