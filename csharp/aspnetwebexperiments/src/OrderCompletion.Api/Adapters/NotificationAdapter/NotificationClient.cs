using Microsoft.Extensions.Options;
using OrderCompletion.Api.Adapters.Enums;
using OrderCompletion.Api.Core.Ports;

namespace OrderCompletion.Api.Adapters.NotificationAdapter;

public class NotificationClient : INotificationClientAsync
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<NotificationClient> _logger;
    private readonly OrderNotificationAdapterSettings _settings;

    public NotificationClient(IHttpClientFactory httpClientFactory, 
        IOptions<OrderNotificationAdapterSettings> settings,
        ILogger<NotificationClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _settings = settings.Value;
    }

    /// <summary>
    /// Call the notification API to synchronously notify an order is complete; use the async version as much as possible
    /// </summary>
    public void OrderCompleted(int orderId) => OrderCompletedAsync(orderId, CancellationToken.None).GetAwaiter().GetResult();
    
    /// <summary>
    /// Call the notification API to asynchronously notify an order is complete
    /// </summary>
    public async Task<bool> OrderCompletedAsync(int orderId, CancellationToken cancellationToken)
    {
        using var httpClient = _httpClientFactory.CreateClient(nameof(HttpClientsEnum.OrderCompletionClient));
        var res = await httpClient.GetAsync($"{_settings.Host}/{NotificationEndpoints.NotifyForOrderId}{orderId}", cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            var content = await res.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Call to OrderNotification API failed: {StatusCode} {ReasonPhrase}. Response: {Content}",
                (int)res.StatusCode,
                res.ReasonPhrase,
                content);
        }    
        return res.IsSuccessStatusCode;
    }
}