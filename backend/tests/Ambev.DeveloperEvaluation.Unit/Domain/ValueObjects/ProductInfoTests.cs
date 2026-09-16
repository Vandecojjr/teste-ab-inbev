using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.ValueObjects;

public class ProductInfoTests
{
    [Fact(DisplayName = "Given valid data When creating ProductInfo Then should succeed")]
    public void Given_ValidData_When_CreatingProductInfo_Then_ShouldSucceed()
    {
        var id = 5;
        var name = "Product A";

        var productInfo = new ProductInfo(id, name);

        productInfo.Id.Should().Be(id);
        productInfo.Name.Should().Be(name);
    }

    [Theory(DisplayName = "Given invalid ID When creating ProductInfo Then should throw DomainException")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidId_When_CreatingProductInfo_Then_ShouldThrowException(int invalidId)
    {
        var act = () => new ProductInfo(invalidId, "Valid Name");
        
        act.Should().Throw<DomainException>()
           .WithMessage("Product ID must be greater than zero.");
    }

    [Theory(DisplayName = "Given invalid Name When creating ProductInfo Then should throw DomainException")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidName_When_CreatingProductInfo_Then_ShouldThrowException(string invalidName)
    {
        var act = () => new ProductInfo(10, invalidName);
        
        act.Should().Throw<DomainException>()
           .WithMessage("Product Name cannot be empty.");
    }
}
