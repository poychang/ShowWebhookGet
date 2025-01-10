using ShowWebhookGet;
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
app.MapPost("api/webhook", (HttpContext context, QueueService queueService) =>
{
    using var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8);
    var content = new
    {
        WebhookHeaders = JsonSerializer.Serialize(context.Request.Headers),
        WebhookQuery = JsonSerializer.Serialize(context.Request.Query.Select(p => KeyValuePair.Create<string, string>(p.Key, p.Value))),
        WebhookBody = reader.ReadToEndAsync().GetAwaiter().GetResult()
    };
    queueService.Push(JsonSerializer.Serialize(content).Replace(System.Environment.NewLine, string.Empty));
});
app.MapGet("api/webhook", (HttpContext context, QueueService queueService) =>
{
    var content = new
    {
        WebhookHeaders = JsonSerializer.Serialize(context.Request.Headers),
        WebhookQuery = JsonSerializer.Serialize(context.Request.Query.Select(p => KeyValuePair.Create<string, string>(p.Key, p.Value))),
        WebhookBody = "HTTP GET has no body in general."
    };
    queueService.Push(JsonSerializer.Serialize(content).Replace(System.Environment.NewLine, string.Empty));
});
// Message hub
app.MapHub<MessageHub>("/messageHub");

app.Run();
