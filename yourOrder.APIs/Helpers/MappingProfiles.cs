using AutoMapper;
using yourOrder.APIs.DTOs.Account;
using yourOrder.APIs.DTOs.BasketDto;
using yourOrder.APIs.DTOs.ProductDto;
using yourOrder.Core.Entity.BasketAggregate;
using yourOrder.Core.Entity.Identity;
using yourOrder.Core.Entity.OrderAggregate;
using yourOrder.Core.Entity.ProductAggregate;

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
                .ForMember(d => d.Address, o => o.MapFrom(s => new Address
                {
                    Country = s.Country,
                    City = s.City,
                    Street = s.Street
                }));


            CreateMap<AddressDto , OrderAddress>();

            CreateMap<AppUser, UserDto>();
                





        }
    }
}
