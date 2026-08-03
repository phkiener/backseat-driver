using System.Net.Http.Json;
using System.Text.Json.Serialization;
using BackseatDriver.Core;
using BackseatDriver.Core.Abstractions;

namespace BackseatDriver.OpenAI;

/// <summary>
/// A <see cref="ICompletionProvider"/> that works for OpenAI and compatible APIs.
/// </summary>
public class CompletionProvider : ICompletionProvider, IDisposable
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// Create a new instance of the <see cref="CompletionProvider"/>.
    /// </summary>
    /// <param name="baseUri">The base URI of the host to connect to.</param>
    /// <param name="configureClient">Optional configuration of the <see cref="HttpClient"/> that will be used.</param>
    public CompletionProvider(Uri baseUri, Action<HttpClient>? configureClient = null)
    {
        httpClient = new HttpClient { BaseAddress = baseUri };
        configureClient?.Invoke(httpClient);
    }

    /// <inheritdoc />
    public async Task<IAssistantMessage> GenerateAsync(IEnumerable<IMessage> history, IEnumerable<ITool> availableTools)
    {
        var chatMessages = history.Select(OpenAiMessage.From).Where(static m => m is not (null or { Content: "" })).ToList();
        var tools = availableTools.Select(OpenAiFunctionTool.From).Select(f => new { function = f, type = "function" }).ToList();

        var response = await httpClient.PostAsJsonAsync("v1/chat/completions", new { messages = chatMessages, tools = tools });
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var parsedResponse = await response.Content.ReadFromJsonAsync<OpenAiCompletion>();
            var selectedAnswer = parsedResponse?.Choices.FirstOrDefault();

            if (selectedAnswer?.FinishReason is null or not ("stop" or "tool_calls"))
            {
                throw new InvalidOperationException("Model answer invalid.");
            }

            if (selectedAnswer.Message.ToolCalls is not (null or []))
            {
                // Only handle the first one.. for now.
                return new Message.ToolRequest(selectedAnswer.Message.ToolCalls.First().Function.Name);
            }

            new Message.AssistantResponse(selectedAnswer.Message.Content) { Reasoning = selectedAnswer.Message.Reasoning };
        }

        throw new InvalidOperationException("Model answer invalid.");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        httpClient.Dispose();
    }

    private sealed record OpenAiFunctionTool(
        [property: JsonPropertyName("name"), JsonRequired] string Name,
        [property: JsonPropertyName("description"), JsonRequired] string Description)
    {
        public static OpenAiFunctionTool From(ITool tool)
        {
            return new OpenAiFunctionTool(tool.Name, tool.Description);
        }
    }


    private sealed record OpenAiMessage(
        [property: JsonPropertyName("role"), JsonRequired] string Role,
        [property: JsonPropertyName("content"), JsonRequired] string Content)
    {
        public static OpenAiMessage? From(IMessage message)
        {
            return message switch
            {
                Message.AssistantResponse assistantResponse => new OpenAiMessage("assistant", assistantResponse.Content),
                Message.SystemPrompt systemPrompt => new OpenAiMessage("system", systemPrompt.Content),
                Message.UserPrompt userPrompt => new OpenAiMessage("user", userPrompt.Content),
                _ => null
            };
        }
    }

    private sealed record OpenAiMessageAnswer(
        [property: JsonPropertyName("content"), JsonRequired] string Content,
        [property: JsonPropertyName("reasoning_content")] string? Reasoning = null,
        [property: JsonPropertyName("tool_calls")] OpenAiToolCall[]? ToolCalls = null);

    private sealed record OpenAiToolCall([property: JsonPropertyName("function"), JsonRequired] OpenAiToolCall.FunctionCall Function)
    {
        public sealed record FunctionCall([property: JsonPropertyName("name"), JsonRequired] string Name);
    }

    private sealed record OpenAiCompletion([property: JsonPropertyName("choices"), JsonRequired] OpenAiCompletion.CompletionChoice[] Choices)
    {
        public sealed record CompletionChoice(
            [property: JsonPropertyName("message"), JsonRequired] OpenAiMessageAnswer Message,
            [property: JsonPropertyName("finish_reason"), JsonRequired] string FinishReason);
    }
}
