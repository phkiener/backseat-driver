using BackseatDriver.Core.Abstractions;

namespace BackseatDriver.Core.Defaults;

/// <summary>
/// A <see cref="IToolProvider"/> that will automatically expose all <see cref="ITool"/>s that have
/// been registered with the <see cref="IServiceProvider"/>.
/// </summary>
public sealed class ServiceCollectionToolProvider(IEnumerable<ITool> registeredTools) : IToolProvider
{
    private readonly ITool[] tools = [.. registeredTools];

    /// <inheritdoc />
    public IEnumerable<ITool> Tools => tools.AsReadOnly();
}
