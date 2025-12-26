namespace PantryGo.Helpers;

public static class Constants
{
    // API Base URL - For physical device, use laptop's IP address
    // For emulator, use 10.0.2.2 (Android) or localhost (Windows)
    public static string ApiBaseUrl => DeviceInfo.Platform == DevicePlatform.Android
        ? "http://10.220.222.11:5000/api" // Laptop IP on hotspot
        : "http://localhost:5000/api"; // Windows
    
    // Local storage keys
    public const string TokenKey = "auth_token";
    public const string RefreshTokenKey = "refresh_token";
    public const string UserKey = "current_user";
    
    // Product categories
    public static readonly string[] Categories = 
    {
        "Fruits",
        "Vegetables", 
        "Dairy",
        "Snacks",
        "Beverages"
    };
    
    // Order status colors
    public static Color GetStatusColor(string status) => status switch
    {
        "Pending" => Colors.Orange,
        "Confirmed" => Colors.Blue,
        "Packed" => Colors.Purple,
        "OutForDelivery" => Colors.Teal,
        "Delivered" => Colors.Green,
        "Cancelled" => Colors.Red,
        _ => Colors.Gray
    };
    
    // Order status icons
    public static string GetStatusIcon(string status) => status switch
    {
        "Pending" => "⏳",
        "Confirmed" => "✓",
        "Packed" => "📦",
        "OutForDelivery" => "🚚",
        "Delivered" => "✅",
        "Cancelled" => "❌",
        _ => "•"
    };
}
