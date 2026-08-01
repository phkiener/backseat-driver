namespace BackseatDriver.Abstractions;

/// <summary>
/// A message as part of a message history.
/// </summary>
public interface IChatMessage
{
    /// <summary>
    /// Content of the chat message.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Whom the message was sent by.
    /// </summary>
    public ChatRole Sender { get; }
}

/// <summary>
/// Base class for all system messages.
/// </summary>
public abstract class SystemMessageBase : IChatMessage
{
    /// <inheritdoc />
    public abstract string Content { get; }

    /// <inheritdoc />
    public ChatRole Sender => ChatRole.System;
}

/// <summary>
/// Base class for all messages sent by the user.
/// </summary>
public abstract class UserMessageBase : IChatMessage
{
    /// <inheritdoc />
    public abstract string Content { get; }

    /// <inheritdoc />
    public ChatRole Sender => ChatRole.User;
}

/// <summary>
/// Base class for all messages sent by the assistant.
/// </summary>
public abstract class AssistantMessageBase : IChatMessage
{
    /// <inheritdoc />
    public abstract string Content { get; }

    /// <inheritdoc />
    public ChatRole Sender => ChatRole.Assistant;
}
