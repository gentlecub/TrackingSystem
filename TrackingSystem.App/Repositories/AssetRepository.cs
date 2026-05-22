using Microsoft.EntityFrameworkCore;
using TrackingSystem.App.Data;
using TrackingSystem.App.Models;

namespace TrackingSystem.App.Repositories;

public class AssetRepository : Repository<Asset>, IAssetRepository
{
    public AssetRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Asset>> GetAllWithRelationsAsync()
    {
        return await _dbSet
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .Include(a => a.MaintenanceRecords)
            .OrderBy(a => a.Office.Name)
            .ThenBy(a => a.PurchaseDate)
            .ToListAsync();
    }

    public async Task<Asset?> GetByIdWithRelationsAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .Include(a => a.MaintenanceRecords)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Asset>> GetByOfficeAsync(int officeId)
    {
        return await _dbSet
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .Where(a => a.OfficeId == officeId)
            .OrderBy(a => a.PurchaseDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asset>> GetExpiringAssetsAsync(int monthsThreshold)
    {
        var thresholdDate = DateTime.Now.AddMonths(monthsThreshold);

        return await _dbSet
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .Where(a => a.PurchaseDate.AddYears(3) <= thresholdDate)
            .OrderBy(a => a.PurchaseDate)
            .ToListAsync();
    }

    public async Task<Asset?> GetBySerialNumberAsync(string serialNumber)
    {
        return await _dbSet
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.SerialNumber == serialNumber);
    }

    public async Task<IEnumerable<Asset>> GetByEmployeeAsync(int employeeId)
    {
        return await _dbSet
            .Include(a => a.Office)
            .Where(a => a.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asset>> GetUnassignedAssetsAsync()
    {
        return await _dbSet
            .Include(a => a.Office)
            .Where(a => a.EmployeeId == null)
            .ToListAsync();
    }
}
