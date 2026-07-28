namespace BackseatDriver.Cli.Interface;

/// <summary>
/// The possible states an <see cref="Interaction"/> can be in.
/// </summary>
public enum InteractionState
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
