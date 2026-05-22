using TrackingSystem.App.Models;
using TrackingSystem.App.Repositories;

namespace TrackingSystem.App.Services;

public class AssetService
{
    private readonly IAssetRepository _assetRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly CurrencyService _currencyService;

    public AssetService(
        IAssetRepository assetRepository,
        IOfficeRepository officeRepository,
        CurrencyService currencyService)
    {
        _assetRepository = assetRepository;
        _officeRepository = officeRepository;
        _currencyService = currencyService;
    }

    public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
    {
        return await _assetRepository.GetAllWithRelationsAsync();
    }

    public async Task<Asset?> GetAssetByIdAsync(int id)
    {
        return await _assetRepository.GetByIdWithRelationsAsync(id);
    }

    public async Task<Asset> CreateAssetAsync(Asset asset)
    {
        // Validate that serial number doesn't exist
        var existing = await _assetRepository.GetBySerialNumberAsync(asset.SerialNumber);
        if (existing != null)
        {
            throw new Exception($"An asset with serial number already exists: {asset.SerialNumber}");
        }

        // Validate price
        if (asset.PurchasePriceUSD <= 0)
        {
            throw new Exception("Price must be greater than 0");
        }

        // Calculate local price
        asset.LocalPrice = await _currencyService.ConvertToLocalAsync(
            asset.PurchasePriceUSD,
            asset.OfficeId
        );

        return await _assetRepository.AddAsync(asset);
    }

    public async Task UpdateAssetAsync(Asset asset)
    {
        // Recalculate local price if USD price changed
        asset.LocalPrice = await _currencyService.ConvertToLocalAsync(
            asset.PurchasePriceUSD,
            asset.OfficeId
        );

        await _assetRepository.UpdateAsync(asset);
    }

    public async Task DeleteAssetAsync(int id)
    {
        await _assetRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Asset>> GetAssetsByOfficeAsync(int officeId)
    {
        return await _assetRepository.GetByOfficeAsync(officeId);
    }

    public async Task<IEnumerable<Asset>> GetExpiringAssetsAsync()
    {
        return await _assetRepository.GetExpiringAssetsAsync(6);
    }

    public async Task<IEnumerable<Asset>> GetAssetsByEmployeeAsync(int employeeId)
    {
        return await _assetRepository.GetByEmployeeAsync(employeeId);
    }

    public async Task AssignToEmployeeAsync(int assetId, int employeeId)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId);
        if (asset == null)
        {
            throw new Exception("Asset not found");
        }

        asset.EmployeeId = employeeId;
        await _assetRepository.UpdateAsync(asset);
    }

    public async Task UnassignFromEmployeeAsync(int assetId)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId);
        if (asset == null)
        {
            throw new Exception("Asset not found");
        }

        asset.EmployeeId = null;
        await _assetRepository.UpdateAsync(asset);
    }

    public async Task<IEnumerable<Asset>> GetUnassignedAssetsAsync()
    {
        return await _assetRepository.GetUnassignedAssetsAsync();
    }
}
