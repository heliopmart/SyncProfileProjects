using Box.Sdk.Gen;
using WindowsApp.Models;
using WindowsApp.Managers.Uploaders;

namespace WindowsApp.Managers
{
    
    public class SyncManager(BoxClient auth, QueueManager queueManager)
    {
        private readonly QueueManager _queueManager = queueManager;

        public async Task ProcessQueueAsync()
        {
            while (!_queueManager.IsEmpty())
            {
                if (_queueManager.TryDequeue(out var fileChange))
                {
                    try
                    {
                        await ProcessFileChangeAsync(auth, fileChange);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing file change: {ex.Message}");
                    }
                }
            }
        }
        private static async Task ProcessFileChangeAsync(BoxClient auth, FileChange fileChange){
            Console.WriteLine("Uploading...");
            if(fileChange.ChangeType == "FolderRenamed" || fileChange.ChangeType == "FileRenamed"){
                if(!await new BoxUploader().UploadManager(auth, fileChange.FilePath, fileChange.ChangeType, fileChange.OldFilePath)){
                    throw new InvalidOperationException($"SyncManager : ProcessFileChangeAsync() => UploadManager(), Erro: Upload Rename não concluido!");
                }
            }else{
                if(!await new BoxUploader().UploadManager(auth, fileChange.FilePath, fileChange.ChangeType, null)){
                    throw new InvalidOperationException($"SyncManager : ProcessFileChangeAsync() => UploadManager(), Erro: Upload não concluido!");
                }
            }
        }
    }
}