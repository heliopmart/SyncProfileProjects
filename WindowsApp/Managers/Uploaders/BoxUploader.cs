using Box.Sdk.Gen;
using WindowsApp.Utils;
using WindowsApp.Managers.Uploaders.Folders;
using WindowsApp.Managers.Uploaders.Files;
namespace WindowsApp.Managers.Uploaders{
    public class BoxUploader
    {
        private static string? _parentFolderId;

        public async Task<bool> UploadManager(BoxClient auth, string filePath, string type, string? OldFilePath, string? parentFolderId = null){
            
            _parentFolderId = parentFolderId;

            if(parentFolderId == null){
                var parentFolderIdObject = CentralCache.Instance.GetFromCache("FolderId") ?? throw new InvalidOperationException("FolderId not found in cache.");
                parentFolderId = parentFolderIdObject.ToString() ?? throw new InvalidOperationException("FolderId not found in cache.");
                _parentFolderId = parentFolderId;
            }

            switch(type){
                case "FileCreated":
                    return await UploadFile(auth, filePath);
                case "FolderCreated": 
                    return await UploadFolder(auth, filePath);
                case "FileChanged":
                    return await ChangeFiles(auth, filePath);
                case "FileOffChange":
                    // TODO: Fazer o upload de um arquivo que foi modificado em local para nuvem
                    return true;
                case "FileDeleted":
                    return await DeleteFiles(auth, filePath);
                case "FolderDeleted":
                    return await DeleteFolders(auth, filePath);
                case "FileRenamed": 
                    return await RenameFiles(auth, filePath, OldFilePath);
                case "FolderRenamed":
                    return await RenameFolders(auth, filePath, OldFilePath);
                case "mainFolder": 
                    return await MainFolderProject(auth, filePath);                   
                default:
                    return false;
            }
        }

        async Task<bool> MainFolderProject(BoxClient auth, string filePath){
            var nameProjectObj = CentralCache.Instance.GetFromCache("NameProject");
            string? NameProject = nameProjectObj != null ? nameProjectObj.ToString() : string.Empty;

            var folderId = await ManagerFolders.UploadFolder(auth, filePath, "0");
            return await ManagerProject.ChangeProjectData(NameProject, "FolderId", folderId);
        }

        static async Task<bool> UploadFile(BoxClient auth, string filePath){
            return await ManagerFiles.UploadFileAsync(auth, filePath, _parentFolderId);
        }

        static async Task<bool> UploadFolder(BoxClient auth, string filePath){
            return await ManagerFolders.UploadFolder(auth, filePath, null) != null;
        }

        static async Task<bool> DeleteFiles(BoxClient auth, string filePath){
            return await ManagerFiles.DeleteFiles(auth, filePath, null, _parentFolderId);
        }

        static async Task<bool> DeleteFolders(BoxClient auth, string filePath){
            return await ManagerFolders.DeleteFolders(auth, filePath, null);
        }

        static async Task<bool> ChangeFiles(BoxClient auth, string filePath){
            return await ManagerFiles.ChangeFileAsync(auth, filePath, _parentFolderId);
        }

        static async Task<bool> RenameFiles(BoxClient auth, string filePath, string? oldFilePath){
            return await ManagerFiles.RenameFile(auth, filePath, oldFilePath, _parentFolderId);
        }

        static async Task<bool> RenameFolders(BoxClient auth, string filePath, string? oldFilePath){
            return await ManagerFolders.RenameFolder(auth, filePath, oldFilePath);
        }

        public static string GetRelativePathFromRoot(string folderPath){
            string? rootFolderName = GetNameProjectByDynamicPath(folderPath);

            if(rootFolderName == null){
                var nameProjectObj = CentralCache.Instance.GetFromCache("NameProject");
                rootFolderName = StringUtils.SanitizeString(nameProjectObj != null ? nameProjectObj.ToString() : string.Empty);
            }
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

        public static async Task<bool> UpdateMetaDataProject(string? NameProject = null){
            if(NameProject == null){
                var nameProjectObj = CentralCache.Instance.GetFromCache("NameProject");
                NameProject = nameProjectObj != null ? nameProjectObj.ToString() : string.Empty;
            }
            
            bool ChangeStatus = await ManagerProject.ChangeProjectData(NameProject, "Status", "1");
            bool ChangeAsnyc = await ManagerProject.ChangeProjectData(NameProject, "AsyncTime", DateTime.Now.ToString());

            if(ChangeAsnyc && ChangeStatus){
                return true;
            }else{
                return true;
            }
        }

        private static string? GetNameProjectByDynamicPath(string filePath)
        {
            // Obtém os diretórios do caminho
            var directories = Path.GetDirectoryName(filePath)?.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Encontra o índice da pasta "Projects"
            int projectIndex = Array.IndexOf(directories, "Projects");

            if (projectIndex != -1 && projectIndex + 1 < directories.Length)
            {
                // Retorna a pasta seguinte à "Projects"
                return directories[projectIndex + 1];
            }

            return null;
        }
    }
}