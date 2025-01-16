using Box.Sdk.Gen;
using Box.Sdk.Gen.Managers;
using Box.Sdk.Gen.Schemas;
using System.Text.Json;
using WindowsAppSync.Managers.Uploaders;
using WindowsApp.Managers.Uploaders.Files;

namespace WindowsApp.Managers.Uploaders.Folders{
    public class ManagerFolders{
        public static async Task<bool> DeleteFolderRecursivelyAsync(BoxClient client, string folderId){
            var parentFolderIdObject = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
            string parentFolderId = parentFolderIdObject.ToString() ?? throw new InvalidOperationException("FolderId not found in cache.");

            try{
                // Obtenha os itens da pasta
                var folderItems = await client.Folders.GetFolderItemsAsync(folderId);
                if (folderItems?.Entries != null)
                {
                    foreach (var item in folderItems.Entries)
                    {
                        // Serializa o item como JSON
                        string jsonItem = JsonSerializer.Serialize(item);
                        using var jsonDoc = JsonDocument.Parse(jsonItem);
                        var root = jsonDoc.RootElement;
                        // Verifique se o tipo do item é "file"
                        if (root.TryGetProperty("type", out var typeProperty) && typeProperty.GetString() == "file")
                        {
                            var id = root.GetProperty("id").GetString();
                            await ManagerFiles.DeleteFiles(client, null, id, parentFolderId);
                        }
                        // Verifique se o tipo do item é "folder"
                        else if (root.TryGetProperty("type", out typeProperty) && typeProperty.GetString() == "folder")
                        {
                            var id = root.GetProperty("id").GetString();
                            var name = root.GetProperty("name").GetString();
                            await DeleteFolderRecursivelyAsync(client, id); // Chamada recursiva para subpastas
                        }
                    }
                }
                await Delete(client, folderId);
                return true;
            }
            catch (Exception ex){
                Console.WriteLine($"Error deleting folder with ID {folderId}: {ex.Message}");
                return false;
            }
            static async Task<bool> Delete(BoxClient client, string folderId){
                try
                {
                    // Exclua a pasta e todo o seu conteúdo
                    await client.Folders.DeleteFolderByIdAsync(folderId);
                    Console.WriteLine("Exclued");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting folder with ID {folderId}: {ex.Message}");
                    return false;
                }
            }
        }
    
        public static async Task<bool> DeleteFolders(BoxClient client, string folderPath, string? folderId){
            var parentFolderIdObject = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
            string? parentFolderId = parentFolderIdObject.ToString();

            if(folderId == null){
                folderId = await GetOrCreateFolderByPathAsync(client, folderPath, parentFolderId);
            }

            return await DeleteFolderRecursivelyAsync(client, folderId);
        }

        public static async Task<string?> GetOrCreateFolderByPathAsync(BoxClient client, string folderPath, string parentFolderId){
            string relativePath = BoxUploader.GetRelativePathFromRoot(folderPath);

            if(relativePath == null || relativePath == ""){

                return parentFolderId;
            }

            string[] pathSegments = relativePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in pathSegments)
            {
                // Obtenha os itens na pasta atual
                IReadOnlyList<FileFullOrFolderMiniOrWebLink>? Entries = (await client.Folders.GetFolderItemsAsync(parentFolderId)).Entries;

                string? subFolderId = null;
                if (Entries != null)
                {
                    foreach (var item in Entries)
                    {
                        // Serializa o item como JSON
                        string jsonItem = JsonSerializer.Serialize(item);

                        // Converte o JSON para um JsonDocument
                        using var jsonDoc = JsonDocument.Parse(jsonItem);
                        var root = jsonDoc.RootElement;

                        // Verifique se o tipo do item é "folder"
                        if (root.TryGetProperty("type", out var typeProperty) && typeProperty.GetString() == "folder")
                        {
                            var id = root.GetProperty("id").GetString();
                            var name = root.GetProperty("name").GetString();
                            if(name == segment){
                                subFolderId = id;
                            }
                        }
                    }
                }
                
                if (subFolderId != null)
                {
                    // Pasta já existe
                    parentFolderId = subFolderId;
                }
                else
                {
                    // Cria a nova pasta se não existir
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
    
        public static async Task<string?> GetFolderIdForSubPasta(BoxClient client, string folderPath, string parentFolderId){            
            if(parentFolderId == "0"){
                return "0";
            }

            string relativePath = BoxUploader.GetRelativePathFromRoot(folderPath);

            if(relativePath == null || relativePath == ""){
                return parentFolderId;
            }
            
            string[] pathSegments = relativePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            // Verifique o comprimento e retorne o elemento apropriado
            string? result = pathSegments.Length == 1 
                ? pathSegments[0] // Retorna o único elemento
                : pathSegments.Length > 1 
                    ? pathSegments[^2] // Retorna o penúltimo elemento
                    : null; // Caso vazio (nunca acontece com StringSplitOptions.RemoveEmptyEntries)

            // Obtenha os itens na pasta atual
            IReadOnlyList<FileFullOrFolderMiniOrWebLink>? Entries = (await client.Folders.GetFolderItemsAsync(parentFolderId)).Entries;


            string? subFolderId = parentFolderId;
            if (Entries != null)
            {
                foreach (var item in Entries)
                {
                    // Serializa o item como JSON
                    string jsonItem = JsonSerializer.Serialize(item);
                    // Converte o JSON para um JsonDocument
                    using var jsonDoc = JsonDocument.Parse(jsonItem);
                    var root = jsonDoc.RootElement;
                    // Verifique se o tipo do item é "folder"
                    if (root.TryGetProperty("type", out var typeProperty) && typeProperty.GetString() == "folder")
                    {
                        var id = root.GetProperty("id").GetString();
                        var name = root.GetProperty("name").GetString();
                        if(name == result){
                            subFolderId = id;
                        }
                    }
                }
            }else{
                throw new InvalidOperationException("ManagerFolders: GetFolderIdForSubPasta(), Error: Entries is Null");
            }


            // TODO: Mudança de else dentro de (name == result) para: 
            subFolderId ??= parentFolderId;

            return subFolderId;
        }

        public static async Task<string> UploadFolder(BoxClient auth, string folderPath, string? parentFolderId){
            if(parentFolderId == null){
                var folderIdObj = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
                parentFolderId = folderIdObj.ToString();
            }else{
                parentFolderId = "0"; // para criar um novo projeto
            }

            string? FolderId = await GetFolderIdForSubPasta(auth, folderPath, parentFolderId);

            if(FolderId == null){
                return "false";
            }

            await BoxUploader.UpdateMetaDataProject();
            return await FolderSync.SyncFolderToCloud(auth, folderPath, FolderId ?? throw new ArgumentNullException(nameof(parentFolderId)));
        }

        public static async Task<bool> RenameFolder(BoxClient client, string filePath, string oldFilePath){
            if(oldFilePath == null){
                throw new InvalidOperationException($"ManagerFiles : RenameFile(), Erro: OldFilePath is null");
            }

            var folderIdObj = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
            string parentFolderId = folderIdObj?.ToString() ?? throw new InvalidOperationException("FolderId not found in cache.");

            string FolderId = await GetOrCreateFolderByPathAsync(client, oldFilePath, parentFolderId) ?? throw new InvalidOperationException("FileiD is null");

            try{
                 // Obter o novo nome do arquivo
                string newFolderName = Path.GetFileName(filePath);

                // Atualizar o nome do arquivo na nuvem
                var updateRequest = new UpdateFolderByIdRequestBody
                {
                    Name = newFolderName
                };

                await client.Folders.UpdateFolderByIdAsync(FolderId, updateRequest);
                return true;
            }
            catch (Exception Err){
                throw new InvalidOperationException($"ManagerFolders : RenameFolder(), Erro: {Err}");
            }
        }
    }
}