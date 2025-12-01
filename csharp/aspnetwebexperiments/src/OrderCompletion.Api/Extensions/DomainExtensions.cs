using OrderCompletion.Api.Core.Ports;
using OrderCompletion.Api.Core.UseCases;

namespace OrderCompletion.Api.Extensions;

public static class DomainExtensions
{
    public static void RegisterDomainUseCases(this IServiceCollection services)
    {
        services.AddTransient<IOrderCompletionUseCase, OrderCompletionUseCase>();
    }
}