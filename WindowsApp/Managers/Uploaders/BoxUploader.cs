using Box.Sdk.Gen;
using Box.Sdk.Gen.Managers;
using Box.Sdk.Gen.Schemas;
using System.Text.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using WindowsApp.Utils;
using WindowsAppSync.Services.API;
using WindowsAppSync.Managers.Uploaders;


namespace WindowsApp.Managers.Uploaders{
    public class BoxUploader
    {
        private static readonly Authenticator authenticator = new Authenticator();

        public async Task<bool> UploadManager(string filePath, string type, string? OldFilePath){
            
            var nameProjectObj = CentralCache.Instance.GetFromCache("NameProject");
            string? NameProject = nameProjectObj != null ? nameProjectObj.ToString() : string.Empty;

            switch(type){
                case "FileCreated":
                    return await UploadFileAsync(filePath);
                case "FolderCreated": 
                    // return await UploadFolder(filePath, null) != null;
                case "FileChanged":
                    break;
                case "FolderChanged":
                    break;
                case "FileDeleted":
                    break;
                case "FolderDeleted":
                    break;
                case "FileRenamed":
                    break;
                case "FolderRenamed":
                    break;
                case "mainFolder": 
                    if(NameProject != null){
                        return await MainFolderProject(NameProject);
                    }else{
                        return false;
                    }
                default:
                    return false;
            }


            async Task<bool> MainFolderProject(string NameProject){
                var folderId = await UploadFolder(filePath, "0");
                return await new ManagerProject().ChangeProjectData(NameProject, "FolderId", folderId);
            }

            return false;
        }

        static async Task<bool> UploadFileAsync_1(string filePath, string? folderId){

            if(folderId == null){
                var folderIdObj = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
                folderId = folderIdObj.ToString();
            }

            // Obtenha o cliente autenticado
            BoxClient client = await authenticator.Auth();

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var fileName = Path.GetFileName(filePath);

                // Construa os atributos do arquivo para o upload
                var attributes = new UploadFileRequestBodyAttributesField(
                    name: fileName, // Nome do arquivo
                    parent: new UploadFileRequestBodyAttributesParentField(id: folderId ?? throw new ArgumentNullException(nameof(folderId))) // ID da pasta
                );

                // Construa o corpo da requisição para o upload
                var requestBody = new UploadFileRequestBody(
                    attributes: attributes, // Atributos do arquivo
                    file: fileStream         // Arquivo em forma de Stream
                );

                // Faça o upload do arquivo
                Files file = await client.Uploads.UploadFileAsync(requestBody);

                var resultJson = JsonSerializer.Serialize(file, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine($"Result object: {resultJson}");
            }
            return true;

            static bool IsInSubFolder(string filePath){
                // 
                
                return filePath.Contains("\\");
            }
        }

        static async Task<bool> UploadFileAsync(string filePath, string parentFolderPath = "/"){
            var parentFolderIdObject = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
            string parentFolderId = parentFolderIdObject.ToString();

            Console.WriteLine($"Uploading file: {filePath} to folder path: {parentFolderPath}");

            // Obtenha o cliente autenticado
            BoxClient client = await authenticator.Auth();

            // Verifique o caminho da subpasta e crie, se necessário
            string? folderId = await GetOrCreateFolderByPathAsync(filePath, parentFolderId);

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

               // Faça o upload do arquivo
               Files file = await client.Uploads.UploadFileAsync(requestBody);

               var resultJson = JsonSerializer.Serialize(file, new JsonSerializerOptions { WriteIndented = true });
                // TODO: Atualizar o Status do projeto para => snyc
                // Retornar se File deu certo ou errado
                // 
               Console.WriteLine($"Result object: {resultJson}");
            }

           return true;
        }

        static async Task<string?> GetOrCreateFolderByPathAsync(string folderPath, string parentFolderId){
            BoxClient client = await authenticator.Auth();

            string relativePath = GetRelativePathFromRoot(folderPath);
            string[] pathSegments = relativePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            // TODO: pathSegments está criando uma pasta e depois adicionando o arquivo dentro
            // A ideia aqui é remover na "GetRelativePathFromRoot" tembem o arquivo e manter só a pasta.


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

        static string GetRelativePathFromRoot(string folderPath){

            string rootFolderName = StringUtils.SanitizeString(CentralCache.Instance.GetFromCache("NameProject").ToString());
            // Obtenha o caminho relativo normalmente
            string relativePath = Path.GetRelativePath("BaseFolderPath", folderPath);

            // Divida o caminho em segmentos usando o separador de diretório
            string[] pathSegments = relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

            // Localize a pasta raiz no array de segmentos
            int rootIndex = Array.IndexOf(pathSegments, rootFolderName);

            if (rootIndex == -1)
            {
                throw new InvalidOperationException($"A pasta raiz '{rootFolderName}' não foi encontrada no caminho: {relativePath}");
            }

            // Retorne o caminho relativo a partir da pasta raiz
            return string.Join(Path.DirectorySeparatorChar.ToString(), pathSegments.Skip(rootIndex + 1));
        }

        static async Task<string> UploadFolder(string folderPath, string? parentFolderId){
            if(parentFolderId == null){
                var folderIdObj = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
                parentFolderId = folderIdObj.ToString();
            }else{
                parentFolderId = "0";
            }

            return await FolderSync.SyncFolderToCloud(folderPath, parentFolderId ?? throw new ArgumentNullException(nameof(parentFolderId)));
        }
    }
}