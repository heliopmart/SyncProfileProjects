using Google.Cloud.Firestore;
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
        public string? Id {get; set;}
    }

    [FirestoreData] 
    public class FirestoreDocument
    {
        [FirestoreProperty]
        public required string Name { get; set; }
        [FirestoreProperty]
        public required string DateTime { get; set; }
        [FirestoreProperty]
        public required string Device { get; set; }
        [FirestoreProperty]
        public required string AsyncTime { get; set; }
        [FirestoreProperty]
        public required string FolderId {get; set;}
        [FirestoreProperty]
        public int Status { get; set; }
        [FirestoreProperty]
        public string? Id { get; set; }
    }

    public static class ProjectDataConverter
    {
        public static FirestoreDocument ConvertToFirestoreDocument(ProjectData projectData)
        {
            return new FirestoreDocument
            {
                Name = projectData.Name,
                DateTime = projectData.DateTime.ToString("o"), // Usando formato ISO 8601 para garantir consistência
                Device = projectData.Device,
                AsyncTime = projectData.AsyncTime.ToString("o"), // Usando formato ISO 8601 para garantir consistência
                FolderId = projectData.FolderId,
                Status = projectData.Status,
                Id = projectData.Id
            };
        }
        
        public static ProjectData ConvertToProjectData(FirestoreDocument firestoreDocument)
        {
            return new ProjectData
            {
                Name = firestoreDocument.Name,
                DateTime = DateTime.Parse(firestoreDocument.DateTime), // Parse para DateTime
                Device = firestoreDocument.Device,
                AsyncTime = DateTime.Parse(firestoreDocument.AsyncTime), // Parse para DateTime
                FolderId = firestoreDocument.FolderId,
                Status = firestoreDocument.Status,
                Id = firestoreDocument.Id
            };
        }
    }
    // Outras classes similares podem ser adicionadas aqui.
}