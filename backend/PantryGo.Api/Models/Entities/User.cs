using System.ComponentModel.DataAnnotations;
using PantryGo.Api.Models.Enums;

namespace PantryGo.Api.Models.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    public UserRole Role { get; set; } = UserRole.Customer;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Order> DeliveryAssignments { get; set; } = new List<Order>();
}
