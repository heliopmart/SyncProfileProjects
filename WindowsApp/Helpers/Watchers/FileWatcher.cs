using System;
using System.IO;
using WindowsApp.Managers;
using WindowsApp.Models;

namespace WindowsApp.Helpers.Watchers{
    public class FileWatcher{
        private readonly string _pathToWatch;
        private readonly QueueManager _queueManager;
        private readonly SyncManager _syncManager;

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

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            // Verifique se é uma pasta
            if (Directory.Exists(e.FullPath))
            {
                Console.WriteLine($"Folder created: {e.FullPath}");
                HandleFolder(e.FullPath, "FolderCreated");
            }
            // Caso contrário, é um arquivo
            else if (File.Exists(e.FullPath))
            {
                Console.WriteLine($"File created: {e.FullPath}");
                HandleFile(e.FullPath, "FileCreated");
            }
            else
            {
                Console.WriteLine($"Unknown item created: {e.FullPath}");
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                Console.WriteLine($"Folder changed: {e.FullPath}");
                HandleFolder(e.FullPath, "FolderChanged");
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
            if (Directory.Exists(e.FullPath))
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
            AddEventToQueue("FolderCreated", folderPath);
        }

        private void HandleFile(string filePath, string type){
            AddEventToQueue("FileCreated", filePath);
        }

        private void HandleFolderRenamed(string oldFolderPath, string newFolderPath){
            AddEventToQueue("FolderRenamed", newFolderPath, oldFolderPath);
        }

        private void HandleFileRenamed(string oldFilePath, string newFilePath){
            AddEventToQueue("FileRenamed", newFilePath, oldFilePath);
        }       

        private void AddEventToQueue(string changeType, string path, string oldPath = null)
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
