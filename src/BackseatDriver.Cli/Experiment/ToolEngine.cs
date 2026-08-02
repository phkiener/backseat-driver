namespace BackseatDriver.Cli.Experiment;

/// <summary>
/// The execution engine for all tools the assistant wants to invoke.
/// </summary>
public sealed class ToolEngine
{
    private readonly List<ITool> tools = [];

    /// <summary>
    /// Denotes wheter the tool engine is currently executing a tool or is simply idle.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Returns all registered tools.
    /// </summary>
    public IEnumerable<ITool> Tools => tools.AsReadOnly();

    /// <summary>
    /// A callback invoked for every tool that is started; contains the full command as parameter.
    /// </summary>
    /// <seealso cref="OnToolFinished"/>
    public event EventHandler<string>? OnToolStarted;

    /// <summary>
    /// A callback invoked once a started tool is finished.
    /// </summary>
    /// <seealso cref="OnToolStarted"/>
    public event EventHandler? OnToolFinished;

    /// <summary>
    /// Invoke the given tool, returning the output.
    /// </summary>
    /// <param name="name">The name of the tool to invoke.</param>
    /// <returns>Output of the tool.</returns>
    public async Task<string> InvokeAsync(string name)
    {
        IsRunning = true;
        OnToolStarted?.Invoke(this, "");

        await Task.Delay(TimeSpan.FromMilliseconds(250));

        IsRunning = false;
        OnToolFinished?.Invoke(this, EventArgs.Empty);

        return "";
    }

    /// <summary>
    /// Register the tool to be invoked.
    /// </summary>
    /// <param name="tool">The tool to register.</param>
    public void Register(ITool tool)
    {
        tools.Add(tool);
    }
}
