using PaymentService.Formatters;
using PaymentService.Services;

var builder = WebApplication.CreateBuilder(args);

// Controller mit Content Negotiation konfigurieren
// JSON ist Standard, XML und CSV werden zusaetzlich registriert
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.OutputFormatters.Add(new CsvOutputFormatter());
    options.InputFormatters.Add(new CsvInputFormatter());
})
.AddXmlSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Lokalen Datastore als Service registrieren
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
