using BackseatDriver.Core.Abstractions;
using Microsoft.Extensions.Configuration;

namespace BackseatDriver.OpenAI;

/// <summary>
/// A <see cref="ICompletionProviderFactory"/> capable of creating a <see cref="ICompletionProvider"/> for OpenAI compatible APIs.
/// </summary>
public sealed class CompletionProviderFactory : ICompletionProviderFactory
{
    /// <inheritdoc />
    public string Name => "OpenAI";

    /// <inheritdoc />
    public ICompletionProvider Build(IConfigurationSection configuration)
    {
        var url = configuration.GetValue<Uri>("BaseUri")
                  ?? throw new InvalidOperationException($"Missing required 'BaseUri' parameter for provider {Name}");

        return new CompletionProvider(url);
    }
}
