using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelSaleHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "Given valid id When handling Then cancels sale and returns success")]
    public async Task Handle_ValidRequest_CancelsSale()
    {
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var existingSale = new Sale("SALE-123", new Ambev.DeveloperEvaluation.Domain.ValueObjects.CustomerInfo(1, "Customer"), new Ambev.DeveloperEvaluation.Domain.ValueObjects.BranchInfo(1, "Branch"));

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        existingSale.IsCancelled.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(existingSale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsException()
    {
        var command = new CancelSaleCommand(Guid.NewGuid());
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given empty id When handling Then throws validation exception")]
    public async Task Handle_EmptyId_ThrowsValidationException()
    {
        var command = new CancelSaleCommand(Guid.Empty);
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
