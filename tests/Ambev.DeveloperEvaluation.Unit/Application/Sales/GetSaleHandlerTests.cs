using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid id When handling Then returns sale result")]
    public async Task Handle_ValidRequest_ReturnsSaleResult()
    {
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);
        
        var saleEntity = new Sale("SALE-123", new Ambev.DeveloperEvaluation.Domain.ValueObjects.CustomerInfo(1, "Customer"), new Ambev.DeveloperEvaluation.Domain.ValueObjects.BranchInfo(1, "Branch"));
        var expectedResult = new GetSaleResult { Id = saleId, SaleNumber = "SALE-123" };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(saleEntity);
        _mapper.Map<GetSaleResult>(saleEntity).Returns(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        result.SaleNumber.Should().Be("SALE-123");
        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent id When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentId_ThrowsKeyNotFoundException()
    {
        var command = new GetSaleCommand(Guid.NewGuid());
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    [Fact(DisplayName = "Given empty id When handling Then throws validation exception")]
    public async Task Handle_EmptyId_ThrowsValidationException()
    {
        var command = new GetSaleCommand(Guid.Empty);
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
