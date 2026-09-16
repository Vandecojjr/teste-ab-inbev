using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.ValueObjects;

public class QuantityTests
{
    [Fact(DisplayName = "Given valid amount When creating Quantity Then should succeed")]
    public void Given_ValidAmount_When_CreatingQuantity_Then_ShouldSucceed()
    {
        var quantity = new Quantity(10);
        quantity.Value.Should().Be(10);
    }

    [Theory(DisplayName = "Given amount greater than MaxQuantity When creating Quantity Then throws SaleItemDomainException")]
    [InlineData(21)]
    [InlineData(100)]
    public void Given_AmountGreaterThanMaxQuantity_When_CreatingQuantity_Then_ThrowsException(int invalidQuantity)
    {
        var act = () => new Quantity(invalidQuantity);
        act.Should().Throw<SaleItemDomainException>()
           .WithMessage($"Cannot sell more than {Quantity.MaxQuantity} identical items.");
    }

    [Theory(DisplayName = "Given amount less than 1 When creating Quantity Then throws SaleItemDomainException")]
    [InlineData(0)]
    [InlineData(-5)]
    public void Given_AmountLessThanOne_When_CreatingQuantity_Then_ThrowsException(int invalidQuantity)
    {
        Action act = () => new Quantity(invalidQuantity);
        act.Should().Throw<SaleItemDomainException>()
           .WithMessage("Quantity cannot be less than 1.");
    }
}
