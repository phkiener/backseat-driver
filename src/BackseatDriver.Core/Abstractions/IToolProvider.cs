namespace BackseatDriver.Core.Abstractions;

/// <summary>
/// Provides a list of registered tools.
/// </summary>
public interface IToolProvider
{
    /// <summary>
    /// Returns all registered tools.
    /// </summary>
    public IEnumerable<ITool> Tools { get; }
}
