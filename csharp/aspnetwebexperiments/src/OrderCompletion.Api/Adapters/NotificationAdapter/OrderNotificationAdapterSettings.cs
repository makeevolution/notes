using System.ComponentModel.DataAnnotations;

namespace OrderCompletion.Api.Adapters.NotificationAdapter;

public class OrderNotificationAdapterSettings
{
    [Required]
    public required string Host { get; set; }
}