using BackseatDriver.Core.Abstractions;

namespace BackseatDriver.Core.Providers;

/// <summary>
/// A <see cref="ICompletionProvider"/> that works for OpenAI and compatible APIs.
/// </summary>
public sealed class OpenAiCompletionProvider : ICompletionProvider, IDisposable
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// Create a new instance of the <see cref="OpenAiCompletionProvider"/>.
    /// </summary>
    /// <param name="baseUri">The base URI of the host to connect to.</param>
    /// <param name="configureClient">Optional configuration of the <see cref="HttpClient"/> that will be used.</param>
    public OpenAiCompletionProvider(Uri baseUri, Action<HttpClient>? configureClient = null)
    {
        httpClient = new HttpClient { BaseAddress = baseUri };
        configureClient?.Invoke(httpClient);
    }

    /// <inheritdoc />
    public Task<IAssistantMessage> GenerateAsync(IEnumerable<IMessage> history, IEnumerable<ITool> availableTools)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        httpClient.Dispose();
    }
}
