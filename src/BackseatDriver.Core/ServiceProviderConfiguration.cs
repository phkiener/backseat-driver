using BackseatDriver.Core.Abstractions;
using BackseatDriver.Core.Defaults;
using BackseatDriver.Core.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BackseatDriver.Core;

/// <summary>
/// Extensions to register the required services with an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceProviderConfiguration
{
    /// <summary>
    /// Register all required services for an <see cref="IModelSession"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the services in.</param>
    /// <returns>The passed-in <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddBackseatDriver(this IServiceCollection services)
    {
        services.TryAddScoped<IModelSession, DefaultModelSession>();
        services.TryAddScoped<IToolEngine, DefaultToolEngine>();
        services.TryAddScoped<ICompletionProvider>(static sp => sp.GetRequiredService<ConfigurationCompletionProviderFactory>().Build());

        services.TryAddSingleton<ISystemPromptProvider, DefaultSystemPromptProvider>();
        services.TryAddSingleton<IToolProvider, ServiceCollectionToolProvider>();
        services.TryAddSingleton<ConfigurationCompletionProviderFactory>();

        return services;
    }

    /// <summary>
    /// Registers the given tool <typeparamref name="TTool"/> as an <see cref="ITool"/> for the default <see cref="IToolProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the services in.</param>
    /// <typeparam name="TTool">The <see cref="ITool"/> to register.</typeparam>
    /// <returns>The passed-in <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddTool<TTool>(this IServiceCollection services) where TTool : class, ITool
    {
        services.TryAddScoped<ITool, TTool>();
        return services;
    }
}
