using AutoMapper;
using yourOrder.APIs.DTOs;
using yourOrder.Core.Entity;
using yourOrder.Core.Entity.Identity;

namespace yourOrder.APIs.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            // from left to right
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.productBrand.Name))
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.productType.Name));
            
            CreateMap<BasketItem, BasketItemDto>();
            CreateMap<CustomerBasket, CustomerBasketDto>();
            CreateMap<BasketItem, BasketItemDto>().ReverseMap();
            CreateMap<CustomerBasket, CustomerBasketDto>().ReverseMap();

            CreateMap<RegisterDto, AppUser>()
                .ForMember(d => d.Addresse, o => o.MapFrom(s => new Address
                {
                    Country = s.Country,
                    City = s.City,
                    Street = s.Street
                }));




            CreateMap<AppUser, UserDto>();
                





        }
    }
}
