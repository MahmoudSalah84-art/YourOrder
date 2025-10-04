using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yourOrder.Core.Entity
{
    public class CustomerBasket
    {
        // Id here is the key in Redis
        public string Id { get; set; }
        // list of items in the basket
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    }
}
