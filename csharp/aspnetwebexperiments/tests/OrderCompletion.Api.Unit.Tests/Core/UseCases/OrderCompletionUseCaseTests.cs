using Microsoft.Extensions.Logging;
using Moq;
using OrderCompletion.Api.Core.Entities;
using OrderCompletion.Api.Core.Ports;
using OrderCompletion.Api.Core.UseCases;
using OrderCompletion.Api.Shared;
using Xunit;

namespace OrderCompletion.Api.Unit.Tests.Core.UseCases;

public class OrderCompletionUseCaseTests : IDisposable
{
    private static readonly DateTime Yesterday = DateTime.UtcNow.AddDays(-1);
    private static readonly DateTime Yesteryear = DateTime.UtcNow.AddYears(-1);

    private readonly Mock<IOrderCompletionRepository> _orderCompletionRepository = new();
    private readonly Mock<INotificationClientAsync> _notificationClientMock = new();
    private readonly Mock<ILogger<IOrderCompletionUseCase>> _loggerMock = new();

    private readonly IOrderCompletionUseCase _sut;
    private readonly TimeSpan _originalCompletionTimeout;

    public OrderCompletionUseCaseTests()
    {
        _originalCompletionTimeout = OrderCompletionUseCase.CompletionTimeout;
        _sut = new OrderCompletionUseCase(_orderCompletionRepository.Object, _notificationClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GivenRecentOrder_CompleteOrders_OrderIsNotCompleted()
    {
        const int orderId = 1;

        _notificationClientMock.Setup(x => x.OrderCompletedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
        SetupRecentOrder(orderId);

        await _sut.CompleteOrders([orderId], It.IsAny<CancellationToken>());

        _orderCompletionRepository.Verify(x => x.CompleteOrderAsync(orderId, It.IsAny<CancellationToken>()), Times.Never);
        _notificationClientMock.Verify(x => x.OrderCompletedAsync(orderId, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenOrderIsNotFullyDelivered_CompleteOrders_OrderIsNotCompleted()
    {
        const int orderId = 2;

        _notificationClientMock.Setup(x => x.OrderCompletedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
        SetupIncompleteOrder(orderId);

        await _sut.CompleteOrders([orderId], It.IsAny<CancellationToken>());

        _orderCompletionRepository.Verify(x => x.CompleteOrderAsync(orderId, It.IsAny<CancellationToken>()), Times.Never);
        _notificationClientMock.Verify(x => x.OrderCompletedAsync(orderId, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenOldOrderIsFullyDelivered_CompleteOrders_OrderCompleted()
    {
        const int orderId = 3;

        _notificationClientMock.Setup(x => x.OrderCompletedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
        SetupOldCompletedOrder(orderId);

        await _sut.CompleteOrders([orderId], It.IsAny<CancellationToken>());

        _orderCompletionRepository.Verify(x => x.CompleteOrderAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _notificationClientMock.Verify(x => x.OrderCompletedAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenMultipleOrders_CompleteOrders_OnlyOldFullyDeliveredOrdersAreCompleted()
    {
        SetupOldCompletedOrder(1);
        SetupRecentOrder(2);
        SetupOldCompletedOrder(3);
        SetupIncompleteOrder(4);

        _notificationClientMock.Setup(x => x.OrderCompletedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
        await _sut.CompleteOrders([1, 2, 3, 4], It.IsAny<CancellationToken>());

        _orderCompletionRepository.Verify(x => x.CompleteOrderAsync(It.IsIn(1, 3), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _orderCompletionRepository.Verify(x => x.CompleteOrderAsync(It.IsNotIn(1, 3), It.IsAny<CancellationToken>()), Times.Never);
        _notificationClientMock.Verify(x => x.OrderCompletedAsync(It.IsIn(1, 3), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _notificationClientMock.Verify(x => x.OrderCompletedAsync(It.IsNotIn(1, 3), It.IsAny<CancellationToken>()), Times.Never);
    }

    // New tests
    
    [Fact]
    public async Task GivenNonexistentOrders_CompleteOrders_RefuseToProcessRequest()
    {
        SetupOldCompletedOrder(1);
        SetupNonExistentOrder(5);
        var res = await _sut.CompleteOrders([1, 5], It.IsAny<CancellationToken>());
        Assert.True(res.IsFailed);
    }

    [Fact]
    public async Task GivenNoOrders_CompleteOrders_ReturnsFailure()
    {
        var res = await _sut.CompleteOrders([], It.IsAny<CancellationToken>());
        _notificationClientMock.Verify(x => x.OrderCompletedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Contains(res.Errors, msg => msg.Message.Contains(Messages.OneOrMoreOrderIdsRequired));
    }

    [Fact]
    public async Task GivenCompletingOrderTakesTooLong_CompleteOrders_ReturnsTimeout()
    {
        SetupOldCompletedOrder(1);
        _notificationClientMock.Setup(x => x.OrderCompletedAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
        SetupOrderNotificationTakesTooLongForOrder(5);

        var res = await _sut.CompleteOrders([1, 5], It.IsAny<CancellationToken>());
        Assert.True(res.IsFailed);
        Assert.Contains(res.Reasons, msg => msg.Message.Contains(Messages.TimedOutCompletingOrders));
    }

    [Fact]
    public async Task GivenNotificationSuccessfulButDbUpdateFails_CompleteOrders_ReturnsFailure()
    {
        int orderId = 1;
        SetupOldCompletedOrder(orderId);
        _notificationClientMock.Setup(x => x.OrderCompletedAsync(orderId, It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
        _orderCompletionRepository.Setup(x => x.CompleteOrderAsync(orderId, It.IsAny<CancellationToken>())).Throws(new Exception());
        var res = await _sut.CompleteOrders([orderId], It.IsAny<CancellationToken>());
        Assert.True(res.IsFailed);
        Assert.Contains(res.Reasons, msg => msg.Message.Contains(Messages.InternalProcessingError));
        _orderCompletionRepository.Verify(x => x.MarkOrderNotifyingAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GivenRenotificationAttemptSuccessful_CompleteOrders_ReturnsSuccess()
    {
        int orderId = 1;
        SetupCompletedOrderUnderRenotificationAttempt(orderId);
        _notificationClientMock.Setup(x => x.OrderCompletedAsync(orderId, It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
        var res = await _sut.CompleteOrders([orderId], It.IsAny<CancellationToken>());
        Assert.True(res.IsSuccess);
        Assert.Contains(res.Reasons, msg => msg.Message.Contains(Messages.SuccessfullyCompletedAllOrders));
        _orderCompletionRepository.Verify(x => x.MarkOrderNotifyingAsync(orderId, It.IsAny<CancellationToken>()), Times.Never);
        _orderCompletionRepository.Verify(x => x.CompleteOrderAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    private void SetupRecentOrder(int orderId)
    {
        _orderCompletionRepository
            .Setup(x => x.GetOrderById(orderId))
            .Returns(new Order
            {
                Id = orderId,
                OrderDate = Yesterday,
                OrderLines =
                [
                    new OrderLine { ProductId = 1, OrderedQuantity = 10, DeliveredQuantity = 10 }
                ]
            });
    }

    private void SetupIncompleteOrder(int orderId)
    {
        _orderCompletionRepository
            .Setup(x => x.GetOrderById(orderId))
            .Returns(new Order
            {
                Id = orderId,
                OrderState = OrderState.Submitted,
                OrderDate = Yesteryear,
                OrderLines =
                [
                    new OrderLine { ProductId = 1, OrderedQuantity = 10, DeliveredQuantity = 10 },
                    new OrderLine { ProductId = 2, OrderedQuantity = 10, }
                ]
            });
    }

    private void SetupOldCompletedOrder(int orderId)
    {
        _orderCompletionRepository
            .Setup(x => x.GetOrderById(orderId))
            .Returns(new Order
            {
                Id = orderId,
                OrderState = OrderState.Submitted,
                OrderDate = Yesteryear,
                OrderLines =
                [
                    new OrderLine { ProductId = 1, OrderedQuantity = 10, DeliveredQuantity = 10 },
                    new OrderLine { ProductId = 2, OrderedQuantity = 2, DeliveredQuantity = 2}
                ]
            });
    }

    private void SetupCompletedOrderUnderRenotificationAttempt(int orderId)
    {
        _orderCompletionRepository
            .Setup(x => x.GetOrderById(orderId))
            .Returns(new Order
            {
                Id = orderId,
                OrderState = OrderState.Notifying,
                OrderDate = Yesteryear,
                OrderLines =
                [
                    new OrderLine { ProductId = 1, OrderedQuantity = 10, DeliveredQuantity = 10 },
                    new OrderLine { ProductId = 2, OrderedQuantity = 2, DeliveredQuantity = 2}
                ]
            });
    }
    
    private void SetupNonExistentOrder(int orderId)
    {
        _orderCompletionRepository
            .Setup(x => x.GetOrderById(orderId))
            .Verifiable();
    }

    private void SetupOrderNotificationTakesTooLongForOrder(int orderId)
    {
        OrderCompletionUseCase.CompletionTimeout = TimeSpan.FromSeconds(1);  // Make tests fast
        SetupOldCompletedOrder(orderId);
        _notificationClientMock.Setup(x => x.OrderCompletedAsync(orderId, It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                await Task.Delay(OrderCompletionUseCase.CompletionTimeout * 2);
                return true;
            });
    }

    public void Dispose()
    {
        OrderCompletionUseCase.CompletionTimeout = _originalCompletionTimeout;
    }
}