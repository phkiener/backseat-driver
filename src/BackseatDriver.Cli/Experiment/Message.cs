namespace BackseatDriver.Cli.Experiment;

/// <summary>
/// Represents a message in the history of a session.
/// </summary>
public abstract class Message
{
    /// <summary>
    /// The content of the message.
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// Whom this message was sent by.
    /// </summary>
    public string Sender { get; private set; }

    /// <summary>
    /// Construct a new message.
    /// </summary>
    /// <param name="sender">Whom this message was sent by.</param>
    /// <param name="content">Content of the message.</param>
    private Message(string sender, string content)
    {
        Content = content;
        Sender = sender;
    }

    /// <summary>
    /// A system prompt, i.e. a message sent by the host itself to control the behaviour of the assistant.
    /// </summary>
    /// <param name="content">The content of the message.</param>
    public sealed class SystemPrompt(string content) : Message("system", content), ISystemMessage;

    /// <summary>
    /// A message sent by the user.
    /// </summary>
    /// <param name="content">The content of the message.</param>
    public sealed class UserPrompt(string content) : Message("user", content), IUserMessage;

    /// <summary>
    /// A response generated for the assistant.
    /// </summary>
    /// <param name="content">The content of the message.</param>
    public sealed class AssistantResponse(string content) : Message("assistant", content), IAssistantMessage
    {
        /// <summary>
        /// The reasoning behind the generated response, if available.
        /// </summary>
        public string? Reasoning { get; init; }
    }

    /// <summary>
    /// The result of a tool call.
    /// </summary>
    /// <param name="content">The content of the message.</param>
    public sealed class ToolRequest(string content) : Message("assistant", content), IAssistantMessage;

    /// <summary>
    /// The result of a tool call.
    /// </summary>
    /// <param name="content">The full output of the invoked tool.</param>
    /// <seealso cref="Message.ToolRequest"/>
    public sealed class ToolResult(string content) : Message("tool", content), ISystemMessage;
}

/// <summary>
/// A marker interface for any kind of message.
/// </summary>
public interface IMessage;

/// <summary>
/// An empty marker interface meant to identify <seea cref="Message"/>s sent by the harness itself.
/// </summary>
public interface ISystemMessage : IMessage;

/// <summary>
/// An empty marker interface meant to identify <seea cref="Message"/>s sent by the user.
/// </summary>
public interface IUserMessage : IMessage;

/// <summary>
/// An empty marker interface meant to identify <seea cref="Message"/>s that are a response by the assistant.
/// </summary>
public interface IAssistantMessage : IMessage;
