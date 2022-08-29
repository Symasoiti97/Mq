namespace Mq.DependencyInjection;

/// <summary>
/// Конфигурация MQ
/// </summary>
public class MqConfig
{
    internal MqConfig()
    {
    }

    internal bool IsUseInMemory { get; private set; }

    public void UseInMemory()
    {
        IsUseInMemory = true;
    }
}