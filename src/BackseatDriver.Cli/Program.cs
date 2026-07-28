using BackseatDriver.Cli;
using BackseatDriver.Cli.Interface;
using BackseatDriver.Cli.Provider;

var baseUri = new Uri("http://localhost:8080");
var provider = new OpenAiChatCompletion(baseUri);

await using var session = new Session(provider);

var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellationTokenSource.Cancel();

await session.RunAsync(cancellationTokenSource.Token);
