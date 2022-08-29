using Mq.DependencyInjection;
using Mq.GrpcApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddMq(config => { config.UseInMemory(); });

var app = builder.Build();

app.MapGrpcService<MqGrpcService>();

app.Run();