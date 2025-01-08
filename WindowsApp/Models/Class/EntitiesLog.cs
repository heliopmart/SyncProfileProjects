namespace WindowsApp.Models.Class
{
    public class Metadata
    {
        public Dictionary<string, ProjectData> LocalProjects { get; set; }
    }

    public class ProjectData
    {
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string Device { get; set; }
        public int Status { get; set; }
    }
    // Outras classes similares podem ser adicionadas aqui.
}