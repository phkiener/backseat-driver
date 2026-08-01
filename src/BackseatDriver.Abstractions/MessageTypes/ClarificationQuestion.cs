namespace BackseatDriver.Abstractions.MessageTypes;

/// <summary>
/// A clarifiying question, asked by the assistant.
/// </summary>
/// <param name="question">The question for the user to answer.</param>
/// <seealso cref="ClarificationAnswer"/>
public sealed class ClarificationQuestion(string question) : AssistantMessageBase
{
    /// <summary>
    /// The prefix for this kind of response.
    /// </summary>
    public const string Prefix = "PROMPT: ";

    /// <summary>
    /// The question for the user to answer.
    /// </summary>
    public string Question { get; } = question;

    /// <inheritdoc />
    public override string Content => $"{Prefix}{Question}";
}
