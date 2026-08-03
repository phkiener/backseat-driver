using Microsoft.Extensions.Configuration;

namespace BackseatDriver.Core.Abstractions;

/// <summary>
/// A factory capable of creating instances of an <see cref="ICompletionProvider"/>.
/// </summary>
public interface ICompletionProviderFactory
{
    /// <summary>
    /// Name of the completion provider.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Construct an <see cref="ICompletionProvider"/> based on the given <see cref="IConfigurationSection"/>.
    /// </summary>
    /// <param name="configuration">The configuration to use to retrieve settings.</param>
    /// <returns>The built <see cref="ICompletionProvider"/>.</returns>
    public ICompletionProvider Build(IConfigurationSection configuration);
}
