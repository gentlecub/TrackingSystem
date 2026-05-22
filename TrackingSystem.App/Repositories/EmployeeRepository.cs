using Microsoft.EntityFrameworkCore;
using TrackingSystem.App.Data;
using TrackingSystem.App.Models;

namespace TrackingSystem.App.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Employee>> GetAllWithAssetsAsync()
    {
        return await _dbSet
            .Include(e => e.AssignedAssets)
            .ThenInclude(a => a.Office)
            .OrderBy(e => e.FullName)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdWithAssetsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.AssignedAssets)
            .ThenInclude(a => a.Office)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Email == email);
    }
}
