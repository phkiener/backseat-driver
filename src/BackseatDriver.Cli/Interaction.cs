using BackseatDriver.Cli.Provider;

namespace BackseatDriver.Cli;

/// <summary>
/// A single interaction, triggered by the user.
/// </summary>
public sealed class Interaction(IProvider provider, CancellationToken cancellationToken) : IAsyncDisposable
{
    /// <summary>
    /// The possible states an <see cref="Interaction"/> can be in.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// The interaction is not yet started.
        /// </summary>
        Pending,

        /// <summary>
        /// The interaction is currently generating a response.
        /// </summary>
        Running,

        /// <summary>
        /// An intermediate response has been generated and the provider is currently waiting for
        /// a tool result or a prompt to the user before continuing.
        /// </summary>
        Waiting,

        /// <summary>
        /// The interaction is finished, having received the final result.
        /// </summary>
        Finished
    }

    private readonly CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

    /// <summary>
    /// The current state of the interaction.
    /// </summary>
    /// <see cref="State"/>
    public State CurrentState { get; private set; } = State.Pending;

    /// <summary>
    /// The final response, meant to be shown to the user.
    /// </summary>
    public string? Response { get; private set; }

    /// <summary>
    /// Send a message to the harness.
    /// </summary>
    /// <param name="message">The initial message to send.</param>
    public async Task StartAsync(string message)
    {
        CurrentState = State.Running;

        var response = await provider.GenerateAsync([message], cancellationTokenSource.Token);
        Response = response;

        CurrentState = State.Finished;
    }

    public async ValueTask DisposeAsync()
    {
        await cancellationTokenSource.CancelAsync();

        cancellationTokenSource.Dispose();
    }
}
