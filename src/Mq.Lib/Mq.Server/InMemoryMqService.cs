using System.Collections.Concurrent;
using System.Runtime.Caching;
using Microsoft.Extensions.Logging;
using Mq.Server.Exceptions;
using Mq.Server.Messages;
using Mq.Server.Models;

namespace Mq.Server;

internal class InMemoryMqService : IMqService
{
    private readonly ILogger<InMemoryMqService> _logger;
    private readonly ConcurrentDictionary<string, ConcurrentPriorityQueue<int, QueueItem>> _dictionaryPriorityQueues = new();
    private readonly MemoryCache _memoryCache = new(name: "QueueMessages");

    private CacheItemPolicy CacheEntryOptions => new()
    {
        RemovedCallback = RemovedCallback,
        //todo вынести в конфигурацию, таймаут обработки сообщения, если время вышло сообщение возвращается в очередь
        SlidingExpiration = TimeSpan.FromMinutes(5)
    };

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

        if (!_dictionaryPriorityQueues.TryAdd(registryQueueRequest.Queue, new ConcurrentPriorityQueue<int, QueueItem>()))
        {
            _logger.LogInformation("Queue \'{Queue}\' already registered", registryQueueRequest.Queue);
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

        var queueItem = new QueueItem
        {
            Message = sendMessageRequest.Message,
            MessageId = Guid.NewGuid().ToString(),
            Priority = sendMessageRequest.Priority
        };
        if (!queue.TryEnqueue(sendMessageRequest.Priority, queueItem))
        {
            return Task.FromException(new Exception($"Enqueue error in queue '{sendMessageRequest.Queue}'"));
        }

        return Task.CompletedTask;
    }

    public Task<ReceiveMessageResponse?> ReceiveMessage(ReceiveMessageRequest receiveMessageRequest, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromException<ReceiveMessageResponse?>(new OperationCanceledException(cancellationToken));
        }

        if (receiveMessageRequest.MessageId is not null)
        {
            _memoryCache.Remove(receiveMessageRequest.MessageId);

            return Task.FromResult<ReceiveMessageResponse?>(null);
        }

        if (!_dictionaryPriorityQueues.TryGetValue(receiveMessageRequest.Queue, out var queue))
        {
            _logger.LogWarning("Queue \'{Queue}\' not exists", receiveMessageRequest.Queue);

            return Task.FromResult<ReceiveMessageResponse?>(null);
        }

        if (!queue.TryDequeue(out var queueItem))
        {
            _logger.LogDebug("Message in queue \'{Queue}\' not exists", receiveMessageRequest.Queue);

            return Task.FromResult<ReceiveMessageResponse?>(null);
        }

        _memoryCache.Set(queueItem.MessageId, new QueueItemCache(queueItem, receiveMessageRequest.Queue), CacheEntryOptions);

        return Task.FromResult<ReceiveMessageResponse?>(new ReceiveMessageResponse(queueItem.MessageId, queueItem.Message));
    }

    private void RemovedCallback(CacheEntryRemovedArguments arguments)
    {
        if (arguments.RemovedReason == CacheEntryRemovedReason.Expired)
        {
            var queueItemCache = (QueueItemCache) arguments.CacheItem.Value;
            if (_dictionaryPriorityQueues.TryGetValue(queueItemCache.Queue, out var queue))
            {
                if (queue.TryEnqueue(queueItemCache.QueueItem.Priority, queueItemCache.QueueItem))
                {
                    _logger.LogWarning("Timeout receive message. Queue message with id \'{MessageId}\' again enqueue", queueItemCache.QueueItem.MessageId);
                }
            }
        }
    }
}