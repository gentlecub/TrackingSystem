using System.ComponentModel.DataAnnotations;
using TrackingSystem.App.Models.Enums;

namespace TrackingSystem.App.Models;

public class Asset
{
    public int Id { get; set; }

    public AssetType Type { get; set; }

    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string ModelName { get; set; } = string.Empty;

    public DateTime PurchaseDate { get; set; }

    public decimal PurchasePriceUSD { get; set; }

    public decimal LocalPrice { get; set; }

    [Required]
    [MaxLength(100)]
    public string SerialNumber { get; set; } = string.Empty;

    public DateTime WarrantyExpirationDate { get; set; }

    // Relationship with Office
    public int OfficeId { get; set; }
    public Office Office { get; set; } = null!;

    // Relationship with Employee (optional - may not be assigned)
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    // Navigation property - An asset can have many maintenance records
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();

    // Calculated properties
    public DateTime EndOfLifeDate => PurchaseDate.AddYears(3);

    public int MonthsRemaining
    {
        get
        {
            var totalDays = (EndOfLifeDate - DateTime.Now).TotalDays;
            return (int)(totalDays / 30);
        }
    }

    public AssetStatus GetLifecycleStatus()
    {
        var monthsLeft = MonthsRemaining;

        if (monthsLeft <= 3)
            return AssetStatus.Red;
        else if (monthsLeft <= 6)
            return AssetStatus.Yellow;
        else
            return AssetStatus.Green;
    }
}
