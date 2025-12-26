using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PantryGo.Api.Models.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    public int Stock { get; set; } = 0;
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    [MaxLength(50)]
    public string? Unit { get; set; } = "piece"; // kg, piece, liter, etc.
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
