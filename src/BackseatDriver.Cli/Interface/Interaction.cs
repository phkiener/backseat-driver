using BackseatDriver.Abstractions;
using BackseatDriver.Abstractions.MessageTypes;

namespace BackseatDriver.Cli.Interface;

/// <summary>
/// A single interaction, triggered by the user.
/// </summary>
public sealed class Interaction(ICompletionProvider provider, CancellationToken cancellationToken) : IAsyncDisposable
{
    private static readonly List<IChatMessage> systemPrompt =
    [
        new SystemPrompt("You are a simple agent. You respond in the shortest possible way, exactly in the format you're told to adhere to."),
        new SystemPrompt("If you need to access data that you do not have, you can use one of the given tools."),
        new SystemPrompt("You can use a tool called 'cat' to read the contents of a file on disk. Invoke it as 'cat $path' to read the contents of the file located at $path."),
        new SystemPrompt("You can use a tool called 'ls' to list all contents of a directory. Invoke it as 'ls $path' to list the contents of the directory located at $path."),
        new SystemPrompt("You are allowed to ask one or more clarifying questions to the user before answering their prompt. Keep it short and succint, feel free to be blunt."),
        new SystemPrompt("Prefix your answer with 'RESPONSE: ' if you intend to answer the prompt. "),
        new SystemPrompt("Prefix your answer with 'TOOL: ' if you need to invoke a tool to provide an answer. The user will respond with 'OUTPUT: ', which will contain the output of the invoked tool. Never prefix your own answer with 'OUTPUT: ', this will always be supplied by the user."),
        new SystemPrompt("Prefix your answer with 'PROMPT: ' if you need to ask the user a clarifying question to proceed. The user will respond with 'ANSWER: ', which will be the user's answer to your prompt. Never prefix your own answer with 'ANSWER: ', this will always be supplied by the user."),
        new SystemPrompt("You may only ever prefix your response with 'RESPONSE: ', 'TOOL: ' or 'PROMPT: '. NEVER provide 'ANSWER: ' or 'OUTPUT: ' yourself.")
    ];

    private readonly CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    private readonly List<IChatMessage> history = [];

    /// <summary>
    /// The current state of the interaction.
    /// </summary>
    /// <see cref="InteractionState"/>
    public InteractionState CurrentState { get; private set; } = InteractionState.Pending;

    /// <summary>
    /// Send a message to the harness.
    /// </summary>
    /// <param name="message">The initial message to send.</param>
    /// <returns>The message returned by the provider.</returns>
    public async Task<AssistantMessageBase> StartAsync(UserPrompt message)
    {
        history.Clear();
        history.AddRange(systemPrompt);
        history.Add(message);

        var response = await GenerateResponseAsync();
        history.Add(response);

        return response;
    }

    /// <summary>
    /// Continue the interaction with the given message.
    /// </summary>
    /// <param name="message">The message send.</param>
    /// <returns>The message returned by the provider.</returns>
    public async Task<AssistantMessageBase> ContinueAsync(UserMessageBase message)
    {
        history.Add(message);

        var response = await GenerateResponseAsync();
        history.Add(response);

        return response;
    }

    private async Task<AssistantMessageBase> GenerateResponseAsync()
    {
        CurrentState = InteractionState.Running;
        var response = await provider.GenerateAsync(history, cancellationTokenSource.Token);

        CurrentState = response.Message switch
        {
            ClarificationQuestion or ToolRequest => InteractionState.Waiting,
            _ => InteractionState.Finished
        };

        return response.Message;
    }

    public async ValueTask DisposeAsync()
    {
        await cancellationTokenSource.CancelAsync();

        cancellationTokenSource.Dispose();
    }
}
