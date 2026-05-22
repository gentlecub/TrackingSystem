using TrackingSystem.App.Models;

namespace TrackingSystem.App.Repositories;

public interface IAssetRepository : IRepository<Asset>
{
    Task<IEnumerable<Asset>> GetAllWithRelationsAsync();
    Task<Asset?> GetByIdWithRelationsAsync(int id);
    Task<IEnumerable<Asset>> GetByOfficeAsync(int officeId);
    Task<IEnumerable<Asset>> GetExpiringAssetsAsync(int monthsThreshold);
    Task<Asset?> GetBySerialNumberAsync(string serialNumber);
    Task<IEnumerable<Asset>> GetByEmployeeAsync(int employeeId);
    Task<IEnumerable<Asset>> GetUnassignedAssetsAsync();
}
