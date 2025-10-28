using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.OrderAggregate;

namespace yourOrder.Core.Specifications.OrderSpecification
{
    public class OrdersWithItemsAndDeliveryMethodsSpecification : BaseSpecifications<Order>
    {
        // this constructor is used to get All orders for a specific User
        public OrdersWithItemsAndDeliveryMethodsSpecification(string buyerEmail) : base(o => o.BuyerEmail == buyerEmail)
        {
            Includes.Add(o => o.OrderItems);
            Includes.Add(o => o.DeliveryMethod);
            AddOrderByDescending(o => o.OrderDate);
        }
        // this constructor is used to get An order for a specific User
        public OrdersWithItemsAndDeliveryMethodsSpecification(int orderId, string buyerEmail)
            : base(o => (o.BuyerEmail == buyerEmail && o.Id == orderId))
        {
            Includes.Add(o => o.OrderItems);
            Includes.Add(o => o.DeliveryMethod);
        }
    }
}
