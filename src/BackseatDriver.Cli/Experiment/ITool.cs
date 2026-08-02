namespace BackseatDriver.Cli.Experiment;

/// <summary>
/// A tool that may be requested by a model, processed by a <seealso cref="ToolEngine"/>.
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

}
