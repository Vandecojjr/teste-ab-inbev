using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        CreateMap<Sale, GetSaleResult>()
            .ForMember(dest => dest.TotalSaleAmount, opt => opt.MapFrom(src => src.TotalSaleAmount.Value));

        CreateMap<SaleItem, GetSaleItemResult>()
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity.Value))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Value))
            .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.Discount.Percentage))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Value));
    }
}
