using System;
using System.IO;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using WindowsApp.Helpers;

namespace WindowsApp.Models.Class
{
    public class getLogs
    {
        public async Task<Metadata?> GetProjectsLogFile()
        {

            var config = ConfigHelper.Instance.GetConfig();
            string caminhoCompleto = $"{config.MetaDataPath}/metadata.yaml";
            if(File.Exists(caminhoCompleto)){
                var serializer = new DeserializerBuilder().Build();
                var projectData = serializer.Deserialize<Metadata>(new StreamReader(caminhoCompleto));

                if(projectData.LocalProjects == null || projectData.LocalProjects.Count == 0){
                    Console.WriteLine("Erro, code: |2281|");
                    // chamar a função que cria um projeto
                    return null;
                }

                return projectData;
            }else{
                var getLogsInstance = new GenerateLog();
                var created = await getLogsInstance.GenerateMetaDataLog();
                
                if(!created){
                    Console.WriteLine($"Error, code: |1292|");
                    return null;    
                }

                return await GetProjectsLogFile();
            }

            
        }
    }

    public class GenerateLog{
        public async Task<bool> GenerateMetaDataLog(){
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
            catch (Exception ex){
                return false;
            }
        }
    }

    /*
        Para visualizar os dados do metadata.yaml: 
            var getLogsInstance = new getLogs(); // Criando uma instância de getLogs
            var metaDataProject = getLogsInstance.GetProjectsLogFile(); // Chamando o método

            // Exibir metadados para fins de teste
            foreach (var project in metaDataProject.LocalProjects)
            {
                Console.WriteLine($"{project.P.Name}:");
                Console.WriteLine($"  Data: {project.P.DateTime}");
                Console.WriteLine($"  Dispositivo: {project.P.Device}");
                Console.WriteLine($"  Status: {project.P.Status}");
            }
    */

    public class Metadata
    {
        public List<ProjectsData> LocalProjects { get; set; }
    }

    public class ProjectsData
    {
        public NameProject Project { get; set; }
    }

    public class NameProject
    {
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string Device { get; set; }
        public int Status { get; set; }
    }
}
