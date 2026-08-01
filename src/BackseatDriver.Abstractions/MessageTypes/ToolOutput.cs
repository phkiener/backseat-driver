namespace BackseatDriver.Abstractions.MessageTypes;

/// <summary>
/// The result of a tool invocation.
/// </summary>
/// <param name="result">The output of the requested tool command.</param>
/// <seealso cref="ToolRequest"/>
public sealed class ToolOutput(string result) : UserMessageBase
{
    /// <summary>
    /// The output of the invocation.
    /// </summary>
    public string Result { get; } = result;

    /// <inheritdoc />
    public override string Content => $"OUTPUT: {Result}";
}
