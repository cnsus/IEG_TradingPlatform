
using MeiShop.Services;
using LoggingService;
using Grpc.Net.Client;

namespace MeiShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // === Aufgabe 3: Resilience + gRPC Logging Setup ===

            // HttpClientFactory registrieren (Best Practice statt new HttpClient())
            builder.Services.AddHttpClient("CreditcardService", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                // Selbstsignierte Zertifikate fuer Entwicklung akzeptieren
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

            // Round-Robin Load Balancer als Singleton (Zustand muss ueber Requests erhalten bleiben)
            var instances = builder.Configuration.GetSection("CreditcardService:Instances").Get<List<string>>()
                            ?? new List<string> { "https://localhost:7231" };
            builder.Services.AddSingleton(new RoundRobinLoadBalancer(instances));

            // gRPC LoggingService Client als Singleton
            var loggingServiceAddress = builder.Configuration.GetValue<string>("LoggingService:Address")
                                        ?? "http://localhost:5500";
            var grpcChannel = GrpcChannel.ForAddress(loggingServiceAddress);
            var grpcClient = new LoggingGrpcService.LoggingGrpcServiceClient(grpcChannel);
            builder.Services.AddSingleton(grpcClient);
            builder.Services.AddSingleton<GrpcLoggingClient>();

            // Resilient Service Caller als Scoped (neuer Caller pro Request)
            builder.Services.AddScoped<ResilientServiceCaller>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
           // if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}