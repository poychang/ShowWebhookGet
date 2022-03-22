using ShowWebhookGet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
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
    var content = reader.ReadToEndAsync().GetAwaiter().GetResult();
    queueService.Push(content.Replace(System.Environment.NewLine, string.Empty));
});
// Pop content from queue
app.MapGet("api/pop", (QueueService queueService) => queueService.Pop());
// server-sent event
app.MapGet("api/server-send-events", async (HttpContext context, QueueService queueService) =>
{
    context.Response.Headers.Add("Content-Type", "text/event-stream");
    while (true)
    {
        await Task.Delay(3000);
        var content = queueService.Pop();

        if (!string.IsNullOrEmpty(content)) await context.Response.WriteAsync($"data: {content}\n\n");

        await context.Response.Body.FlushAsync();
    }
});

app.Run();
