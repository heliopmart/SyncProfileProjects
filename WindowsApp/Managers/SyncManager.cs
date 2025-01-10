using System;
using System.Threading.Tasks;
using WindowsApp.Models;
using WindowsApp.Managers.Uploaders;

namespace WindowsApp.Managers{
    public class SyncManager
    {
        private readonly QueueManager _queueManager;

        public SyncManager(QueueManager queueManager)
        {
            _queueManager = queueManager;
        }

        public async Task ProcessQueueAsync()
        {
            while (!_queueManager.IsEmpty())
            {
                if (_queueManager.TryDequeue(out var fileChange))
                {
                    try
                    {
                        await ProcessFileChangeAsync(fileChange);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing file change: {ex.Message}");
                    }
                }
            }
        }
        private async Task ProcessFileChangeAsync(FileChange fileChange){

            if(fileChange.ChangeType != "FolderRenamed"){
                await new BoxUploader().UploadManager(fileChange.FilePath, fileChange.ChangeType, null);
            }else{
                await new BoxUploader().UploadManager(fileChange.FilePath, fileChange.ChangeType, fileChange.OldFilePath);
            }
        }
    }
}