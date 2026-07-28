using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackseatDriver.Cli.Provider;

/// <summary>
/// A <see cref="IProvider"/> using the OpenAI chat completions API.
/// </summary>
/// <param name="baseAddress">Base address of the host the provider is running on.</param>
public sealed class OpenAiChatCompletion(Uri baseAddress) : IProvider
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new() { Converters = { new OpenAiChatMessageSerializer() }};
    private readonly HttpClient httpClient = new() { BaseAddress = baseAddress };

    /// <inheritdoc />
    public async Task<string> GenerateAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("v1/chat/completions", new { messages = messages }, jsonSerializerOptions, cancellationToken);
        var parsedResponse = await response.Content.ReadFromJsonAsync<CompletionResult>(cancellationToken: cancellationToken);

        return parsedResponse?.Choices.First().Message.Content ?? throw new InvalidOperationException("Provider did not respond as expected.");
    }

    private sealed record CompletionResult([property: JsonPropertyName("choices")] CompletionResult.Choice[] Choices)
    {
        public sealed record Choice([property: JsonPropertyName("message")] ChatMessage Message);
    }

    private sealed class OpenAiChatMessageSerializer : JsonConverter<ChatMessage>
    {
        public override bool HandleNull => false;

        public override ChatMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Expected start of an object, but got {reader.TokenType}");
            }

            string? content = null;
            ChatRole? role = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (content is null)
                    {
                        throw new JsonException($"Missing required property {nameof(content)}.");
                    }

                    if (role is null)
                    {
                        throw new JsonException($"Missing required property {nameof(role)}.");
                    }

                    return new ChatMessage(role.Value, content);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var name = reader.GetString();
                    reader.Read();

                    var value = reader.GetString();
                    switch (name)
                    {
                        case nameof(content):
                            content = value;
                            break;

                        case nameof(role):
                            if (!Enum.TryParse(value, ignoreCase: true, out ChatRole parsedRole))
                            {
                                throw new JsonException($"Invalid role '{value}'.");
                            }

                            role = parsedRole;
                            break;
                    }
                }
            }

            throw new JsonException($"Expected end of object, but got {reader.TokenType}.");
        }

        public override void Write(Utf8JsonWriter writer, ChatMessage value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("role", value.Sender.ToString().ToLowerInvariant());
            writer.WriteString("content", value.Content);
            writer.WriteEndObject();
        }
    }
}
