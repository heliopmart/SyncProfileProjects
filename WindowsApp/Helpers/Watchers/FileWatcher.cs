using System;
using System.IO;

namespace WindowsApp.Helpers.Watchers
{
    public class DirectoryMonitor
    {
        private readonly FileSystemWatcher _fileSystemWatcher;

        public DirectoryMonitor(string path)
        {
            // Configura o FileSystemWatcher para monitorar o diretório
            _fileSystemWatcher = new FileSystemWatcher
            {
                Path = path, // Diretório a ser monitorado
                IncludeSubdirectories = true, // Monitora subpastas
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime // Eventos a serem monitorados
            };

            // Associa os eventos ao manipulador
            _fileSystemWatcher.Created += OnFileChanged;
            _fileSystemWatcher.Changed += OnFileChanged;
            _fileSystemWatcher.Deleted += OnFileDeleted;
            _fileSystemWatcher.Renamed += OnFileRenamed;

            // Inicia o monitoramento
            _fileSystemWatcher.EnableRaisingEvents = true;

            Console.WriteLine($"Monitorando mudanças em: {path}");
        }

        public void StopMonitoring()
        {
            // Desativa o monitoramento
            _fileSystemWatcher.EnableRaisingEvents = false;

            // Remove os manipuladores de eventos
            _fileSystemWatcher.Created -= OnFileChanged;
            _fileSystemWatcher.Changed -= OnFileChanged;
            _fileSystemWatcher.Deleted -= OnFileDeleted;
            _fileSystemWatcher.Renamed -= OnFileRenamed;

            // Libera os recursos usados pelo FileSystemWatcher
            _fileSystemWatcher.Dispose();
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // Evento acionado quando um arquivo é criado ou modificado
            Console.WriteLine($"Arquivo {e.ChangeType}: {e.FullPath}");
            HandleFileChange(e.FullPath, e.ChangeType);
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            // Evento acionado quando um arquivo é excluído
            Console.WriteLine($"Arquivo deletado: {e.FullPath}");
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            // Evento acionado quando um arquivo é renomeado
            Console.WriteLine($"Arquivo renomeado de {e.OldFullPath} para {e.FullPath}");
        }

        private void HandleFileChange(string filePath, WatcherChangeTypes changeType)
        {
            // Aqui você pode implementar a lógica para lidar com as mudanças no arquivo
            Console.WriteLine($"Arquivo alterado: {filePath}, Tipo de mudança: {changeType}");
        }
    }
}
