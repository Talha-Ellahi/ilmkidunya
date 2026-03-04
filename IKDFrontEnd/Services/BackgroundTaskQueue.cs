using IKDFrontEnd.Interfaces;

namespace IKDFrontEnd.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ILogger<BackgroundTaskQueue> _logger;

        public BackgroundTaskQueue(ILogger<BackgroundTaskQueue> logger)
        {
            _logger = logger;
        }

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            Task.Run(async () =>
            {
                try
                {
                    await workItem(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background task failed");
                }
            });
        }
    }

}
