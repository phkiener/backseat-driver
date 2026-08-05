using System.Text;
using BackseatDriver.Core.Abstractions;

namespace BackseatDriver.Core.Tools;

/// <summary>
/// A tool that lists the files of the current directory; basically <c>ls</c>.
/// </summary>
public sealed class ListFiles : ITool
{
    /// <inheritdoc />
    public string Name => "list_files";

    /// <inheritdoc />
    public string Description => "Lists all files in the current working directory with their absolute paths. Hidden files are included. Directories will have a trailing slash.";

    /// <inheritdoc />
    public Task<string> InvokeAsync()
    {
        var targetDirectory = Environment.CurrentDirectory;
        var result = new StringBuilder();

        foreach (var directory in Directory.EnumerateDirectories(targetDirectory))
        {
            result.AppendLine(Path.Combine(targetDirectory, directory) + "/");
        }

        foreach (var directory in Directory.EnumerateFiles(targetDirectory))
        {
            result.AppendLine(Path.Combine(targetDirectory, directory));
        }

        return Task.FromResult(result.ToString());
    }
}
