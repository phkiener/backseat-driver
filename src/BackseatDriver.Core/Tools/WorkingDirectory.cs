using BackseatDriver.Core.Abstractions;

namespace BackseatDriver.Core.Tools;

/// <summary>
/// A tool that handles the current working directory.
/// </summary>
public sealed class WorkingDirectory : ITool
{
    /// <inheritdoc />
    public string Name => "working_directory";

    /// <inheritdoc />
    public string Description => "Returns an absolute path to the current working directory.";

    /// <inheritdoc />
    public Task<string> InvokeAsync()
    {
        return Task.FromResult(Environment.CurrentDirectory);
    }
}
