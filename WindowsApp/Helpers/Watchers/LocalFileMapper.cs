using WindowsApp.Models;
using System.Security.Cryptography;

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
                    Sha1 = null,
                    LastModified = dirInfo.LastWriteTimeUtc.ToUniversalTime(),
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
                    LastModified = fileInfo.LastWriteTimeUtc.ToUniversalTime(),
                    Sha1 = CalculateFileHash(file),
                    IsFolder = false
                });
            }

            return fileList;
        }

        private static string CalculateFileHash(string filePath)
        {
            using (var sha1 = SHA1.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = sha1.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); // Converte para string hexadecimal
                }
            }
        }
    }


}
