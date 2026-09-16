using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; private set; }
    public ProductInfo Product { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Discount Discount { get; private set; }
    public bool IsCancelled { get; private set; }

    
    public Money TotalAmount => new Money(Discount.Apply(UnitPrice.Value * Quantity.Value));
    
    protected SaleItem() { }
    public SaleItem(ProductInfo product, Quantity quantity, Money unitPrice)
    {
        Id = Guid.NewGuid();
        Product = product ?? throw new SaleItemDomainException("Product info cannot be null");
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = new Discount(quantity);
        IsCancelled = false;
    }

    internal void Cancel()
    {
        if (IsCancelled)
            throw new SaleItemDomainException($"Item {Product.Name} is already cancelled.");
        
        IsCancelled = true;
    }
}
