namespace Mq.Server.Models;

internal class QueueItem
{
    public string MessageId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int Priority { get; set; }
}