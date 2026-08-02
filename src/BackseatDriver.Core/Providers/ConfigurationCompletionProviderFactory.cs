using BackseatDriver.Core.Abstractions;
using Microsoft.Extensions.Configuration;

namespace BackseatDriver.Core.Providers;

/// <summary>
/// A factory to build a <see cref="ICompletionProvider"/> based on the <see cref="IConfiguration"/>.
/// </summary>
public sealed class ConfigurationCompletionProviderFactory(IConfiguration configuration)
{
    /// <summary>
    /// Create an <see cref="ICompletionProvider"/>.
    /// </summary>
    /// <returns>The built completion provider.</returns>
    public ICompletionProvider Build()
    {
        var providerType = configuration["Completion:Provider"];
        if (providerType == "OpenAI")
        {
            var options = configuration.GetSection("Completion:OpenAI").Get<OpenAiProviderSettings>();

            return options is null
                ? throw new InvalidOperationException($"Missing configuration for provider '{providerType}'.")
                : new OpenAiCompletionProvider(options.BaseUri);
        }

        throw new InvalidOperationException($"Unknown completion provider '{providerType}'.");
    }

    private sealed record OpenAiProviderSettings(Uri BaseUri);
}
