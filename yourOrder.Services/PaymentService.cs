using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.BasketAggregate;
using yourOrder.Core.Entity.OrderAggregate;
using yourOrder.Core.Interfaces;
using yourOrder.Core.Services;
using yourOrder.Core.Specifications.OrderSpecification;

namespace yourOrder.Services
{
    public class PaymentService : IPaymentService
    {

        private readonly ICachingService _cachingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeSettings _stripe;
        public PaymentService(IOptionsSnapshot<StripeSettings> stripe, ICachingService cachingService, IUnitOfWork unitOfWork)
        {
            _stripe = stripe.Value;
            _cachingService = cachingService;
            _unitOfWork = unitOfWork;
        }


        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _stripe.SecretKey;
            var basket = await _cachingService.GetCachedResponseAsync<CustomerBasket>(basketId);
            if (basket == null) return null;

            decimal subtotal = basket.Items.Sum(item => item.Price * item.Quantity);

            if (!basket.DeliveryMethodId.HasValue)
                throw new Exception("Cannot create payment intent without a delivery method.");
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);

            long totalAmount = (long)(subtotal + deliveryMethod.Price) * 100; // Stripe uses smallest currency unit (cents)

            var service = new PaymentIntentService();
            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                // Create a new Payment Intent
                var options = new PaymentIntentCreateOptions
                {
                    Amount = totalAmount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                // Update existing Payment Intent
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = totalAmount
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            await _cachingService.SetCacheResponseAsync(basket.Id,basket, TimeSpan.FromSeconds(30));
            return basket;
        }


        public async Task UpdateOrderStatusOnSuccess(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpec(spec);

            if (order == null) return; // Order not found, log it

            order.Status = OrderStatus.PaymentReceived;
            await _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();
        }


        public async Task UpdateOrderStatusOnFailure(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpec(spec);

            if (order == null) return;

            order.Status = OrderStatus.PaymentFailed;
            await _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();
        }
    }
}
