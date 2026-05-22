using System.ComponentModel.DataAnnotations;

namespace TrackingSystem.App.Models;

public class Employee
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // Navigation property - An employee can have many assigned assets
    public ICollection<Asset> AssignedAssets { get; set; } = new List<Asset>();

    // Relationship with User (optional)
    public User? User { get; set; }
}
