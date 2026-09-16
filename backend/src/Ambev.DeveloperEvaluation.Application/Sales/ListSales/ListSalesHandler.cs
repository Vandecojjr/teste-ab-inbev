using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesCommand, PaginatedList<GetSaleResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GetSaleResult>> Handle(ListSalesCommand request, CancellationToken cancellationToken)
    {
        var validator = new ListSalesValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var query = _saleRepository.GetQueryable().OrderByDescending(s => s.SaleDate);

        var paginatedSales = await PaginatedList<Domain.Entities.Sale>.CreateAsync(query, request.Page, request.Size);
        var mappedSales = _mapper.Map<List<GetSaleResult>>(paginatedSales);

        return new PaginatedList<GetSaleResult>(
            mappedSales, 
            paginatedSales.TotalCount, 
            paginatedSales.CurrentPage, 
            paginatedSales.PageSize);
    }
}
