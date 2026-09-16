using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public class SaleEventHandlers : 
    INotificationHandler<SaleCreatedEvent>,
    INotificationHandler<SaleModifiedEvent>,
    INotificationHandler<SaleCancelledEvent>,
    INotificationHandler<ItemCancelledEvent>
{
    private readonly ILogger<SaleEventHandlers> _logger;

    public SaleEventHandlers(ILogger<SaleEventHandlers> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event Published: SaleCreated | SaleId: {SaleId}, SaleNumber: {SaleNumber}", notification.SaleId, notification.SaleNumber);
        return Task.CompletedTask;
    }

    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event Published: SaleModified | SaleId: {SaleId}, SaleNumber: {SaleNumber}", notification.SaleId, notification.SaleNumber);
        return Task.CompletedTask;
    }

    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event Published: SaleCancelled | SaleId: {SaleId}", notification.SaleId);
        return Task.CompletedTask;
    }

    public Task Handle(ItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event Published: ItemCancelled | SaleId: {SaleId}, ItemId: {ItemId}", notification.SaleId, notification.ItemId);
        return Task.CompletedTask;
    }
}
