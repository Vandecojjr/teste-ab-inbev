using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new UpdateSaleHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "Given valid command When handling Then updates sale and returns result")]
    public async Task Handle_ValidRequest_UpdatesSale()
    {
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand
        {
            Id = saleId,
            SaleNumber = "SALE-999",
            CustomerId = 1,
            CustomerName = "New Customer",
            BranchId = 1,
            BranchName = "New Branch"
        };

        var existingSale = new Sale("SALE-123", new Ambev.DeveloperEvaluation.Domain.ValueObjects.CustomerInfo(1, "Old Customer"), new Ambev.DeveloperEvaluation.Domain.ValueObjects.BranchInfo(1, "Old Branch"));

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        existingSale.SaleNumber.Should().Be("SALE-999");
        existingSale.Customer.Name.Should().Be("New Customer");
        await _saleRepository.Received(1).UpdateAsync(existingSale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsException()
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "SALE-999",
            CustomerName = "New Customer",
            BranchName = "New Branch"
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);
        
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws validation exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        var command = new UpdateSaleCommand { Id = Guid.NewGuid() };
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
