namespace Ambev.DeveloperEvaluation.Domain.Exceptions;

public class SaleDomainException : DomainException
{
    public SaleDomainException(string message) : base(message)
    {
    }

    public SaleDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
