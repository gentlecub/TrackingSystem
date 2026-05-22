using TrackingSystem.App.Models;

namespace TrackingSystem.App.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> GetAllWithAssetsAsync();
    Task<Employee?> GetByIdWithAssetsAsync(int id);
    Task<Employee?> GetByEmailAsync(string email);
}
