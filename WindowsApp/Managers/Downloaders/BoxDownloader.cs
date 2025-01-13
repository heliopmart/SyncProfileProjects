using Box.Sdk.Gen;
using WindowsAppSync.Services.API;
using WindowsApp.Utils;

namespace WindowsApp.Managers.Downloaders
{
    public static class BoxDownloader
    {

        public static async Task DownloadFileAsync(string fileId, string? filePath)
        {
            BoxClient client = Authenticator.Auth();

            if(filePath == null){
                throw new Exception("BoxDownloader : DownloadFileAsync(), Error: filePath directory is null");
            }
            
            var nameProjectObj = CentralCache.Instance.GetFromCache("ProjectPath");
            string projectPath = nameProjectObj?.ToString() ?? string.Empty;

            string destinationPath = $"{projectPath}/{filePath}";
            IgnoreFiles.HandlePath(destinationPath, "add");

            try
            {
                // Baixar o conteúdo do arquivo da cloud
                using (var stream = await client.Downloads.DownloadFileAsync(fileId))
                {
                    // Garantir que o diretório de destino existe
                    string directory = Path.GetDirectoryName(destinationPath) ?? throw new Exception("BoxDownloader : DownloadFileAsync(), Error: destinationPath directory is null");
                    
                    Console.WriteLine($"LOG : BoxDownloader : DownloadFileAsync(), Directory => {directory}");

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Salvar o arquivo localmente
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                    {
                        if (stream != null)
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                        else
                        {
                            throw new Exception("BoxDownloader : DownloadFileAsync(), Error: Download is null");
                        }
                    }

                    Console.WriteLine($"Arquivo baixado: {destinationPath}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"BoxDownloader : DownloadFileAsync(), Error: Download fail (ID: {fileId}) : {ex.Message}");
            }
        }

        public static void DownloadFolderAsync(string filePath)
        {
            if(filePath == null){
                throw new Exception("BoxDownloader : DownloadFileAsync(), Error: filePath directory is null");
            }
            
            var nameProjectObj = CentralCache.Instance.GetFromCache("ProjectPath");
            string projectPath = nameProjectObj?.ToString() ?? string.Empty;

            string destinationPath = $"{projectPath}/{filePath}";
            IgnoreFiles.HandlePath(destinationPath, "add");

            if(destinationPath == null){
                throw new Exception("BoxDownloader : DownloadFileAsync(), Error: destinationPath directory is null");
            }

            try
            {
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"BoxDownloader : DownloadFileAsync(), Error: Download fail (Path: {filePath}) : {ex.Message}");
            }
        }
    }
}