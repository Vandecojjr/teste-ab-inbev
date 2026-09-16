using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand that defines validation rules for sale creation command.
/// </summary>
public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleValidator with defined validation rules.
    /// </summary>
    public CreateSaleValidator()
    {
        RuleFor(sale => sale.SaleNumber).NotEmpty().WithMessage("SaleNumber is required.");
        RuleFor(sale => sale.CustomerName).NotEmpty().WithMessage("CustomerName is required.");
        RuleFor(sale => sale.BranchName).NotEmpty().WithMessage("BranchName is required.");
        RuleFor(sale => sale.Items).NotEmpty().WithMessage("A sale must contain at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new CreateSaleItemValidator());
    }
}

public class CreateSaleItemValidator : AbstractValidator<CreateSaleItemCommand>
{
    public CreateSaleItemValidator()
    {
        RuleFor(item => item.ProductName).NotEmpty().WithMessage("ProductName is required.");
        RuleFor(item => item.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        RuleFor(item => item.Quantity).LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 identical items.");
        RuleFor(item => item.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("UnitPrice cannot be negative.");
    }
}
