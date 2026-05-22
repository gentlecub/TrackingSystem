using Microsoft.EntityFrameworkCore;
using TrackingSystem.App.Data;
using TrackingSystem.App.Models;
using TrackingSystem.App.Models.Enums;

namespace TrackingSystem.App.Services;

public class OfficeValueReport
{
    public string OfficeName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal TotalValueUSD { get; set; }
    public decimal TotalValueLocal { get; set; }
    public int AssetCount { get; set; }
}

public class ReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    // Report: Assets sorted by office and date
    public async Task<IEnumerable<Asset>> GetAssetsReportAsync()
    {
        return await _context.Assets
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .OrderBy(a => a.Office.Name)
            .ThenBy(a => a.PurchaseDate)
            .ToListAsync();
    }

    // Report: Expiring assets with colors
    public async Task<IEnumerable<(Asset Asset, ConsoleColor Color)>> GetExpiringReportAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .ToListAsync();

        return assets
            .Where(a => a.GetLifecycleStatus() != AssetStatus.Green)
            .OrderBy(a => a.EndOfLifeDate)
            .Select(a => (
                Asset: a,
                Color: a.GetLifecycleStatus() == AssetStatus.Red
                    ? ConsoleColor.Red
                    : ConsoleColor.Yellow
            ));
    }

    // Report: Assets by type
    public async Task<Dictionary<AssetType, List<Asset>>> GetAssetsByTypeReportAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .ToListAsync();

        return assets
            .GroupBy(a => a.Type)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    // Report: Total value by office
    public async Task<IEnumerable<OfficeValueReport>> GetOfficeValueReportAsync()
    {
        return await _context.Offices
            .Include(o => o.Assets)
            .Select(o => new OfficeValueReport
            {
                OfficeName = o.Name,
                Country = o.Country,
                CurrencyCode = o.CurrencyCode,
                TotalValueUSD = o.Assets.Sum(a => a.PurchasePriceUSD),
                TotalValueLocal = o.Assets.Sum(a => a.LocalPrice),
                AssetCount = o.Assets.Count
            })
            .OrderByDescending(r => r.TotalValueUSD)
            .ToListAsync();
    }

    // Report: Assets by employee
    public async Task<Dictionary<string, List<Asset>>> GetAssetsByEmployeeReportAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .Where(a => a.Employee != null)
            .ToListAsync();

        return assets
            .GroupBy(a => a.Employee!.FullName)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    // Report: Unassigned assets
    public async Task<IEnumerable<Asset>> GetUnassignedAssetsReportAsync()
    {
        return await _context.Assets
            .Include(a => a.Office)
            .Where(a => a.EmployeeId == null)
            .OrderBy(a => a.Office.Name)
            .ToListAsync();
    }
}
