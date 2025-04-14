namespace Exercises.Tests;
using Xunit;
public class UnitTest1
{
    [Fact]
    public void Should_throw_exception_when_trying_to_create_order_with_no_customer()
    {
        // Arrange
        var orderService = new OrderService("StoreA");
        var orderId = "12345";
        var newStatus = 1; // Some status
        // Act
        Assert.Throws<Exception>(() => orderService.CreateOrder(new List<OrderItem>(), false, "Test order"));
    }

}
