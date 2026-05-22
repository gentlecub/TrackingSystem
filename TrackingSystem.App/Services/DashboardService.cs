using Microsoft.EntityFrameworkCore;
using TrackingSystem.App.Data;
using TrackingSystem.App.Models.Enums;

namespace TrackingSystem.App.Services;

public class DashboardStats
{
    public int TotalAssets { get; set; }
    public decimal TotalValueUSD { get; set; }
    public int ExpiringAssets { get; set; }
    public int RedAssets { get; set; }
    public int YellowAssets { get; set; }
    public int GreenAssets { get; set; }
    public int TotalEmployees { get; set; }
    public int TotalOffices { get; set; }
    public Dictionary<AssetType, int> AssetsByType { get; set; } = new();
    public Dictionary<string, decimal> ValueByOffice { get; set; } = new();
    public Dictionary<string, int> AssetsPerEmployee { get; set; } = new();
    public AssetType? MostUsedAssetType { get; set; }
    public int UnassignedAssets { get; set; }
}

public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStats> GetStatsAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .ToListAsync();

        var employees = await _context.Employees.ToListAsync();
        var offices = await _context.Offices.ToListAsync();

        var stats = new DashboardStats
        {
            TotalAssets = assets.Count,
            TotalValueUSD = assets.Sum(a => a.PurchasePriceUSD),
            TotalEmployees = employees.Count,
            TotalOffices = offices.Count,

            // Count assets by status
            RedAssets = assets.Count(a => a.GetLifecycleStatus() == AssetStatus.Red),
            YellowAssets = assets.Count(a => a.GetLifecycleStatus() == AssetStatus.Yellow),
            GreenAssets = assets.Count(a => a.GetLifecycleStatus() == AssetStatus.Green),

            UnassignedAssets = assets.Count(a => a.EmployeeId == null),

            // Assets by type
            AssetsByType = assets
                .GroupBy(a => a.Type)
                .ToDictionary(g => g.Key, g => g.Count()),

            // Value by office
            ValueByOffice = assets
                .GroupBy(a => a.Office.Name)
                .ToDictionary(g => g.Key, g => g.Sum(a => a.PurchasePriceUSD)),

            // Assets per employee
            AssetsPerEmployee = assets
                .Where(a => a.Employee != null)
                .GroupBy(a => a.Employee!.FullName)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        stats.ExpiringAssets = stats.RedAssets + stats.YellowAssets;

        // Most used asset type
        if (stats.AssetsByType.Any())
        {
            stats.MostUsedAssetType = stats.AssetsByType
                .OrderByDescending(x => x.Value)
                .First().Key;
        }

        return stats;
    }
}
