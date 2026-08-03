using System.Text.Json.Serialization;

namespace BackseatDriver.OpenAI.Model;

public sealed class ChatCompletionRequest
{
    /// <summary>
    /// A list of messages comprising the conversation so far.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("messages")]
    public ChatCompletionMessageParam[] Messages { get; init; } = [];

    /// <summary>
    /// A list of tools the model may call.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("tools")]
    public ChatCompletionFunctionTool[] Tools { get; init; } = [];

    /// <summary>
    /// Whether to enable parallel function calling during tool use.
    /// </summary>
    [JsonPropertyName("parallel_tool_calls")]
    public bool ParallelToolCalls => false;

    /// <summary>
    /// Constrains effort on reasoning for reasoning models.
    /// </summary>
    /// <remarks>
    /// Currently supported values are none, minimal, low, medium, high, xhigh, and max.
    /// </remarks>
    [JsonPropertyName("reasoning_effort")]
    public string? ReasoningEffort => "medium";
}
