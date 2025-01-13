#pragma warning disable IDE0130 // O namespace não corresponde à estrutura da pasta
namespace WindowsApp.Models.Class
#pragma warning restore IDE0130 // O namespace não corresponde à estrutura da pasta
{
    public class Metadata
    {
        public Dictionary<string, ProjectData>? LocalProjects { get; set; }
    }

    public class ProjectData
    {
        public required string Name { get; set; }
        public DateTime DateTime { get; set; }
        public required string Device { get; set; }
        public DateTime AsyncTime { get; set; }
        public required string FolderId {get; set;}
        public int Status { get; set; }
    }
    // Outras classes similares podem ser adicionadas aqui.
}