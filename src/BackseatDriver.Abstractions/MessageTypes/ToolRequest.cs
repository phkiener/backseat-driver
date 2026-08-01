namespace BackseatDriver.Abstractions.MessageTypes;

/// <summary>
/// A request to invoke a specific tool, sent by the assistant.
/// </summary>
/// <param name="invocation">The command to invoke.</param>
/// <seealso cref="ToolOutput"/>
public sealed class ToolRequest(string invocation) : AssistantMessageBase
{
    /// <summary>
    /// The prefix for this kind of response.
    /// </summary>
    public const string Prefix = "TOOL: ";

    /// <summary>
    /// Full tool command to invoke.
    /// </summary>
    public string Invocation { get; } = invocation;

    /// <inheritdoc />
    public override string Content => $"{Prefix}{Invocation}";
}
