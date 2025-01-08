using System;
using System.Collections.Generic;
using System.IO;
using WindowsApp.Models.Class; // Importa FileModel e Project
using WindowsApp.Managers;

namespace WindowsApp.Managers{
        public class ProjectManager{
        private List<Project> Projects { get; set; } = new List<Project>();

        public async Task<bool> AddProject(string NameProject){ // Adiciona um novo projeto - Variavel Local
            
            var DataProject = new Project {
                Id = Guid.NewGuid().ToString(),
                Name = NameProject,
                DateTime = DateTime.Now,
                Device = Environment.MachineName.ToString(),
                Status = 0
            };
            
            if(await new ManagerProject().CreateProject(DataProject)){
                return true;
            }else{
                return false;
            }
        }

        public async Task<bool> DeleteProject(string NameProject){
            if(await new ManagerProject().DeleteProject(NameProject)){
                return true;
            }else{
                return false;
            }
        }

        public async Task<bool> ChangeProjectData(string NameProject, string KeyForChange, int ValueForChange){
            if(await new ManagerProject().ChangeProjectData(NameProject, KeyForChange, ValueForChange)){
                return true;
            }else{
                return false;
            }
        }

        public void ListProjects(){ // Lista todos os projetos - Variavel Local
            if(Projects.Count == 0){
                Console.WriteLine("Nenhum projeto encontrado");
                return;
            }

            foreach (var project in Projects){
                // Console.WriteLine($"ID: {project.Id}, Nome: {project.Name}, Diretório: {project.DirectoryPath}");
            }
        }

        public void MapFiles(string Name){ // Mapeia arquivos de um projeto - Variavel Local
            var project = Projects.Find(p => p.Name == Name);

            // if(project != null){
            //     project.Files.Clear(); // limpa a lista de arquivos antigos
            //     var files = Directory.GetFiles(project.DirectoryPath); // Obtém arquivos da pasta
            //     foreach (var filePath in files){
            //         var fileInfo = new FileInfo(filePath);

            //         project.Files.Add(new FileModel{
            //             FileName = fileInfo.Name,
            //             FilePath = filePath,
            //             FileSize = fileInfo.Length,
            //             LastModified = fileInfo.LastWriteTime,
            //             Status = 0 // Status 0 => Padrão 
            //         });
            //     }

            //     Console.WriteLine($"Os arquivos do projeto '{project.Name}' foram mapeados com sucesso");
            // }else{
            //     Console.WriteLine($"Projeto '{Name}' não encontrado.");
            // }
        }

        public void ListFiles(string Name){ // Lista arquivos mapeados - Variavel Local
            var project = Projects.Find(p => p.Name == Name); // Localiza o projeto pelo ID

            if (project != null){
                if (project.Files.Count == 0)
                {
                    Console.WriteLine($"Nenhum arquivo mapeado no projeto '{project.Name}'.");
                    return;
                }

                Console.WriteLine($"Arquivos do projeto '{project.Name}':");
                foreach (var file in project.Files) // Itera sobre os arquivos
                {
                    Console.WriteLine($"- Nome: {file.FileName}, Caminho: {file.FilePath}, Tamanho: {file.FileSize} bytes, Modificado: {file.LastModified}");
                }
            }
            else{
                Console.WriteLine($"Projeto com ID '{Name}' não encontrado.");
            }
        }

    }
}
