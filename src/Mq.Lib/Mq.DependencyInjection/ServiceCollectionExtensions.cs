using Microsoft.Extensions.DependencyInjection;
using Mq.Server;

namespace Mq.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация зависимостей MQ
    /// <list type="bullet">
    ///     <item>Регестирует <see cref="IMqService"/>, как <see cref="ServiceLifetime.Singleton"/></item>
    /// </list>
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="config">Конфигурация mq</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddMq(this IServiceCollection services, Action<MqConfig> config)
    {
        var mqConfig = new MqConfig();

        config.Invoke(mqConfig);

        if (mqConfig.IsUseInMemory)
        {
            services.AddSingleton<IMqService, InMemoryMqService>();
        }

        return services;
    }
}