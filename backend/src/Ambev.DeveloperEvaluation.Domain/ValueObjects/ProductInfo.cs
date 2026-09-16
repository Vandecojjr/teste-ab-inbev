using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public sealed record ProductInfo
{
    public int Id { get; }
    public string Name { get; }

    private ProductInfo() { }
    
    public ProductInfo(int id, string name)
    {
        if (id <= 0)
            throw new DomainException("Product ID must be greater than zero.");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product Name cannot be empty.");

        Id = id;
        Name = name;
    }
}
