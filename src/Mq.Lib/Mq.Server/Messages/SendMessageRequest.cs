namespace Mq.Server.Messages;

/// <summary>
/// Запрос на отправку сообщения
/// </summary>
public class SendMessageRequest
{
    public SendMessageRequest(string queue, int priority, string message)
    {
        if (string.IsNullOrWhiteSpace(queue))
        {
            throw new ArgumentException("Can't be empty", nameof(queue));
        }

        Queue = queue;

        Priority = priority;

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Can't be empty", nameof(message));
        }

        Message = message;
    }

    /// <summary>
    /// Очередь
    /// </summary>
    public string Queue { get; }

    /// <summary>
    /// Приоритет
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Сообщение
    /// </summary>
    public string Message { get; }
}