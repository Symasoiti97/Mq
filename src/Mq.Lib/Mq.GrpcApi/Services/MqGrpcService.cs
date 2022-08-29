using Google.Protobuf.WellKnownTypes;
using gRpc;
using Grpc.Core;
using Mq.Server;

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
        await _mqService.RegistryQueue(new Mq.Server.Messages.RegistryQueueRequest(request.Queue), context.CancellationToken);

        return new Empty();
    }

    /// <summary>
    /// Отправить сообщение
    /// </summary>
    /// <param name="request">Информация об очереди и сообщении</param>
    /// <param name="context">Контекст запроса</param>
    public override async Task<Empty> SendMessage(gRpc.SendMessageRequest request, ServerCallContext context)
    {
        await _mqService.SendMessage(new Mq.Server.Messages.SendMessageRequest(request.Queue, request.Priority, request.Message), context.CancellationToken);

        return new Empty();
    }

    /// <summary>
    /// Получить сообщение из очереди
    /// </summary>
    /// <param name="requestStream">Информация об очереди</param>
    /// <param name="responseStream">Сообщение</param>
    /// <param name="context">Контекст запроса</param>
    public override async Task ReceiveMessage(IAsyncStreamReader<gRpc.ReceiveMessageRequest> requestStream,
        IServerStreamWriter<gRpc.ReceiveMessageResponse> responseStream, ServerCallContext context)
    {
        await foreach (var request in requestStream.ReadAllAsync())
        {
            var receiveMessageRequest = new Mq.Server.Messages.ReceiveMessageRequest(request.MessageId, request.Queue);
            var receiveMessageResponse = await _mqService.ReceiveMessage(receiveMessageRequest, context.CancellationToken);

            var result = new gRpc.ReceiveMessageResponse();
            if (receiveMessageResponse is not null)
            {
                result.MessageId = receiveMessageResponse.MessageId;
                result.Message = receiveMessageResponse.Message;
            }
            else
            {
                result.IsEmpty = true;
            }

            await responseStream.WriteAsync(result);
        }
    }
}