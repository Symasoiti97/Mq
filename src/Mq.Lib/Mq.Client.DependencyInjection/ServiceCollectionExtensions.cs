using Microsoft.Extensions.DependencyInjection;

namespace Mq.Client.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация зависимостей MQ
    /// <list type="bullet">
    ///     <item>Регестирует <see cref="IMqSender"/>, как <see cref="ServiceLifetime.Singleton"/></item>
    ///     <item>Регестирует <see cref="gRpc.Mq.MqClient"/>, как <see cref="ServiceLifetime.Transient"/></item>
    /// </list>
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация mq</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddMq(this IServiceCollection services, Action<MqConfig> configuration)
    {
        var mqConfig = new MqConfig();

        services.AddSingleton<IMqClient, MqClient>();
        services.AddSingleton<IMqSender>(provider => provider.GetRequiredService<IMqClient>());
        services.AddSingleton<IMqReceiver>(provider => provider.GetRequiredService<IMqClient>());
        services.AddGrpcClient<gRpc.Mq.MqClient>(o => o.Address = mqConfig.Address);
        services.AddSingleton(mqConfig);

        configuration(mqConfig);

        if (mqConfig.Consumers.Any())
        {
            services.AddHostedService<MqBackgroundService>();

            foreach (var mqConfigConsumer in mqConfig.Consumers)
            {
                services.AddTransient(mqConfigConsumer.Value);
            }
        }

        return services;
    }
}