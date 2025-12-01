using System.Collections.Concurrent;
using FluentResults;
using OrderCompletion.Api.Core.Entities;
using OrderCompletion.Api.Core.Ports;
using OrderCompletion.Api.Shared;
using OrderCompletion.Api.Shared.Errors;

namespace OrderCompletion.Api.Core.UseCases;

public class OrderCompletionUseCase : IOrderCompletionUseCase
{
    private readonly IOrderCompletionRepository _orderCompletionRepository;
    private readonly INotificationClientAsync _notificationClient;
    private readonly ILogger<IOrderCompletionUseCase> _logger;

    private static readonly DateTime SixMonthsAgo = DateTime.UtcNow.AddMonths(-6);

    public static TimeSpan CompletionTimeout = TimeSpan.FromSeconds(30);
    
    public OrderCompletionUseCase(
        IOrderCompletionRepository orderCompletionRepository,
        INotificationClientAsync notificationClient,
        ILogger<IOrderCompletionUseCase> logger)
    {
        _orderCompletionRepository = orderCompletionRepository;
        _notificationClient = notificationClient;
        _logger = logger;
    }

    public async Task<Result> CompleteOrders(IReadOnlyCollection<int> orderIds, CancellationToken cancellationToken)
    {
        if (orderIds.Count == 0)
            return new ValidationError(Messages.OneOrMoreOrderIdsRequired);
        var orderTasks = Task.Run(() => orderIds.Select(id => new
        {
            OrderId = id,
            Order = _orderCompletionRepository.GetOrderById(id)
        }).ToList(), cancellationToken);  // Issue: if there are a lot of orders, ToList() here will be an issue

        var orders = await orderTasks;
        
        var missingOrders = orders
            .Where(x => x.Order is null)
            .ToList();
        if (missingOrders.Count != 0)
            return new NotFoundError($"{Messages.OneOrMoreOrdersNotFound}{string.Join(", ", missingOrders.Select(x => x.OrderId))}");
        
        var ordersAlreadyCompleted = orders
            .Where(x => x.Order.OrderState == OrderState.Completed)
            .ToList();
        if (ordersAlreadyCompleted.Count != 0)
            return new ConflictError($"{Messages.OneOrMoreOrdersAlreadyProcessed}{string.Join(", ", ordersAlreadyCompleted.Select(x => x.OrderId))}");
        
        var failures = new ConcurrentBag<string>();
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4,
            CancellationToken = cancellationToken
        };

        var task = Parallel.ForEachAsync(
            orders.Select(x => x.Order),
            options,
            (order, token) => HandleOrderAsync(order, token, failures)
        );
        
        var timedOut = await WaitForCompletionWithTimeout(task, cancellationToken);
        
        // It is more important to inform of any failed completions than timeout, since that one is the business
        // requirement 
        if (failures.Any())
            return new FailedProcessingError($"{Messages.FailedCompletingOrders}{string.Join(", ", failures)}");

        return timedOut ?
            new TimedOutError(Messages.TimedOutCompletingOrders) : 
            Result.Ok().WithSuccess(Messages.SuccessfullyCompletedAllOrders);
    }

    private async ValueTask HandleOrderAsync(Order order, CancellationToken token, ConcurrentBag<string> failures)
    {
        try
        {
            if (order.OrderState == OrderState.Notifying) { }
            else
            {
                if (!OrderValidForCompletion(order))
                {   
                    failures.Add($"Not completing order {order.Id} due to failure to meet requirements");
                    return;
                }
            }

            if (!await CompleteOrder(order, token))
                failures.Add($"{Messages.InternalProcessingError}{order.Id}!");
        }
        catch (Exception exc)
        {
            _logger.LogError($"Exception occurred during order processing: {exc}");
            failures.Add($"{Messages.InternalProcessingError}{order.Id}!");
        }
    }
    
    private static bool OrderValidForCompletion(Order order)
    {
        var oldOrder = order.OrderDate < SixMonthsAgo;
        var allOrderLinesSuccess = order.OrderLines.All(orderLine =>
            orderLine.DeliveredQuantity.HasValue &&
            orderLine.DeliveredQuantity == orderLine.OrderedQuantity);
        return oldOrder && allOrderLinesSuccess;
    }

    private async Task<bool> CompleteOrder(Order order, CancellationToken cancellationToken)
    {
        if (order.OrderState != OrderState.Notifying)
            // This if is to avoid needlessly spamming the db to set order to Notifying for those already in Notifying
            await _orderCompletionRepository.MarkOrderNotifyingAsync(order.Id, cancellationToken);

        var notifRes = await _notificationClient.OrderCompletedAsync(order.Id, cancellationToken);
        if (notifRes)
            await _orderCompletionRepository.CompleteOrderAsync(order.Id, cancellationToken);
        return notifRes;
    }

    /// <summary>
    /// Wait for the task to complete; if the task times out, returns true, otherwise false
    /// </summary>
    private async Task<bool> WaitForCompletionWithTimeout(Task task, CancellationToken cancellationToken)
    {
        if (await Task.WhenAny(task, Task.Delay(CompletionTimeout, cancellationToken)) == task)
        {
            await task;
            return false;
        }
        return true;
    }
}