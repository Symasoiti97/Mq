using Mq.Server.Messages;

namespace Mq.Server;

/// <summary>
/// Сервис для работы с очередями
/// </summary>
public interface IMqService
{
    /// <summary>
    /// Зарегестировать очередь
    /// </summary>
    /// <param name="registryQueueRequest">Информация об очереди</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <remarks>Если очередь уже существует, то ничего не делает</remarks>
    Task RegistryQueue(RegistryQueueRequest registryQueueRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить сообщение в очередь
    /// </summary>
    /// <param name="sendMessageRequest">Информация об очереди и сообщении</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SendMessage(SendMessageRequest sendMessageRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить сообщение из очереди
    /// </summary>
    /// <param name="receiveMessageRequest">Информация об очереди</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Сообщение из очереди</returns>
    Task<ReceiveMessageResponse> ReceiveMessage(ReceiveMessageRequest receiveMessageRequest, CancellationToken cancellationToken = default);
}