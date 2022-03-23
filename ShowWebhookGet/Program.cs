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
        WebhookHeaders = context.Request.Headers,
        WebhookQuery = context.Request.Query,
        WebhookBody = reader.ReadToEndAsync().GetAwaiter().GetResult()
    };
    queueService.Push(JsonSerializer.Serialize(content).Replace(System.Environment.NewLine, string.Empty));
});
// Message hub
app.MapHub<MessageHub>("/messageHub");

app.Run();
