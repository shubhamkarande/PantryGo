using Newtonsoft.Json;

namespace PantryGo.Models;

// API Request/Response DTOs

public class LoginRequest
{
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("phone")]
    public string? Phone { get; set; }
}

public class AuthResponse
{
    [JsonProperty("token")]
    public string Token { get; set; } = string.Empty;
    
    [JsonProperty("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
    
    [JsonProperty("user")]
    public User User { get; set; } = null!;
}

public class ApiResponse<T>
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("message")]
    public string? Message { get; set; }
    
    [JsonProperty("data")]
    public T? Data { get; set; }
}

public class PagedResponse<T>
{
    [JsonProperty("items")]
    public List<T> Items { get; set; } = new();
    
    [JsonProperty("page")]
    public int Page { get; set; }
    
    [JsonProperty("pageSize")]
    public int PageSize { get; set; }
    
    [JsonProperty("totalCount")]
    public int TotalCount { get; set; }
    
    [JsonProperty("totalPages")]
    public int TotalPages { get; set; }
}

public class CreateOrderRequest
{
    [JsonProperty("addressId")]
    public Guid AddressId { get; set; }
    
    [JsonProperty("items")]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    [JsonProperty("productId")]
    public Guid ProductId { get; set; }
    
    [JsonProperty("quantity")]
    public int Quantity { get; set; }
}

public class CreateAddressRequest
{
    [JsonProperty("label")]
    public string Label { get; set; } = "Home";
    
    [JsonProperty("addressLine")]
    public string AddressLine { get; set; } = string.Empty;
    
    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;
    
    [JsonProperty("pincode")]
    public string Pincode { get; set; } = string.Empty;
    
    [JsonProperty("isDefault")]
    public bool IsDefault { get; set; }
}

public class PaymentOrderResponse
{
    [JsonProperty("orderId")]
    public string OrderId { get; set; } = string.Empty;
    
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; } = "INR";
    
    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;
}
