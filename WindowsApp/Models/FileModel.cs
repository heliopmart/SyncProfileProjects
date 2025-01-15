namespace WindowsApp.Models // Namespace reflete a estrutura da pasta
{
    public class FileModel
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
        public int Status { get; set; } = 0;
    }

    public class FileChange
    {
        public required string FilePath { get; set; }
        public required string ChangeType { get; set; } // Exemplo: "Created", "Modified", "Deleted"
        public string? OldFilePath { get; set; } // Caminho antigo, se houver
        public DateTime ChangeTime { get; set; }
    }

    public class FileItem
    {
        public required string Path { get; set; }
        public required string FullPath { get; set; }
        public string? Sha1 { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsFolder { get; set; }
    }

    public class CloudFileItem
    {
        public required string Id { get; set; }
        public required string Path { get; set; }
        public string? Sha1 { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsFolder { get; set; }
    }

}

