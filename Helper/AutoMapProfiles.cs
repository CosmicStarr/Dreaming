using AutoMapper;
using Models;
using Models.DTOs;
using Models.Order;
using Models.ShoppingCart;

namespace Dreaming.Helper
{
    public class AutoMapProfiles:Profile
    {
        public AutoMapProfiles()
        {
            CreateMap<ShoppingCart,ShoppingCartDTO>();
            CreateMap<CartItems,CartItemsDTO>();
            CreateMap<OrderItems, OrderItemDTO>()
                .ForMember(d => d.ItemId, o=>o.MapFrom(s => s.ItemsOrdered.id))
                .ForMember(d => d.Name, o=>o.MapFrom(s => s.ItemsOrdered.ProductItemName))
                .ForMember(d => d.ImageUrl, o=>o.MapFrom(s => s.ItemsOrdered.ItemImageUrl));
            CreateMap<Orders,ActualOrderDTO>().ForMember(d => d.Deliveries,m => m.MapFrom(s =>s.Deliveries.DeliveryId))
                                              .ForMember(d => d.Total,m =>m.MapFrom(s => s.GetTotal()))
                                              .ForMember(d => d.BuyerEmail,m =>m.MapFrom(s =>s.Email));                                        
            CreateMap<Address,AddressDTO>().ReverseMap().ForMember(d => d.AddressId,m => m.MapFrom(s =>s.AddressId));
            CreateMap<Brand,BrandDTO>().ForMember(p =>p.BrandId,m =>m.MapFrom(s =>s.Id));
            CreateMap<ApplicationUser,ActualOrderDTO>().ForMember(p => p.BuyerEmail,m => m.MapFrom(s => s.Email));
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