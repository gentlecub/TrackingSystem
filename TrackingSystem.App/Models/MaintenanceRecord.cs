using System.ComponentModel.DataAnnotations;

namespace TrackingSystem.App.Models;

public class MaintenanceRecord
{
    public int Id { get; set; }

    // Relationship with Asset
    public int AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    public DateTime MaintenanceDate { get; set; }

    public DateTime NextMaintenanceDate { get; set; }

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;

    [MaxLength(150)]
    public string PerformedBy { get; set; } = string.Empty;
}
