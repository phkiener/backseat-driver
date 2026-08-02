using BackseatDriver.Core.Abstractions;

namespace BackseatDriver.Core.Defaults;

/// <summary>
/// The default implementation of an <see cref="IToolEngine"/>.
/// </summary>
/// <param name="toolProvider">The <see cref="IToolProvider"/> to use.</param>
public sealed class DefaultToolEngine(IToolProvider toolProvider) : IToolEngine
{
    /// <inheritdoc />
    public bool IsRunning { get; private set; }

    /// <inheritdoc />
    public IEnumerable<ITool> Tools => toolProvider.Tools;

    /// <inheritdoc />
    public event EventHandler<string>? OnToolStarted;

    /// <inheritdoc />
    public event EventHandler? OnToolFinished;

    /// <inheritdoc />
    public async Task<string> InvokeAsync(string name)
    {
        IsRunning = true;
        OnToolStarted?.Invoke(this, name);

        try
        {
            var tool = toolProvider.Tools.SingleOrDefault(t => t.Name == name);
            if (tool is null)
            {
                return $"Error: Tool '{name}' is not registered.";
            }

            return await tool.InvokeAsync();
        }
        catch (Exception e)
        {
            return $"Error: Tool '{name}' threw an exception:\n{e.Message}.";
        }
        finally
        {
            IsRunning = false;
            OnToolFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
