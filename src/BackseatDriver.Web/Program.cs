using BackseatDriver.Core;
using BackseatDriver.Core.Tools;
using BackseatDriver.OpenAI;
using BackseatDriver.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(static o => o.SingleLine = true)
    .AddFilter("Microsoft", static f => f >= LogLevel.Warning)
    .AddFilter("Microsoft.Hosting.Lifetime", static f => f >= LogLevel.Information);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBackseatDriver()
    .AddOpenAI()
    .AddTool<GetCurrentDateTime>()
    .AddTool<WorkingDirectory>()
    .AddTool<ListFiles>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
