namespace BackseatDriver.Core.Abstractions;

/// <summary>
/// An interaction session with a model, handling tool calls on its own.
/// </summary>
public interface IModelSession
{
    /// <summary>
    /// Denotes wheter the session is currently processing a message or is simply idle.
    /// </summary>
    public bool IsWorking { get; }

    /// <summary>
    /// A callback invoked everytime the processing of a message has started.
    /// </summary>
    public event EventHandler? OnProcessingStarted;

    /// <summary>
    /// A callback invoked everytime the processing of a message is completed.
    /// </summary>
    public event EventHandler? OnProcessingCompleted;

    /// <summary>
    /// A callback invoked everytime a message is added to the message history.
    /// </summary>
    public event EventHandler<IMessage>? OnMessageAdded;

    /// <summary>
    /// Send a message to the model.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <returns><see langword="true"/> when the message was accepted, <see langword="false"/> when the model is currently busy.</returns>
    /// <remarks>
    /// A handful of special commands are supported which are not sent to the model.
    /// </remarks>
    public Task<bool> SendAsync(string message);
}
