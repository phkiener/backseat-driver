using System.Net.Http.Json;
using BackseatDriver.Core;
using BackseatDriver.Core.Abstractions;
using BackseatDriver.OpenAI.Model;

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
        var request = new ChatCompletionRequest
        {
            Messages = history.Select(MapMessage).Where(m => m is not null).ToArray()!,
            Tools = availableTools.Select(MapTool).ToArray()
        };

        var response = await httpClient.PostAsJsonAsync("v1/chat/completions", request);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var parsedResponse = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();
            var selectedAnswer = parsedResponse?.Choices.FirstOrDefault();

            if (selectedAnswer?.FinishReason is null or ChatCompletionResponse.FinishReason.Length or ChatCompletionResponse.FinishReason.ContentFilter)
            {
                throw new InvalidOperationException("Model answer invalid.");
            }

            if (selectedAnswer.FinishReason is ChatCompletionResponse.FinishReason.ToolCalls or ChatCompletionResponse.FinishReason.FunctionCalls)
            {
                var functionCall = selectedAnswer.Message.ToolCalls?.FirstOrDefault()
                                   ?? throw new InvalidOperationException("Model stopped for a tool call but no tool call was returned.");

                return new Message.ToolRequest(functionCall.Id, functionCall.Function.Name) { Reasoning = selectedAnswer.Message.Reasoning };
            }

            return new Message.AssistantResponse(selectedAnswer.Message.Content ?? "") { Reasoning = selectedAnswer.Message.Reasoning };
        }

        throw new InvalidOperationException($"API returned non-success status code {(int)response.StatusCode}.");
    }

    private static ChatCompletionMessageParam? MapMessage(IMessage message)
    {
        return message switch
        {
            Message.AssistantResponse assistantResponse => new ChatCompletionMessageParam.AssistantMessage { Content = assistantResponse.Content },
            Message.ToolRequest toolRequest => new ChatCompletionMessageParam.AssistantMessage { ToolCalls = [new ChatCompletionFunctionToolCall { Id = toolRequest.Id, Function = new() { Name = toolRequest.Content }}]},
            Message.SystemPrompt systemPrompt => new ChatCompletionMessageParam.SystemMessage { Content = systemPrompt.Content },
            Message.ToolResult toolResult => new ChatCompletionMessageParam.FunctionMessage { ToolCallId = toolResult.Id, Content = toolResult.Content },
            Message.UserPrompt userPrompt => new ChatCompletionMessageParam.UserMessage { Content = userPrompt.Content },
            _ => null,
        };
    }

    private static ChatCompletionFunctionTool MapTool(ITool tool)
    {
        return new ChatCompletionFunctionTool
        {
            Function = new ChatCompletionFunctionTool.FunctionToolDefinition
            {
                Name = tool.Name,
                Description = tool.Description
            }
        };
    }

    /// <inheritdoc />
    public void Dispose()
    {
        httpClient.Dispose();
    }
}
