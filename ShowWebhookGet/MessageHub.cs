using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace ShowWebhookGet
{
    public class MessageHub : Hub
    {
        private readonly QueueService _queueService;

        public MessageHub(QueueService queueService)
        {
            _queueService = queueService;
        }

        public async IAsyncEnumerable<string> Streaming([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var content = _queueService.Pop();
                if (!string.IsNullOrEmpty(content)) yield return content;
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
