using BackseatDriver.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BackseatDriver.OpenAI;

/// <summary>
/// Extensions to register the the OpenAI <see cref="ICompletionProvider"/>.
/// </summary>
public static class ServiceProviderConfiguration
{
    /// <summary>
    /// Add the <see cref="ICompletionProviderFactory"/> for the OpenAI completion provider to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the services in.</param>
    /// <returns>The passed-in <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddOpenAI(this IServiceCollection services)
    {
        services.AddSingleton<ICompletionProviderFactory, CompletionProviderFactory>();

        return services;
    }
}
