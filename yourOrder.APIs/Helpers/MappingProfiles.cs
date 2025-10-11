using AutoMapper;
using yourOrder.APIs.DTOs;
using yourOrder.Core.Entity;

namespace yourOrder.APIs.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.productBrand.Name))
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.productType.Name));
            
            CreateMap<BasketItem, BasketItemDto>();
            CreateMap<CustomerBasket, CustomerBasketDto>();
            CreateMap<BasketItem, BasketItemDto>().ReverseMap();
            CreateMap<CustomerBasket, CustomerBasketDto>().ReverseMap();

            
           
        }
    }
}
