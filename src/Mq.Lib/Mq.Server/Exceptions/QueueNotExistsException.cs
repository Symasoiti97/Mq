namespace Mq.Server.Exceptions;

/// <summary>
/// Очередь не существует
/// </summary>
public class QueueNotExistsException : Exception
{
    /// <summary>
    /// Очередь
    /// </summary>
    public string Queue { get; }

    public QueueNotExistsException(string queue) : base($"Queue '{queue}' not exists")
    {
        Queue = queue;
    }
}