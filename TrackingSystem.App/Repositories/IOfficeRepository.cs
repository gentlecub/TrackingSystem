using TrackingSystem.App.Models;

namespace TrackingSystem.App.Repositories;

public interface IOfficeRepository : IRepository<Office>
{
    Task<IEnumerable<Office>> GetAllWithAssetsAsync();
    Task<Office?> GetByIdWithAssetsAsync(int id);
}
