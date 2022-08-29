namespace Mq.Client;

internal interface IMqReceiver
{
    Task<string?> Receive(string queue, CancellationToken cancellationToken);
}