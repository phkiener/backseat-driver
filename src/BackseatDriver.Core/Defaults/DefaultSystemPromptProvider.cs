using BackseatDriver.Core.Abstractions;

namespace BackseatDriver.Core.Defaults;

/// <summary>
/// Provides a default system prompt.
/// </summary>
public sealed class DefaultSystemPromptProvider : ISystemPromptProvider
{
    /// <inheritdoc />
    public Message.SystemPrompt SystemPrompt { get; } = new("React to the user's prompts truthfully. Prefer short responses. No redundant fluff, no padding. Do not use markdown or Emojis, prefer plain text. The less verbose, the better.");
}
