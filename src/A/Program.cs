using Mq.Client.DependencyInjection;

namespace A;

internal static class Program
{
    public const string ConsoleOutMessageQueue = "ConsoleOutMessage";

    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddMq(config =>
                {
                    config
                        .SetAddress(new Uri(context.Configuration["ConnectionStrings:MqUri"]))
                        .RegisterQueue(ConsoleOutMessageQueue);
                });
                services.AddHostedService<Worker>();
            })
            .Build();

        await host.RunAsync();
    }
}