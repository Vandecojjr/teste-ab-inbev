using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Sale>()
            .ConstructUsing(cmd => new Sale(
                cmd.SaleNumber, 
                new CustomerInfo(cmd.CustomerId, cmd.CustomerName), 
                new BranchInfo(cmd.BranchId, cmd.BranchName)))
            .ForMember(dest => dest.Items, opt => opt.Ignore());

        CreateMap<Sale, CreateSaleResult>()
            .ForMember(dest => dest.TotalSaleAmount, opt => opt.MapFrom(src => src.TotalSaleAmount.Value));
    }
}
