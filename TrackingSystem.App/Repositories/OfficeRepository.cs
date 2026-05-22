using Microsoft.EntityFrameworkCore;
using TrackingSystem.App.Data;
using TrackingSystem.App.Models;

namespace TrackingSystem.App.Repositories;

public class OfficeRepository : Repository<Office>, IOfficeRepository
{
    public OfficeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Office>> GetAllWithAssetsAsync()
    {
        return await _dbSet
            .Include(o => o.Assets)
            .OrderBy(o => o.Name)
            .ToListAsync();
    }

    public async Task<Office?> GetByIdWithAssetsAsync(int id)
    {
        return await _dbSet
            .Include(o => o.Assets)
            .ThenInclude(a => a.Employee)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
