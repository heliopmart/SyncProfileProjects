using Box.Sdk.Gen;
using Box.Sdk.Gen.Managers;
using Box.Sdk.Gen.Schemas;
using System.Text.Json;
using WindowsAppSync.Services.API;
using WindowsApp.Managers.Uploaders.Folders;
using Microsoft.IdentityModel.Tokens;

#pragma warning disable IDE0130 // O namespace não corresponde à estrutura da pasta
namespace WindowsApp.Managers.Uploaders.Files{
#pragma warning restore IDE0130 // O namespace não corresponde à estrutura da pasta

    class ManagerFiles{
        private static readonly Authenticator authenticator = new Authenticator();

        public static async Task<bool> DeleteFiles(string folderPath, string? fileId){
            var parentFolderIdObject = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
            string? parentFolderId = parentFolderIdObject.ToString() ?? throw new InvalidOperationException("FolderId not found in cache.");
            
            fileId ??= await GetOrCreateFileByPathAsync(folderPath, parentFolderId);
            
            if(!fileId.IsNullOrEmpty()){
                try
                {
                    BoxClient client = await authenticator.Auth();

                    // Exclua a pasta e todo o seu conteúdo
                    await client.Files.DeleteFileByIdAsync(fileId);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file with ID {fileId}: {ex.Message}");
                    return false;
                }
            }else{
                throw new InvalidOperationException("ManagersFiles : DeleteFiles(), error: FileId is null or empty");
            }
        }

        public static async Task<string?> GetOrCreateFileByPathAsync(string filePath, string parentFolderId)
        {
            BoxClient client = await authenticator.Auth();

            // Obtenha o caminho relativo do arquivo
            string relativePath = BoxUploader.GetRelativePathFromRoot(filePath);
            // Divida o caminho em segmentos
            string[] filePathSegments = filePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);            
            // Extraia o nome do arquivo (último segmento)
            string fileName = filePathSegments[^1];

            string folderIdHasFile = parentFolderId;
            if (!string.IsNullOrEmpty(relativePath)){
                folderIdHasFile = await subfolderRoot(relativePath, client, parentFolderId);
            }

            // Verifique se o arquivo já existe na pasta
            IReadOnlyList<FileFullOrFolderMiniOrWebLink>? fileEntries = (await client.Folders.GetFolderItemsAsync(folderIdHasFile)).Entries;
                
            string? fileId = null;
            if (fileEntries != null)
            {
                foreach (var item in fileEntries)
                {
                    string jsonItem = JsonSerializer.Serialize(item);
                    using var jsonDoc = JsonDocument.Parse(jsonItem);
                    var root = jsonDoc.RootElement;

                    // Verifique se o tipo do item é "file"
                    if (root.TryGetProperty("type", out var typeProperty) && typeProperty.GetString() == "file")
                    {
                        var id = root.GetProperty("id").GetString();
                        var name = root.GetProperty("name").GetString();
                        if (name == fileName)
                        {
                            fileId = id;
                        }
                    }
                }
            }

            if (fileId != null)
            {
                // Arquivo já existe
                return fileId;
            }
            else
            {
                throw new InvalidOperationException("ManagerFiles: GetOrCreateFileByPathAsync(), Error: Arquivo não encontrado!");
                // return null;
            }
        
            static async Task<string> subfolderRoot(string filePath, BoxClient client, string parentFolderId){
                string[] pathSegments = filePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                // Navegue pelas pastas e obtenha o folderId do destino
                foreach (var segment in pathSegments)
                {
                    IReadOnlyList<FileFullOrFolderMiniOrWebLink>? entries = (await client.Folders.GetFolderItemsAsync(parentFolderId)).Entries;
                    
                    string? subFolderId = null;
                    if (entries != null)
                    {
                        foreach (var item in entries)
                        {
                            string jsonItem = JsonSerializer.Serialize(item);
                            using var jsonDoc = JsonDocument.Parse(jsonItem);
                            var root = jsonDoc.RootElement;

                            // Verifique se o tipo do item é "folder"
                            if (root.TryGetProperty("type", out var typeProperty) && typeProperty.GetString() == "folder")
                            {
                                var id = root.GetProperty("id").GetString();
                                var name = root.GetProperty("name").GetString();
                                if (name == segment)
                                {
                                    subFolderId = id;
                                }
                            }
                        }
                    }

                    if (subFolderId != null){
                        parentFolderId = subFolderId;
                    }
                    else
                    {
                        var createFolderRequest = new CreateFolderRequestBody(
                            name: segment,
                            parent: new CreateFolderRequestBodyParentField(parentFolderId)
                        );

                        var createdFolder = await client.Folders.CreateFolderAsync(createFolderRequest);
                        parentFolderId = createdFolder.Id;
                    }
                }

                return parentFolderId;
            }

            
        }

        public static async Task<bool> UploadFileAsync(string filePath, string parentFolderPath = "/"){
            var parentFolderIdObject = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
            string parentFolderId = parentFolderIdObject.ToString() ?? throw new InvalidOperationException("FolderId not found in cache.");

            // Obtenha o cliente autenticado
            BoxClient client = await authenticator.Auth();

            // Verifique o caminho da subpasta e crie, se necessário
            string? folderId = await ManagerFolders.GetOrCreateFolderByPathAsync(filePath, parentFolderId);

            if (folderId == null){
                throw new InvalidOperationException($"Folder path '{parentFolderPath}' could not be located or created.");
            }

            using (var fileStream = new FileStream(filePath, FileMode.Open)){
               var fileName = Path.GetFileName(filePath);

               // Construa os atributos do arquivo para o upload
               var attributes = new UploadFileRequestBodyAttributesField(
                   name: fileName, // Nome do arquivo
                   parent: new UploadFileRequestBodyAttributesParentField(id: folderId) // ID da pasta
               );

               // Construa o corpo da requisição para o upload
               var requestBody = new UploadFileRequestBody(
                   attributes: attributes, // Atributos do arquivo
                   file: fileStream         // Arquivo em forma de Stream
               );

                try{
                    Box.Sdk.Gen.Schemas.Files file = await client.Uploads.UploadFileAsync(requestBody);
                    return await BoxUploader.UpdateMetaDataProject();
                }catch{
                    return false;
                }
            }
        }

        public static async Task<bool> ChangeFileAsync(string filePath){
            BoxClient client = await authenticator.Auth();
            var folderIdObj = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
            string parentFolderId = folderIdObj?.ToString() ?? throw new InvalidOperationException("FolderId not found in cache.");

            string fileId = await GetOrCreateFileByPathAsync(filePath, parentFolderId) ?? throw new InvalidOperationException("FileiD is null");
            try{
                
                // Fazer upload da nova versão do arquivo
                await client.Uploads.UploadFileVersionAsync(
                    fileId: fileId,
                    requestBody: new UploadFileVersionRequestBody(
                        attributes: new UploadFileVersionRequestBodyAttributesField(name: Path.GetFileName(filePath)),
                        file: new FileStream(filePath, FileMode.Open)
                    )
                );
                return true;
            }catch{
                throw new InvalidOperationException("ManagerFiles: ChangeFileAsync(), Error: Upload file new version");
            }

        }
    
        public static async Task<bool> RenameFile(string filePath, string oldFilePath){
            if(oldFilePath == null){
                throw new InvalidOperationException($"ManagerFiles : RenameFile(), Erro: OldFilePath is null");
            }

            BoxClient client = await authenticator.Auth();
            var folderIdObj = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
            string parentFolderId = folderIdObj?.ToString() ?? throw new InvalidOperationException("FolderId not found in cache.");

            string fileId = await GetOrCreateFileByPathAsync(oldFilePath, parentFolderId) ?? throw new InvalidOperationException("FileiD is null");

            try{
                 // Obter o novo nome do arquivo
                string newFileName = Path.GetFileName(filePath);

                // Atualizar o nome do arquivo na nuvem
                var updateRequest = new UpdateFileByIdRequestBody
                {
                    Name = newFileName
                };

                await client.Files.UpdateFileByIdAsync(fileId, updateRequest);
                return true;
            }
            catch (System.Exception Err){
                throw new InvalidOperationException($"ManagerFiles : RenameFile(), Erro: {Err}");
            }
        }
    
    }
}