using B.Consumers;
using Mq.Client.DependencyInjection;

namespace B;

internal static class Program
{
    private const string ConsoleOutMessageQueue = "ConsoleOutMessage";

    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddMq(config =>
                {
                    config
                        .SetAddress(new Uri(context.Configuration["ConnectionStrings:MqUri"]))
                        .SetInterval(TimeSpan.FromSeconds(2))
                        .RegisterConsumer<ConsoleOutMessageConsumer>(ConsoleOutMessageQueue);
                });
                services.AddHostedService<Worker>();
            })
            .Build();

        await host.RunAsync();
    }
}