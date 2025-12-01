namespace OrderCompletion.Api.Core.Ports;

public interface INotificationClient
{
    void OrderCompleted(int orderId);
}

public interface INotificationClientAsync : INotificationClient
{
    Task<bool> OrderCompletedAsync(int orderId, CancellationToken cancellationToken);
}