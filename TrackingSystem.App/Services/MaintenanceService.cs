using Microsoft.EntityFrameworkCore;
using TrackingSystem.App.Data;
using TrackingSystem.App.Models;

namespace TrackingSystem.App.Services;

public class MaintenanceService
{
    private readonly AppDbContext _context;

    public MaintenanceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MaintenanceRecord>> GetAllMaintenanceRecordsAsync()
    {
        return await _context.MaintenanceRecords
            .Include(m => m.Asset)
            .ThenInclude(a => a.Office)
            .OrderByDescending(m => m.MaintenanceDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceRecord>> GetByAssetAsync(int assetId)
    {
        return await _context.MaintenanceRecords
            .Include(m => m.Asset)
            .Where(m => m.AssetId == assetId)
            .OrderByDescending(m => m.MaintenanceDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceRecord>> GetUpcomingMaintenanceAsync()
    {
        var today = DateTime.Now;
        var nextMonth = today.AddMonths(1);

        return await _context.MaintenanceRecords
            .Include(m => m.Asset)
            .ThenInclude(a => a.Office)
            .Where(m => m.NextMaintenanceDate >= today && m.NextMaintenanceDate <= nextMonth)
            .OrderBy(m => m.NextMaintenanceDate)
            .ToListAsync();
    }

    public async Task<MaintenanceRecord> AddMaintenanceRecordAsync(MaintenanceRecord record)
    {
        await _context.MaintenanceRecords.AddAsync(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task UpdateMaintenanceRecordAsync(MaintenanceRecord record)
    {
        _context.MaintenanceRecords.Update(record);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMaintenanceRecordAsync(int id)
    {
        var record = await _context.MaintenanceRecords.FindAsync(id);
        if (record != null)
        {
            _context.MaintenanceRecords.Remove(record);
            await _context.SaveChangesAsync();
        }
    }
}
