using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PantryGo.Api.Models.Entities;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid OrderId { get; set; }
    
    public Guid ProductId { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } // Price at time of order
    
    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
