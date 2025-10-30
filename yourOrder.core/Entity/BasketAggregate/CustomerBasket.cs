using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yourOrder.Core.Entity.BasketAggregate
{
    public class CustomerBasket
    {
        // Id here is the key in Redis (email of user)
        public string Id { get; set; }
        // list of items in the basket
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }
        public CustomerBasket(string id)
        {
            Id = id;
        }
    }
}
