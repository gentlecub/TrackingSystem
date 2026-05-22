using TrackingSystem.App.Data;

namespace TrackingSystem.App.Services;

public class CurrencyService
{
    private readonly AppDbContext _context;

    public CurrencyService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> ConvertToLocalAsync(decimal amountUSD, int officeId)
    {
        var office = await _context.Offices.FindAsync(officeId);
        if (office == null)
        {
            throw new Exception("Office not found");
        }

        return amountUSD * office.ExchangeRate;
    }

    public decimal ConvertToLocal(decimal amountUSD, decimal exchangeRate)
    {
        return amountUSD * exchangeRate;
    }

    public string FormatLocalPrice(decimal amount, string currencyCode)
    {
        return $"{amount:N2} {currencyCode}";
    }

    public string FormatUSD(decimal amount)
    {
        return $"${amount:N2} USD";
    }
}
