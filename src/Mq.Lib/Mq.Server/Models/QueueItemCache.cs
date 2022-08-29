namespace Mq.Server.Models;

internal class QueueItemCache
{
    public QueueItemCache(QueueItem queueItem, string queue)
    {
        QueueItem = queueItem;
        Queue = queue;
    }

    public QueueItem QueueItem { get; }
    public string Queue { get; }
}