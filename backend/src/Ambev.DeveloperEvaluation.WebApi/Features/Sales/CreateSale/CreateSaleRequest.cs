namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
public class CreateSaleItemRequest
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateSaleRequest
{
    public string SaleNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public System.Collections.Generic.List<CreateSaleItemRequest> Items { get; set; } = new();
}
