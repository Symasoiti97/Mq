namespace Mq.Client;

/// <summary>
/// Потребитель
/// </summary>
public interface IMqConsumer
{
    /// <summary>
    /// Обработка сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task ExecuteAsync(string message, CancellationToken cancellationToken);
}