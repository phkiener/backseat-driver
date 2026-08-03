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
}
