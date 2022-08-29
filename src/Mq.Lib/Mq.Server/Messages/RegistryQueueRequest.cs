namespace Mq.Server.Messages;

/// <summary>
/// Запрос на регистрацию очереди
/// </summary>
public class RegistryQueueRequest
{
    public RegistryQueueRequest(string queue)
    {
        if (string.IsNullOrWhiteSpace(queue))
        {
            throw new ArgumentException("Can't be empty", nameof(queue));
        }

        Queue = queue;
    }

    /// <summary>
    /// Очередь
    /// </summary>
    public string Queue { get; }
}