using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mq.Client;

internal class MqBackgroundService : BackgroundService
{
    private readonly IMqReceiver _mqReceiver;
    private readonly ILogger<MqBackgroundService> _logger;

    public MqBackgroundService(IMqReceiver mqReceiver, ILogger<MqBackgroundService> logger)
    {
        _mqReceiver = mqReceiver;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MqBackgroundService running at: {Time}", DateTimeOffset.Now);

        await _mqReceiver.Receive(cancellationToken);
    }
}