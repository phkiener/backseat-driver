namespace BackseatDriver.Core.Abstractions;

/// <summary>
/// Provides the system prompt for a <see cref="IModelSession"/>.
/// </summary>
public interface ISystemPromptProvider
{
    /// <summary>
    /// The system prompt to use.
    /// </summary>
    Message.SystemPrompt SystemPrompt { get; }
}
