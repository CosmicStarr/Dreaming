using AutoMapper;
using Models;
using Models.CreateOrder;
using Models.DTOs;

namespace Dreaming.Helper
{
    public class AutoMapProfiles:Profile
    {
        public AutoMapProfiles()
        {
            CreateMap<OrderedItem,OrderItemDTO>().ForMember(d =>d.ItemId,m => m.MapFrom(s => s.Id))
                                                 .ForMember(d =>d.Name,m =>m.MapFrom(s => s.ItemsOrdered.Name))
                                                 .ForMember(d =>d.ImageUrl,m =>m.MapFrom(s => s.ItemsOrdered.ImageUrl));                                            
            CreateMap<ActualOrder,OrderForClientToSeeDTO>().ForMember(d => d.DeliveryName,m =>m.MapFrom(s =>s.Delivery.Name))
                                                           .ForMember(d => d.DeliveryPrice,m =>m.MapFrom(s =>s.Delivery.price))
                                                           .ForMember(d => d.ID,m =>m.MapFrom(s =>s.ActualId))
                                                           .ForMember(d => d.Total,m =>m.MapFrom(s =>s.GetTotal()));                                                     
            CreateMap<Address,AddressDTO>().ReverseMap().ForMember(p => p.Id,m => m.MapFrom(s => s.AddressId));
            CreateMap<Brand,BrandDTO>().ForMember(p =>p.BrandId,m =>m.MapFrom(s =>s.Id));
            CreateMap<ApplicationUser,LoginModelDTO>().ForMember(p => p.Email,m => m.MapFrom(s => s.Email));
            CreateMap<LoginModel,LoginModelDTO>();
            CreateMap<RegisterModel,RegisterModelDTO>();
            CreateMap<Products,ProductsDTO>().ForMember(p =>p.ProductId,m => m.MapFrom(s => s.Id))
                                             .ForMember(p => p.Category,m =>m.MapFrom(c => c.Category.CatName))   
                                             .ForMember(p =>p.Brand,m =>m.MapFrom(b => b.Brand.BrandName))
                                             .ForMember(p => p.ImageUrl,m =>m.MapFrom(c => c.ImageUrl));
            CreateMap<Category,CategoryDTO>().ForMember(p =>p.CategoryId,m =>m.MapFrom(c => c.CatId));
                                             
        }
    }
}