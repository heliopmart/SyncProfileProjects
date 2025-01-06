namespace WindowsApp.Models.Class
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DirectoryPath { get; set; }
        public List<FileModel> Files { get; set; }

        public Project()
        {
            Files = new List<FileModel>();
        }

        public Project(string name, string directoryPath)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            DirectoryPath = directoryPath;
            Files = new List<FileModel>();
        }
    }
}