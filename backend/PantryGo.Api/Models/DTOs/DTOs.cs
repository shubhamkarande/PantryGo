using System.ComponentModel.DataAnnotations;
using PantryGo.Api.Models.Enums;

namespace PantryGo.Api.Models.DTOs;

// ==================== Auth DTOs ====================

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? Phone { get; set; }
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

// ==================== User DTOs ====================

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ==================== Product DTOs ====================

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public string? Unit { get; set; }
    public bool IsActive { get; set; }
}

public class CreateProductRequest
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue)]
    public int Stock { get; set; } = 0;
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    [MaxLength(50)]
    public string? Unit { get; set; } = "piece";
}

public class UpdateProductRequest
{
    [MaxLength(255)]
    public string? Name { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal? Price { get; set; }
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    [Range(0, int.MaxValue)]
    public int? Stock { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    [MaxLength(50)]
    public string? Unit { get; set; }
    
    public bool? IsActive { get; set; }
}

public class ProductFilter
{
    public string? Category { get; set; }
    public string? Search { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ==================== Address DTOs ====================

public class AddressDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class CreateAddressRequest
{
    [MaxLength(50)]
    public string Label { get; set; } = "Home";
    
    [Required]
    [MaxLength(500)]
    public string AddressLine { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(10)]
    public string Pincode { get; set; } = string.Empty;
    
    public bool IsDefault { get; set; } = false;
}

// ==================== Order DTOs ====================

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public AddressDto? Address { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class CreateOrderRequest
{
    [Required]
    public Guid AddressId { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public class UpdateOrderStatusRequest
{
    [Required]
    public OrderStatus Status { get; set; }
}

// ==================== Payment DTOs ====================

public class CreatePaymentRequest
{
    [Required]
    public Guid OrderId { get; set; }
}

public class PaymentOrderResponse
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Key { get; set; } = string.Empty;
}

public class VerifyPaymentRequest
{
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    public string RazorpayPaymentId { get; set; } = string.Empty;
    
    [Required]
    public string RazorpayOrderId { get; set; } = string.Empty;
    
    [Required]
    public string RazorpaySignature { get; set; } = string.Empty;
}

// ==================== Common DTOs ====================

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    
    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };
    
    public static ApiResponse<T> Fail(string message) => new()
    {
        Success = false,
        Message = message
    };
}
