using System.ComponentModel.DataAnnotations;

namespace PantryGo.Api.Models.Entities;

public class Address
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid UserId { get; set; }
    
    [MaxLength(50)]
    public string Label { get; set; } = "Home"; // Home, Work, Other
    
    [Required]
    [MaxLength(500)]
    public string AddressLine { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string Pincode { get; set; } = string.Empty;
    
    public bool IsDefault { get; set; } = false;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
