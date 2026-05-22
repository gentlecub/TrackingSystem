using System.ComponentModel.DataAnnotations;
using TrackingSystem.App.Models.Enums;

namespace TrackingSystem.App.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    // Relationship with Employee (optional)
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}
