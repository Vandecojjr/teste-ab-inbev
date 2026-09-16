using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Given valid data When creating Sale Then creates active sale")]
    public void Given_ValidData_When_CreatingSale_Then_CreatesActiveSale()
    {
        var sale = new Sale("SALE-123", new CustomerInfo(1, "Customer"), new BranchInfo(1, "Branch"));

        sale.SaleNumber.Should().Be("SALE-123");
        sale.IsCancelled.Should().BeFalse();
        sale.Items.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given valid item When adding to Sale Then recalculates total")]
    public void Given_ValidItem_When_AddingToSale_Then_RecalculatesTotal()
    {
        var sale = new Sale("SALE-123", new CustomerInfo(1, "Customer"), new BranchInfo(1, "Branch"));
        sale.AddItem(new ProductInfo(1, "Product A"), 5, 100m);

        sale.Items.Should().HaveCount(1);
        sale.TotalSaleAmount.Value.Should().Be(450m);
    }

    [Fact(DisplayName = "Given active sale When cancelling sale Then cancels all items")]
    public void Given_ActiveSale_When_CancellingSale_Then_CancelsAllItems()
    {
        var sale = new Sale("SALE-123", new CustomerInfo(1, "Customer"), new BranchInfo(1, "Branch"));
        sale.AddItem(new ProductInfo(1, "Product A"), 5, 100m);
        sale.AddItem(new ProductInfo(1, "Product B"), 2, 50m);
        sale.CancelSale();

        sale.IsCancelled.Should().BeTrue();
        sale.Items.All(i => i.IsCancelled).Should().BeTrue();
        sale.TotalSaleAmount.Value.Should().Be(0m);
    }

    [Fact(DisplayName = "Given active sale When cancelling one item Then recalculates total")]
    public void Given_ActiveSale_When_CancellingOneItem_Then_RecalculatesTotal()
    {
        var sale = new Sale("SALE-123", new CustomerInfo(1, "Customer"), new BranchInfo(1, "Branch"));
        sale.AddItem(new ProductInfo(1, "Product A"), 5, 100m);
        sale.AddItem(new ProductInfo(1, "Product B"), 2, 50m);
        
        var itemIdToCancel = sale.Items.First(i => i.Product.Name == "Product A").Id;
        sale.CancelItem(itemIdToCancel);

        sale.TotalSaleAmount.Value.Should().Be(100m);
        sale.Items.First(i => i.Id == itemIdToCancel).IsCancelled.Should().BeTrue();
    }
    
    [Fact(DisplayName = "Given cancelled sale When adding item Then throws SaleDomainException")]
    public void Given_CancelledSale_When_AddingItem_Then_ThrowsException()
    {
        var sale = new Sale("SALE-123", new CustomerInfo(1, "Customer"), new BranchInfo(1, "Branch"));
        sale.CancelSale();
        var act = () => sale.AddItem(new ProductInfo(1, "Product A"), 1, 100m);

        act.Should().Throw<SaleDomainException>().WithMessage("Cannot add items to a cancelled sale.");
    }
}
