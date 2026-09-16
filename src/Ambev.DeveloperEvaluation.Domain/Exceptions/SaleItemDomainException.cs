namespace Ambev.DeveloperEvaluation.Domain.Exceptions;

public class SaleItemDomainException : DomainException
{
    public SaleItemDomainException(string message) : base(message)
    {
    }

    public SaleItemDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
