using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.ValueObjects;

public class CustomerInfoTests
{
    [Fact(DisplayName = "Given valid data When creating CustomerInfo Then should succeed")]
    public void Given_ValidData_When_CreatingCustomerInfo_Then_ShouldSucceed()
    {
        var id = 100;
        var name = "John Doe";

        var customerInfo = new CustomerInfo(id, name);

        customerInfo.Id.Should().Be(id);
        customerInfo.Name.Should().Be(name);
    }

    [Theory(DisplayName = "Given invalid ID When creating CustomerInfo Then should throw DomainException")]
    [InlineData(0)]
    [InlineData(-10)]
    public void Given_InvalidId_When_CreatingCustomerInfo_Then_ShouldThrowException(int invalidId)
    {
        var act = () => new CustomerInfo(invalidId, "Valid Name");
        
        act.Should().Throw<DomainException>()
           .WithMessage("Customer ID must be greater than zero.");
    }

    [Theory(DisplayName = "Given invalid Name When creating CustomerInfo Then should throw DomainException")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidName_When_CreatingCustomerInfo_Then_ShouldThrowException(string invalidName)
    {
        var act = () => new CustomerInfo(10, invalidName);
        
        act.Should().Throw<DomainException>()
           .WithMessage("Customer Name cannot be empty.");
    }
}
