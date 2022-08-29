namespace Mq.Server.Messages;

/// <summary>
/// Запрос на получения сообщения
/// </summary>
public class ReceiveMessageRequest
{
    public ReceiveMessageRequest(string? messageId, string queue)
    {
        if (string.IsNullOrWhiteSpace(queue))
        {
            throw new ArgumentException("Can't be empty", nameof(queue));
        }

        MessageId = messageId;
        Queue = queue;
    }

    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    public string? MessageId { get; }

    /// <summary>
    /// Очередь
    /// </summary>
    public string Queue { get; }
}