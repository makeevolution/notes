using OrderCompletion.Api.Adapters.Enums;
using OrderCompletion.Api.Adapters.NotificationAdapter;
using OrderCompletion.Api.Adapters.OrderCompletionAdapter;
using OrderCompletion.Api.Extensions;
using OrderCompletion.Api.Infrastructure.Exceptions;
using OrderCompletion.Api.Infrastructure.Policies;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterOrderCompletionAdapter(builder.Configuration);
builder.Services.RegisterNotificationAdapter(builder.Configuration);
builder.Services.RegisterDomainUseCases();
builder.Services.RegisterCors(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();
builder.Services.AddHttpClient(nameof(HttpClientsEnum.OrderCompletionClient))
    .AddPolicyHandler(RetryPolicies.RetryWithExponentialBackoffPolicy);
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();

public partial class Program;