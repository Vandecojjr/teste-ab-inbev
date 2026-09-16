using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public sealed record BranchInfo
{
    public int Id { get; }
    public string Name { get; }

    private BranchInfo() { }
    
    public BranchInfo(int id, string name)
    {
        if (id <= 0)
            throw new DomainException("Branch ID must be greater than zero.");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Branch Name cannot be empty.");

        Id = id;
        Name = name;
    }
}
