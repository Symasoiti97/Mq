namespace Mq.Server.Messages;

/// <summary>
/// Запрос на получения сообщения
/// </summary>
public class ReceiveMessageRequest
{
    public ReceiveMessageRequest(string queue)
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