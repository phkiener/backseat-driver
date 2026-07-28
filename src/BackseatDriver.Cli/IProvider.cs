namespace BackseatDriver.Cli.Provider;

/// <summary>
/// Represents a completion provider, i.e. a model capable of responding to a message.
/// </summary>
public interface IProvider
{
    /// <summary>
    /// Generate an answer (a <em>completion</em>) for the given list of messages.
    /// </summary>
    /// <param name="messages">The messages to send to the provider.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The generated response.</returns>
    Task<string> GenerateAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default);
}
