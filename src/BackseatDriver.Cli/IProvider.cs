namespace BackseatDriver.Cli.Provider;

/// <summary>
/// Represents a completion provider, i.e. a model capable of responding to a message.
/// </summary>
public interface IProvider
{
    /// <summary>
    /// Generate an answer (a <em>completion</em>) for the given message.
    /// </summary>
    /// <param name="message">The message to send to the provider.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation.</param>
    /// <returns>The generated response.</returns>
    /// <seealso cref="GenerateAsync(IEnumerable{string}, CancellationToken)"/>
    Task<string> GenerateAsync(string message, CancellationToken cancellationToken = default)
        => GenerateAsync([message], cancellationToken);

    /// <summary>
    /// Generate an answer (a <em>completion</em>) for the given list of messages.
    /// </summary>
    /// <param name="messages">The messages to send to the provider.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The generated response.</returns>
    /// <seealso cref="GenerateAsync(string, CancellationToken)"/>
    Task<string> GenerateAsync(IEnumerable<string> messages, CancellationToken cancellationToken = default);
}
