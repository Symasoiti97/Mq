using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Mq.Server.Exceptions;
using Mq.Server.Messages;

namespace Mq.Server;

internal class InMemoryMqService : IMqService
{
    private readonly ILogger<InMemoryMqService> _logger;
    private readonly ConcurrentDictionary<string, ConcurrentPriorityQueue<int, string>> _dictionaryPriorityQueues = new();

    public InMemoryMqService(ILogger<InMemoryMqService> logger)
    {
        _logger = logger;
    }

    public Task RegistryQueue(RegistryQueueRequest registryQueueRequest, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromException(new OperationCanceledException(cancellationToken));
        }

        if (!_dictionaryPriorityQueues.TryAdd(registryQueueRequest.Queue, new ConcurrentPriorityQueue<int, string>()))
        {
            return Task.FromException(new Exception(""));
        }

        return Task.CompletedTask;
    }

    public Task SendMessage(SendMessageRequest sendMessageRequest, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromException(new OperationCanceledException(cancellationToken));
        }

        if (!_dictionaryPriorityQueues.TryGetValue(sendMessageRequest.Queue, out var queue))
        {
            return Task.FromException(new QueueNotExistsException(sendMessageRequest.Queue));
        }

        if (!queue.TryEnqueue(sendMessageRequest.Priority, sendMessageRequest.Message))
        {
            return Task.FromException(new Exception($"Enqueue error in queue '{sendMessageRequest.Queue}'"));
        }

        return Task.CompletedTask;
    }

    public Task<ReceiveMessageResponse> ReceiveMessage(ReceiveMessageRequest receiveMessageRequest, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromException<ReceiveMessageResponse>(new OperationCanceledException(cancellationToken));

        if (!_dictionaryPriorityQueues.TryGetValue(receiveMessageRequest.Queue, out var queue))
        {
            _logger.LogWarning("Queue \'{Queue}\' not exists", receiveMessageRequest.Queue);
            return Task.FromResult(new ReceiveMessageResponse());
        }

        if (!queue.TryDequeue(out var message))
        {
            _logger.LogDebug("Message in queue \'{Queue}\' not exists", receiveMessageRequest.Queue);
            return Task.FromResult(new ReceiveMessageResponse());
        }

        return Task.FromResult(new ReceiveMessageResponse(message));
    }
}