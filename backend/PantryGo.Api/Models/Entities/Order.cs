using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PantryGo.Api.Models.Enums;

namespace PantryGo.Api.Models.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid UserId { get; set; }
    
    public Guid? AddressId { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }
    
    [MaxLength(255)]
    public string? PaymentId { get; set; }
    
    [MaxLength(255)]
    public string? RazorpayOrderId { get; set; }
    
    public bool IsPaid { get; set; } = false;
    
    public Guid? DeliveryPartnerId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Address? Address { get; set; }
    public virtual User? DeliveryPartner { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
