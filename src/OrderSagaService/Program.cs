using OrderSagaService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Lokalen Datastore als Service registrieren (Singleton, da In-Memory-Speicher)
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

// Saga-Orchestrator als Singleton registrieren
builder.Services.AddSingleton<IOrderSagaOrchestrator, OrderSagaOrchestrator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
