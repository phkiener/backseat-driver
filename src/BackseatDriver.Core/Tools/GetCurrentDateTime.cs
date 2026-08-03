using BackseatDriver.Core.Abstractions;

namespace BackseatDriver.Core.Tools;

/// <summary>
/// A simple <see cref="ITool"/> serving as an example.
/// </summary>
public sealed class GetCurrentDateTime : ITool
{
    /// <inheritdoc />
    public string Name => "get_datetime_now";

    /// <inheritdoc />
    public string Description => "Returns the current date and time, formatted as YYYY-MM-DDTHH:mm:ssZ. THis function handles timezones correctly.";

    /// <inheritdoc />
    public Task<string> InvokeAsync()
    {
        return Task.FromResult(DateTime.Now.ToString("O"));
    }
}
