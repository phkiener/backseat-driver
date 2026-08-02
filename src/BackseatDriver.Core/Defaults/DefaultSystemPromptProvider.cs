using BackseatDriver.Core.Abstractions;

namespace BackseatDriver.Core.Defaults;

/// <summary>
/// Provides a default system prompt.
/// </summary>
public sealed class DefaultSystemPromptProvider : ISystemPromptProvider
{
    /// <inheritdoc />
    public Message.SystemPrompt SystemPrompt { get; } = new("");
}
