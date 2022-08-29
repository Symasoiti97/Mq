namespace Mq.Client;

/// <summary>
/// Отправитель сообщений MQ
/// </summary>
public interface IMqSender
{
    /// <summary>
    /// Отправить сообщение
    /// </summary>
    /// <param name="queue">Очередь</param>
    /// <param name="priority">Приоритет</param>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SendMessage(string queue, int priority, string message, CancellationToken cancellationToken);
}