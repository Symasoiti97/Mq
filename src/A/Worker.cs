using Mq.Client;

namespace A;

public class Worker : BackgroundService
{
    private readonly IMqSender _mqSender;
    private readonly ILogger<Worker> _logger;

    public Worker(IMqSender mqSender, ILogger<Worker> logger)
    {
        _mqSender = mqSender;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

        var random = new Random();
        var i = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            await _mqSender.SendMessage(Program.ConsoleOutMessageQueue, random.Next(0, 100), $"Message {i}", cancellationToken);
            await Task.Delay(random.Next(10, 10_000), cancellationToken);
            i++;
        }
    }
}