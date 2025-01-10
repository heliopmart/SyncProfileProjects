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
        public string FilePath { get; set; }
        public string ChangeType { get; set; } // Exemplo: "Created", "Modified", "Deleted"
        public string OldFilePath { get; set; } // Caminho antigo, se houver
        public DateTime ChangeTime { get; set; }
    }
}