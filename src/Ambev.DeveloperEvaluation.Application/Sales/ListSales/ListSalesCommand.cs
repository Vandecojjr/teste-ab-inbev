using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Command for retrieving a paginated list of sales.
/// </summary>
public class ListSalesCommand : IRequest<Ambev.DeveloperEvaluation.Common.Pagination.PaginatedList<Ambev.DeveloperEvaluation.Application.Sales.GetSale.GetSaleResult>>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}
