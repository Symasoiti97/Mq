using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mq.Server;
using Mq.Server.Messages;

namespace Mq.GrpcApi.Services;

/// <summary>
/// Апи сервис для работы с MQ
/// </summary>
public class MqGrpcService : gRpc.Mq.MqBase
{
    private readonly IMqService _mqService;

    public MqGrpcService(IMqService mqService)
    {
        _mqService = mqService;
    }

    /// <summary>
    /// Зарегистировать очередь
    /// </summary>
    /// <param name="request">Информация об очереди</param>
    /// <param name="context">Контекст запроса</param>
    public override async Task<Empty> RegistryQueue(gRpc.RegistryQueueRequest request, ServerCallContext context)
    {
        await _mqService.RegistryQueue(new RegistryQueueRequest(request.Queue), context.CancellationToken);

        return new Empty();
    }

    /// <summary>
    /// Отправить сообщение
    /// </summary>
    /// <param name="request">Информация об очереди и сообщении</param>
    /// <param name="context">Контекст запроса</param>
    public override async Task<Empty> SendMessage(gRpc.SendMessageRequest request, ServerCallContext context)
    {
        await _mqService.SendMessage(new SendMessageRequest(request.Queue, request.Priority, request.Message), context.CancellationToken);

        return new Empty();
    }

    /// <summary>
    /// Получить сообщение из очереди
    /// </summary>
    /// <param name="request">Информация об очереди</param>
    /// <param name="context">Контекст запроса</param>
    /// <remarks>Если сообщение null, то очередь пустая</remarks>
    /// <returns>Сообщение</returns>
    public override async Task<gRpc.ReceiveMessageResponse> ReceiveMessage(gRpc.ReceiveMessageRequest request, ServerCallContext context)
    {
        var receiveMessage = await _mqService.ReceiveMessage(new ReceiveMessageRequest(request.Queue), context.CancellationToken);

        return new gRpc.ReceiveMessageResponse
        {
            Message = receiveMessage.Message
        };
    }
}