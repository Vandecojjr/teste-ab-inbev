using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public sealed record Money
{
    public decimal Value { get; }

    private Money() { }
    
    public Money(decimal value)
    {
        if (value < 0)
            throw new SaleItemDomainException("Money amount cannot be negative");
        
        Value = value;
    }

    public static implicit operator decimal(Money money) => money.Value;
    public static implicit operator Money(decimal value) => new(value);
}
