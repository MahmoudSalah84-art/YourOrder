using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.BasketAggregate;

namespace yourOrder.Core.Services
{
    public interface IPaymentService
    {
        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId);
        Task UpdateOrderStatusOnFailure(string id);
        Task UpdateOrderStatusOnSuccess(string id);
    }
}
