using System.Diagnostics.CodeAnalysis;
using gRpc;
using Microsoft.Extensions.DependencyInjection;

namespace Mq.Client;

internal class MqClient : IMqClient
{
    private readonly gRpc.Mq.MqClient _gRpcMqClient;
    private readonly MqConfig _mqConfig;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MqClient(gRpc.Mq.MqClient gRpcMqClient, MqConfig mqConfig, IServiceScopeFactory serviceScopeFactory)
    {
        _gRpcMqClient = gRpcMqClient;
        _mqConfig = mqConfig;
        _serviceScopeFactory = serviceScopeFactory;

        RegistrationQueues(mqConfig.Queues).GetAwaiter().GetResult();
    }

    private async Task RegistrationQueues(IEnumerable<string> queues, CancellationToken cancellationToken = default)
    {
        foreach (var queue in queues)
        {
            await _gRpcMqClient.RegistryQueueAsync(new RegistryQueueRequest {Queue = queue}, cancellationToken: cancellationToken);
        }
    }

    public async Task SendMessage(string queue, int priority, string message, CancellationToken cancellationToken)
    {
        await _gRpcMqClient.SendMessageAsync(new SendMessageRequest
        {
            Queue = queue,
            Priority = priority,
            Message = message
        }, cancellationToken: cancellationToken);
    }

    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    public async Task Receive(CancellationToken cancellationToken)
    {
        using var call = _gRpcMqClient.ReceiveMessage(cancellationToken: cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var (consumerQueue, consumerType) in _mqConfig.Consumers)
            {
                await call.RequestStream.WriteAsync(new ReceiveMessageRequest {Queue = consumerQueue}, cancellationToken);
                await call.ResponseStream.MoveNext(cancellationToken);

                var message = call.ResponseStream.Current.Message;

                if (message != null)
                {
                    using var serviceScope = _serviceScopeFactory.CreateScope();
                    var consumer = (IMqConsumer) serviceScope.ServiceProvider.GetRequiredService(consumerType);
                    await consumer.ExecuteAsync(message, cancellationToken);
                }

                await Task.Delay(_mqConfig.Interval, cancellationToken);
            }
        }
    }
}