using System.Diagnostics.CodeAnalysis;
using gRpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mq.Client;

internal class MqClient : IMqClient
{
    private readonly gRpc.Mq.MqClient _gRpcMqClient;
    private readonly MqConfig _mqConfig;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<MqClient> _logger;

    public MqClient(gRpc.Mq.MqClient gRpcMqClient, MqConfig mqConfig, IServiceScopeFactory serviceScopeFactory, ILogger<MqClient> logger)
    {
        _gRpcMqClient = gRpcMqClient;
        _mqConfig = mqConfig;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        RegistrationQueues();
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
                var receiveMessageRequest = new ReceiveMessageRequest { Queue = consumerQueue };
                await call.RequestStream.WriteAsync(receiveMessageRequest, cancellationToken);
                await call.ResponseStream.MoveNext(cancellationToken);

                var receiveMessageResponse = call.ResponseStream.Current;

                if (receiveMessageResponse.IsEmpty == false)
                {
                    using var serviceScope = _serviceScopeFactory.CreateScope();
                    var consumer = (IMqConsumer)serviceScope.ServiceProvider.GetRequiredService(consumerType);
                    await consumer.ExecuteAsync(receiveMessageResponse.Message, cancellationToken);

                    _logger.LogInformation("Receive message! MessageId \'{MessageId}\'\tMessage: \'{Message}\'", receiveMessageResponse.MessageId,
                        receiveMessageResponse.Message);
                }

                var confirmReceiveMessageRequest = new ReceiveMessageRequest { Queue = consumerQueue, MessageId = receiveMessageResponse.MessageId };
                await call.RequestStream.WriteAsync(confirmReceiveMessageRequest, cancellationToken);
                await call.ResponseStream.MoveNext(cancellationToken);

                await Task.Delay(_mqConfig.Interval, cancellationToken);
            }
        }
    }

    private void RegistrationQueues()
    {
        foreach (var queue in _mqConfig.Queues)
        {
            _gRpcMqClient.RegistryQueue(new RegistryQueueRequest { Queue = queue });
        }
    }
}