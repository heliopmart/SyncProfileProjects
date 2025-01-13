using YamlDotNet.Serialization;
using WindowsApp.Helpers;
using WindowsApp.Models.Class;

namespace WindowsApp.Models
{
    public class GenerateLog
    {
        public async Task<bool> GenerateMetaDataLog()
        {
            var config = ConfigHelper.Instance.GetConfig();
            string filePath = $"{config.MetaDataPath}/metadata.yaml";
            string conteudo = "LocalProjects:";

            try
            {
                // Criar o arquivo e escrever o conteúdo
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    await writer.WriteAsync(conteudo);
                }
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: Unauthorized access to the file during upgrade. {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: I/O problem trying change metadata {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: chanding metadata error. {ex.Message}");
            }
        }
    }

    public class UpdateMetaData
    {
        public static async Task<bool> UpdateMetaDataLog(string ProjectName, ProjectData DataProject)
        {
            var metaDataProject = await GetLogs.GetProjectsLogFile();

            if ((metaDataProject?.LocalProjects != null) && ProjectName != null)
            {
                metaDataProject.LocalProjects[ProjectName] = new ProjectData
                {
                    Name = DataProject.Name,
                    DateTime = DataProject.DateTime,
                    Device = DataProject.Device,
                    Status = DataProject.Status,
                    AsyncTime = DateTime.Now,
                    FolderId = DataProject.FolderId
                };

                return await ChangeMetaData(metaDataProject);
            }
            else
            {
                Console.WriteLine("Erro, código: |2242| - Metadados ou nome do projeto inválido.");
                return false;
            }
        }

        public static async Task<bool> DeleteMetaDataByName(string ProjectName){
            var metaDataProject = await GetLogs.GetProjectsLogFile();
            
            if(metaDataProject?.LocalProjects != null && ProjectName != null){
                metaDataProject.LocalProjects.Remove(ProjectName); // remove pelo nome do projeto (index key)

                return await ChangeMetaData(metaDataProject);
            }else
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: Metadata or Project Name not invalid.");
            }
        }

        private static async Task<bool> ChangeMetaData(Metadata metaDataProject){
            var config = ConfigHelper.Instance.GetConfig();
            try
            {
                var serializer = new SerializerBuilder().Build();
                using (StreamWriter writer = new StreamWriter($"{config.MetaDataPath}/metadata.yaml", false))
                {
                    await writer.WriteAsync(serializer.Serialize(metaDataProject));
                }
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: Unauthorized access to the file during upgrade. {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: I/O problem trying change metadata {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: chanding metadata error. {ex.Message}");
            }
        }
    }

    public class GetLogs
    {
        public static async Task<Metadata> GetProjectsLogFile()
        {
            var config = ConfigHelper.Instance.GetConfig();
            string caminhoCompleto = $"{config.MetaDataPath}/metadata.yaml";

            if (File.Exists(caminhoCompleto))
            {
                try
                {
                    var serializer = new DeserializerBuilder().Build();
                    using (StreamReader reader = new StreamReader(caminhoCompleto))
                    {
                        return serializer.Deserialize<Metadata>(await reader.ReadToEndAsync());
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new Exception($"GetLogs : GetProjectsLogFile(), error: {ex.Message}");
                }
                catch (IOException ex)
                {
                   throw new Exception($"GetLogs : GetProjectsLogFile(), error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"GetLogs : GetProjectsLogFile(), error: {ex.Message}");
                }
            }
            else
            {
                var generateLog = new GenerateLog();
                var created = await generateLog.GenerateMetaDataLog();
                if (!created)
                {
                    throw new Exception($"GetLogs : GetProjectsLogFile(), error:  Falha ao criar arquivo metadata.yaml.");
                }
                return await GetProjectsLogFile();
            }
        }

        public static async Task<ProjectData> GetProjectsByName(string ProjectName){
            var metaDataProject = await GetProjectsLogFile();
            if(metaDataProject?.LocalProjects != null && ProjectName != null){
                if(metaDataProject.LocalProjects.ContainsKey(ProjectName)){
                    return metaDataProject.LocalProjects[ProjectName];
                }else{
                    throw new Exception($"GetLogs : GetProjectsByName(), error: Project not found.");
                }            
            }else{  
                throw new Exception($"GetLogs : GetProjectsByName(), error: Metadata or Project Name not invalid.");
            }            
        }
    }
    
}
