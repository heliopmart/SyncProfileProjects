using Box.Sdk.Gen;
using Box.Sdk.Gen.Managers;
using Box.Sdk.Gen.Schemas;
using System.Text.Json;
using WindowsApp.Managers.Uploaders.Folders;
using Microsoft.IdentityModel.Tokens;

namespace WindowsApp.Managers.Uploaders.Files{

    class ManagerFiles{
        public static async Task<bool> DeleteFiles(BoxClient client, string? folderPath, string? fileId, string parentFolderId){
            fileId ??= await GetOrCreateFileByPathAsync(client, folderPath, parentFolderId);
            
            if(!fileId.IsNullOrEmpty()){
                try
                {
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

        public static async Task<string?> GetOrCreateFileByPathAsync(BoxClient client, string filePath, string parentFolderId)
        {
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

        public static async Task<bool> UploadFileAsync(BoxClient client, string filePath, string parentFolderId, string parentFolderPath = "/"){
            string? folderId = await ManagerFolders.GetOrCreateFolderByPathAsync(client, filePath, parentFolderId);

            if (folderId == null){
                throw new InvalidOperationException($"Folder path '{parentFolderPath}' could not be located or created.");
            }

            using (var fileStream = new FileStream(filePath, FileMode.Open)){
               var fileName = Path.GetFileName(filePath);

               var attributes = new UploadFileRequestBodyAttributesField(
                   name: fileName, 
                   parent: new UploadFileRequestBodyAttributesParentField(id: folderId) 
               );

               var requestBody = new UploadFileRequestBody(
                   attributes: attributes,
                   file: fileStream 
               );

                try{
                    Box.Sdk.Gen.Schemas.Files file = await client.Uploads.UploadFileAsync(requestBody);
                    return await BoxUploader.UpdateMetaDataProject();
                }catch(Exception ex){
                    var errorDetails = JsonSerializer.Deserialize<JsonElement>(ex.Message);
                    if (errorDetails.TryGetProperty("code", out var codeProperty)){
                        string errorCode = codeProperty.GetString() ?? string.Empty;
                        if (errorCode == "item_name_in_use"){
                            /*
                                TEMPORÁRIO

                                Alguns arquivos não modificam o arquivo atual, mas sim cria um por cima, 
                                o que acaba chamando o evento de criar e não o de modificar.

                                Uma alternativa é manter os arquivos e pastas dentro de .yaml.
                            */
                            await ChangeCallByCreate(client, fileStream, filePath, parentFolderId);
                            return await BoxUploader.UpdateMetaDataProject();
                        }else{
                            throw new Exception($"ManagerFiles : UploadFileAsync(), Erro: Upload not compleate ({ex})");
                        }
                    }else{
                        throw new Exception($"ManagerFiles : UploadFileAsync(), Erro: Upload not compleate ({ex})");
                    }
                }
            }
        }

        public static async Task<bool> ChangeFileAsync(BoxClient client, string filePath, string parentFolderId){
            string fileId = await GetOrCreateFileByPathAsync(client, filePath, parentFolderId) ?? throw new InvalidOperationException("FileiD is null");
            try{
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Fazer upload da nova versão do arquivo
                    await client.Uploads.UploadFileVersionAsync(
                        fileId: fileId,
                        requestBody: new UploadFileVersionRequestBody(
                            attributes: new UploadFileVersionRequestBodyAttributesField(
                                name: Path.GetFileName(filePath)
                            ),
                            file: fileStream
                        )
                    );
                }

                return await BoxUploader.UpdateMetaDataProject();
            }catch(Exception ex){
                throw new InvalidOperationException($"ManagerFiles: ChangeFileAsync(), Error: Upload file new version ({ex})");
            }
        }
    
        public static async Task<bool> RenameFile(BoxClient client, string filePath, string oldFilePath, string parentFolderId){
            if(oldFilePath == null){
                throw new InvalidOperationException($"ManagerFiles : RenameFile(), Erro: OldFilePath is null");
            }

            string fileId = await GetOrCreateFileByPathAsync(client, oldFilePath, parentFolderId) ?? throw new InvalidOperationException("FileiD is null");

            try{
                string newFileName = Path.GetFileName(filePath);

                var updateRequest = new UpdateFileByIdRequestBody
                {
                    Name = newFileName
                };

                await client.Files.UpdateFileByIdAsync(fileId, updateRequest);
                return await BoxUploader.UpdateMetaDataProject();
            }
            catch (Exception ex){
                throw new InvalidOperationException($"ManagerFiles : RenameFile(), Erro: {ex}");
            }
        }

        // TEMPORÁRIO => Chamado pelo Exception dentro de UploadFileAsync()
        private static async Task<bool> ChangeCallByCreate(BoxClient client, FileStream fileStream, string filePath, string parentFolderId){
            string fileId = await GetOrCreateFileByPathAsync(client, filePath, parentFolderId) ?? throw new InvalidOperationException("FileiD is null");
            try{
                await client.Uploads.UploadFileVersionAsync(
                fileId: fileId,
                requestBody: new UploadFileVersionRequestBody(
                    attributes: new UploadFileVersionRequestBodyAttributesField(
                        name: Path.GetFileName(filePath)
                    ),
                    file: fileStream
                )
                );

                return true;
            }catch(Exception ex){
                throw new Exception($"ManagerFiles : ChangeCallByCreate(), error: {ex}");
            }
        }
    
    }
}