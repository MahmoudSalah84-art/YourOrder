using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.OrderAggregate;

namespace yourOrder.Core.Specifications.OrderSpecification
{
    public class OrderWithPaymentIntentSpecification :  BaseSpecifications<Order>
    {
        public OrderWithPaymentIntentSpecification(string PaymentIntentId) : base(o => o.PaymentIntentId == PaymentIntentId)
        {

        }
    }
}
