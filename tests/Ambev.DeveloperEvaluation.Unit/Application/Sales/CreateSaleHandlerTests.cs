using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "Given valid sale command When handling Then creates sale and returns result")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-123",
            CustomerId = 1,
            CustomerName = "Test Customer",
            BranchId = 1,
            BranchName = "Test Branch",
            Items = new List<CreateSaleItemCommand>
            {
                new()
                {
                    ProductId = 1,
                    ProductName = "Product 1",
                    Quantity = 2,
                    UnitPrice = 100m
                }
            }
        };

        var saleEntity = new Sale(command.SaleNumber, 
            new Ambev.DeveloperEvaluation.Domain.ValueObjects.CustomerInfo(command.CustomerId, command.CustomerName), 
            new Ambev.DeveloperEvaluation.Domain.ValueObjects.BranchInfo(command.BranchId, command.BranchName));
        var expectedResult = new CreateSaleResult { Id = saleEntity.Id, SaleNumber = saleEntity.SaleNumber, TotalSaleAmount = 200m };

        _mapper.Map<Sale>(command).Returns(saleEntity);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(saleEntity);
        _mapper.Map<CreateSaleResult>(saleEntity).Returns(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(saleEntity.Id);
        result.TotalSaleAmount.Should().Be(200m);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid sale command When handling Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var command = new CreateSaleCommand();
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
