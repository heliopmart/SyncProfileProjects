namespace WindowsApp.Models.Class // Namespace reflete a estrutura da pasta
{
    public class FileModel
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
        public int Status { get; set; } = 0;
    }
}