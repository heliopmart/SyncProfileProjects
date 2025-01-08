using System;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using WindowsApp.Helpers;
using WindowsApp.Models.Class;

// TODO: Percorrer as função que retorna um objeto ou false e fazer o método "Either"

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
                Console.WriteLine("Arquivo metadata.yaml criado com sucesso.");
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Erro, código: |1293| - Acesso não autorizado ao arquivo.");
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (IOException ex)
            {
                Console.WriteLine("Erro, código: |1294| - Problema de I/O ao criar metadata.yaml.");
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro, código: |1295| - Erro desconhecido ao criar metadata.yaml.");
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }

    public class UpdateMetaData
    {
        public async Task<bool> UpdateMetaDataLog(string ProjectName, ProjectData DataProject)
        {
            var getLogsInstance = new getLogs();
            var metaDataProject = await getLogsInstance.GetProjectsLogFile();
            var config = ConfigHelper.Instance.GetConfig();

            if ((metaDataProject?.LocalProjects != null) && ProjectName != null)
            {
                metaDataProject.LocalProjects[ProjectName] = new ProjectData
                {
                    Name = DataProject.Name,
                    DateTime = DataProject.DateTime,
                    Device = DataProject.Device,
                    Status = DataProject.Status
                };

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
                    Console.WriteLine("Erro, código: |2293| - Acesso não autorizado ao arquivo durante atualização.");
                    Console.WriteLine(ex.Message);
                    return false;
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Erro, código: |2294| - Problema de I/O ao atualizar metadata.yaml.");
                    Console.WriteLine(ex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro, código: |2295| - Erro desconhecido ao atualizar metadata.yaml.");
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Erro, código: |2242| - Metadados ou nome do projeto inválido.");
                return false;
            }
        }

        public async Task<bool> DeleteMetaDataByName(string ProjectName){
            var getLogsInstance = new getLogs();
            var metaDataProject = await getLogsInstance.GetProjectsLogFile();
            var config = ConfigHelper.Instance.GetConfig();
            
            if(metaDataProject?.LocalProjects != null && ProjectName != null){
                metaDataProject.LocalProjects.Remove(ProjectName); // remove pelo nome do projeto (index key)

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
                    Console.WriteLine("Erro, código: |2293| - Acesso não autorizado ao arquivo durante atualização.");
                    Console.WriteLine(ex.Message);
                    return false;
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Erro, código: |2294| - Problema de I/O ao atualizar metadata.yaml.");
                    Console.WriteLine(ex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro, código: |2295| - Erro desconhecido ao atualizar metadata.yaml.");
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }else
            {
                Console.WriteLine("Erro, código: |2242| - Metadados ou nome do projeto inválido.");
                return false;
            }
        }
    }

    public class getLogs
    {
        public async Task<Metadata?> GetProjectsLogFile()
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
                    Console.WriteLine("Erro, código: |2283| - Acesso não autorizado ao arquivo ao ler metadados.");
                    Console.WriteLine(ex.Message);
                    return null;
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Erro, código: |2284| - Problema de I/O ao ler metadata.yaml.");
                    Console.WriteLine(ex.Message);
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro, código: |2285| - Erro desconhecido ao ler metadata.yaml.");
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            else
            {
                var generateLog = new GenerateLog();
                var created = await generateLog.GenerateMetaDataLog();
                if (!created)
                {
                    Console.WriteLine("Erro, código: |2286| - Falha ao criar arquivo metadata.yaml.");
                    return null;
                }
                return await GetProjectsLogFile();
            }
        }

        public async Task<ProjectData?> GetProjectsByName(string ProjectName){
            var metaDataProject = await new getLogs().GetProjectsLogFile();
            if(metaDataProject?.LocalProjects != null && ProjectName != null){
                if(metaDataProject.LocalProjects.ContainsKey(ProjectName)){
                    return metaDataProject.LocalProjects[ProjectName];
                }else{
                    Console.WriteLine("Erro, código: |2243| - Projeto não encontrado.");
                    return null;
                }            
            }else{  
                Console.WriteLine("Erro, código: |2242| - Metadados ou nome do projeto inválido.");
                return null;
            }            
        }
    }
    
}
