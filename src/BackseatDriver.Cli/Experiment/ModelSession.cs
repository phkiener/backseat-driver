namespace BackseatDriver.Cli.Experiment;

/// <summary>
/// An interaction session with a model, handling tool calls on its own.
/// </summary>
/// <param name="toolEngine">The tool engine responsible for providing and invoking tools.</param>
/// <param name="completionProvider">The completion provider used to generate responses.</param>
public sealed class ModelSession(ToolEngine toolEngine, ICompletionProvider completionProvider)
{
    private readonly List<IMessage> history = [];
    // System Prompt?

    /// <summary>
    /// Denotes wheter the session is currently processing a message or is simply idle.
    /// </summary>
    public bool IsWorking { get; private set; }

    /// <summary>
    /// A callback invoked everytime the processing of a message has started.
    /// </summary>
    public event EventHandler? OnProcessingStarted;

    /// <summary>
    /// A callback invoked everytime the processing of a message is completed.
    /// </summary>
    public event EventHandler? OnProcessingCompleted;

    /// <summary>
    /// A callback invoked everytime a message is added to the message history.
    /// </summary>
    public event EventHandler<IMessage>? OnMessageAdded;

    /// <summary>
    /// Send a message to the model.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <remarks>
    /// A handful of special commands are supported which are not sent to the model.
    /// </remarks>
    public async Task SendAsync(string message)
    {
        IsWorking = true;
        OnProcessingStarted?.Invoke(this, EventArgs.Empty);

        IAssistantMessage? response = null;
        IMessage request = new Message.UserPrompt(message);

        while (response is not Message.AssistantResponse)
        {
            response = await GenerateAsync(request);

            if (request is Message.ToolRequest toolRequest)
            {
                var output = await toolEngine.InvokeAsync(toolRequest.Content);
                request = new Message.ToolResult(output);
            }
        }

        IsWorking = false;
        OnProcessingCompleted?.Invoke(this, EventArgs.Empty);
    }

    private async Task<IAssistantMessage> GenerateAsync(IMessage message)
    {
        history.Add(message);
        OnMessageAdded?.Invoke(this, message);

        var response = await completionProvider.GenerateAsync(history, toolEngine.Tools);
        history.Add(response);
        OnMessageAdded?.Invoke(this, response);

        return response;
    }
}
