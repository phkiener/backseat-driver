namespace BackseatDriver.Abstractions.MessageTypes;

/// <summary>
/// A system prompt; a message that sets up the behavior of the assistant.
/// </summary>
/// <param name="content">Content of the system prompt.</param>
public sealed class SystemPrompt(string content) : SystemMessageBase
{
    /// <inheritdoc />
    public override string Content { get; } = content;
}
