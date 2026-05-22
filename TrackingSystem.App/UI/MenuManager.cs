using TrackingSystem.App.Models;
using TrackingSystem.App.Models.Enums;
using TrackingSystem.App.Services;

namespace TrackingSystem.App.UI;

public class MenuManager
{
    private readonly AuthService _authService;
    private readonly AssetService _assetService;
    private readonly EmployeeService _employeeService;
    private readonly OfficeService _officeService;
    private readonly MaintenanceService _maintenanceService;
    private readonly DashboardService _dashboardService;
    private readonly ReportService _reportService;

    public MenuManager(
        AuthService authService,
        AssetService assetService,
        EmployeeService employeeService,
        OfficeService officeService,
        MaintenanceService maintenanceService,
        DashboardService dashboardService,
        ReportService reportService)
    {
        _authService = authService;
        _assetService = assetService;
        _employeeService = employeeService;
        _officeService = officeService;
        _maintenanceService = maintenanceService;
        _dashboardService = dashboardService;
        _reportService = reportService;
    }

    public async Task RunAsync()
    {
        await ShowLoginAsync();

        while (true)
        {
            ShowMainMenu();
            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await ShowDashboardAsync();
                    break;
                case "2":
                    await ShowAssetMenuAsync();
                    break;
                case "3":
                    await ShowEmployeeMenuAsync();
                    break;
                case "4":
                    await ShowOfficeMenuAsync();
                    break;
                case "5":
                    await ShowMaintenanceMenuAsync();
                    break;
                case "6":
                    await ShowReportsMenuAsync();
                    break;
                case "0":
                    ConsoleHelper.PrintInfo("Goodbye!");
                    return;
                default:
                    ConsoleHelper.PrintError("Invalid option");
                    ConsoleHelper.WaitForKey();
                    break;
            }
        }
    }

    private async Task ShowLoginAsync()
    {
        while (!_authService.IsLoggedIn)
        {
            ConsoleHelper.PrintHeader("ASSET TRACKING SYSTEM");
            ConsoleHelper.PrintSubHeader("Login");

            var username = ConsoleHelper.ReadString("Username: ");
            Console.Write("Password: ");
            var password = ReadPassword();

            if (await _authService.LoginAsync(username, password))
            {
                ConsoleHelper.PrintSuccess($"Welcome, {_authService.CurrentUser!.Username}!");
                ConsoleHelper.WaitForKey();
            }
            else
            {
                ConsoleHelper.PrintError("Invalid username or password");
                ConsoleHelper.WaitForKey();
            }
        }
    }

    private string ReadPassword()
    {
        var password = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);
        Console.WriteLine();
        return password;
    }

    private void ShowMainMenu()
    {
        ConsoleHelper.PrintHeader("ASSET TRACKING SYSTEM");
        Console.WriteLine($"User: {_authService.CurrentUser!.Username} ({_authService.CurrentUser.Role})");
        Console.WriteLine();
        Console.WriteLine("1. Dashboard");
        Console.WriteLine("2. Manage Assets");
        Console.WriteLine("3. Manage Employees");
        Console.WriteLine("4. Manage Offices");
        Console.WriteLine("5. Maintenance");
        Console.WriteLine("6. Reports");
        Console.WriteLine("0. Exit");
        Console.WriteLine();
        Console.Write("Select an option: ");
    }

    // ==================== DASHBOARD ====================
    private async Task ShowDashboardAsync()
    {
        ConsoleHelper.PrintHeader("DASHBOARD");

        var stats = await _dashboardService.GetStatsAsync();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                      GENERAL STATISTICS                       ║");
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.ResetColor();

        Console.WriteLine($"║  Total Assets:          {stats.TotalAssets,6}                              ║");
        Console.WriteLine($"║  Total Value (USD):     ${stats.TotalValueUSD,12:N2}                      ║");
        Console.WriteLine($"║  Total Employees:       {stats.TotalEmployees,6}                              ║");
        Console.WriteLine($"║  Total Offices:         {stats.TotalOffices,6}                              ║");
        Console.WriteLine($"║  Unassigned Assets:     {stats.UnassignedAssets,6}                              ║");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║                       ASSET STATUS                            ║");
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.ResetColor();

        Console.Write("║  ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"Green (OK):           {stats.GreenAssets,6}");
        Console.ResetColor();
        Console.WriteLine("                              ║");

        Console.Write("║  ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"Yellow (Warning):     {stats.YellowAssets,6}");
        Console.ResetColor();
        Console.WriteLine("                              ║");

        Console.Write("║  ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"Red (Critical):       {stats.RedAssets,6}");
        Console.ResetColor();
        Console.WriteLine("                              ║");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║                      ASSETS BY TYPE                           ║");
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.ResetColor();

        foreach (var type in stats.AssetsByType)
        {
            Console.WriteLine($"║  {type.Key,-20}: {type.Value,6}                              ║");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║                     VALUE BY OFFICE                           ║");
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.ResetColor();

        foreach (var office in stats.ValueByOffice)
        {
            Console.WriteLine($"║  {office.Key,-20}: ${office.Value,12:N2}                  ║");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();

        if (stats.MostUsedAssetType.HasValue)
        {
            Console.WriteLine();
            ConsoleHelper.PrintInfo($"Most used asset type: {stats.MostUsedAssetType}");
        }

        ConsoleHelper.WaitForKey();
    }

    // ==================== ACTIVOS ====================
    private async Task ShowAssetMenuAsync()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("ASSET MANAGEMENT");
            Console.WriteLine("1. View all assets");
            Console.WriteLine("2. View asset by ID");
            Console.WriteLine("3. Add new asset");
            Console.WriteLine("4. Edit asset");
            Console.WriteLine("5. Delete asset");
            Console.WriteLine("6. Assign asset to employee");
            Console.WriteLine("7. Unassign asset");
            Console.WriteLine("8. View assets by office");
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Select an option: ");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await ShowAllAssetsAsync();
                    break;
                case "2":
                    await ShowAssetByIdAsync();
                    break;
                case "3":
                    if (_authService.IsManager())
                        await CreateAssetAsync();
                    else
                        ConsoleHelper.PrintError("You don't have permission for this action");
                    ConsoleHelper.WaitForKey();
                    break;
                case "4":
                    if (_authService.IsManager())
                        await EditAssetAsync();
                    else
                        ConsoleHelper.PrintError("You don't have permission for this action");
                    ConsoleHelper.WaitForKey();
                    break;
                case "5":
                    if (_authService.IsAdmin())
                        await DeleteAssetAsync();
                    else
                        ConsoleHelper.PrintError("Only administrators can delete assets");
                    ConsoleHelper.WaitForKey();
                    break;
                case "6":
                    if (_authService.IsManager())
                        await AssignAssetAsync();
                    else
                        ConsoleHelper.PrintError("You don't have permission for this action");
                    ConsoleHelper.WaitForKey();
                    break;
                case "7":
                    if (_authService.IsManager())
                        await UnassignAssetAsync();
                    else
                        ConsoleHelper.PrintError("You don't have permission for this action");
                    ConsoleHelper.WaitForKey();
                    break;
                case "8":
                    await ShowAssetsByOfficeAsync();
                    break;
                case "0":
                    return;
            }
        }
    }

    private async Task ShowAllAssetsAsync()
    {
        ConsoleHelper.PrintSubHeader("ALL ASSETS");
        var assets = await _assetService.GetAllAssetsAsync();
        ConsoleHelper.PrintAssetTable(assets);
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowAssetByIdAsync()
    {
        var id = ConsoleHelper.ReadInt("Enter asset ID: ", 1);
        var asset = await _assetService.GetAssetByIdAsync(id);

        if (asset != null)
        {
            ConsoleHelper.PrintSubHeader("ASSET DETAILS");
            ConsoleHelper.PrintAssetDetails(asset);
        }
        else
        {
            ConsoleHelper.PrintError("Asset not found");
        }
        ConsoleHelper.WaitForKey();
    }

    private async Task CreateAssetAsync()
    {
        ConsoleHelper.PrintSubHeader("ADD NEW ASSET");

        var offices = (await _officeService.GetAllOfficesAsync()).ToList();
        if (!offices.Any())
        {
            ConsoleHelper.PrintError("No offices registered. First add an office.");
            return;
        }

        var asset = new Asset
        {
            Type = ConsoleHelper.ReadEnum<AssetType>("Select asset type:"),
            Brand = ConsoleHelper.ReadString("Brand: "),
            ModelName = ConsoleHelper.ReadString("Model: "),
            SerialNumber = ConsoleHelper.ReadString("Serial Number: "),
            PurchasePriceUSD = ConsoleHelper.ReadDecimal("Price (USD): ", 0.01m),
            PurchaseDate = ConsoleHelper.ReadDate("Purchase date (yyyy-MM-dd): "),
            WarrantyExpirationDate = ConsoleHelper.ReadDate("Warranty expiration date (yyyy-MM-dd): ")
        };

        Console.WriteLine("\nAvailable offices:");
        foreach (var office in offices)
        {
            Console.WriteLine($"  {office.Id}. {office.Name} ({office.City}, {office.Country})");
        }
        asset.OfficeId = ConsoleHelper.ReadInt("Select office (ID): ", 1);

        try
        {
            await _assetService.CreateAssetAsync(asset);
            ConsoleHelper.PrintSuccess("Asset created successfully");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task EditAssetAsync()
    {
        var id = ConsoleHelper.ReadInt("Enter asset ID to edit: ", 1);
        var asset = await _assetService.GetAssetByIdAsync(id);

        if (asset == null)
        {
            ConsoleHelper.PrintError("Asset not found");
            return;
        }

        ConsoleHelper.PrintSubHeader("EDIT ASSET");
        ConsoleHelper.PrintAssetDetails(asset);
        Console.WriteLine();

        Console.WriteLine("Leave empty to keep current value");

        var brand = ConsoleHelper.ReadString($"Brand [{asset.Brand}]: ", false);
        if (!string.IsNullOrEmpty(brand)) asset.Brand = brand;

        var model = ConsoleHelper.ReadString($"Model [{asset.ModelName}]: ", false);
        if (!string.IsNullOrEmpty(model)) asset.ModelName = model;

        Console.Write($"Price USD [{asset.PurchasePriceUSD}]: ");
        var priceStr = Console.ReadLine();
        if (!string.IsNullOrEmpty(priceStr) && decimal.TryParse(priceStr, out var price))
            asset.PurchasePriceUSD = price;

        try
        {
            await _assetService.UpdateAssetAsync(asset);
            ConsoleHelper.PrintSuccess("Asset updated successfully");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task DeleteAssetAsync()
    {
        var id = ConsoleHelper.ReadInt("Enter asset ID to delete: ", 1);
        var asset = await _assetService.GetAssetByIdAsync(id);

        if (asset == null)
        {
            ConsoleHelper.PrintError("Asset not found");
            return;
        }

        ConsoleHelper.PrintAssetDetails(asset);

        if (ConsoleHelper.Confirm("Are you sure you want to delete this asset?"))
        {
            await _assetService.DeleteAssetAsync(id);
            ConsoleHelper.PrintSuccess("Asset deleted successfully");
        }
    }

    private async Task AssignAssetAsync()
    {
        var unassigned = (await _assetService.GetUnassignedAssetsAsync()).ToList();
        if (!unassigned.Any())
        {
            ConsoleHelper.PrintWarning("No unassigned assets");
            return;
        }

        ConsoleHelper.PrintSubHeader("UNASSIGNED ASSETS");
        ConsoleHelper.PrintAssetTable(unassigned);

        var assetId = ConsoleHelper.ReadInt("Enter asset ID to assign: ", 1);

        var employees = (await _employeeService.GetAllEmployeesAsync()).ToList();
        ConsoleHelper.PrintEmployeeTable(employees);

        var employeeId = ConsoleHelper.ReadInt("Enter employee ID: ", 1);

        try
        {
            await _assetService.AssignToEmployeeAsync(assetId, employeeId);
            ConsoleHelper.PrintSuccess("Asset assigned successfully");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task UnassignAssetAsync()
    {
        var id = ConsoleHelper.ReadInt("Enter asset ID to unassign: ", 1);

        try
        {
            await _assetService.UnassignFromEmployeeAsync(id);
            ConsoleHelper.PrintSuccess("Asset unassigned successfully");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task ShowAssetsByOfficeAsync()
    {
        var offices = (await _officeService.GetAllOfficesAsync()).ToList();
        ConsoleHelper.PrintOfficeTable(offices);

        var officeId = ConsoleHelper.ReadInt("Enter office ID: ", 1);
        var assets = await _assetService.GetAssetsByOfficeAsync(officeId);

        ConsoleHelper.PrintSubHeader("OFFICE ASSETS");
        ConsoleHelper.PrintAssetTable(assets);
        ConsoleHelper.WaitForKey();
    }

    // ==================== EMPLEADOS ====================
    private async Task ShowEmployeeMenuAsync()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("EMPLOYEE MANAGEMENT");
            Console.WriteLine("1. View all employees");
            Console.WriteLine("2. View employee by ID");
            Console.WriteLine("3. Add new employee");
            Console.WriteLine("4. Edit employee");
            Console.WriteLine("5. Delete employee");
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Select an option: ");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await ShowAllEmployeesAsync();
                    break;
                case "2":
                    await ShowEmployeeByIdAsync();
                    break;
                case "3":
                    if (_authService.IsManager())
                        await CreateEmployeeAsync();
                    else
                        ConsoleHelper.PrintError("You don't have permission for this action");
                    ConsoleHelper.WaitForKey();
                    break;
                case "4":
                    if (_authService.IsManager())
                        await EditEmployeeAsync();
                    else
                        ConsoleHelper.PrintError("You don't have permission for this action");
                    ConsoleHelper.WaitForKey();
                    break;
                case "5":
                    if (_authService.IsAdmin())
                        await DeleteEmployeeAsync();
                    else
                        ConsoleHelper.PrintError("Only administrators can delete employees");
                    ConsoleHelper.WaitForKey();
                    break;
                case "0":
                    return;
            }
        }
    }

    private async Task ShowAllEmployeesAsync()
    {
        ConsoleHelper.PrintSubHeader("ALL EMPLOYEES");
        var employees = await _employeeService.GetAllEmployeesAsync();
        ConsoleHelper.PrintEmployeeTable(employees);
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowEmployeeByIdAsync()
    {
        var id = ConsoleHelper.ReadInt("Enter employee ID: ", 1);
        var employee = await _employeeService.GetEmployeeByIdAsync(id);

        if (employee != null)
        {
            ConsoleHelper.PrintSubHeader("EMPLOYEE DETAILS");
            Console.WriteLine($"  ID: {employee.Id}");
            Console.WriteLine($"  Name: {employee.FullName}");
            Console.WriteLine($"  Department: {employee.Department}");
            Console.WriteLine($"  Email: {employee.Email}");
            Console.WriteLine($"  Assigned assets: {employee.AssignedAssets.Count}");

            if (employee.AssignedAssets.Any())
            {
                Console.WriteLine("\n  Assets:");
                ConsoleHelper.PrintAssetTable(employee.AssignedAssets);
            }
        }
        else
        {
            ConsoleHelper.PrintError("Employee not found");
        }
        ConsoleHelper.WaitForKey();
    }

    private async Task CreateEmployeeAsync()
    {
        ConsoleHelper.PrintSubHeader("ADD NEW EMPLOYEE");

        var employee = new Employee
        {
            FullName = ConsoleHelper.ReadString("Full name: "),
            Department = ConsoleHelper.ReadString("Department: "),
            Email = ConsoleHelper.ReadString("Email: ")
        };

        try
        {
            await _employeeService.CreateEmployeeAsync(employee);
            ConsoleHelper.PrintSuccess("Employee created successfully");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task EditEmployeeAsync()
    {
        var id = ConsoleHelper.ReadInt("Enter employee ID to edit: ", 1);
        var employee = await _employeeService.GetEmployeeByIdAsync(id);

        if (employee == null)
        {
            ConsoleHelper.PrintError("Employee not found");
            return;
        }

        Console.WriteLine("Leave empty to keep current value");

        var name = ConsoleHelper.ReadString($"Name [{employee.FullName}]: ", false);
        if (!string.IsNullOrEmpty(name)) employee.FullName = name;

        var dept = ConsoleHelper.ReadString($"Department [{employee.Department}]: ", false);
        if (!string.IsNullOrEmpty(dept)) employee.Department = dept;

        var email = ConsoleHelper.ReadString($"Email [{employee.Email}]: ", false);
        if (!string.IsNullOrEmpty(email)) employee.Email = email;

        try
        {
            await _employeeService.UpdateEmployeeAsync(employee);
            ConsoleHelper.PrintSuccess("Employee updated successfully");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task DeleteEmployeeAsync()
    {
        var id = ConsoleHelper.ReadInt("Enter employee ID to delete: ", 1);

        if (ConsoleHelper.Confirm("Are you sure you want to delete this employee?"))
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                ConsoleHelper.PrintSuccess("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError(ex.Message);
            }
        }
    }

    // ==================== OFICINAS ====================
    private async Task ShowOfficeMenuAsync()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("OFFICE MANAGEMENT");
            Console.WriteLine("1. View all offices");
            Console.WriteLine("2. View office by ID");
            Console.WriteLine("3. Add new office");
            Console.WriteLine("4. Edit office");
            Console.WriteLine("5. Delete office");
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Select an option: ");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await ShowAllOfficesAsync();
                    break;
                case "2":
                    await ShowOfficeByIdAsync();
                    break;
                case "3":
                    if (_authService.IsAdmin())
                        await CreateOfficeAsync();
                    else
                        ConsoleHelper.PrintError("Only administrators can add offices");
                    ConsoleHelper.WaitForKey();
                    break;
                case "4":
                    if (_authService.IsAdmin())
                        await EditOfficeAsync();
                    else
                        ConsoleHelper.PrintError("Only administrators can edit offices");
                    ConsoleHelper.WaitForKey();
                    break;
                case "5":
                    if (_authService.IsAdmin())
                        await DeleteOfficeAsync();
                    else
                        ConsoleHelper.PrintError("Only administrators can delete offices");
                    ConsoleHelper.WaitForKey();
                    break;
                case "0":
                    return;
            }
        }
    }

    private async Task ShowAllOfficesAsync()
    {
        ConsoleHelper.PrintSubHeader("ALL OFFICES");
        var offices = await _officeService.GetAllOfficesAsync();
        ConsoleHelper.PrintOfficeTable(offices);
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowOfficeByIdAsync()
    {
        var id = ConsoleHelper.ReadInt("Enter office ID: ", 1);
        var office = await _officeService.GetOfficeByIdAsync(id);

        if (office != null)
        {
            ConsoleHelper.PrintSubHeader("OFFICE DETAILS");
            Console.WriteLine($"  ID: {office.Id}");
            Console.WriteLine($"  Name: {office.Name}");
            Console.WriteLine($"  Country: {office.Country}");
            Console.WriteLine($"  City: {office.City}");
            Console.WriteLine($"  Currency: {office.CurrencyCode}");
            Console.WriteLine($"  Exchange rate: {office.ExchangeRate}");
            Console.WriteLine($"  Total assets: {office.Assets.Count}");
            Console.WriteLine($"  Total value USD: ${office.Assets.Sum(a => a.PurchasePriceUSD):N2}");

            if (office.Assets.Any())
            {
                Console.WriteLine("\n  Assets:");
                ConsoleHelper.PrintAssetTable(office.Assets);
            }
        }
        else
        {
            ConsoleHelper.PrintError("Office not found");
        }
        ConsoleHelper.WaitForKey();
    }

    private async Task CreateOfficeAsync()
    {
        ConsoleHelper.PrintSubHeader("ADD NEW OFFICE");

        var office = new Office
        {
            Name = ConsoleHelper.ReadString("Name: "),
            Country = ConsoleHelper.ReadString("Country: "),
            City = ConsoleHelper.ReadString("City: "),
            CurrencyCode = ConsoleHelper.ReadString("Currency code (e.g.: EUR, SEK, GBP): "),
            ExchangeRate = ConsoleHelper.ReadDecimal("Exchange rate (USD to local): ", 0.0001m)
        };

        try
        {
            await _officeService.CreateOfficeAsync(office);
            ConsoleHelper.PrintSuccess("Office created successfully");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task EditOfficeAsync()
    {
        var id = ConsoleHelper.ReadInt("Enter office ID to edit: ", 1);
        var office = await _officeService.GetOfficeByIdAsync(id);

        if (office == null)
        {
            ConsoleHelper.PrintError("Office not found");
            return;
        }

        Console.WriteLine("Leave empty to keep current value");

        var name = ConsoleHelper.ReadString($"Name [{office.Name}]: ", false);
        if (!string.IsNullOrEmpty(name)) office.Name = name;

        Console.Write($"Exchange rate [{office.ExchangeRate}]: ");
        var rateStr = Console.ReadLine();
        if (!string.IsNullOrEmpty(rateStr) && decimal.TryParse(rateStr, out var rate))
            office.ExchangeRate = rate;

        try
        {
            await _officeService.UpdateOfficeAsync(office);
            ConsoleHelper.PrintSuccess("Office updated successfully");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task DeleteOfficeAsync()
    {
        var id = ConsoleHelper.ReadInt("Enter office ID to delete: ", 1);

        if (ConsoleHelper.Confirm("Are you sure you want to delete this office?"))
        {
            try
            {
                await _officeService.DeleteOfficeAsync(id);
                ConsoleHelper.PrintSuccess("Office deleted successfully");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError(ex.Message);
            }
        }
    }

    // ==================== MANTENIMIENTO ====================
    private async Task ShowMaintenanceMenuAsync()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("MAINTENANCE MANAGEMENT");
            Console.WriteLine("1. View all maintenance records");
            Console.WriteLine("2. View upcoming maintenance");
            Console.WriteLine("3. Add maintenance record");
            Console.WriteLine("4. View maintenance for an asset");
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Select an option: ");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await ShowAllMaintenanceAsync();
                    break;
                case "2":
                    await ShowUpcomingMaintenanceAsync();
                    break;
                case "3":
                    if (_authService.IsManager())
                        await CreateMaintenanceRecordAsync();
                    else
                        ConsoleHelper.PrintError("You don't have permission for this action");
                    ConsoleHelper.WaitForKey();
                    break;
                case "4":
                    await ShowMaintenanceByAssetAsync();
                    break;
                case "0":
                    return;
            }
        }
    }

    private async Task ShowAllMaintenanceAsync()
    {
        ConsoleHelper.PrintSubHeader("MAINTENANCE RECORDS");
        var records = await _maintenanceService.GetAllMaintenanceRecordsAsync();

        Console.WriteLine("┌──────┬────────────┬──────────────────────┬────────────────┬────────────────┐");
        Console.WriteLine("│  ID  │ Asset ID   │        Asset         │     Date       │     Next       │");
        Console.WriteLine("├──────┼────────────┼──────────────────────┼────────────────┼────────────────┤");

        foreach (var record in records)
        {
            Console.WriteLine($"│ {record.Id,4} │ {record.AssetId,10} │ {record.Asset.Brand + " " + record.Asset.ModelName,-20} │ {record.MaintenanceDate:dd/MM/yyyy}     │ {record.NextMaintenanceDate:dd/MM/yyyy}     │");
        }

        Console.WriteLine("└──────┴────────────┴──────────────────────┴────────────────┴────────────────┘");
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowUpcomingMaintenanceAsync()
    {
        ConsoleHelper.PrintSubHeader("UPCOMING MAINTENANCE (30 days)");
        var records = await _maintenanceService.GetUpcomingMaintenanceAsync();

        if (!records.Any())
        {
            ConsoleHelper.PrintInfo("No maintenance scheduled for the next 30 days");
        }
        else
        {
            foreach (var record in records)
            {
                Console.WriteLine($"  [{record.NextMaintenanceDate:dd/MM/yyyy}] {record.Asset.Brand} {record.Asset.ModelName} - {record.Asset.Office.Name}");
            }
        }
        ConsoleHelper.WaitForKey();
    }

    private async Task CreateMaintenanceRecordAsync()
    {
        ConsoleHelper.PrintSubHeader("ADD MAINTENANCE RECORD");

        var assetId = ConsoleHelper.ReadInt("Asset ID: ", 1);
        var asset = await _assetService.GetAssetByIdAsync(assetId);

        if (asset == null)
        {
            ConsoleHelper.PrintError("Asset not found");
            return;
        }

        var record = new MaintenanceRecord
        {
            AssetId = assetId,
            MaintenanceDate = ConsoleHelper.ReadDate("Maintenance date (yyyy-MM-dd): "),
            NextMaintenanceDate = ConsoleHelper.ReadDate("Next maintenance date (yyyy-MM-dd): "),
            Notes = ConsoleHelper.ReadString("Notes: ", false),
            PerformedBy = ConsoleHelper.ReadString("Performed by: ")
        };

        await _maintenanceService.AddMaintenanceRecordAsync(record);
        ConsoleHelper.PrintSuccess("Maintenance record added successfully");
    }

    private async Task ShowMaintenanceByAssetAsync()
    {
        var assetId = ConsoleHelper.ReadInt("Asset ID: ", 1);
        var records = await _maintenanceService.GetByAssetAsync(assetId);

        if (!records.Any())
        {
            ConsoleHelper.PrintInfo("No maintenance records for this asset");
        }
        else
        {
            ConsoleHelper.PrintSubHeader("MAINTENANCE HISTORY");
            foreach (var record in records)
            {
                Console.WriteLine($"  [{record.MaintenanceDate:yyyy-MM-dd}] By: {record.PerformedBy}");
                Console.WriteLine($"    Notes: {record.Notes}");
                Console.WriteLine($"    Next: {record.NextMaintenanceDate:yyyy-MM-dd}");
                Console.WriteLine();
            }
        }
        ConsoleHelper.WaitForKey();
    }

    // ==================== REPORTES ====================
    private async Task ShowReportsMenuAsync()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("REPORTS");
            Console.WriteLine("1. Expiring assets");
            Console.WriteLine("2. Assets by type");
            Console.WriteLine("3. Value by office");
            Console.WriteLine("4. Assets by employee");
            Console.WriteLine("5. Unassigned assets");
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Select an option: ");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await ShowExpiringAssetsReportAsync();
                    break;
                case "2":
                    await ShowAssetsByTypeReportAsync();
                    break;
                case "3":
                    await ShowOfficeValueReportAsync();
                    break;
                case "4":
                    await ShowAssetsByEmployeeReportAsync();
                    break;
                case "5":
                    await ShowUnassignedAssetsReportAsync();
                    break;
                case "0":
                    return;
            }
        }
    }

    private async Task ShowExpiringAssetsReportAsync()
    {
        ConsoleHelper.PrintSubHeader("EXPIRING ASSETS");
        var report = await _reportService.GetExpiringReportAsync();

        if (!report.Any())
        {
            ConsoleHelper.PrintSuccess("No expiring assets");
        }
        else
        {
            foreach (var (asset, color) in report)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"  [{asset.GetLifecycleStatus()}] {asset.Brand} {asset.ModelName}");
                Console.WriteLine($"        Office: {asset.Office.Name}");
                Console.WriteLine($"        End of life: {asset.EndOfLifeDate:yyyy-MM-dd}");
                Console.WriteLine($"        Months remaining: {asset.MonthsRemaining}");
                Console.ResetColor();
                Console.WriteLine();
            }
        }
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowAssetsByTypeReportAsync()
    {
        ConsoleHelper.PrintSubHeader("ASSETS BY TYPE");
        var report = await _reportService.GetAssetsByTypeReportAsync();

        foreach (var group in report)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n{group.Key} ({group.Value.Count} assets)");
            Console.ResetColor();
            ConsoleHelper.PrintAssetTable(group.Value);
        }
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowOfficeValueReportAsync()
    {
        ConsoleHelper.PrintSubHeader("VALUE BY OFFICE");
        var report = await _reportService.GetOfficeValueReportAsync();

        Console.WriteLine("┌──────────────────────┬────────────────┬────────────────────┬──────────┐");
        Console.WriteLine("│        Office        │   Value USD    │   Local Value      │  Assets  │");
        Console.WriteLine("├──────────────────────┼────────────────┼────────────────────┼──────────┤");

        foreach (var office in report)
        {
            Console.WriteLine($"│ {office.OfficeName,-20} │ ${office.TotalValueUSD,13:N2} │ {office.TotalValueLocal,14:N2} {office.CurrencyCode} │ {office.AssetCount,8} │");
        }

        Console.WriteLine("└──────────────────────┴────────────────┴────────────────────┴──────────┘");
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowAssetsByEmployeeReportAsync()
    {
        ConsoleHelper.PrintSubHeader("ASSETS BY EMPLOYEE");
        var report = await _reportService.GetAssetsByEmployeeReportAsync();

        foreach (var group in report)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n{group.Key} ({group.Value.Count} assets)");
            Console.ResetColor();
            ConsoleHelper.PrintAssetTable(group.Value);
        }
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowUnassignedAssetsReportAsync()
    {
        ConsoleHelper.PrintSubHeader("UNASSIGNED ASSETS");
        var report = await _reportService.GetUnassignedAssetsReportAsync();

        if (!report.Any())
        {
            ConsoleHelper.PrintInfo("All assets are assigned");
        }
        else
        {
            ConsoleHelper.PrintAssetTable(report);
        }
        ConsoleHelper.WaitForKey();
    }
}
