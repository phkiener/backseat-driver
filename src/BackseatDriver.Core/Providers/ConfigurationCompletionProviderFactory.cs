using BackseatDriver.Core.Abstractions;
using Microsoft.Extensions.Configuration;

namespace BackseatDriver.Core.Providers;

/// <summary>
/// A factory to build a <see cref="ICompletionProvider"/> based on the <see cref="IConfiguration"/>.
/// </summary>
public sealed class ConfigurationCompletionProviderFactory(IConfiguration configuration, IEnumerable<ICompletionProviderFactory> factories)
{
    /// <summary>
    /// Create an <see cref="ICompletionProvider"/>.
    /// </summary>
    /// <returns>The built completion provider.</returns>
    public ICompletionProvider Build()
    {
        var providerType = configuration["Completion:Provider"];
        var factory = factories.SingleOrDefault(f => f.Name == providerType);

        return factory is null
            ? throw new InvalidOperationException($"Unknown completion provider '{providerType}'.")
            : factory.Build(configuration.GetSection($"Completion:{factory.Name}"));
    }
}
