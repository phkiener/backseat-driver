using BackseatDriver.Cli.Provider;

namespace BackseatDriver.Cli.Interface;

/// <summary>
/// A single interaction, triggered by the user.
/// </summary>
public sealed class Interaction(IProvider provider, CancellationToken cancellationToken) : IAsyncDisposable
{
    private static readonly List<ChatMessage> systemPrompt =
    [
        new(ChatRole.System, "You are a simple agent. You respond in the shortest possible way, exactly in the format you're told to adhere to."),
        new(ChatRole.System, "If you need to access data that you do not have, you can use one of the given tools."),
        new(ChatRole.System, "You can use a tool called 'cat' to read the contents of a file on disk. Invoke it as 'cat $path' to read the contents of the file located at $path."),
        new(ChatRole.System, "You can use a tool called 'ls' to list all contents of a directory. Invoke it as 'ls $path' to list the contents of the directory located at $path."),
        new(ChatRole.System, "You are allowed to ask one or more clarifying questions to the user before answering their prompt. Keep it short and succint, feel free to be blunt."),
        new(ChatRole.System, "Prefix your answer with 'RESPONSE: ' if you intend to answer the prompt. "),
        new(ChatRole.System, "Prefix your answer with 'TOOL: ' if you need to invoke a tool to provide an answer. The user will respond with 'OUTPUT: ', which will contain the output of the invoked tool. Never prefix your own answer with 'OUTPUT: ', this will always be supplied by the user."),
        new(ChatRole.System, "Prefix your answer with 'PROMPT: ' if you need to ask the user a clarifying question to proceed. The user will respond with 'ANSWER: ', which will be the user's answer to your prompt. Never prefix your own answer with 'ANSWER: ', this will always be supplied by the user."),
        new(ChatRole.System, "You may only ever prefix your response with 'RESPONSE: ', 'TOOL: ' or 'PROMPT: '.")
    ];

    private readonly CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    private readonly List<ChatMessage> history = [];

    /// <summary>
    /// The current state of the interaction.
    /// </summary>
    /// <see cref="InteractionState"/>
    public InteractionState CurrentState { get; private set; } = InteractionState.Pending;

    /// <summary>
    /// The final response, meant to be shown to the user.
    /// </summary>
    public string? Response { get; private set; }

    /// <summary>
    /// Send a message to the harness.
    /// </summary>
    /// <param name="message">The initial message to send.</param>
    public async Task StartAsync(string message)
    {
        CurrentState = InteractionState.Running;

        history.Clear();
        history.AddRange(systemPrompt);
        history.Add(new ChatMessage(ChatRole.User, message));

        var response = await provider.GenerateAsync(history, cancellationTokenSource.Token);

        Response = response;
        CurrentState = InteractionState.Finished;
    }

    public async ValueTask DisposeAsync()
    {
        await cancellationTokenSource.CancelAsync();

        cancellationTokenSource.Dispose();
    }
}
