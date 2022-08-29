namespace Mq.Server.Messages;

/// <summary>
/// Ответ на получения сообщения
/// </summary>
public class ReceiveMessageResponse
{
    public ReceiveMessageResponse(string messageId, string message)
    {
        if (string.IsNullOrWhiteSpace(messageId))
        {
            throw new ArgumentException("Can't be empty", nameof(messageId));
        }

        MessageId = messageId;

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Can't be empty", nameof(message));
        }

        Message = message;
    }

    /// <summary>
    /// Сообщение
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    public string MessageId { get; }
}