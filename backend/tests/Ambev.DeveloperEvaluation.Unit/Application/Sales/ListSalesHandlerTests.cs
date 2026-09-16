using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid command When handling Then returns paginated result")]
    public async Task Handle_ValidRequest_ReturnsPaginatedResult()
    {
        var command = new ListSalesCommand { Page = 1, Size = 10 };
        var saleEntity = new Sale("SALE-123", new CustomerInfo(1, "Customer"), new BranchInfo(1, "Branch"));
        var salesList = new List<Sale> { saleEntity };

        var mappedResult = new List<GetSaleResult>
        {
            new GetSaleResult { Id = saleEntity.Id, SaleNumber = "SALE-123" }
        };

        var queryable = salesList.AsQueryable();
        _saleRepository.GetQueryable().Returns(queryable);
        
        var paginatedSales = new PaginatedList<Sale>(salesList, 1, 1, 10);
        _mapper.Map<List<GetSaleResult>>(Arg.Any<PaginatedList<Sale>>()).Returns(mappedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);
        
        _saleRepository.Received(1).GetQueryable();
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var command = new ListSalesCommand { Page = 0, Size = -1 }; // Invalid values
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
