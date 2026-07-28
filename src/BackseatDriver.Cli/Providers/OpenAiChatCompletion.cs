using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BackseatDriver.Cli.Provider;

/// <summary>
/// A <see cref="IProvider"/> using the OpenAI chat completions API.
/// </summary>
/// <param name="baseAddress">Base address of the host the provider is running on.</param>
public sealed class OpenAiChatCompletion(Uri baseAddress) : IProvider
{
    private readonly HttpClient httpClient = new() { BaseAddress = baseAddress };

    /// <inheritdoc />
    public async Task<string> GenerateAsync(IEnumerable<string> messages, CancellationToken cancellationToken = default)
    {
        ChatMessage[] chatMessages = [
            new("system", "You are a simple agent. You respond in the shortest possible way, exactly in the format you're told to adhere to."),
            new("system", "If you need to access data that you do not have, you can use one of the given tools."),
            new("system", "You can use a tool called 'cat' to read the contents of a file on disk. Invoke it as 'cat $path' to read the contents of the file located at $path."),
            new("system", "You can use a tool called 'ls' to list all contents of a directory. Invoke it as 'ls $path' to list the contents of the directory located at $path."),
            new("system", "You are allowed to ask one or more clarifying questions to the user before answering their prompt. Keep it short and succint, feel free to be blunt."),
            new("system", "Prefix your answer with 'RESPONSE: ' if you intend to answer the prompt. Prefix your answer with 'TOOL: ' if you need to invoke a tool to provide an answer. Prefix your answer with 'PROMPT: ' if you need to ask the user a clarifying question to proceed."),
            ..messages.Where(static m => !string.IsNullOrWhiteSpace(m)).Select(static m => new ChatMessage("user", m))
        ];

        var response = await httpClient.PostAsJsonAsync("v1/chat/completions", new { messages = chatMessages }, cancellationToken);
        var parsedResponse = await response.Content.ReadFromJsonAsync<CompletionResult>(cancellationToken: cancellationToken);

        return parsedResponse?.Choices.First().Message.Content ?? throw new InvalidOperationException("Provider did not respond as expected.");
    }

    private sealed record ChatMessage([property: JsonPropertyName("role")] string Role, [property: JsonPropertyName("content")] string Content);

    private sealed record CompletionResult([property: JsonPropertyName("choices")] CompletionResult.Choice[] Choices)
    {
        public sealed record Choice([property: JsonPropertyName("message")] ChatMessage Message);
    }
}
