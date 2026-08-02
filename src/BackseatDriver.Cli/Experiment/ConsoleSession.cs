namespace BackseatDriver.Cli.Experiment;

/// <summary>
/// Renders a <seealso cref="ModelSession"/> and the contained <seealso cref="ToolEngine"/> via stdout.
/// </summary>
/// <remarks>
/// Rendering is started as soon as initialization finishes; there may only ever be a single instance running at the same time.
/// The rendering continues until the instance is <seealso cref="Dispose"/>d.
/// </remarks>
public sealed class ConsoleSession : IDisposable
{
    private readonly ModelSession session;
    private readonly ToolEngine toolEngine;

    public ConsoleSession(ModelSession session, ToolEngine toolEngine)
    {
        this.session = session;
        this.toolEngine = toolEngine;

        // Start reader.
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Dunno yet.
    }
}
