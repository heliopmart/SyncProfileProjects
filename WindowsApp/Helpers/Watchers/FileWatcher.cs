using WindowsApp.Managers;
using WindowsApp.Models;
using WindowsApp.Utils;

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
            var watcher = new FileSystemWatcher(_pathToWatch)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
                IncludeSubdirectories = true
            };

            watcher.Created += OnCreated;
            watcher.Changed += OnChanged;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            watcher.EnableRaisingEvents = true;

            Console.WriteLine("FileWatcher started...");
        }

        private void OnCreated(object sender, FileSystemEventArgs e){        
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
                            CallCreatedHandle(e);
                        }
                    }
                });
            }
        }
        private void OnChanged(object sender, FileSystemEventArgs e){
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
                Console.WriteLine($"Folder changed: {e.FullPath}");
                // HandleFolder(e.FullPath, "FolderChanged");
            }
            else if (File.Exists(e.FullPath))
            {
                Console.WriteLine($"File changed: {e.FullPath}");
                HandleFile(e.FullPath, "FileChanged");
            }
            else
            {
                Console.WriteLine($"Unknown item changed: {e.FullPath}");
            }
        }
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (!Path.HasExtension(e.FullPath))
            {
                Console.WriteLine($"Folder deleted: {e.FullPath}");
                HandleFolder(e.FullPath, "FolderDeleted");
            }
            else
            {
                Console.WriteLine($"File deleted: {e.FullPath}");
                HandleFile(e.FullPath, "FileDeleted");
            }
        }
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            lock (PendingCreations)
            {
                // Verifique se o item renomeado estava pendente de criação
                if (PendingCreations.ContainsKey(e.OldFullPath))
                {
                    if(!PendingCreations.Remove(e.OldFullPath)){
                        throw new InvalidOperationException("FileWatcher : OnRenamed(), Error: OldFullPath didn't delete in PendingCreations.");
                    }

                    // Atualize o evento de criação para o novo nome
                    // TODO: Removido -> PendingCreations[e.FullPath] = DateTime.Now;        

                    Console.WriteLine($"Folder renamed during creation: {e.OldFullPath} -> {e.FullPath}");
                    CallCreatedHandle(e); // Chama a função que cria o argumento
                    return; // Não processe como um renome normal
                }
            }       

            // Caso contrário, trate como um evento de renomeação normal
            if (Directory.Exists(e.FullPath))
            {
                Console.WriteLine($"Folder renamed from {e.OldFullPath} to {e.FullPath}");
                HandleFolderRenamed(e.OldFullPath, e.FullPath);
            }
            else if (File.Exists(e.FullPath))
            {
                Console.WriteLine($"File renamed from {e.OldFullPath} to {e.FullPath}");
                HandleFileRenamed(e.OldFullPath, e.FullPath);
            }
            else
            {
                Console.WriteLine($"Unknown item renamed: {e.OldFullPath} to {e.FullPath}");
            }
        }
       
        private void HandleFolder(string folderPath, string type){
            AddEventToQueue(type, folderPath);
        }

        private void HandleFile(string filePath, string type){
            AddEventToQueue(type, filePath);
        }

        private void HandleFolderRenamed(string oldFolderPath, string newFolderPath){
            AddEventToQueue("FolderRenamed", newFolderPath, oldFolderPath);
        }

        private void HandleFileRenamed(string oldFilePath, string newFilePath){
            AddEventToQueue("FileRenamed", newFilePath, oldFilePath);
        }       

        private void CallCreatedHandle(FileSystemEventArgs e){
            if (Directory.Exists(e.FullPath)){
                Console.WriteLine($"Folder created by Call: {e.FullPath}");
                HandleFolder(e.FullPath, "FolderCreated");
            }
            else if (File.Exists(e.FullPath)){
                Console.WriteLine($"File created by Call: {e.FullPath}");
                HandleFile(e.FullPath, "FileCreated");
            }
            else{
                Console.WriteLine($"Unknown item Created by Call: {e.FullPath}");
            }
        }

        private void AddEventToQueue(string changeType, string path, string? oldPath = null)
        {
            var fileChange = new FileChange
            {
                ChangeType = changeType,
                FilePath = path,
                OldFilePath = oldPath,
                ChangeTime = DateTime.Now
            };

            _queueManager.Enqueue(fileChange);
            // Processa a fila automaticamente
            _syncManager.ProcessQueueAsync().Wait();
        }
    }
}
