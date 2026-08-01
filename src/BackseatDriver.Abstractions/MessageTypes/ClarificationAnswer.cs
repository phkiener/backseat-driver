namespace BackseatDriver.Abstractions.MessageTypes;

/// <summary>
/// The answer to a clarifying question.
/// </summary>
/// <param name="answer">The answer to the question asked.</param>
/// <seealso cref="ClarificationQuestion"/>
public sealed class ClarificationAnswer(string answer) : UserMessageBase
{
    /// <summary>
    /// The answer to the question asked.
    /// </summary>
    public string Answer { get; } = answer;

    /// <inheritdoc />
    public override string Content => $"ANSWER: {Answer}";
}
