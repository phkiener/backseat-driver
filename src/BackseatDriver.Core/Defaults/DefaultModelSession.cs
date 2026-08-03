using BackseatDriver.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace BackseatDriver.Core.Defaults;

/// <summary>
/// The default implementation of an <see cref="IModelSession"/>.
/// </summary>
/// <param name="systemPromptProvider">The <see cref="ISystemPromptProvider"/> to supply the system prompt for the model.</param>
/// <param name="toolEngine">The <see cref="IToolEngine"/> to invoke <see cref="ITool"/>s.</param>
/// <param name="completionProvider">The <see cref="ICompletionProvider"/> to generate the assistant's response.</param>
/// <param name="logger">The <see cref="ILogger"/> to use.</param>
public sealed class DefaultModelSession(ISystemPromptProvider systemPromptProvider, IToolEngine toolEngine, ICompletionProvider completionProvider, ILogger<DefaultModelSession> logger) : IModelSession
{
    private readonly List<IMessage> messageHistory = [systemPromptProvider.SystemPrompt];

    /// <inheritdoc />
    public bool IsWorking { get; private set; }

    /// <inheritdoc />
    public event EventHandler? OnProcessingStarted;

    /// <inheritdoc />
    public event EventHandler? OnProcessingCompleted;

    /// <inheritdoc />
    public event EventHandler<IMessage>? OnMessageAdded;

    /// <inheritdoc />
    public async Task<bool> SendAsync(string message)
    {
        if (IsWorking)
        {
            return false;
        }

        IsWorking = true;
        OnProcessingStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            IAssistantMessage? response = null;
            IMessage request = new Message.UserPrompt(message);

            while (response is not Message.AssistantResponse)
            {
                response = await GenerateAsync(request);

                if (response is Message.ToolRequest toolRequest)
                {
                    var output = await toolEngine.InvokeAsync(toolRequest.Content);
                    request = new Message.ToolResult(toolRequest.Id, output);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to generate response.");
        }
        finally
        {
            IsWorking = false;
            OnProcessingCompleted?.Invoke(this, EventArgs.Empty);
        }

        return true;
    }

    private async Task<IAssistantMessage> GenerateAsync(IMessage message)
    {
        messageHistory.Add(message);
        OnMessageAdded?.Invoke(this, message);

        var response = await completionProvider.GenerateAsync(messageHistory, toolEngine.Tools);
        messageHistory.Add(response);
        OnMessageAdded?.Invoke(this, response);

        return response;
    }
}
