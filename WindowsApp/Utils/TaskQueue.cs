namespace WindowsApp.Utils{
    public class TaskQueue
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public async Task Enqueue(Func<Task> task)
        {
            await _semaphore.WaitAsync();
            try
            {
                await task();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}