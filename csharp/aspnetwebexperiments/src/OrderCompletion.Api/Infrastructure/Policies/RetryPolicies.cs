using Polly;
using Polly.Extensions.Http;

namespace OrderCompletion.Api.Infrastructure.Policies;

public static class RetryPolicies
{
    private const int MaxRetryAttempts = 3;

    public static IAsyncPolicy<HttpResponseMessage> RetryWithExponentialBackoffPolicy(IServiceProvider serviceProvider,
        HttpRequestMessage request)
        =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(MaxRetryAttempts, async (response, retryCount, _) =>
                    {
                        await Task.Delay(
                            TimeSpan.FromSeconds(Math.Pow(2,
                                retryCount))); // Retry with backoff between retries to allow chance for external dependency to recover
                    }
                );
}