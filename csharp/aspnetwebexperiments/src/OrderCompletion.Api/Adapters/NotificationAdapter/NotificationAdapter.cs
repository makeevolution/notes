using OrderCompletion.Api.Core.Ports;

namespace OrderCompletion.Api.Adapters.NotificationAdapter;

public static class NotificationAdapter
{
    public static void RegisterNotificationAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OrderNotificationAdapterSettings>(configuration.GetSection(nameof(OrderNotificationAdapterSettings)));
        services.AddTransient<INotificationClientAsync, NotificationClient>();
    }
}