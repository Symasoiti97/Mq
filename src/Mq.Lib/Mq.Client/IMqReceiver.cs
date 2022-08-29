namespace Mq.Client;

internal interface IMqReceiver
{
    Task Receive(CancellationToken cancellationToken);
}