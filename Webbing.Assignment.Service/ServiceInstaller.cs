using Webbing.Assignment.Service.Consumers;

namespace Webbing.Assignment.Service;

public static class ServiceInstaller
{
    public static IServiceCollection AddAppService(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("InMemory");
            //options.LogTo(Console.WriteLine);

        });

        services.AddSingleton<NetworkEventProcessor>();

        services.AddHostedService(provider => new NetworkEventConsumer(
            provider.GetRequiredService<ILogger<NetworkEventConsumer>>(),
            provider.GetRequiredService<IConfiguration>(),
            provider.GetRequiredService<NetworkEventProcessor>()));

        return services;
    }

}