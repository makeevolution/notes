using FluentResults;

namespace OrderCompletion.Api.Core.Ports;

public interface IOrderCompletionUseCase
{
    Task<Result> CompleteOrders(IReadOnlyCollection<int> orderIds, CancellationToken cancellationToken);
}