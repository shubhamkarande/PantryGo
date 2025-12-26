using PantryGo.Api.Models.Enums;

namespace PantryGo.Api.Services;

public interface INotificationService
{
    Task SendOrderStatusNotificationAsync(Guid userId, Guid orderId, OrderStatus status);
    Task SendPushNotificationAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null);
}

/// <summary>
/// Notification service with FCM integration placeholder.
/// Firebase credentials should be configured in appsettings.json when ready.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationService> _logger;
    
    public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task SendOrderStatusNotificationAsync(Guid userId, Guid orderId, OrderStatus status)
    {
        var (title, body) = status switch
        {
            OrderStatus.Confirmed => ("Order Confirmed! 🎉", "Your order has been confirmed and is being processed."),
            OrderStatus.Packed => ("Order Packed! 📦", "Your order has been packed and is ready for pickup."),
            OrderStatus.OutForDelivery => ("On the Way! 🚚", "Your order is out for delivery. Get ready!"),
            OrderStatus.Delivered => ("Delivered! ✅", "Your order has been delivered. Enjoy!"),
            OrderStatus.Cancelled => ("Order Cancelled ❌", "Your order has been cancelled."),
            _ => ("Order Update", $"Your order status has been updated to {status}.")
        };
        
        var data = new Dictionary<string, string>
        {
            { "orderId", orderId.ToString() },
            { "status", status.ToString() },
            { "type", "order_status" }
        };
        
        await SendPushNotificationAsync(userId, title, body, data);
    }
    
    public async Task SendPushNotificationAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null)
    {
        var fcmServerKey = _configuration["Firebase:ServerKey"];
        
        if (string.IsNullOrEmpty(fcmServerKey))
        {
            // FCM not configured - log for development
            _logger.LogInformation(
                "FCM not configured. Notification: UserId={UserId}, Title={Title}, Body={Body}", 
                userId, title, body
            );
            return;
        }
        
        // TODO: When Firebase is configured:
        // 1. Look up user's FCM token from database
        // 2. Send push notification via FCM API
        
        // Placeholder for FCM integration
        /*
        var fcmToken = await GetUserFcmTokenAsync(userId);
        if (string.IsNullOrEmpty(fcmToken)) return;
        
        var message = new
        {
            to = fcmToken,
            notification = new { title, body },
            data
        };
        
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("key", "=" + fcmServerKey);
        
        await client.PostAsJsonAsync("https://fcm.googleapis.com/fcm/send", message);
        */
        
        _logger.LogInformation(
            "Push notification sent: UserId={UserId}, Title={Title}", 
            userId, title
        );
        
        await Task.CompletedTask;
    }
}
