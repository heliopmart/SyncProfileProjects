namespace WindowsApp.Models.Class
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<FileModel> Files { get; set; }
        public DateTime DateTime { get; set; }
        public string Device { get; set; }
        public int Status { get; set; }

        public Project()
        {
            Files = new List<FileModel>();
        }

        public Project(string name, string directoryPath)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Files = new List<FileModel>();
        }
    }

    public class DataProjectForLog{
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string Device { get; set; }
        public int Status { get; set; }
    }
}