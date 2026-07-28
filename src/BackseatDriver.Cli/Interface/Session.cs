using BackseatDriver.Cli.Provider;

namespace BackseatDriver.Cli.Interface;

/// <summary>
/// A session started by the user, able to repeatedly send prompts to the given <see cref="IProvider"/>.
/// </summary>
/// <param name="provider">The <see cref="IProvider"/> to use when generating responses.</param>
public sealed class Session(IProvider provider) : IAsyncDisposable
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
                var processing = interaction.StartAsync(prompt);

                Console.WriteLine("Thinking...");
                await processing;

                if (interaction.CurrentState is not InteractionState.Finished)
                {
                    Console.WriteLine("ERR: Model did not respond as expected. Sucks to be you right now.");
                    continue;
                }

                Console.WriteLine(interaction.Response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                continue;
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
}
