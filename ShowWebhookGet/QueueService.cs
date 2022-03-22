namespace ShowWebhookGet;

public class QueueService
{
    private Queue<string> Queue;

    public QueueService()
    {
        Queue = new Queue<string>();
    }

    public void Push(string content) { Queue.Enqueue(content); }

    public string Pop() => Queue.TryDequeue(out var result) ? result : string.Empty;
}
