using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mq.Client;

internal class MqBackgroundService : BackgroundService
{
    private readonly MqConfig _mqConfig;
    private readonly IMqReceiver _mqReceiver;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MqBackgroundService(MqConfig mqConfig, IMqReceiver mqReceiver, IServiceScopeFactory serviceScopeFactory)
    {
        _mqConfig = mqConfig;
        _mqReceiver = mqReceiver;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var mqConfigQueue in _mqConfig.Consumers)
            {
                var message = await _mqReceiver.Receive(mqConfigQueue.Key, cancellationToken);

                if (message != null)
                {
                    using var serviceScope = _serviceScopeFactory.CreateScope();
                    var consumer = (IMqConsumer) serviceScope.ServiceProvider.GetRequiredService(mqConfigQueue.Value);
                    await consumer.ExecuteAsync(message, cancellationToken);
                }
            }

            await Task.Delay(_mqConfig.Interval, cancellationToken);
        }
    }
}