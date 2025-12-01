using Dapper;
using MySql.Data.MySqlClient;
using OrderCompletion.Api.Adapters.OrderCompletionAdapter.Dtos;
using OrderCompletion.Api.Adapters.OrderCompletionAdapter.Mappers;
using OrderCompletion.Api.Core.Entities;
using OrderCompletion.Api.Core.Ports;

namespace OrderCompletion.Api.Adapters.OrderCompletionAdapter;

internal class OrderCompletionRepository : IOrderCompletionRepository
{
    private readonly string _connectionString;

    public OrderCompletionRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private const string UpdateOrdersQuery = "UPDATE ORDERS SET OrderStateId = @OrderStateId WHERE Id = @Id";

    public void CompleteOrder(int orderId) => CompleteOrderAsync(orderId, CancellationToken.None).GetAwaiter().GetResult();
    
    public async Task CompleteOrderAsync(int orderId, CancellationToken token)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(token);
        await connection.ExecuteAsync(UpdateOrdersQuery, new { OrderStateId = (int)OrderState.Completed, Id = orderId });
    }
    
    public async Task MarkOrderNotifyingAsync(int orderId, CancellationToken token)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(token);
        await connection.ExecuteAsync(UpdateOrdersQuery, new { OrderStateId = (int)OrderState.Notifying, Id = orderId });
    }
    
    public Order GetOrderById(int orderId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            var orderQuery = "SELECT * FROM ORDERS WHERE Id = @Id";
            var orderDto = connection.QuerySingleOrDefault<OrderDto>(orderQuery, new { Id = orderId });

            if (orderDto == null) return null;

            var orderLinesQuery = "SELECT * FROM ORDER_LINES WHERE OrderId = @OrderId";
            var orderLines = connection.Query<OrderLineDto>(orderLinesQuery, new { OrderId = orderId }).ToList();

            orderDto.OrderLines = orderLines;

            return orderDto.ToDomain();
        }
    }
}