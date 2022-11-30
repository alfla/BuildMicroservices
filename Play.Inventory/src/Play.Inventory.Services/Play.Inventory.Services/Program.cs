using MassTransit;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;
using Play.Inventory.Services.Client;
using Play.Inventory.Services.Dtos.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var servicesSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();



builder.Services.AddMongo()
    .AddMongoRepository<InventoryItem>(servicesSettings!.InventoryCollection)
    .AddMongoRepository<CatalogItem>(servicesSettings!.CatalogCollection)
    .AddMassTransitWithRabbitMq();

AddCatalogClient(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void AddCatalogClient(WebApplicationBuilder webApplicationBuilder)
{
    Random jitterier = new Random();

    webApplicationBuilder.Services.AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7064");
        })
        .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            5,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                            + TimeSpan.FromMilliseconds(jitterier.Next(0, 1000))))
        .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            3,
            TimeSpan.FromSeconds(15)
        ))
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}