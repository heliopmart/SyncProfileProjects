using Box.Sdk.Gen;
using WindowsApp.Models;
using WindowsApp.Managers.Cloud;
using WindowsApp.Helpers.Watchers;
using WindowsApp.Managers.Uploaders;
using WindowsApp.Managers.Downloaders;
using System.Text.Json;

namespace WindowsApp.Managers
{
    public static class SyncProcessor
    {
        private static Timer? _syncTimer;
        private static bool _isRunning = false;

        public static void StartSync(BoxClient client, string localRootPath, string cloudRootFolderId, int intervalInSeconds = 300)
        {
            // Configure o Timer para sincronização periódica
            _syncTimer = new Timer(async _ =>
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                    await Sync(client, localRootPath, cloudRootFolderId);
                    _isRunning = false;
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalInSeconds));
        }

        public static async Task Sync(BoxClient client, string localRootPath, string cloudRootFolderId)
        {
            try
            {
                Console.WriteLine("Starting sync by time...");

                // Mapear arquivos locais
                var localFiles = LocalFileMapper.MapLocalFiles(localRootPath);

                // Mapear arquivos na cloud
                var cloudFiles = await CloudFileMapper.MapCloudFilesAsync(client, cloudRootFolderId);

                // Comparar arquivos locais e cloud
                await CompareAndSync(client, localFiles, cloudFiles, localRootPath);

            }
            catch (Exception ex)
            {
                throw new Exception($"SyncManager : Sync(), error: {ex.Message}");
            }
        }

        // TODO Client here
        private static async Task CompareAndSync(BoxClient client, List<FileItem> localFiles, List<CloudFileItem> cloudFiles, string localRootPath)
        {
            // Sincronizar arquivos locais que não existem na cloud
            foreach (var localFile in localFiles)
            {   
                var correspondingCloudFile = cloudFiles.FirstOrDefault(c => NormalizePath(c.Path, localRootPath) == NormalizePath(localFile.FullPath, localRootPath));

                if (correspondingCloudFile == null)
                {
                    // Upload de novos arquivos ou pastas
                    if (localFile.IsFolder)
                    {
                        Console.WriteLine($"Criando pasta na cloud: {localFile.Path}");
                        await new BoxUploader().UploadManager(client, localFile.FullPath, "FolderCreated", null);
                    }
                    else
                    {
                        Console.WriteLine($"Fazendo upload do arquivo: {localFile.Path}");
                        await new BoxUploader().UploadManager(client, localFile.FullPath, "FileCreated", null);
                    }
                }
                else if (localFile.LastModified > correspondingCloudFile.LastModified)
                {
                    // Atualizar arquivos modificados
                    Console.WriteLine($"Atualizando arquivo na cloud: {localFile.Path}");
                    // TODO => Fazer ainda 
                    await new BoxUploader().UploadManager(client, localFile.FullPath, "FileOffChange", null);
                }
            }

            // Sincronizar arquivos na cloud que não existem localmente (opcional: deletar ou baixar)
            foreach (var cloudFile in cloudFiles)
            {
                var correspondingLocalFile = localFiles.FirstOrDefault(l => NormalizePath(l.FullPath, localRootPath) == NormalizePath(cloudFile.Path, localRootPath));

                if (correspondingLocalFile == null)
                {

                    if(cloudFile.IsFolder){
                        BoxDownloader.DownloadFolderAsync(cloudFile.Path);
                    }else{
                        await BoxDownloader.DownloadFileAsync(client, cloudFile.Id, cloudFile.Path);
                    }

                    Console.WriteLine($"Arquivo/pasta na cloud baixado localmente: {cloudFile.Path}");
                    
                    /*
                        TODO: 

                        Download de arquivos e diretório: OK
                        Delete files local: --------------------------------------- Deve ser feito?
                        Update files local and cloud: ----------------------------- Deve ser feito
                        Rename files local and cloud: ----------------------------- Deve ser feito

                        preciso de uma função que guarde as modificações locais, e que quando feito o upload 
                        ela use esses dados em vez dados dinâmicos. 
                    */
                }
            }
        }

        public static void StopSync()
        {
            _syncTimer?.Dispose();
            Console.WriteLine("SyncManager stopped.");
        }

        private static string NormalizePath(string path, string localRootPath)
        {
            // Substitui todos os separadores por '/' para garantir consistência
            string normalizedPath = path.Replace(localRootPath, "").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');

            // Remove separadores extras no início ou fim do caminho
            return normalizedPath.Trim('/');
        }
    }
}
