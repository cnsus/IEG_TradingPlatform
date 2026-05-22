using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using ProductODataService.Models;
using ProductODataService.Services;

var builder = WebApplication.CreateBuilder(args);

// OData EDM (Entity Data Model) aufbauen
// Definiert die Entitaeten und deren Beziehungen fuer das OData-Protokoll
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Product>("Products");

// Controller mit OData-Unterstuetzung registrieren
// EnableQueryFeatures aktiviert $filter, $orderby, $top, $skip, $count, $select
builder.Services.AddControllers()
    .AddOData(options => options
        .EnableQueryFeatures()
        .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository als Service registrieren (Decentralized Data Management)
builder.Services.AddScoped<IProductODataRepository, ProductODataRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
