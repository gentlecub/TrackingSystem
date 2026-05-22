using System.ComponentModel.DataAnnotations;

namespace TrackingSystem.App.Models;

public class Office
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = string.Empty;

    public decimal ExchangeRate { get; set; }

    // Navigation property - An office has many assets
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();

    // Calculates the total value of all assets in this office
    public decimal GetTotalValueUSD()
    {
        return Assets.Sum(a => a.PurchasePriceUSD);
    }

    public decimal GetTotalValueLocal()
    {
        return Assets.Sum(a => a.LocalPrice);
    }
}
