using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; private set; }
    public DateTime SaleDate { get; private set; }
    public CustomerInfo Customer { get; private set; }
    public BranchInfo Branch { get; private set; }
    public bool IsCancelled { get; private set; }
    
    private readonly List<SaleItem> _items = [];
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    public Money TotalSaleAmount => new(_items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount));

    protected Sale() { }
    public Sale(string saleNumber, CustomerInfo customer, BranchInfo branch)
    {
        Id = Guid.NewGuid();
        SaleNumber = saleNumber ?? throw new SaleDomainException("Sale number cannot be null");
        SaleDate = DateTime.UtcNow;
        Customer = customer ?? throw new SaleDomainException("Customer info cannot be null");
        Branch = branch ?? throw new SaleDomainException("Branch info cannot be null");
        IsCancelled = false;
    }

    public void AddItem(ProductInfo product, int quantity, decimal unitPrice)
    {
        if (IsCancelled)
            throw new SaleDomainException("Cannot add items to a cancelled sale.");

        var item = new SaleItem(product, new Quantity(quantity), new Money(unitPrice));
        _items.Add(item);
    }
    
    public void ClearItems()
    {
        _items.Clear();
    }

    public void Update(string saleNumber, CustomerInfo customer, BranchInfo branch)
    {
        if (IsCancelled)
            throw new SaleDomainException("Cannot update a cancelled sale.");

        SaleNumber = saleNumber ?? throw new SaleDomainException("Sale number cannot be null");
        Customer = customer ?? throw new SaleDomainException("Customer info cannot be null");
        Branch = branch ?? throw new SaleDomainException("Branch info cannot be null");
    }

    public void CancelItem(Guid itemId)
    {
        if (IsCancelled)
            throw new SaleDomainException("Cannot cancel an item of a cancelled sale.");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new SaleDomainException($"Item with id {itemId} not found in this sale.");

        item.Cancel();
    }

    public void CancelSale()
    {
        if (IsCancelled)
            throw new SaleDomainException("Sale is already cancelled.");
        
        IsCancelled = true;
        
        foreach (var item in _items.Where(i => !i.IsCancelled))
        {
            item.Cancel();
        }
    }
}
