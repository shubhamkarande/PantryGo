namespace PantryGo.Models;

public enum UserRole
{
    Customer,
    Delivery,
    Admin
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Packed,
    OutForDelivery,
    Delivered,
    Cancelled
}

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Product
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

public class Address
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Address? Address { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class CartItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public int Quantity { get; set; }
    public int Stock { get; set; }
    
    public decimal TotalPrice => Price * Quantity;
}
