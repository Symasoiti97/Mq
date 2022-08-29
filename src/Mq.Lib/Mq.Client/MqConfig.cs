namespace Mq.Client;

/// <summary>
/// Конфигурация MQ
/// </summary>
public class MqConfig
{
    internal MqConfig()
    {
    }

    private readonly HashSet<string> _queues = new();
    internal IEnumerable<string> Queues => _queues;

    private readonly Dictionary<string, Type> _consumers = new();
    internal IReadOnlyDictionary<string, Type> Consumers => _consumers;
    internal TimeSpan Interval { get; private set; } = TimeSpan.FromMinutes(1);

    internal Uri? Address { get; private set; }

    /// <summary>
    /// Установить адрес mq сервера
    /// </summary>
    /// <param name="address">Адрес</param>
    /// <returns>Конфигурация MQ</returns>
    public MqConfig SetAddress(Uri address)
    {
        Address = address;
        return this;
    }

    /// <summary>
    /// Установить интервал опроса сообщений
    /// </summary>
    /// <remarks>По умолчанию 1 минута</remarks>
    /// <param name="interval">Интервал</param>
    /// <returns>Конфигурация MQ</returns>
    public MqConfig SetInterval(TimeSpan interval)
    {
        Interval = interval;

        return this;
    }

    /// <summary>
    /// Указать очередь для регистрации
    /// </summary>
    /// <param name="queue">Очередь</param>
    /// <returns>Конфигурация MQ</returns>
    public MqConfig RegisterQueue(string queue)
    {
        _queues.Add(queue);

        return this;
    }

    /// <summary>
    /// Указать потребителя сообщений очереди
    /// </summary>
    /// <param name="queue">Очередь</param>
    /// <typeparam name="TMqConsumer">Потребитель</typeparam>
    /// <returns>Конфигурация MQ</returns>
    public MqConfig RegisterConsumer<TMqConsumer>(string queue) where TMqConsumer : IMqConsumer
    {
        _consumers[queue] = typeof(TMqConsumer);

        return this;
    }
}