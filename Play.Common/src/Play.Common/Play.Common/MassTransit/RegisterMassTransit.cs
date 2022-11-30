using System.Reflection;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit;

public static class RegisterMassTransit
{
    

    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();
        var serviceSettings = configuration!.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
        var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(rabbitMQSettings!.Host);
                configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings!.ServiceName, false));
            });
        });
        services.AddMassTransitHostedService(); 
        return services;
    }
}