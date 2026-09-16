using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Sale ID is required.");
        RuleFor(sale => sale.SaleNumber).NotEmpty().WithMessage("SaleNumber is required.");
        RuleFor(sale => sale.CustomerName).NotEmpty().WithMessage("CustomerName is required.");
        RuleFor(sale => sale.BranchName).NotEmpty().WithMessage("BranchName is required.");
    }
}
