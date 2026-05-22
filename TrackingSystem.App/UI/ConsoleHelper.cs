using TrackingSystem.App.Models;
using TrackingSystem.App.Models.Enums;

namespace TrackingSystem.App.UI;

public static class ConsoleHelper
{
    public static void PrintHeader(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║{title.PadLeft(30 + title.Length / 2).PadRight(60)}║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    public static void PrintSubHeader(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"=== {title} ===");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintAssetTable(IEnumerable<Asset> assets)
    {
        Console.WriteLine();
        Console.WriteLine("┌──────┬────────────────┬──────────┬────────────────────┬────────────┬────────────────┬────────┐");
        Console.WriteLine("│  ID  │      Type      │   Brand  │       Model        │   Price    │     Office     │ Status │");
        Console.WriteLine("├──────┼────────────────┼──────────┼────────────────────┼────────────┼────────────────┼────────┤");

        foreach (var asset in assets)
        {
            var status = asset.GetLifecycleStatus();
            var statusColor = status switch
            {
                AssetStatus.Red => ConsoleColor.Red,
                AssetStatus.Yellow => ConsoleColor.Yellow,
                _ => ConsoleColor.Green
            };

            Console.Write($"│ {asset.Id,4} ");
            Console.Write($"│ {Truncate(asset.Type.ToString(), 14),-14} ");
            Console.Write($"│ {Truncate(asset.Brand, 8),-8} ");
            Console.Write($"│ {Truncate(asset.ModelName, 18),-18} ");
            Console.Write($"│ ${asset.PurchasePriceUSD,9:N2} ");
            Console.Write($"│ {Truncate(asset.Office?.Name ?? "N/A", 14),-14} ");

            Console.ForegroundColor = statusColor;
            Console.Write($"│ {status,-6} ");
            Console.ResetColor();
            Console.WriteLine("│");
        }

        Console.WriteLine("└──────┴────────────────┴──────────┴────────────────────┴────────────┴────────────────┴────────┘");
    }

    public static void PrintAssetDetails(Asset asset)
    {
        var status = asset.GetLifecycleStatus();
        var statusColor = status switch
        {
            AssetStatus.Red => ConsoleColor.Red,
            AssetStatus.Yellow => ConsoleColor.Yellow,
            _ => ConsoleColor.Green
        };

        Console.WriteLine($"  ID: {asset.Id}");
        Console.WriteLine($"  Type: {asset.Type}");
        Console.WriteLine($"  Brand: {asset.Brand}");
        Console.WriteLine($"  Model: {asset.ModelName}");
        Console.WriteLine($"  Serial: {asset.SerialNumber}");
        Console.WriteLine($"  Price USD: ${asset.PurchasePriceUSD:N2}");
        Console.WriteLine($"  Local Price: {asset.LocalPrice:N2} {asset.Office?.CurrencyCode}");
        Console.WriteLine($"  Purchase Date: {asset.PurchaseDate:yyyy-MM-dd}");
        Console.WriteLine($"  Warranty Expires: {asset.WarrantyExpirationDate:yyyy-MM-dd}");
        Console.WriteLine($"  End of Life: {asset.EndOfLifeDate:yyyy-MM-dd}");
        Console.WriteLine($"  Months Remaining: {asset.MonthsRemaining}");
        Console.Write($"  Status: ");
        Console.ForegroundColor = statusColor;
        Console.WriteLine(status);
        Console.ResetColor();
        Console.WriteLine($"  Office: {asset.Office?.Name ?? "N/A"}");
        Console.WriteLine($"  Assigned to: {asset.Employee?.FullName ?? "Unassigned"}");
    }

    public static void PrintEmployeeTable(IEnumerable<Employee> employees)
    {
        Console.WriteLine();
        Console.WriteLine("┌──────┬──────────────────────────┬────────────────┬──────────────────────────────┬──────────┐");
        Console.WriteLine("│  ID  │          Name            │   Department   │            Email             │  Assets  │");
        Console.WriteLine("├──────┼──────────────────────────┼────────────────┼──────────────────────────────┼──────────┤");

        foreach (var emp in employees)
        {
            Console.Write($"│ {emp.Id,4} ");
            Console.Write($"│ {Truncate(emp.FullName, 24),-24} ");
            Console.Write($"│ {Truncate(emp.Department, 14),-14} ");
            Console.Write($"│ {Truncate(emp.Email, 28),-28} ");
            Console.Write($"│ {emp.AssignedAssets.Count,8} ");
            Console.WriteLine("│");
        }

        Console.WriteLine("└──────┴──────────────────────────┴────────────────┴──────────────────────────────┴──────────┘");
    }

    public static void PrintOfficeTable(IEnumerable<Office> offices)
    {
        Console.WriteLine();
        Console.WriteLine("┌──────┬──────────────────────┬────────────────┬──────────┬──────────┬────────────────┬──────────┐");
        Console.WriteLine("│  ID  │         Name         │    Country     │   City   │ Currency │  Total Value   │  Assets  │");
        Console.WriteLine("├──────┼──────────────────────┼────────────────┼──────────┼──────────┼────────────────┼──────────┤");

        foreach (var office in offices)
        {
            var totalValue = office.Assets.Sum(a => a.PurchasePriceUSD);
            Console.Write($"│ {office.Id,4} ");
            Console.Write($"│ {Truncate(office.Name, 20),-20} ");
            Console.Write($"│ {Truncate(office.Country, 14),-14} ");
            Console.Write($"│ {Truncate(office.City, 8),-8} ");
            Console.Write($"│ {office.CurrencyCode,-8} ");
            Console.Write($"│ ${totalValue,13:N2} ");
            Console.Write($"│ {office.Assets.Count,8} ");
            Console.WriteLine("│");
        }

        Console.WriteLine("└──────┴──────────────────────┴────────────────┴──────────┴──────────┴────────────────┴──────────┘");
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[OK] {message}");
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.ResetColor();
    }

    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARNING] {message}");
        Console.ResetColor();
    }

    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[INFO] {message}");
        Console.ResetColor();
    }

    public static void WaitForKey()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public static string ReadString(string prompt, bool required = true)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim() ?? "";

            if (!required || !string.IsNullOrEmpty(input))
                return input;

            PrintError("This field is required.");
        }
    }

    public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value))
            {
                if (value >= min && value <= max)
                    return value;

                PrintError($"Value must be between {min} and {max}");
            }
            else
            {
                PrintError("Please enter a valid number");
            }
        }
    }

    public static decimal ReadDecimal(string prompt, decimal min = 0)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out decimal value))
            {
                if (value >= min)
                    return value;

                PrintError($"Value must be greater than or equal to {min}");
            }
            else
            {
                PrintError("Please enter a valid number");
            }
        }
    }

    public static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out DateTime value))
            {
                return value;
            }
            else
            {
                PrintError("Please enter a valid date (e.g.: 2024-01-15)");
            }
        }
    }

    public static T ReadEnum<T>(string prompt) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        Console.WriteLine(prompt);
        for (int i = 0; i < values.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {values[i]}");
        }

        while (true)
        {
            Console.Write("Select an option: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= values.Length)
            {
                return values[choice - 1];
            }
            PrintError("Invalid option");
        }
    }

    public static bool Confirm(string message)
    {
        Console.Write($"{message} (y/n): ");
        var response = Console.ReadLine()?.Trim().ToLower();
        return response == "y" || response == "yes";
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value ?? "";
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 2) + "..";
    }
}
