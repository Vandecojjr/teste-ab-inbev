using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public sealed record CustomerInfo
{
    public int Id { get; }
    public string Name { get; }

    private CustomerInfo() { }
    
    public CustomerInfo(int id, string name)
    {
        if (id <= 0)
            throw new DomainException("Customer ID must be greater than zero.");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer Name cannot be empty.");

        Id = id;
        Name = name;
    }
}
