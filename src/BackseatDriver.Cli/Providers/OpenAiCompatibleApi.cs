using System.Net.Http.Json;
using System.Text.Json.Serialization;
using BackseatDriver.Abstractions;
using BackseatDriver.Abstractions.MessageTypes;

namespace BackseatDriver.Cli.Provider;

/// <summary>
/// A completion provider for OpenAI compatible APIs.
/// </summary>
public sealed class OpenAiCompatibleApi : ICompletionProvider, IDisposable
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// Create a new instance of the <seealso cref="OpenAiCompatibleApi"/>.
    /// </summary>
    /// <param name="baseUri">Base URI of the API.</param>
    /// <param name="configureClient">Additional configuration for the used <see cref="httpClient"/>.</param>
    public OpenAiCompatibleApi(Uri baseUri, Action<HttpClient>? configureClient = null)
    {
        var client = new HttpClient { BaseAddress = baseUri };
        configureClient?.Invoke(client);

        httpClient = client;
    }

    /// <inheritdoc/>
    public async Task<GeneratedResponse> GenerateAsync(IEnumerable<Abstractions.IChatMessage> messages, CancellationToken cancellationToken = default)
    {
        var chatMessages = messages.Select(OpenAiMessage.From).ToList();
        var response = await httpClient.PostAsJsonAsync("v1/chat/completions", new { messages = chatMessages }, cancellationToken: cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var parsedResponse = await response.Content.ReadFromJsonAsync<OpenAiCompletion>(cancellationToken: cancellationToken);
            var selectedAnswer = parsedResponse?.Choices.FirstOrDefault();

            if (selectedAnswer?.FinishReason is null or not "stop")
            {
                throw new InvalidOperationException("Model answer invalid.");
            }

            var message = Parse(selectedAnswer.Message.Content);
            return new GeneratedResponse(message) { ReasoningMessage = selectedAnswer.Message.Reasoning };
        }

        throw new InvalidOperationException("Model answer invalid.");
    }

    private static AssistantMessageBase Parse(string content)
    {
        if (content.StartsWith(AssistantResponse.Prefix))
        {
            return new AssistantResponse(content[AssistantResponse.Prefix.Length..]);
        }

        if (content.StartsWith(Abstractions.MessageTypes.ToolRequest.Prefix))
        {
            return new Abstractions.MessageTypes.ToolRequest(content[Abstractions.MessageTypes.ToolRequest.Prefix.Length..]);
        }

        if (content.StartsWith(ClarificationQuestion.Prefix))
        {
            return new ClarificationQuestion(content[ClarificationQuestion.Prefix.Length..]);
        }

        throw new InvalidOperationException("Model answer invalid.");
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        httpClient.Dispose();
    }

    private sealed record OpenAiMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content,
        [property: JsonPropertyName("reasoning")] string? Reasoning = null)
    {
        public static OpenAiMessage From(Abstractions.IChatMessage message)
        {
            return new OpenAiMessage(message.Sender.ToString().ToLowerInvariant(), message.Content);
        }
    }

    private sealed record OpenAiCompletion([property: JsonPropertyName("choices"), JsonRequired] OpenAiCompletion.CompletionChoice[] Choices)
    {
        public sealed record CompletionChoice(
            [property: JsonPropertyName("message"), JsonRequired] OpenAiMessage Message,
            [property: JsonPropertyName("finish_reason"), JsonRequired] string FinishReason);
    }
}
