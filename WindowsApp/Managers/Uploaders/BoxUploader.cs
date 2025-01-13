using WindowsApp.Utils;
using WindowsApp.Managers.Uploaders.Folders;
using WindowsApp.Managers.Uploaders.Files;
#pragma warning disable IDE0130 // O namespace não corresponde à estrutura da pasta
namespace WindowsApp.Managers.Uploaders{
#pragma warning restore IDE0130 // O namespace não corresponde à estrutura da pasta
    public class BoxUploader
    {
        public async Task<bool> UploadManager(string filePath, string type, string? OldFilePath){

            switch(type){
                case "FileCreated":
                    return await UploadFile(filePath);
                case "FolderCreated": 
                    return await UploadFolder(filePath);
                case "FileChanged":
                    return await ChangeFiles(filePath);
                case "FolderChanged":
                    return true;
                case "FileDeleted":
                    return await DeleteFiles(filePath);
                case "FolderDeleted":
                    return await DeleteFolders(filePath);
                case "FileRenamed": 
                    return await RenameFiles(filePath, OldFilePath);
                case "FolderRenamed":
                    return await RenameFolders(filePath, OldFilePath);
                case "mainFolder": 
                    return await MainFolderProject(filePath);                   
                default:
                    return false;
            }
        }

        async Task<bool> MainFolderProject(string filePath){
            var nameProjectObj = CentralCache.Instance.GetFromCache("NameProject");
            string? NameProject = nameProjectObj != null ? nameProjectObj.ToString() : string.Empty;

            var folderId = await ManagerFolders.UploadFolder(filePath, "0");
            return await new ManagerProject().ChangeProjectData(NameProject, "FolderId", folderId);
        }

        static async Task<bool> UploadFile(string filePath){
            return await ManagerFiles.UploadFileAsync(filePath);
        }

        static async Task<bool> UploadFolder(string filePath){
            return await ManagerFolders.UploadFolder(filePath, null) != null;
        }

        static async Task<bool> DeleteFiles(string filePath){
            return await ManagerFiles.DeleteFiles(filePath, null);
        }

        static async Task<bool> DeleteFolders(string filePath){
            return await ManagerFolders.DeleteFolders(filePath, null);
        }

        static async Task<bool> ChangeFiles(string filePath){
            return await ManagerFiles.ChangeFileAsync(filePath);
        }

        static async Task<bool> RenameFiles(string filePath, string? oldFilePath){
            return await ManagerFiles.RenameFile(filePath, oldFilePath);
        }

        static async Task<bool> RenameFolders(string filePath, string? oldFilePath){
            return await ManagerFolders.RenameFolder(filePath, oldFilePath);
        }

        public static string GetRelativePathFromRoot(string folderPath){
            var nameProjectObj = CentralCache.Instance.GetFromCache("NameProject");
            string rootFolderName = StringUtils.SanitizeString(nameProjectObj != null ? nameProjectObj.ToString() : string.Empty);
            // Obtenha o caminho relativo completo
            string relativePath = Path.GetRelativePath("BaseFolderPath", folderPath);

            // Divida o caminho em segmentos
            string[] pathSegments = relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

            // Localize a pasta raiz no array de segmentos
            int rootIndex = Array.IndexOf(pathSegments, rootFolderName);

            if (rootIndex == -1)
            {
                throw new InvalidOperationException($"A pasta raiz '{rootFolderName}' não foi encontrada no caminho: {relativePath}");
            }

            // Extraia os segmentos após a pasta raiz
            var pathAfterRoot = pathSegments.Skip(rootIndex + 1).ToArray();

            // Verifique se o último segmento é um arquivo e remova-o
            if (pathAfterRoot.Length > 0 && Path.HasExtension(pathAfterRoot[^1]))
            {
                pathAfterRoot = pathAfterRoot.Take(pathAfterRoot.Length - 1).ToArray();
            }

            // Reconstrua o caminho das pastas
            return string.Join(Path.DirectorySeparatorChar.ToString(), pathAfterRoot);
        }

        public static async Task<bool> UpdateMetaDataProject(){
            var FunManagerProjects = new ManagerProject();
            var nameProjectObj = CentralCache.Instance.GetFromCache("NameProject");
            string? NameProject = nameProjectObj != null ? nameProjectObj.ToString() : string.Empty;
            
            bool ChangeStatus = await FunManagerProjects.ChangeProjectData(NameProject, "Status", "1");
            bool ChangeAsnyc = await FunManagerProjects.ChangeProjectData(NameProject, "AsyncTime", DateTime.Now.ToString());

            if(ChangeAsnyc && ChangeStatus){
                return true;
            }else{
                return true;
            }
        }
    }
}