using yourOrder.APIs.DTOs.Account;

namespace yourOrder.APIs.DTOs.OrderAggregateDto
{
    public class OrderDto
    {
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; }
        public AddressDto ShipToAddress { get; set; }
    }
}
