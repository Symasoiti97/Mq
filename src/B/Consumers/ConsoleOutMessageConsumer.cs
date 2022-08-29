using System.Diagnostics.CodeAnalysis;
using Mq.Client;

namespace B.Consumers;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ConsoleOutMessageConsumer : IMqConsumer
{
    public Task ExecuteAsync(string message, CancellationToken cancellationToken)
    {
        Console.WriteLine(message);

        return Task.CompletedTask;
    }
}