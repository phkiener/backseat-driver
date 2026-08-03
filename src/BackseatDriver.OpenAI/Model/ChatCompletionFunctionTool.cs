using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackseatDriver.OpenAI.Model;

/// <summary>
/// A function tool that can be used to generate a response.
/// </summary>
public sealed class ChatCompletionFunctionTool
{
    /// <summary>
    /// The type of the tool.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type => "function";

    /// <summary>
    /// The definition of the tool.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("function")]
    public required FunctionToolDefinition Function { get; init; }

    /// <summary>
    /// The definition of a function tool.
    /// </summary>
    public sealed class FunctionToolDefinition
    {
        /// <summary>
        /// The name of the function to be called.
        /// </summary>
        [JsonRequired]
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        /// <summary>
        /// A description of what the function does, used by the model to choose when and how to call the function.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; init; }

        /// <summary>
        /// The parameters the functions accepts, described as a JSON Schema object.
        /// </summary>
        [JsonPropertyName("parameters")]
        public JsonDocument? Parameters { get; init; }
    }
}
