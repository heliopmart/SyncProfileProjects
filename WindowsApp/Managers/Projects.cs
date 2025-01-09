using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using WindowsApp.Models; // Importa FileModel e Project
using WindowsApp.Helpers;
using WindowsApp.Models.Class; // Importa FileModel e Project
using WindowsApp.Utils;


namespace WindowsApp.Managers{
    public class ManagerProject{
        
        private static readonly APPConfig _config = ConfigHelper.Instance.GetConfig();
        
        public async Task<bool> DeleteProject(string ProjectName){
            var DefaultPathForProjects = _config.DefaultPathForProjects;

            if(await DeleteMetaDataProject_Local(ProjectName)){
                return DeleteFolderProject_Local(ProjectName, DefaultPathForProjects);
            }

            return false;
            
            async Task<bool> DeleteMetaDataProject_Local(string ProjectName){
                return await new UpdateMetaData().DeleteMetaDataByName(ProjectName);
            }

            bool DeleteFolderProject_Local(string NameProject, string Path){
                var ProjectPath = $"{Path}/{StringUtils.SanitizeString(NameProject)}";
                var (hasFiles, hasFolders) = CheckFolderContents(ProjectPath);
                bool HasFiles = false;
                if(hasFiles || hasFolders){
                    HasFiles = true;
                }

                if(DeleteFolderRecursively(ProjectPath, HasFiles)){
                    return true;
                }else{
                    Console.WriteLine("Erro! Não foi possivel deletar a pasta do projeto. Código de erro: |3294|");
                    return false;
                }

                (bool hasFiles, bool hasFolders) CheckFolderContents(string folderPath){
                    try
                    {
                        if (Directory.Exists(folderPath))
                        {
                            // Verifica arquivos
                            bool hasFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories).Length > 0;

                            // Verifica pastas
                            bool hasFolders = Directory.GetDirectories(folderPath).Length > 0;

                            return (hasFiles, hasFolders);
                        }
                        else
                        {
                            Console.WriteLine($"A pasta '{folderPath}' não existe.");
                            return (false, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao verificar conteúdo da pasta: {ex.Message}");
                        return (false, false);
                    }
                }

                bool DeleteFolderRecursively(string folderPath, bool HasFiles){
                    try{
                        if (Directory.Exists(folderPath)){
                            Directory.Delete(folderPath, HasFiles);
                            return true;
                        }
                        else{
                            return false;
                        }
                    }
                    catch (Exception ex){
                        Console.WriteLine($"Erro ao deletar a pasta: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task<bool> CreateProject(Project DataProject){
            var DefaultPathForProjects = _config.DefaultPathForProjects;

            if(CreateFolderProject_Local(DataProject.Name, DefaultPathForProjects)){
                return await CreateMetaDataProject_Local(DataProject);
            }

            return false; 

            async Task<bool> CreateMetaDataProject_Local(Project DataProject){
                
                var DataProjectForLog = new ProjectData{
                    Name = DataProject.Name,
                    DateTime = DataProject.DateTime,
                    Device = DataProject.Device,
                    Status = DataProject.Status
                };
                var MetaData = await new UpdateMetaData().UpdateMetaDataLog(DataProjectForLog.Name, DataProjectForLog);
                if(MetaData){
                    return true;
                }else{
                    Console.WriteLine("Erro! Não foi possivel adicionar o Projeto no metadata local. Código de erro: |3263|");
                    return false;
                }
            }

            bool CreateFolderProject_Local(string NameProject, string Path){
                
                string folderPath = $"{Path}/{StringUtils.SanitizeString(NameProject)}";

                try{
                    // Verifica se a pasta já existe
                    if (!Directory.Exists(folderPath))
                    {
                        // Cria a pasta
                        Directory.CreateDirectory(folderPath);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"A pasta já existe: {folderPath}. Código do Erro: |5123|");
                        return false; // Código de erro 5123: Pasta já existente
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Erro ao criar a pasta: Permissão negada. Código do Erro: |3123|");
                    Console.WriteLine($"Detalhes do erro: {ex.Message}");
                    return false; // Código de erro 3123: Permissão negada
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Erro ao criar a pasta: Caminho inválido. Código do Erro: |3223|");
                    Console.WriteLine($"Detalhes do erro: {ex.Message}");
                    return false; // Código de erro 3223: Caminho inválido
                }
                catch (PathTooLongException ex)
                {
                    Console.WriteLine($"Erro ao criar a pasta: Caminho muito longo. Código do Erro: |3323|");
                    Console.WriteLine($"Detalhes do erro: {ex.Message}");
                    return false; // Código de erro 3323: Caminho muito longo
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Erro ao criar a pasta: Erro de I/O. Código do Erro: |3423|");
                    Console.WriteLine($"Detalhes do erro: {ex.Message}");
                    return false; // Código de erro 3423: Erro de I/O
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro desconhecido ao criar a pasta. Código do Erro: |3523|");
                    Console.WriteLine($"Detalhes do erro: {ex.Message}");
                    return false; // Código de erro 3523: Erro desconhecido
                }
            }

            async Task<bool> CloudProjectSync_Sync(Project DataProject){
                // TODO: 
                // Adicionar o projeto em nuvem 
                // sincronizar os arquivos com o Box.com
                return false;
            }
        }

        public async Task<bool> ChangeProjectData(string NameProject, string KeyForChange, int ValueForChange){

            var changedProjectData = await ChangeMetaDataProject_Local(NameProject, KeyForChange, ValueForChange);
            if(changedProjectData != null){
                return await new UpdateMetaData().UpdateMetaDataLog(NameProject, changedProjectData);
            }else{
                Console.WriteLine("Erro! Não foi possivel alterar o Projeto no metadata local. Código de erro: |3263|");
                return false;
            }

            async Task<ProjectData?> ChangeMetaDataProject_Local(string NameProject, string KeyForChange, int ValueForChange){
                var projects = await new getLogs().GetProjectsLogFile();
                var metadataSingleProject = projects?.LocalProjects[NameProject];


                if(metadataSingleProject != null && projects != null){
                    var property = typeof(ProjectData).GetProperty(KeyForChange, BindingFlags.Public | BindingFlags.Instance);
                    if (property != null && property.CanWrite){
                        try
                        {
                            // Converte o ValueForChange para o tipo correto da propriedade
                            var convertedValue = Convert.ChangeType(ValueForChange, property.PropertyType);
                            property.SetValue(metadataSingleProject, convertedValue);

                            return metadataSingleProject;
                        }catch{
                            Console.WriteLine("Erro ao alterar o valor da propriedade.");
                            return null;
                        }
                    }else{
                        Console.WriteLine("Propriedade não encontrada ou não pode ser alterada.");
                        return null;
                    }
                }else{
                    Console.WriteLine("Projeto não encontrado.");
                    return null;
                }
            }

            
        }

        public async Task<bool> ListProjects(){
            var dataProjects = await new getLogs().GetProjectsLogFile();

            if(dataProjects != null){
                foreach (var project in dataProjects.LocalProjects)
                {
                    Console.WriteLine($"Nome: {project.Value.Name}, Data: {project.Value.DateTime}, Dispositivo: {project.Value.Device}, Status: {project.Value.Status}");
                }
                return true;
            }else{
                Console.WriteLine("Nenhum projeto encontrado.");
                return false;
            }

        }
    }
        
    
}