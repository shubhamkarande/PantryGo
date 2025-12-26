namespace PantryGo.Api.Models.Enums;

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
