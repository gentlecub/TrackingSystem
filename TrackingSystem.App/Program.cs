using Microsoft.EntityFrameworkCore;
using TrackingSystem.App.Data;
using TrackingSystem.App.Repositories;
using TrackingSystem.App.Services;
using TrackingSystem.App.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;

try
{
    // Crear el contexto de base de datos
    var context = new AppDbContext();

    // Mostrar mensaje de inicio
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("Iniciando Sistema de Seguimiento de Activos...");
    Console.ResetColor();

    // Aplicar migraciones pendientes y crear base de datos
    Console.WriteLine("Verificando base de datos...");
    await context.Database.MigrateAsync();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Base de datos lista!");
    Console.ResetColor();

    // Crear repositories
    var assetRepository = new AssetRepository(context);
    var employeeRepository = new EmployeeRepository(context);
    var officeRepository = new OfficeRepository(context);

    // Crear services
    var authService = new AuthService(context);
    var currencyService = new CurrencyService(context);
    var assetService = new AssetService(assetRepository, officeRepository, currencyService);
    var employeeService = new EmployeeService(employeeRepository);
    var officeService = new OfficeService(officeRepository);
    var maintenanceService = new MaintenanceService(context);
    var dashboardService = new DashboardService(context);
    var reportService = new ReportService(context);

    // Crear y ejecutar menu
    var menuManager = new MenuManager(
        authService,
        assetService,
        employeeService,
        officeService,
        maintenanceService,
        dashboardService,
        reportService
    );

    await menuManager.RunAsync();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nError critico: {ex.Message}");
    Console.WriteLine("\nDetalles del error:");
    Console.WriteLine(ex.ToString());
    Console.ResetColor();
    Console.WriteLine("\nPresione cualquier tecla para salir...");
    Console.ReadKey();
}
