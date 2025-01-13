using WindowsApp.Models;

namespace WindowsApp.Helpers.Watchers{
    public static class LocalFileMapper
    {
        public static List<FileItem> MapLocalFiles(string rootPath)
        {
            var fileList = new List<FileItem>();


             foreach (var folder in Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories))
            {
                var dirInfo = new DirectoryInfo(folder);
                fileList.Add(new FileItem
                {
                    Path = folder.Replace(rootPath, ""), // Caminho relativo
                    FullPath = folder,
                    LastModified = dirInfo.LastWriteTimeUtc,
                    IsFolder = true
                });
            }

            foreach (var file in Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(file);
                fileList.Add(new FileItem
                {
                    Path = file.Replace(rootPath, ""), // Caminho relativo
                    FullPath = file,
                    LastModified = fileInfo.LastWriteTimeUtc,
                    IsFolder = false
                });
            }

            return fileList;
        }
    }


}
