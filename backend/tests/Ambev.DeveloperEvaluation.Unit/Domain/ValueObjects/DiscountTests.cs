using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.ValueObjects;

public class DiscountTests
{
    [Theory(DisplayName = "Given amount less than 4 When calculating discount Then returns 0 percentage")]
    [InlineData(1)]
    [InlineData(3)]
    public void Given_AmountLessThan4_When_CalculatingDiscount_Then_Returns0Percentage(int amount)
    {
        var quantity = new Quantity(amount);
        var discount = new Discount(quantity);
        discount.Percentage.Should().Be(0m);
    }

    [Theory(DisplayName = "Given amount between 4 and 9 When calculating discount Then returns 10 percentage")]
    [InlineData(4)]
    [InlineData(9)]
    public void Given_AmountBetween4And9_When_CalculatingDiscount_Then_Returns10Percentage(int amount)
    {
        var quantity = new Quantity(amount);
        var discount = new Discount(quantity);
        discount.Percentage.Should().Be(0.10m);
    }

    [Theory(DisplayName = "Given amount between 10 and 20 When calculating discount Then returns 20 percentage")]
    [InlineData(10)]
    [InlineData(20)]
    public void Given_AmountBetween10And20_When_CalculatingDiscount_Then_Returns20Percentage(int amount)
    {
        var quantity = new Quantity(amount);
        var discount = new Discount(quantity);
        discount.Percentage.Should().Be(0.20m);
    }

    [Fact(DisplayName = "Given total amount When applying discount Then reduces the total amount")]
    public void Given_TotalAmount_When_ApplyingDiscount_Then_ReducesTotalAmount()
    {
        var quantity = new Quantity(10);
        var discount = new Discount(quantity);
        const decimal originalTotal = 100m;

        var discountedAmount = discount.Apply(originalTotal);
        discountedAmount.Should().Be(80m);
    }
}
