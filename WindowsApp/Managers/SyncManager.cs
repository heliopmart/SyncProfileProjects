using System;
using System.Threading.Tasks;
using WindowsApp.Models;
using WindowsAppSync.Managers.Uploaders;

namespace WindowsApp.Managers
{
    public class SyncManager
    {
        private readonly QueueManager _queueManager;
        private readonly BatchUploader _batchUploader;

        public SyncManager(QueueManager queueManager, BatchUploader batchUploader)
        {
            _queueManager = queueManager;
            _batchUploader = batchUploader;
        }

        public async Task StartSync()
        {
            Console.WriteLine("Iniciando sincronização...");

            while (!_queueManager.IsEmpty())
            {
                if (_queueManager.TryDequeue(out var fileChange))
                {
                    try
                    {
                        // Processa o arquivo com base no tipo de mudança
                        await ProcessFileChange(fileChange);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar {fileChange.FilePath}: {ex.Message}");
                        // Lógica para reprocessamento ou log de erro
                    }
                }
            }

            Console.WriteLine("Sincronização concluída.");
        }

        private async Task ProcessFileChange(FileChange fileChange)
        {
            switch (fileChange.ChangeType)
            {
                case "Created":
                case "Modified":
                    Console.WriteLine($"Fazendo upload do arquivo: {fileChange.FilePath}");
                    await _batchUploader.UploadFileAsync(fileChange.FilePath);
                    break;

                case "Deleted":
                    Console.WriteLine($"Excluindo arquivo na nuvem: {fileChange.FilePath}");
                    await _batchUploader.DeleteFileAsync(fileChange.FilePath);
                    break;

                case "Renamed":
                    Console.WriteLine($"Renomeando arquivo na nuvem: {fileChange.FilePath}");
                    // Adicione lógica para renomeação, se necessário
                    break;

                default:
                    Console.WriteLine($"Tipo de mudança desconhecido: {fileChange.ChangeType}");
                    break;
            }
        }
    }
}
