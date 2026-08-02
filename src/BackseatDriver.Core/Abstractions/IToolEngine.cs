namespace BackseatDriver.Core.Abstractions;

/// <summary>
/// The engine handling execution of <see cref="ITool"/>s.
/// </summary>
public interface IToolEngine
{
    /// <summary>
    /// Denotes wheter the tool engine is currently executing a tool or is simply idle.
    /// </summary>
    public bool IsRunning { get; }

    /// <summary>
    /// Returns all registered tools.
    /// </summary>
    public IEnumerable<ITool> Tools { get; }

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
    public Task<string> InvokeAsync(string name); // TODO: Parameters
}
