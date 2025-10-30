using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.Identity;
using yourOrder.Core.Entity.OrderAggregate;
using yourOrder.Core.Entity.ProductAggregate;
using yourOrder.Core.Interfaces;
using yourOrder.Core.Services;
using yourOrder.Core.Specifications.OrderSpecification;
using yourOrder.Infrastructure.Repositories;

namespace yourOrder.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork , IPaymentService paymentService) => (_basketRepo , _unitOfWork , _paymentService) = (basketRepository , unitOfWork , paymentService);

        
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, OrderAddress shipToAddress)
        {
            
            var basket = await _basketRepo.GetBasketAsync(basketId);

            // نتحقق إذا كان هناك طلب تم إنشاؤه بالفعل بنفس نية الدفع
            var spec = new OrderWithPaymentIntentSpecification(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetByIdWithSpec(spec);

            if (existingOrder != null)
                await _unitOfWork.Repository<Order>().Delete(existingOrder.Id);


            // product == BasketItem
            // 2. Loop over basket items to get product details from SQL DB
            var orderItems = new List<OrderItem>(); // productItemOrder,price,Quantity 
            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                orderItems.Add(orderItem);
            }

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

            var order = new Order(buyerEmail, shipToAddress, deliveryMethod, orderItems, subtotal);

            _ = _unitOfWork.Repository<Order>().Add(order);


            var result = await _unitOfWork.CompleteAsync();

            await _basketRepo.DeleteBasketAsync(basketId);


            return (result <= 0) ? null : order;

        }

        public async Task<Order> GetOrderByIdForUser(int orderId, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndDeliveryMethodsSpecification(orderId, buyerEmail);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpec(spec);
            return order;
        }


        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndDeliveryMethodsSpecification(buyerEmail);
            var orders = (IReadOnlyList<Order>)await _unitOfWork.Repository<Order>().GetAllWithSpec(spec);
            return orders;
        }


        

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
            => (IReadOnlyList<DeliveryMethod>) await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();

        
    }
}
