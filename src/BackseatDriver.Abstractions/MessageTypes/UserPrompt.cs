namespace BackseatDriver.Abstractions.MessageTypes;

/// <summary>
/// A question or task for the assistant.
/// </summary>
/// <param name="content">The content of the message.</param>
public sealed class UserPrompt(string content) : UserMessageBase
{
    /// <inheritdoc />
    public override string Content { get; } = content;
}
