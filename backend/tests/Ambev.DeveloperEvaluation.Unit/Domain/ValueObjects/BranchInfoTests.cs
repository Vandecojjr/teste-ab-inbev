using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.ValueObjects;

public class BranchInfoTests
{
    [Fact(DisplayName = "Given valid data When creating BranchInfo Then should succeed")]
    public void Given_ValidData_When_CreatingBranchInfo_Then_ShouldSucceed()
    {
        const int id = 10;
        const string name = "Main Branch";

        var branchInfo = new BranchInfo(id, name);
        branchInfo.Id.Should().Be(id);
        branchInfo.Name.Should().Be(name);
    }

    [Theory(DisplayName = "Given invalid ID When creating BranchInfo Then should throw DomainException")]
    [InlineData(0)]
    [InlineData(-5)]
    public void Given_InvalidId_When_CreatingBranchInfo_Then_ShouldThrowException(int invalidId)
    {
        var act = () => new BranchInfo(invalidId, "Valid Name");
        act.Should().Throw<DomainException>()
           .WithMessage("Branch ID must be greater than zero.");
    }

    [Theory(DisplayName = "Given invalid Name When creating BranchInfo Then should throw DomainException")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidName_When_CreatingBranchInfo_Then_ShouldThrowException(string invalidName)
    {
        var act = () => new BranchInfo(10, invalidName);
        act.Should().Throw<DomainException>()
           .WithMessage("Branch Name cannot be empty.");
    }
}
