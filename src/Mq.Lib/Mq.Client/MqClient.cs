using gRpc;

namespace Mq.Client;

internal class MqClient : IMqClient
{
    private readonly gRpc.Mq.MqClient _gRpcMqClient;

    public MqClient(gRpc.Mq.MqClient gRpcMqClient, MqConfig mqConfig)
    {
        _gRpcMqClient = gRpcMqClient;

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

    public async Task<string?> Receive(string queue, CancellationToken cancellationToken)
    {
        var result = await _gRpcMqClient.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            Queue = queue
        }, cancellationToken: cancellationToken);

        return result.Message;
    }
}