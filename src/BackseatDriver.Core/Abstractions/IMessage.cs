namespace BackseatDriver.Core.Abstractions;

/// <summary>
/// A marker interface for any kind of message.
/// </summary>
public interface IMessage;

/// <summary>
/// An empty marker interface meant to identify <seea cref="IMessage"/>s sent by the harness itself.
/// </summary>
public interface ISystemMessage : IMessage;

/// <summary>
/// An empty marker interface meant to identify <seea cref="IMessage"/>s sent by the user.
/// </summary>
public interface IUserMessage : IMessage;

/// <summary>
/// An empty marker interface meant to identify <seea cref="IMessage"/>s that are a response by the assistant.
/// </summary>
public interface IAssistantMessage : IMessage;
