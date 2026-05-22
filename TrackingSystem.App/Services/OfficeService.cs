using TrackingSystem.App.Models;
using TrackingSystem.App.Repositories;

namespace TrackingSystem.App.Services;

public class OfficeService
{
    private readonly IOfficeRepository _officeRepository;

    public OfficeService(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<IEnumerable<Office>> GetAllOfficesAsync()
    {
        return await _officeRepository.GetAllWithAssetsAsync();
    }

    public async Task<Office?> GetOfficeByIdAsync(int id)
    {
        return await _officeRepository.GetByIdWithAssetsAsync(id);
    }

    public async Task<Office> CreateOfficeAsync(Office office)
    {
        if (office.ExchangeRate <= 0)
        {
            throw new Exception("Exchange rate must be greater than 0");
        }

        return await _officeRepository.AddAsync(office);
    }

    public async Task UpdateOfficeAsync(Office office)
    {
        await _officeRepository.UpdateAsync(office);
    }

    public async Task DeleteOfficeAsync(int id)
    {
        var office = await _officeRepository.GetByIdWithAssetsAsync(id);
        if (office != null && office.Assets.Any())
        {
            throw new Exception("Cannot delete an office with assets. First move or delete the assets.");
        }

        await _officeRepository.DeleteAsync(id);
    }
}
