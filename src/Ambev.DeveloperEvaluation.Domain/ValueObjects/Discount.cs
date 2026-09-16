namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public sealed record Discount
{
    public decimal Percentage { get; }

    public Discount(Quantity quantity)
    {
        Percentage = CalculatePercentage(quantity);
    }

    private Discount() { }

    private static decimal CalculatePercentage(int quantity)
    {
        return quantity switch
        {
            >= 4 and < 10 => 0.10m,
            >= 10 and <= 20 => 0.20m,
            _ => 0m
        };
    }

    public decimal Apply(decimal totalAmount)
        => totalAmount * (1 - Percentage);
}
