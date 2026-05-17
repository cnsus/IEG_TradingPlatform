# Feature: OData Service & Client for Product Catalog (Aufgabe 8)

## 1. Task Summary

Implement a complete **OData Service & Client** scenario for the IEG Trading Platform to fulfill Aufgabe 8 (10 Punkte). The OData service will be a **new standalone microservice** (`ProductODataService`) that exposes the product catalog through a standards-compliant OData v4 endpoint, supporting `$filter`, `$orderby`, `$top`, `$skip`, `$count`, and `$select` query capabilities. The MeiShop API Gateway will gain an **OData client** controller that consumes this service. This demonstrates the OData protocol's value for flexible, SQL-like querying of resources within a microservice architecture — fitting the project's "most wanTED" context of providing queryable data interfaces.

## 2. Context Constraints

### DO:
- **DO** create a new standalone ASP.NET Core Web API project (`ProductODataService`) — following the existing pattern of one project per bounded context.
- **DO** use `Microsoft.AspNetCore.OData` (latest version compatible with `net10.0`) for the OData endpoint.
- **DO** follow the existing project patterns: controller-based routing (`[Route("api/[controller]")]`), repository pattern (`IProductODataRepository` → `ProductODataRepository`), constructor DI, in-memory datastore.
- **DO** use German comments consistent with the existing codebase style.
- **DO** register the new project in `SolTradingPlatform.sln`.
- **DO** use a port that avoids conflicts: **HTTPS 7500 / HTTP 5500** — WAIT, port 5500 is used by LoggingService. Use **HTTPS 7500 / HTTP 5501** instead.
- **DO** add a Swagger UI endpoint consistent with other services.
- **DO** add the service to `start-all.sh` and `start-all.bat`.
- **DO** update `documentation/Aufgabe8.md` with the implementation documentation.

### DO NOT:
- **DO NOT** modify any existing service logic (ProductService, PaymentService, etc.).
- **DO NOT** introduce a real database — use in-memory data matching the existing pattern.
- **DO NOT** use ports already in use (5009, 5229, 5230, 5400, 5500, 7024, 7200, 7231-7233, 7300, 7400, 8500).
- **DO NOT** break backward compatibility of existing API endpoints.

## 3. Dependencies & Prerequisites

- .NET 10 SDK (already installed: `10.0.103`)
- NuGet package: `Microsoft.AspNetCore.OData` (use latest stable or preview compatible with net10.0)
- No other prerequisites.

## 4. Affected Files Overview

### New Files (ProductODataService project):
- `[NEW]` `ProductODataService/ProductODataService.csproj`
- `[NEW]` `ProductODataService/Program.cs`
- `[NEW]` `ProductODataService/Models/Product.cs`
- `[NEW]` `ProductODataService/Models/Category.cs`
- `[NEW]` `ProductODataService/Services/IProductODataRepository.cs`
- `[NEW]` `ProductODataService/Services/ProductODataRepository.cs`
- `[NEW]` `ProductODataService/Controllers/ProductsController.cs`
- `[NEW]` `ProductODataService/Properties/launchSettings.json`
- `[NEW]` `ProductODataService/appsettings.json`
- `[NEW]` `ProductODataService/appsettings.Development.json`

### New Files (OData Client in MeiShop):
- `[NEW]` `MeiShop/Controllers/ODataProductsController.cs`

### Modified Files:
- `[MODIFY]` `SolTradingPlatform.sln` — add new project reference
- `[MODIFY]` `start-all.sh` — add ProductODataService startup
- `[MODIFY]` `start-all.bat` — add ProductODataService startup
- `[MODIFY]` `documentation/Aufgabe8.md` — add implementation documentation
- `[MODIFY]` `Readme.md` — add ProductODataService to the services table

## 5. Step-by-Step Implementation Plan

### Phase 1: Data Layer — ProductODataService Project

#### Step 1.1: Create project file `ProductODataService/ProductODataService.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OData" Version="9.1.1" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.14" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
  </ItemGroup>
</Project>
```
Note: Use the latest stable version of `Microsoft.AspNetCore.OData` that is compatible. If `9.1.1` fails, try `9.0.0` or the latest available. Run `dotnet add package Microsoft.AspNetCore.OData` to resolve the correct version.

#### Step 1.2: Create `ProductODataService/Models/Category.cs`
A simple category enum/class for products:
```csharp
namespace ProductODataService.Models
{
    // Produktkategorie fuer die OData-Filterung
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
```

#### Step 1.3: Create `ProductODataService/Models/Product.cs`
A rich product model with properties suitable for OData querying:
```csharp
namespace ProductODataService.Models
{
    // Produkt-Entitaet fuer den OData-Service
    // Ermoeglicht $filter, $orderby, $select, $top, $skip Abfragen
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string Vendor { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsAvailable { get; set; }
    }
}
```

#### Step 1.4: Create `ProductODataService/Services/IProductODataRepository.cs`
```csharp
namespace ProductODataService.Services
{
    using ProductODataService.Models;

    public interface IProductODataRepository
    {
        IQueryable<Product> GetAll();
        Product? GetById(int id);
    }
}
```

#### Step 1.5: Create `ProductODataService/Services/ProductODataRepository.cs`
In-memory datastore with ~10 sample products across categories:
```csharp
namespace ProductODataService.Services
{
    using ProductODataService.Models;

    // Lokaler Datastore dieses Microservices (Decentralized Data Management)
    // Stellt Produktdaten fuer OData-Abfragen bereit
    public class ProductODataRepository : IProductODataRepository
    {
        private static readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Gaming Laptop", Description = "High-End Gaming Laptop mit RTX 4090", Price = 2499.99m, Category = "Laptops", StockQuantity = 15, Vendor = "TechStore", CreatedAt = new DateTime(2026, 1, 10), IsAvailable = true },
            new Product { Id = 2, Name = "Business Ultrabook", Description = "Leichtes Ultrabook fuer Geschaeftsreisen", Price = 1299.99m, Category = "Laptops", StockQuantity = 30, Vendor = "TechStore", CreatedAt = new DateTime(2026, 1, 15), IsAvailable = true },
            new Product { Id = 3, Name = "Smartphone Pro", Description = "Flagship Smartphone mit 200MP Kamera", Price = 999.99m, Category = "Smartphones", StockQuantity = 50, Vendor = "MobileWorld", CreatedAt = new DateTime(2026, 2, 1), IsAvailable = true },
            new Product { Id = 4, Name = "Budget Phone", Description = "Guenstiges Smartphone fuer den Alltag", Price = 199.99m, Category = "Smartphones", StockQuantity = 100, Vendor = "MobileWorld", CreatedAt = new DateTime(2026, 2, 5), IsAvailable = true },
            new Product { Id = 5, Name = "Wireless Headphones", Description = "Noise-Cancelling Over-Ear Kopfhoerer", Price = 349.99m, Category = "Audio", StockQuantity = 45, Vendor = "AudioMax", CreatedAt = new DateTime(2026, 2, 10), IsAvailable = true },
            new Product { Id = 6, Name = "Smartwatch Elite", Description = "Premium Smartwatch mit Gesundheitstracking", Price = 449.99m, Category = "Wearables", StockQuantity = 25, Vendor = "WearTech", CreatedAt = new DateTime(2026, 3, 1), IsAvailable = true },
            new Product { Id = 7, Name = "Tablet 12 Zoll", Description = "Grosses Tablet fuer Kreative", Price = 799.99m, Category = "Tablets", StockQuantity = 20, Vendor = "TechStore", CreatedAt = new DateTime(2026, 3, 10), IsAvailable = true },
            new Product { Id = 8, Name = "USB-C Hub", Description = "7-in-1 USB-C Docking Station", Price = 59.99m, Category = "Zubehoer", StockQuantity = 200, Vendor = "TechStore", CreatedAt = new DateTime(2026, 3, 15), IsAvailable = true },
            new Product { Id = 9, Name = "Mechanische Tastatur", Description = "RGB Gaming Tastatur mit Cherry MX Switches", Price = 149.99m, Category = "Zubehoer", StockQuantity = 60, Vendor = "AudioMax", CreatedAt = new DateTime(2026, 3, 20), IsAvailable = true },
            new Product { Id = 10, Name = "4K Monitor", Description = "32 Zoll 4K IPS Monitor fuer Profis", Price = 599.99m, Category = "Monitore", StockQuantity = 0, Vendor = "TechStore", CreatedAt = new DateTime(2026, 4, 1), IsAvailable = false }
        };

        public IQueryable<Product> GetAll() => _products.AsQueryable();
        public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);
    }
}
```

### Phase 2: OData Endpoint — Controller & Program.cs

#### Step 2.1: Create `ProductODataService/Controllers/ProductsController.cs`
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ProductODataService.Models;
using ProductODataService.Services;

namespace ProductODataService.Controllers
{
    /// <summary>
    /// OData-faehiger Controller fuer Produktabfragen.
    /// Unterstuetzt $filter, $orderby, $top, $skip, $count, $select.
    /// Beispiele:
    ///   GET /odata/Products?$filter=Price gt 500
    ///   GET /odata/Products?$orderby=Price desc&$top=5
    ///   GET /odata/Products?$filter=Category eq 'Laptops'&$select=Name,Price
    ///   GET /odata/Products?$count=true&$top=3
    ///   GET /odata/Products(1)
    /// </summary>
    public class ProductsController : ODataController
    {
        private readonly IProductODataRepository _repository;

        public ProductsController(IProductODataRepository repository)
        {
            _repository = repository;
        }

        [EnableQuery(PageSize = 20)]
        public IActionResult Get()
        {
            return Ok(_repository.GetAll());
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var product = _repository.GetById(key);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}
```

#### Step 2.2: Create `ProductODataService/Program.cs`
```csharp
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using ProductODataService.Models;
using ProductODataService.Services;

var builder = WebApplication.CreateBuilder(args);

// OData EDM Model aufbauen
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Product>("Products");

// Controller mit OData-Unterstuetzung registrieren
builder.Services.AddControllers()
    .AddOData(options => options
        .EnableQueryFeatures() // Aktiviert $filter, $orderby, $top, $skip, $count, $select
        .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository als Service registrieren
builder.Services.AddScoped<IProductODataRepository, ProductODataRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

#### Step 2.3: Create `ProductODataService/Properties/launchSettings.json`
```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5501",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7500;http://localhost:5501",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

#### Step 2.4: Create `ProductODataService/appsettings.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### Step 2.5: Create `ProductODataService/appsettings.Development.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Phase 3: OData Client — MeiShop Controller

#### Step 3.1: Create `MeiShop/Controllers/ODataProductsController.cs`
An OData client in the MeiShop API Gateway that demonstrates consuming the OData service and forwarding OData query parameters:
```csharp
using Microsoft.AspNetCore.Mvc;

namespace MeiShop.Controllers
{
    /// <summary>
    /// OData Client Controller – konsumiert den ProductODataService.
    /// Leitet OData-Query-Parameter ($filter, $orderby, $top, $skip, $select, $count) weiter.
    /// 
    /// Beispiele:
    ///   GET /api/ODataProducts?$filter=Price gt 500
    ///   GET /api/ODataProducts?$orderby=Price desc&$top=5
    ///   GET /api/ODataProducts?$filter=Category eq 'Laptops'
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ODataProductsController : ControllerBase
    {
        private readonly ILogger<ODataProductsController> _logger;
        private static readonly string odataServiceUrl = "https://localhost:7500/odata/Products";

        public ODataProductsController(ILogger<ODataProductsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Leitet die Anfrage an den OData ProductService weiter.
        /// Alle OData-Query-Parameter werden transparent durchgereicht.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            // OData Query-Parameter aus dem Request uebernehmen
            var queryString = HttpContext.Request.QueryString.Value ?? "";
            var fullUrl = odataServiceUrl + queryString;

            _logger.LogInformation("OData-Client: Weiterleitung an {Url}", fullUrl);

            using var httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

            try
            {
                var response = await httpClient.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("OData-Client: Erfolgreiche Antwort erhalten");
                    return Content(content, "application/json");
                }

                _logger.LogError("OData-Client: Fehler vom Service. Status: {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, new
                {
                    error = "OData Service Error",
                    message = $"Der ProductODataService antwortete mit Status {response.StatusCode}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OData-Client: ProductODataService nicht erreichbar");
                return StatusCode(503, new
                {
                    error = "Service Unavailable",
                    message = "Der ProductODataService ist derzeit nicht erreichbar."
                });
            }
        }
    }
}
```

### Phase 4: Solution Integration

#### Step 4.1: Register project in `SolTradingPlatform.sln`
Run: `dotnet sln SolTradingPlatform.sln add ProductODataService/ProductODataService.csproj`

#### Step 4.2: Update `start-all.sh`
Add the ProductODataService as a new entry (service 9 of 9) before MeiShop, on port 7500.

#### Step 4.3: Update `start-all.bat`
Add equivalent entry for Windows.

### Phase 5: Documentation

#### Step 5.1: Update `documentation/Aufgabe8.md`
Write the complete Ausarbeitung section with:
- OData introduction (what it is, protocol basics)
- Description of the implemented scenario
- Architecture diagram showing how ProductODataService fits
- Example queries and expected results
- Explanation of the OData client in MeiShop

#### Step 5.2: Update `Readme.md`
Add ProductODataService to the services table with port 7500.

## 6. Acceptance Criteria

### Functional:
- [ ] A new `ProductODataService` project exists and compiles without errors.
- [ ] The OData endpoint `GET /odata/Products` returns all products.
- [ ] `$filter` works: e.g., `GET /odata/Products?$filter=Price gt 500` returns only expensive products.
- [ ] `$orderby` works: e.g., `GET /odata/Products?$orderby=Price desc` returns products sorted by price.
- [ ] `$top` and `$skip` work: e.g., `GET /odata/Products?$top=3&$skip=2` returns 3 products starting from the 3rd.
- [ ] `$select` works: e.g., `GET /odata/Products?$select=Name,Price` returns only Name and Price fields.
- [ ] `$count` works: `GET /odata/Products?$count=true` includes the count in the response.
- [ ] Single entity retrieval works: `GET /odata/Products(1)` returns product with Id 1.
- [ ] MeiShop OData client controller `GET /api/ODataProducts` proxies queries to the OData service.
- [ ] The OData service metadata document is accessible at `GET /odata/$metadata`.

### Non-Functional:
- [ ] The entire solution (`dotnet build SolTradingPlatform.sln`) builds without errors.
- [ ] The new service is registered in the solution file.
- [ ] Port 7500/5501 does not conflict with any existing service.
- [ ] `documentation/Aufgabe8.md` is updated with complete documentation.
- [ ] No existing service functionality is broken.
- [ ] Code follows the existing patterns (German comments, controller routing, repository pattern, DI).
