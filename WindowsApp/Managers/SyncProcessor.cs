using Box.Sdk.Gen;
using WindowsApp.Models;
using WindowsApp.Managers.Cloud;
using WindowsApp.Helpers.Watchers;
using WindowsApp.Managers.Uploaders;
using WindowsApp.Managers.Downloaders;
using WindowsApp.Services;

namespace WindowsApp.Managers
{
    public static class SyncProcessor
    {
        private static Timer? _syncTimer;
        private static bool _isRunning = false;
        private static readonly SemaphoreSlim SyncLock = new(1, 1);

        public static void StartSync(BoxClient client, string localRootPath, string cloudRootFolderId, int intervalInSeconds = 300)
        {
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
            if(!await ConnectionChecker.CheckConnectionAsync()){
                return;
            }

            await SyncLock.WaitAsync();
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
            finally{
                SyncLock.Release();
            }
        }

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
                else if (IsLocalFileOutdated(localFile.LastModified, correspondingCloudFile.LastModified, "Upload"))
                {
                    // Atualizar arquivos modificados
                    Console.WriteLine($"Atualizando arquivo na cloud: {localFile.Path}");

                    await new BoxUploader().UploadManager(client, localFile.FullPath, "FileChanged", null);
                }                 
            }

            // Sincronizar arquivos na cloud que não existem localmente (opcional: baixar)
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
                    
                }else if(IsLocalFileOutdated(correspondingLocalFile.LastModified, cloudFile.LastModified, "Download")){
                    if(!cloudFile.IsFolder && correspondingLocalFile.Sha1 != cloudFile.Sha1){
                        Console.WriteLine($"Atualizando Arquivo/pasta localmente: {cloudFile.Path}");
                        await BoxDownloader.DownloadFileAsync(client, cloudFile.Id, cloudFile.Path);
                    }else if(cloudFile.IsFolder){
                        Console.WriteLine("LOG: Pasta Modificada"); // TODO: Talvez renomeada.
                    }
                    /*
                        TODO: 
                        Download de arquivos e diretório: OK
                        Update files local and cloud: ----------------------------- OK
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
            string normalizedPath = path.Replace(localRootPath, "").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');
            return normalizedPath.Trim('/');
        }

        private static bool IsLocalFileOutdated(DateTime? localLastModified, DateTime? cloudLastModified, string Type)
        {
            if (localLastModified == null || cloudLastModified == null)
            {
                return false;
            }

            // Remove segundos e milissegundos para a comparação
            var localTimeTruncated = localLastModified.Value
                .AddSeconds(-localLastModified.Value.Second)
                .AddMilliseconds(-localLastModified.Value.Millisecond);

            var cloudTimeTruncated = cloudLastModified.Value.ToUniversalTime()
                .AddSeconds(-cloudLastModified.Value.ToUniversalTime().Second)
                .AddMilliseconds(-cloudLastModified.Value.ToUniversalTime().Millisecond);

            return Type switch
            {
                "Download" => localTimeTruncated < cloudTimeTruncated,
                "Upload" => localTimeTruncated > cloudTimeTruncated,
                _ => false,
            };
        }
    }
}
