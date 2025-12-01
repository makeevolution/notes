using OrderCompletion.Api.Core.Entities;

namespace OrderCompletion.Api.Core.Ports;

public interface IOrderCompletionRepository
{
    Order GetOrderById(int orderId);

    void CompleteOrder(int orderId);

    Task CompleteOrderAsync(int orderId, CancellationToken cancellationToken);
    
    Task MarkOrderNotifyingAsync(int orderId, CancellationToken cancellationToken);
}