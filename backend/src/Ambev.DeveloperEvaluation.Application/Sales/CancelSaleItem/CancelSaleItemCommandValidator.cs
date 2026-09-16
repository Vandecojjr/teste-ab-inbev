using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemCommandValidator : AbstractValidator<CancelSaleItemCommand>
{
    public CancelSaleItemCommandValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty().WithMessage("Sale ID is required.");
        RuleFor(x => x.ItemId).NotEmpty().WithMessage("Item ID is required.");
    }
}
