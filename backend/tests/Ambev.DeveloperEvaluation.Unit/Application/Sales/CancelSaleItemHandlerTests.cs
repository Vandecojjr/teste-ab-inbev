using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelSaleItemHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "Given valid data When cancelling item Then recalculates total and updates sale")]
    public async Task Handle_ValidRequest_CancelsItemAndUpdates()
    {
        var saleId = Guid.NewGuid();
        var saleEntity = new Sale("SALE-123", new CustomerInfo(1, "Customer"), new BranchInfo(1, "Branch"));
        
        saleEntity.AddItem(new ProductInfo(1, "Product A"), 2, 50m); // total 100
        var item = saleEntity.Items.First();
        item.Id = Guid.NewGuid();
        var itemId = item.Id;

        var command = new CancelSaleItemCommand(saleId, itemId);
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(saleEntity);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.NewTotalSaleAmount.Should().Be(0m); // all items cancelled

        saleEntity.Items.First().IsCancelled.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(saleEntity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid sale id When handling Then throws KeyNotFoundException")]
    public async Task Handle_InvalidSaleId_ThrowsException()
    {
        var command = new CancelSaleItemCommand(Guid.NewGuid(), Guid.NewGuid());
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given empty command When handling Then throws validation exception")]
    public async Task Handle_EmptyCommand_ThrowsValidationException()
    {
        var command = new CancelSaleItemCommand(Guid.Empty, Guid.Empty);
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
