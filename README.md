# Sistema de Seguimiento de Activos Empresariales

## Tabla de Contenidos
1. [Introduccion](#introduccion)
2. [Tecnologias Utilizadas](#tecnologias-utilizadas)
3. [Conceptos Clave para Juniors](#conceptos-clave-para-juniors)
4. [Estructura del Proyecto](#estructura-del-proyecto)
5. [Modelos de Datos](#modelos-de-datos)
6. [Configuracion del Proyecto](#configuracion-del-proyecto)
7. [Entity Framework Core](#entity-framework-core)
8. [Patron Repository y Services](#patron-repository-y-services)
9. [Funcionalidades del Sistema](#funcionalidades-del-sistema)
10. [Guia Paso a Paso](#guia-paso-a-paso)

---

## Introduccion

### Que es este proyecto?

Este proyecto es un **Sistema de Seguimiento de Activos** (Asset Tracking System) que ayuda a las empresas a llevar un registro de sus equipos fisicos como:

- Laptops
- Computadoras de escritorio
- Telefonos moviles
- Tablets
- Equipos de oficina

### Por que es util?

Imagina que trabajas en una empresa con 500 empleados. Cada uno tiene una laptop, algunos tienen telefonos corporativos, hay impresoras en cada piso... Como sabes:
- Que equipos tiene cada empleado?
- Cuando vence la garantia de cada equipo?
- Cuanto dinero tiene invertido cada oficina en equipos?
- Que equipos necesitan mantenimiento pronto?

Este sistema resuelve todos esos problemas!

---

## Tecnologias Utilizadas

### C# (.NET Console Application)
```
C# es el lenguaje de programacion que usamos.
.NET es el framework (herramientas) que nos da Microsoft.
Console Application significa que corre en la terminal/consola.
```

### Entity Framework Core (EF Core)
```
Es un ORM (Object-Relational Mapper).
En palabras simples: te permite trabajar con bases de datos
usando clases de C# en lugar de escribir SQL directamente.
```

### SQL Database (SQLite o SQL Server)
```
Es donde guardamos los datos permanentemente.
SQLite: base de datos en un archivo (facil para desarrollo)
SQL Server: base de datos empresarial (para produccion)
```

### LINQ (Language Integrated Query)
```
Es una forma de hacer consultas a datos usando C#.
En lugar de escribir SQL, escribes codigo C# que parece casi ingles.
```

---

## Conceptos Clave para Juniors

### Que es OOP (Programacion Orientada a Objetos)?

OOP es una forma de organizar tu codigo usando "objetos" que representan cosas del mundo real.

```csharp
// Una CLASE es como un molde/plantilla
public class Asset
{
    public string Brand { get; set; }      // Propiedad
    public decimal Price { get; set; }     // Propiedad

    public void ShowInfo()                 // Metodo
    {
        Console.WriteLine($"Marca: {Brand}, Precio: {Price}");
    }
}

// Un OBJETO es una instancia de esa clase
var miLaptop = new Asset();
miLaptop.Brand = "Dell";
miLaptop.Price = 1200.00m;
```

**Los 4 pilares de OOP:**

1. **Encapsulamiento**: Esconder los detalles internos
   ```csharp
   private decimal _price;  // privado, nadie puede acceder directamente
   public decimal Price     // publico, pero controlamos como se accede
   {
       get { return _price; }
       set { if (value > 0) _price = value; }  // validamos antes de guardar
   }
   ```

2. **Herencia**: Una clase puede heredar de otra
   ```csharp
   public class Laptop : Asset  // Laptop hereda todo de Asset
   {
       public int RamGB { get; set; }  // y agrega sus propias propiedades
   }
   ```

3. **Polimorfismo**: Un objeto puede tomar muchas formas
   ```csharp
   Asset miActivo = new Laptop();  // Un Laptop ES un Asset
   ```

4. **Abstraccion**: Mostrar solo lo necesario, esconder la complejidad

### Que es CRUD?

CRUD son las 4 operaciones basicas con datos:

| Letra | Operacion | SQL | Ejemplo |
|-------|-----------|-----|---------|
| C | Create | INSERT | Agregar un nuevo activo |
| R | Read | SELECT | Ver lista de activos |
| U | Update | UPDATE | Modificar un activo |
| D | Delete | DELETE | Eliminar un activo |

---

## Estructura del Proyecto

```
TrackingSystem/
│
├── TrackingSystem.sln              # Archivo de solucion
│
├── TrackingSystem/                 # Proyecto principal
│   ├── Program.cs                  # Punto de entrada
│   ├── TrackingSystem.csproj       # Configuracion del proyecto
│   │
│   ├── Models/                     # Entidades/Clases de datos
│   │   ├── Asset.cs
│   │   ├── Employee.cs
│   │   ├── Office.cs
│   │   ├── User.cs
│   │   ├── MaintenanceRecord.cs
│   │   └── Enums/
│   │       ├── AssetType.cs
│   │       └── UserRole.cs
│   │
│   ├── Data/                       # Capa de datos
│   │   ├── AppDbContext.cs         # Configuracion de EF Core
│   │   └── Migrations/             # Migraciones de base de datos
│   │
│   ├── Repositories/               # Patron Repository
│   │   ├── IRepository.cs          # Interfaz generica
│   │   ├── Repository.cs           # Implementacion generica
│   │   ├── IAssetRepository.cs
│   │   └── AssetRepository.cs
│   │
│   ├── Services/                   # Logica de negocio
│   │   ├── AssetService.cs
│   │   ├── EmployeeService.cs
│   │   ├── AuthService.cs
│   │   ├── CurrencyService.cs
│   │   └── ReportService.cs
│   │
│   └── UI/                         # Interfaz de usuario (consola)
│       ├── MenuManager.cs
│       ├── ConsoleHelper.cs
│       └── TablePrinter.cs
```

### Explicacion de cada carpeta:

**Models/**: Aqui van las clases que representan los datos (tablas en la base de datos)

**Data/**: Todo lo relacionado con la base de datos

**Repositories/**: Clases que manejan el acceso a datos (CRUD basico)

**Services/**: Clases con la logica de negocio (reglas, calculos, validaciones)

**UI/**: Todo lo relacionado con mostrar informacion al usuario

---

## Modelos de Datos

### Asset (Activo)

```csharp
public class Asset
{
    // Id unico - EF Core lo usa como Primary Key automaticamente
    public int Id { get; set; }

    // Tipo de activo (Laptop, Desktop, Phone, etc.)
    public AssetType Type { get; set; }

    // Marca del equipo
    public string Brand { get; set; }

    // Nombre del modelo
    public string ModelName { get; set; }

    // Fecha de compra - importante para calcular vida util
    public DateTime PurchaseDate { get; set; }

    // Precio en dolares (USD)
    public decimal PurchasePriceUSD { get; set; }

    // Precio en moneda local (se calcula automaticamente)
    public decimal LocalPrice { get; set; }

    // Numero de serie unico del equipo
    public string SerialNumber { get; set; }

    // Fecha de expiracion de garantia
    public DateTime WarrantyExpirationDate { get; set; }

    // --- RELACIONES ---

    // Relacion con Office (un activo pertenece a UNA oficina)
    public int OfficeId { get; set; }
    public Office Office { get; set; }

    // Relacion con Employee (un activo puede estar asignado a UN empleado)
    public int? EmployeeId { get; set; }  // ? significa que puede ser null
    public Employee? Employee { get; set; }

    // Un activo puede tener MUCHOS registros de mantenimiento
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; }

    // --- PROPIEDADES CALCULADAS ---

    // Calcula la fecha de fin de vida (3 anos despues de la compra)
    public DateTime EndOfLifeDate => PurchaseDate.AddYears(3);

    // Calcula los meses restantes de vida util
    public int MonthsRemaining => (int)((EndOfLifeDate - DateTime.Now).TotalDays / 30);

    // Determina el estado del activo basado en vida restante
    public AssetStatus GetLifecycleStatus()
    {
        var monthsLeft = MonthsRemaining;

        if (monthsLeft <= 3)
            return AssetStatus.Red;      // URGENTE: menos de 3 meses
        else if (monthsLeft <= 6)
            return AssetStatus.Yellow;   // ADVERTENCIA: menos de 6 meses
        else
            return AssetStatus.Green;    // OK: mas de 6 meses
    }
}
```

### Employee (Empleado)

```csharp
public class Employee
{
    public int Id { get; set; }

    public string FullName { get; set; }

    public string Department { get; set; }

    public string Email { get; set; }

    // Un empleado puede tener MUCHOS activos asignados
    public ICollection<Asset> AssignedAssets { get; set; }

    // Constructor para inicializar la coleccion
    public Employee()
    {
        AssignedAssets = new List<Asset>();
    }
}
```

### Office (Oficina)

```csharp
public class Office
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    // Codigo de moneda local (EUR, SEK, GBP, etc.)
    public string CurrencyCode { get; set; }

    // Tasa de conversion de USD a moneda local
    public decimal ExchangeRate { get; set; }

    // Una oficina tiene MUCHOS activos
    public ICollection<Asset> Assets { get; set; }

    public Office()
    {
        Assets = new List<Asset>();
    }

    // Calcula el valor total de todos los activos en esta oficina
    public decimal GetTotalValue()
    {
        return Assets.Sum(a => a.PurchasePriceUSD);
    }
}
```

### User (Usuario del Sistema)

```csharp
public class User
{
    public int Id { get; set; }

    public string Username { get; set; }

    // En produccion real, NUNCA guardes passwords en texto plano!
    // Esto es solo para simulacion educativa
    public string Password { get; set; }

    public UserRole Role { get; set; }

    // Relacionar con empleado si aplica
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}
```

### MaintenanceRecord (Registro de Mantenimiento)

```csharp
public class MaintenanceRecord
{
    public int Id { get; set; }

    // Relacion con el activo
    public int AssetId { get; set; }
    public Asset Asset { get; set; }

    // Fecha del ultimo mantenimiento
    public DateTime MaintenanceDate { get; set; }

    // Fecha programada para el proximo mantenimiento
    public DateTime NextMaintenanceDate { get; set; }

    // Notas sobre lo que se hizo
    public string Notes { get; set; }

    // Quien realizo el mantenimiento
    public string PerformedBy { get; set; }
}
```

### Enums (Enumeraciones)

```csharp
// Tipo de activo
public enum AssetType
{
    Laptop = 1,
    Desktop = 2,
    MobilePhone = 3,
    Tablet = 4,
    OfficeEquipment = 5
}

// Rol de usuario en el sistema
public enum UserRole
{
    Admin = 1,      // Puede hacer todo
    Manager = 2,    // Puede ver reportes y asignar activos
    Employee = 3    // Solo puede ver sus activos asignados
}

// Estado del ciclo de vida del activo
public enum AssetStatus
{
    Green,   // OK - mas de 6 meses de vida
    Yellow,  // Advertencia - 3 a 6 meses restantes
    Red      // Critico - menos de 3 meses restantes
}
```

---

## Configuracion del Proyecto

### Paso 1: Crear el proyecto

```bash
# Crear carpeta y entrar
mkdir TrackingSystem
cd TrackingSystem

# Crear solucion
dotnet new sln -n TrackingSystem

# Crear proyecto de consola
dotnet new console -n TrackingSystem

# Agregar proyecto a la solucion
dotnet sln add TrackingSystem/TrackingSystem.csproj
```

### Paso 2: Instalar paquetes NuGet

```bash
cd TrackingSystem

# Entity Framework Core (el ORM)
dotnet add package Microsoft.EntityFrameworkCore

# SQLite como base de datos (facil para desarrollo)
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# Herramientas para crear migraciones
dotnet add package Microsoft.EntityFrameworkCore.Design

# (Opcional) Si quieres usar SQL Server en lugar de SQLite:
# dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### Paso 3: Estructura de carpetas

```bash
# Crear las carpetas
mkdir Models
mkdir Models/Enums
mkdir Data
mkdir Repositories
mkdir Services
mkdir UI
```

---

## Entity Framework Core

### Que es el DbContext?

El `DbContext` es la clase principal de Entity Framework. Es como un "puente" entre tu codigo C# y la base de datos.

```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // Cada DbSet representa una TABLA en la base de datos
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Office> Offices { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

    // Constructor que recibe las opciones de configuracion
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Constructor sin parametros para usar SQLite por defecto
    public AppDbContext()
    {
    }

    // Configurar la conexion a la base de datos
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // SQLite guarda todo en un archivo llamado "tracking.db"
            optionsBuilder.UseSqlite("Data Source=tracking.db");
        }
    }

    // Configurar los modelos y relaciones
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar Asset
        modelBuilder.Entity<Asset>(entity =>
        {
            // SerialNumber debe ser unico
            entity.HasIndex(a => a.SerialNumber).IsUnique();

            // Configurar precision para decimales
            entity.Property(a => a.PurchasePriceUSD)
                .HasPrecision(18, 2);

            entity.Property(a => a.LocalPrice)
                .HasPrecision(18, 2);

            // Relacion: Un Asset pertenece a una Office
            entity.HasOne(a => a.Office)
                .WithMany(o => o.Assets)
                .HasForeignKey(a => a.OfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacion: Un Asset puede estar asignado a un Employee (opcional)
            entity.HasOne(a => a.Employee)
                .WithMany(e => e.AssignedAssets)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configurar Office
        modelBuilder.Entity<Office>(entity =>
        {
            entity.Property(o => o.ExchangeRate)
                .HasPrecision(18, 4);
        });

        // Datos iniciales (Seed Data)
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Crear oficinas iniciales
        modelBuilder.Entity<Office>().HasData(
            new Office
            {
                Id = 1,
                Name = "Stockholm HQ",
                Country = "Sweden",
                City = "Stockholm",
                CurrencyCode = "SEK",
                ExchangeRate = 10.5m
            },
            new Office
            {
                Id = 2,
                Name = "London Office",
                Country = "UK",
                City = "London",
                CurrencyCode = "GBP",
                ExchangeRate = 0.79m
            },
            new Office
            {
                Id = 3,
                Name = "Madrid Office",
                Country = "Spain",
                City = "Madrid",
                CurrencyCode = "EUR",
                ExchangeRate = 0.92m
            }
        );

        // Crear usuario admin
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Password = "admin123", // Solo para demo!
                Role = UserRole.Admin
            }
        );
    }
}
```

### Que son las Migraciones?

Las migraciones son como un "historial de cambios" de tu base de datos. Cada vez que cambias tus modelos, creas una migracion que dice "cambie esto".

```bash
# Crear la primera migracion
dotnet ef migrations add InitialCreate

# Aplicar la migracion (crear/actualizar la base de datos)
dotnet ef database update

# Si cambias un modelo, creas otra migracion
dotnet ef migrations add AddedNewField
dotnet ef database update
```

**Ejemplo practico:**

1. Tienes la clase `Asset` con 5 propiedades
2. Ejecutas `dotnet ef migrations add InitialCreate`
3. Se crea la tabla `Assets` con 5 columnas
4. Despues agregas una nueva propiedad `Notes`
5. Ejecutas `dotnet ef migrations add AddNotesToAsset`
6. Ejecutas `dotnet ef database update`
7. La tabla `Assets` ahora tiene 6 columnas

---

## Patron Repository y Services

### Por que usar Repository?

El patron Repository **separa la logica de acceso a datos** de la logica de negocio. Esto hace tu codigo:
- Mas organizado
- Mas facil de probar (testing)
- Mas facil de mantener

### Interfaz Generica IRepository

```csharp
public interface IRepository<T> where T : class
{
    // Obtener todos los registros
    Task<IEnumerable<T>> GetAllAsync();

    // Obtener uno por ID
    Task<T?> GetByIdAsync(int id);

    // Agregar nuevo
    Task<T> AddAsync(T entity);

    // Actualizar existente
    Task UpdateAsync(T entity);

    // Eliminar
    Task DeleteAsync(int id);

    // Guardar cambios
    Task SaveChangesAsync();
}
```

### Implementacion Generica

```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
```

### Repository Especifico para Asset

```csharp
public interface IAssetRepository : IRepository<Asset>
{
    // Metodos especificos para Asset
    Task<IEnumerable<Asset>> GetByOfficeAsync(int officeId);
    Task<IEnumerable<Asset>> GetExpiringAssetsAsync(int monthsThreshold);
    Task<Asset?> GetBySerialNumberAsync(string serialNumber);
    Task<IEnumerable<Asset>> GetByEmployeeAsync(int employeeId);
}

public class AssetRepository : Repository<Asset>, IAssetRepository
{
    public AssetRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Asset>> GetByOfficeAsync(int officeId)
    {
        return await _dbSet
            .Include(a => a.Office)        // Incluir datos de oficina
            .Include(a => a.Employee)      // Incluir datos de empleado
            .Where(a => a.OfficeId == officeId)
            .OrderBy(a => a.PurchaseDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asset>> GetExpiringAssetsAsync(int monthsThreshold)
    {
        var thresholdDate = DateTime.Now.AddMonths(monthsThreshold);

        return await _dbSet
            .Include(a => a.Office)
            .Where(a => a.PurchaseDate.AddYears(3) <= thresholdDate)
            .OrderBy(a => a.PurchaseDate)
            .ToListAsync();
    }

    public async Task<Asset?> GetBySerialNumberAsync(string serialNumber)
    {
        return await _dbSet
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.SerialNumber == serialNumber);
    }

    public async Task<IEnumerable<Asset>> GetByEmployeeAsync(int employeeId)
    {
        return await _dbSet
            .Include(a => a.Office)
            .Where(a => a.EmployeeId == employeeId)
            .ToListAsync();
    }
}
```

### Service Layer (Capa de Servicios)

Los servicios contienen la **logica de negocio**. Usan los repositories para acceder a datos.

```csharp
public class AssetService
{
    private readonly IAssetRepository _assetRepository;
    private readonly CurrencyService _currencyService;

    public AssetService(IAssetRepository assetRepository, CurrencyService currencyService)
    {
        _assetRepository = assetRepository;
        _currencyService = currencyService;
    }

    // Crear nuevo activo con validaciones
    public async Task<Asset> CreateAssetAsync(Asset asset, int officeId)
    {
        // Validar que el serial number no exista
        var existing = await _assetRepository.GetBySerialNumberAsync(asset.SerialNumber);
        if (existing != null)
        {
            throw new Exception($"Ya existe un activo con serial number: {asset.SerialNumber}");
        }

        // Validar precio
        if (asset.PurchasePriceUSD <= 0)
        {
            throw new Exception("El precio debe ser mayor a 0");
        }

        // Calcular precio local
        asset.LocalPrice = await _currencyService.ConvertToLocalAsync(
            asset.PurchasePriceUSD,
            officeId
        );

        return await _assetRepository.AddAsync(asset);
    }

    // Obtener activos que estan por vencer (Yellow y Red)
    public async Task<IEnumerable<Asset>> GetExpiringAssetsAsync()
    {
        return await _assetRepository.GetExpiringAssetsAsync(6);
    }

    // Asignar activo a empleado
    public async Task AssignToEmployeeAsync(int assetId, int employeeId)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId);
        if (asset == null)
        {
            throw new Exception("Activo no encontrado");
        }

        asset.EmployeeId = employeeId;
        await _assetRepository.UpdateAsync(asset);
    }

    // Desasignar activo de empleado
    public async Task UnassignFromEmployeeAsync(int assetId)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId);
        if (asset == null)
        {
            throw new Exception("Activo no encontrado");
        }

        asset.EmployeeId = null;
        await _assetRepository.UpdateAsync(asset);
    }
}
```

### Currency Service (Servicio de Conversion de Moneda)

```csharp
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
            throw new Exception("Oficina no encontrada");
        }

        return amountUSD * office.ExchangeRate;
    }

    public string FormatLocalPrice(decimal amount, string currencyCode)
    {
        return $"{amount:N2} {currencyCode}";
    }
}
```

### Auth Service (Servicio de Autenticacion)

```csharp
public class AuthService
{
    private readonly AppDbContext _context;
    private User? _currentUser;

    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _context.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u =>
                u.Username == username &&
                u.Password == password);

        if (user != null)
        {
            _currentUser = user;
            return true;
        }

        return false;
    }

    public void Logout()
    {
        _currentUser = null;
    }

    public bool HasRole(UserRole requiredRole)
    {
        if (_currentUser == null) return false;

        // Admin puede hacer todo
        if (_currentUser.Role == UserRole.Admin) return true;

        // Manager puede hacer lo de manager y employee
        if (_currentUser.Role == UserRole.Manager && requiredRole != UserRole.Admin)
            return true;

        // Employee solo puede hacer lo de employee
        return _currentUser.Role == requiredRole;
    }
}
```

---

## Funcionalidades del Sistema

### 1. Dashboard (Tablero de Estadisticas)

```csharp
public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStats> GetStatsAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .ToListAsync();

        var stats = new DashboardStats
        {
            TotalAssets = assets.Count,
            TotalValue = assets.Sum(a => a.PurchasePriceUSD),

            // Contar activos por estado
            ExpiringAssets = assets.Count(a => a.GetLifecycleStatus() != AssetStatus.Green),
            RedAssets = assets.Count(a => a.GetLifecycleStatus() == AssetStatus.Red),
            YellowAssets = assets.Count(a => a.GetLifecycleStatus() == AssetStatus.Yellow),

            // Activos por tipo
            AssetsByType = assets
                .GroupBy(a => a.Type)
                .ToDictionary(g => g.Key, g => g.Count()),

            // Valor por oficina
            ValueByOffice = assets
                .GroupBy(a => a.Office.Name)
                .ToDictionary(g => g.Key, g => g.Sum(a => a.PurchasePriceUSD)),

            // Activos por empleado
            AssetsPerEmployee = assets
                .Where(a => a.Employee != null)
                .GroupBy(a => a.Employee!.FullName)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        // Tipo de activo mas usado
        if (stats.AssetsByType.Any())
        {
            stats.MostUsedAssetType = stats.AssetsByType
                .OrderByDescending(x => x.Value)
                .First().Key;
        }

        return stats;
    }
}

public class DashboardStats
{
    public int TotalAssets { get; set; }
    public decimal TotalValue { get; set; }
    public int ExpiringAssets { get; set; }
    public int RedAssets { get; set; }
    public int YellowAssets { get; set; }
    public Dictionary<AssetType, int> AssetsByType { get; set; } = new();
    public Dictionary<string, decimal> ValueByOffice { get; set; } = new();
    public Dictionary<string, int> AssetsPerEmployee { get; set; } = new();
    public AssetType? MostUsedAssetType { get; set; }
}
```

### 2. Reportes con LINQ

```csharp
public class ReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    // Reporte: Activos ordenados por oficina, luego por fecha de compra
    public async Task<IEnumerable<Asset>> GetAssetsReportAsync()
    {
        return await _context.Assets
            .Include(a => a.Office)
            .Include(a => a.Employee)
            .OrderBy(a => a.Office.Name)
            .ThenBy(a => a.PurchaseDate)
            .ToListAsync();
    }

    // Reporte: Activos proximos a vencer con colores
    public async Task<IEnumerable<(Asset Asset, ConsoleColor Color)>> GetExpiringReportAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Office)
            .ToListAsync();

        return assets
            .Where(a => a.GetLifecycleStatus() != AssetStatus.Green)
            .OrderBy(a => a.EndOfLifeDate)
            .Select(a => (
                Asset: a,
                Color: a.GetLifecycleStatus() == AssetStatus.Red
                    ? ConsoleColor.Red
                    : ConsoleColor.Yellow
            ));
    }

    // Reporte: Activos por tipo
    public async Task<Dictionary<AssetType, List<Asset>>> GetAssetsByTypeReportAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Office)
            .ToListAsync();

        return assets
            .GroupBy(a => a.Type)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    // Reporte: Valor total por oficina
    public async Task<IEnumerable<OfficeValueReport>> GetOfficeValueReportAsync()
    {
        return await _context.Offices
            .Include(o => o.Assets)
            .Select(o => new OfficeValueReport
            {
                OfficeName = o.Name,
                Country = o.Country,
                CurrencyCode = o.CurrencyCode,
                TotalValueUSD = o.Assets.Sum(a => a.PurchasePriceUSD),
                TotalValueLocal = o.Assets.Sum(a => a.LocalPrice),
                AssetCount = o.Assets.Count
            })
            .OrderByDescending(r => r.TotalValueUSD)
            .ToListAsync();
    }
}

public class OfficeValueReport
{
    public string OfficeName { get; set; }
    public string Country { get; set; }
    public string CurrencyCode { get; set; }
    public decimal TotalValueUSD { get; set; }
    public decimal TotalValueLocal { get; set; }
    public int AssetCount { get; set; }
}
```

### 3. Menu de Consola

```csharp
public class MenuManager
{
    private readonly AuthService _authService;
    private readonly AssetService _assetService;
    private readonly DashboardService _dashboardService;
    private readonly ReportService _reportService;

    public async Task RunAsync()
    {
        // Mostrar login primero
        await ShowLoginAsync();

        // Menu principal
        while (true)
        {
            Console.Clear();
            ShowHeader();

            Console.WriteLine("\n=== MENU PRINCIPAL ===\n");
            Console.WriteLine("1. Dashboard");
            Console.WriteLine("2. Gestionar Activos");
            Console.WriteLine("3. Gestionar Empleados");
            Console.WriteLine("4. Reportes");
            Console.WriteLine("5. Configuracion");
            Console.WriteLine("0. Salir");

            Console.Write("\nSeleccione una opcion: ");
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
                    await ShowReportsMenuAsync();
                    break;
                case "5":
                    await ShowConfigMenuAsync();
                    break;
                case "0":
                    return;
            }
        }
    }

    private void ShowHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║   SISTEMA DE SEGUIMIENTO DE ACTIVOS    ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.ResetColor();

        if (_authService.IsLoggedIn)
        {
            Console.WriteLine($"Usuario: {_authService.CurrentUser!.Username} ({_authService.CurrentUser.Role})");
        }
    }
}
```

### 4. Mostrar Tablas con Colores

```csharp
public static class ConsoleHelper
{
    public static void PrintAssetTable(IEnumerable<Asset> assets)
    {
        Console.WriteLine();
        Console.WriteLine("┌──────┬──────────┬──────────┬──────────────┬────────────┬──────────────┬────────┐");
        Console.WriteLine("│  ID  │   Tipo   │   Marca  │    Modelo    │   Precio   │   Oficina    │ Estado │");
        Console.WriteLine("├──────┼──────────┼──────────┼──────────────┼────────────┼──────────────┼────────┤");

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
            Console.Write($"│ {asset.Type,-8} ");
            Console.Write($"│ {asset.Brand,-8} ");
            Console.Write($"│ {Truncate(asset.ModelName, 12),-12} ");
            Console.Write($"│ ${asset.PurchasePriceUSD,9:N2} ");
            Console.Write($"│ {Truncate(asset.Office?.Name ?? "N/A", 12),-12} ");

            Console.ForegroundColor = statusColor;
            Console.Write($"│ {status,-6} ");
            Console.ResetColor();
            Console.WriteLine("│");
        }

        Console.WriteLine("└──────┴──────────┴──────────┴──────────────┴────────────┴──────────────┴────────┘");
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 2) + "..";
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"✗ {message}");
        Console.ResetColor();
    }

    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⚠ {message}");
        Console.ResetColor();
    }

    public static void WaitForKey()
    {
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}
```

---

## Guia Paso a Paso

### Paso 1: Configurar el Proyecto

```bash
# 1. Crear carpeta del proyecto
mkdir TrackingSystem
cd TrackingSystem

# 2. Crear solucion y proyecto
dotnet new sln -n TrackingSystem
dotnet new console -n TrackingSystem
dotnet sln add TrackingSystem/TrackingSystem.csproj

# 3. Instalar paquetes
cd TrackingSystem
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design

# 4. Crear estructura de carpetas
mkdir Models Models/Enums Data Repositories Services UI
```

### Paso 2: Crear los Modelos

Crea todos los archivos de modelos en la carpeta `Models/`:
- `Asset.cs`
- `Employee.cs`
- `Office.cs`
- `User.cs`
- `MaintenanceRecord.cs`
- `Enums/AssetType.cs`
- `Enums/UserRole.cs`
- `Enums/AssetStatus.cs`

### Paso 3: Crear el DbContext

Crea `Data/AppDbContext.cs` con toda la configuracion.

### Paso 4: Crear Migracion Inicial

```bash
# Desde la carpeta TrackingSystem/
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Paso 5: Crear Repositories

- `Repositories/IRepository.cs`
- `Repositories/Repository.cs`
- `Repositories/IAssetRepository.cs`
- `Repositories/AssetRepository.cs`

### Paso 6: Crear Services

- `Services/AssetService.cs`
- `Services/AuthService.cs`
- `Services/CurrencyService.cs`
- `Services/DashboardService.cs`
- `Services/ReportService.cs`

### Paso 7: Crear UI

- `UI/MenuManager.cs`
- `UI/ConsoleHelper.cs`

### Paso 8: Configurar Program.cs

```csharp
using Microsoft.EntityFrameworkCore;

// Crear el contexto de base de datos
var context = new AppDbContext();

// Asegurar que la base de datos existe
await context.Database.EnsureCreatedAsync();

// Crear servicios
var authService = new AuthService(context);
var currencyService = new CurrencyService(context);
var assetRepository = new AssetRepository(context);
var assetService = new AssetService(assetRepository, currencyService);
var dashboardService = new DashboardService(context);
var reportService = new ReportService(context);

// Crear y ejecutar menu
var menuManager = new MenuManager(
    authService,
    assetService,
    dashboardService,
    reportService
);

await menuManager.RunAsync();
```

### Paso 9: Ejecutar

```bash
dotnet run
```

---

## Tips y Buenas Practicas

### 1. Manejo de Excepciones

```csharp
try
{
    await assetService.CreateAssetAsync(newAsset, officeId);
    ConsoleHelper.PrintSuccess("Activo creado exitosamente");
}
catch (Exception ex)
{
    ConsoleHelper.PrintError($"Error: {ex.Message}");
}
```

### 2. Validacion de Entrada

```csharp
public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int value))
        {
            if (value >= min && value <= max)
                return value;

            ConsoleHelper.PrintError($"El valor debe estar entre {min} y {max}");
        }
        else
        {
            ConsoleHelper.PrintError("Por favor ingrese un numero valido");
        }
    }
}
```

### 3. Async/Await

```csharp
// CORRECTO: Usar async/await para operaciones de base de datos
public async Task<List<Asset>> GetAssetsAsync()
{
    return await _context.Assets.ToListAsync();
}

// INCORRECTO: Bloquear el hilo principal
public List<Asset> GetAssets()
{
    return _context.Assets.ToList();  // No recomendado en aplicaciones grandes
}
```

### 4. Disposicion de Recursos

```csharp
// Usar 'using' para asegurar que los recursos se liberen
using (var context = new AppDbContext())
{
    // Usar el contexto
} // Se libera automaticamente aqui
```

---

## Glosario de Terminos

| Termino | Significado |
|---------|-------------|
| **ORM** | Object-Relational Mapper - herramienta que mapea objetos a tablas |
| **DbContext** | Clase principal de EF Core que maneja la conexion a la BD |
| **DbSet** | Representa una tabla en la base de datos |
| **Migration** | Archivo que describe cambios en la estructura de la BD |
| **Repository** | Patron que encapsula la logica de acceso a datos |
| **Service** | Capa que contiene la logica de negocio |
| **LINQ** | Language Integrated Query - consultas en C# |
| **Async/Await** | Programacion asincrona para no bloquear el hilo |
| **Seed Data** | Datos iniciales que se insertan automaticamente |
| **Foreign Key** | Clave que relaciona dos tablas |

---

## Recursos Adicionales

- [Documentacion oficial de Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Tutorial de LINQ](https://docs.microsoft.com/dotnet/csharp/linq/)
- [Patron Repository](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)

---

**Autor:** Sistema de Seguimiento de Activos
**Version:** 1.0
**Ultima actualizacion:** Mayo 2026
