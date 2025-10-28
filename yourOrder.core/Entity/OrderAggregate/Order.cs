using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.Identity;

namespace yourOrder.Core.Entity.OrderAggregate
{
    public class Order :BaseEntity
    {
        public Order() { }
        public Order(string buyerEmail, OrderAddress shippingAddress, DeliveryMethod deliveryMethod, List<OrderItem> orderItems, decimal subtotal)
        {
            BuyerEmail = buyerEmail;
            ShipToAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = orderItems;
            Subtotal = subtotal;
        }
        public OrderStatus Status { get; set; } = OrderStatus.Pending; //until paymenting
        public IReadOnlyList<OrderItem> OrderItems { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public OrderAddress ShipToAddress { get; set; }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public decimal Subtotal { get; set; }
        public string PaymentIntentId { get; set; }

        public decimal GetTotal() => Subtotal + DeliveryMethod.Price;
    }
}
