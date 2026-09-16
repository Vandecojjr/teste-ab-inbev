using AutoMapper; 

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale; 

public class CancelSaleProfile : Profile 
{ 
    public CancelSaleProfile() 
    { 
        CreateMap<Ambev.DeveloperEvaluation.Application.Sales.CancelSale.CancelSaleResult, CancelSaleResponse>();
    } 
}