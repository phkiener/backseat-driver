namespace BackseatDriver.Core.Abstractions;

/// <summary>
/// A tool that may be requested by an assistant, processed by an <seealso cref="IToolEngine"/>.
/// </summary>
public interface ITool
{
    /// <summary>
    /// The (unique) name for this tool.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// A description of this tool; the decision on whether to request a tool will be guided by this.
    /// </summary>
    public string Description { get; }

    // TODO: Parameters

    /// <summary>
    /// Invoke the tool, returning the full output.
    /// </summary>
    /// <returns>The complete output of the tool.</returns>
    public Task<string> InvokeAsync();
}
