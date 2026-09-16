using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public record Quantity
{
    public const int MaxQuantity = 20;
    public int Value { get; }

    private Quantity() { }
    
    public Quantity(int value)
    {
        if (value < 1)
            throw new SaleItemDomainException($"Quantity cannot be less than 1.");
        
        if (value > MaxQuantity)
            throw new SaleItemDomainException($"Cannot sell more than {MaxQuantity} identical items.");

        Value = value;
    }
    
    public static implicit operator int(Quantity quantity) => quantity.Value;
    public static implicit operator Quantity(int value) => new(value);
}
