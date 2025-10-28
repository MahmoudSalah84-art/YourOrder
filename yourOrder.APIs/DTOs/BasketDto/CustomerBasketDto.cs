namespace yourOrder.APIs.DTOs.BasketDto
{
    public class CustomerBasketDto
    {
        public string id {  get; set; }
        public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }


    }
}
