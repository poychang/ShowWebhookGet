using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSingleton<QueueService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) app.UseHsts();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();

// Webhook endpoint
app.MapPost("api/webhook", (HttpContext context, QueueService queue) => queue.Push(new Content(context)));
app.MapGet("api/webhook", (HttpContext context, QueueService queue) => queue.Push(new Content(context)));
// Message hub
app.MapHub<MessageHub>("/messageHub");

app.Run();

class MessageHub(QueueService queueService) : Hub
{
    public async IAsyncEnumerable<string> Streaming([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (true)
        {
            var content = queueService.Pop();
            if (!string.IsNullOrEmpty(content)) yield return content;
            await Task.Delay(1000, cancellationToken);
        }
    }
}

class QueueService
{
    private readonly Queue<string> _queue = new();

    public void Push(string content) { _queue.Enqueue(content); }

    public void Push<T>(T content) { _queue.Enqueue(JsonSerializer.Serialize(content).Replace(Environment.NewLine, string.Empty)); }

    public string Pop() => _queue.TryDequeue(out var result) ? result : string.Empty;
}

class Content(HttpContext context)
{
    public string WebhookHeaders { get => JsonSerializer.Serialize(context.Request.Headers); }
    public string WebhookQuery { get => JsonSerializer.Serialize(context.Request.Query.Select(p => KeyValuePair.Create(p.Key, $"{p.Value}"))); }
    public string WebhookBody
    {
        get => context.Request.Method switch
        {
            "POST" => new StreamReader(context.Request.Body, System.Text.Encoding.UTF8).ReadToEndAsync().GetAwaiter().GetResult(),
            "GET" => "HTTP GET has no body in general.",
            _ => "Unsupported HTTP method."
        };
    }
}
