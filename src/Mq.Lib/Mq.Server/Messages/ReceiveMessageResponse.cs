namespace Mq.Server.Messages;

/// <summary>
/// Ответ на получения сообщения
/// </summary>
public class ReceiveMessageResponse
{
    public ReceiveMessageResponse()
    {
    }

    public ReceiveMessageResponse(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Can't be empty", nameof(message));
        }

        Message = message;
    }

    /// <summary>
    /// Сообщение
    /// </summary>
    public string? Message { get; }
}