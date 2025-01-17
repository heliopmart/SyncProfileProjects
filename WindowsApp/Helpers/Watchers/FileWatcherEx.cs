using FileWatcherEx;
using WindowsApp.Managers;
using WindowsApp.Models;
using WindowsApp.Utils;
using WindowsApp.Services;

namespace WindowsApp.Helpers.Watchers{
    public class FileWatcher{
        private readonly string _pathToWatch;
        private readonly QueueManager _queueManager;
        private readonly SyncManager _syncManager;

        private static readonly Dictionary<string, DateTime> CreatedFilesCache = new();
        private static readonly TimeSpan EventCooldown = TimeSpan.FromSeconds(10);
        private static readonly Dictionary<string, DateTime> PendingCreations = new();

        public FileWatcher(string pathToWatch, QueueManager queueManager, SyncManager syncManager)
        {
            _pathToWatch = pathToWatch;
            _queueManager = queueManager;
            _syncManager = syncManager;
        }

        public void StartWatching()
        {
            var _fw = new FileSystemWatcherEx(_pathToWatch){
                IncludeSubdirectories = true  // Garantir que subpastas sejam monitoradas
            };

            _fw.OnRenamed += OnRenamed;
            _fw.OnCreated += OnCreated;
            _fw.OnDeleted += OnDeleted;
            _fw.OnChanged += OnChanged;

            _fw.Start();

            Console.WriteLine("FileWatcher started...");
        }

        public void StopWatching(){
            var _fw = new FileSystemWatcherEx(_pathToWatch);
            _fw.Stop();
        }

        private void OnCreated(object? sender, FileChangedEvent e){        
            lock (PendingCreations)
            {
                // Adicione o arquivo/pasta ao cache de criações pendentes
                PendingCreations[e.FullPath] = DateTime.Now;

                // Aguarde um curto período antes de processar o evento
                Task.Delay(EventCooldown).ContinueWith(_ =>
                {
                    lock (PendingCreations)
                    {
                        // Verifique se o item ainda está pendente (não foi renomeado)
                        if (PendingCreations.ContainsKey(e.FullPath))
                        {
                            PendingCreations.Remove(e.FullPath);
                            _ = CallCreatedHandle(e);
                        }
                    }
                });
            }
        }
        private void OnChanged(object? sender, FileChangedEvent  e){
            lock (CreatedFilesCache)
            {
                if (CreatedFilesCache.ContainsKey(e.FullPath) &&
                    DateTime.Now - CreatedFilesCache[e.FullPath] < EventCooldown)
                {
                    Console.WriteLine($"Change ignored for recently created file: {e.FullPath}");
                    return;
                }
            }

            if (Directory.Exists(e.FullPath))
            {
                // HandleFolder(e.FullPath, "FolderChanged"); --------> Talvez não precise.
            }
            if (File.Exists(e.FullPath))
            {
                Console.WriteLine($"File changed: {e.FullPath}");
                _ = HandleFile(e.FullPath, "FileChanged");
            }
            else
            {
                Console.WriteLine($"Unknown item changed: {e.FullPath}");
            }
        }
        private void OnDeleted(object? sender, FileChangedEvent  e)
        {
            if (!Path.HasExtension(e.FullPath))
            {
                Console.WriteLine($"Folder deleted: {e.FullPath}");
                _ = HandleFolder(e.FullPath, "FolderDeleted");
            }
            else
            {
                Console.WriteLine($"File deleted: {e.FullPath}");
                _ = HandleFile(e.FullPath, "FileDeleted");
            }
        }

        private static bool IsTemporaryFile(string path)
        {
            string fileName = Path.GetFileName(path);

            // Arquivos temporários ou de sistema a serem ignorados
            if (fileName.StartsWith("~$") || fileName.EndsWith(".tmp") || fileName.EndsWith(".lock"))
            {
                return true;
            }

            return false;
        }

        private void OnRenamed(object? sender, FileChangedEvent  e)
        {
            lock (PendingCreations)
            {
                if (PendingCreations.ContainsKey(e.OldFullPath))
                {
                    if(!PendingCreations.Remove(e.OldFullPath)){
                        throw new InvalidOperationException("FileWatcher : OnRenamed(), Error: OldFullPath didn't delete in PendingCreations.");
                    }

                    // TODO: Removido -> PendingCreations[e.FullPath] = DateTime.Now;        
                    _ = CallCreatedHandle(e);
                    return;
                }
            }       

            // Caso contrário, trate como um evento de renomeação normal
            if (Directory.Exists(e.FullPath))
            {
                Console.WriteLine($"Folder renamed from {e.OldFullPath} to {e.FullPath}");
                _ = HandleFolderRenamed(e.OldFullPath, e.FullPath);
            }
            else if (File.Exists(e.FullPath))
            {
                Console.WriteLine($"File renamed from {e.OldFullPath} to {e.FullPath}");
                _ = HandleFileRenamed(e.OldFullPath, e.FullPath);
            }
            else
            {
                Console.WriteLine($"Unknown item renamed: {e.OldFullPath} to {e.FullPath}");
            }
        }
       
        private async Task HandleFolder(string folderPath, string type){
            await AddEventToQueue(type, folderPath);
        }

        private async Task HandleFile(string filePath, string type){
            await AddEventToQueue(type, filePath);
        }

        private async Task HandleFolderRenamed(string oldFolderPath, string newFolderPath){
            await AddEventToQueue("FolderRenamed", newFolderPath, oldFolderPath);
        }

        private async Task HandleFileRenamed(string oldFilePath, string newFilePath){
            await AddEventToQueue("FileRenamed", newFilePath, oldFilePath);
        }       

        private async Task CallCreatedHandle(FileChangedEvent  e){
            if (Directory.Exists(e.FullPath)){
                Console.WriteLine($"Folder created by Call: {e.FullPath}");
                await HandleFolder(e.FullPath, "FolderCreated");
            }
            else if (File.Exists(e.FullPath)){
                Console.WriteLine($"File created by Call: {e.FullPath}");
                await HandleFile(e.FullPath, "FileCreated");
            }
            else{
                Console.WriteLine($"Unknown item Created by Call: {e.FullPath}");
            }
        }

        private async Task AddEventToQueue(string changeType, string path, string? oldPath = null)
        {

            if(IgnoreFiles.HandlePath(path, "get")){
                Console.WriteLine("Mudança controlada por aplicação detectada \n");
                return; 
            }

            if(IsTemporaryFile(path)){
                Console.WriteLine("Arquivo Temporário Identificado");
                return;
            }

            
            if(FileExpirationManager.CheckFile(path)){
                Console.WriteLine("Arquivo modificado muito recentemente");
                return;
            }

            var fileChange = new FileChange
            {
                ChangeType = changeType,
                FilePath = path,
                OldFilePath = oldPath,
                ChangeTime = DateTime.Now
            };

            if(await ConnectionChecker.CheckConnectionAsync()){
                _queueManager.Enqueue(fileChange);
                _ = _syncManager.ProcessQueueAsync();
            }else{
                await ProjectManager.SetPeddingSincronization();
            }

            FileExpirationManager.AddFile(path);
        }
    }
}
