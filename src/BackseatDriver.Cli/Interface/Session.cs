using System.Text.RegularExpressions;
using BackseatDriver.Abstractions;
using BackseatDriver.Abstractions.MessageTypes;

namespace BackseatDriver.Cli.Interface;

/// <summary>
/// A session started by the user, able to repeatedly send prompts to the given <see cref="ICompletionProvider"/>.
/// </summary>
/// <param name="provider">The <see cref="ICompletionProvider"/> to use when generating responses.</param>
public sealed partial class Session(ICompletionProvider provider) : IAsyncDisposable
{
    private CancellationTokenSource? currentSession;

    /// <summary>
    /// Start a new session, capturing the console in the process.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to signal the end of the session.</param>
    /// <exception cref="InvalidOperationException">When a session is already running.</exception>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        if (currentSession is not null)
        {
            throw new InvalidOperationException("A session is already running on this instance.");
        }

        currentSession = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        while (!currentSession.IsCancellationRequested)
        {
            Console.Write("> ");
            var prompt = Console.ReadLine();

            if (prompt is null or "")
            {
                continue;
            }

            if (prompt is "/bye" or "/quit")
            {
                break;
            }

            try
            {
                await using var interaction = new Interaction(provider, currentSession.Token);
                var processing = interaction.StartAsync(new UserPrompt(prompt));

                while (interaction.CurrentState is not InteractionState.Finished)
                {
                    Console.WriteLine(" ... running ...");
                    var response = await processing;

                    if (response is AssistantResponse assistantResponse)
                    {
                        Console.WriteLine($"=> {assistantResponse.Text}");
                        continue;
                    }

                    if (response is ToolRequest toolRequest)
                    {
                        Console.WriteLine($"The assistant wants to invoke '{toolRequest.Invocation}'");

                        var lsMatch = LsToolRegex.Match(toolRequest.Invocation);
                        if (lsMatch.Success)
                        {
                            var filePath = lsMatch.Groups["Path"].Value;

                            string? content;
                            try
                            {
                                var contents = Directory.GetFileSystemEntries(filePath);
                                if (Directory.GetParent(filePath) is not null)
                                {
                                    contents = ["..", ..contents];
                                }

                                content = string.Join("\n", contents);
                            }
                            catch (Exception e)
                            {
                                content = $"IO Exception: {e.Message}";
                            }

                            processing = interaction.ContinueAsync(new ToolOutput(content));
                            continue;
                        }

                        var catMatch = CatToolRegex.Match(toolRequest.Invocation);
                        if (catMatch.Success)
                        {
                            var filePath = catMatch.Groups["Path"].Value;

                            string? content;
                            try
                            {
                                content = await File.ReadAllTextAsync(filePath, cancellationToken);
                            }
                            catch (Exception e)
                            {
                                content = $"IO Exception: {e.Message}";
                            }

                            processing = interaction.ContinueAsync(new ToolOutput(content));
                            continue;
                        }

                        processing = interaction.ContinueAsync(new ToolOutput("Unknown tool."));
                        continue;
                    }

                    if (response is ClarificationQuestion clarificationQuestion)
                    {
                        Console.WriteLine($"=> {clarificationQuestion.Question}");
                        Console.Write(">> ");
                        var answer = Console.ReadLine() ?? "";

                        processing = interaction.ContinueAsync(new ClarificationAnswer(answer));
                        continue;
                    }

                    Console.WriteLine("ERR: Model did not respond as expected. Sucks to be you right now.");
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (currentSession is not null)
        {
            await currentSession.CancelAsync();
            currentSession.Dispose();
        }
    }

    [GeneratedRegex("^ls (?<Path>.+)$")]
    private static partial Regex LsToolRegex { get; }

    [GeneratedRegex("^cat (?<Path>.+)$")]
    private static partial Regex CatToolRegex { get; }
}
