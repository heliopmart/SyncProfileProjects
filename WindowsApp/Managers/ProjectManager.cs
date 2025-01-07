using System;
using System.Collections.Generic;
using System.IO;
using WindowsApp.Models.Class; // Importa FileModel e Project
using WindowsApp.Helpers;

namespace WindowsApp.Managers{
    public class ProjectManager{
        private List<Project> Projects { get; set; } = new List<Project>();

        public void AddProject(string name){ // Adiciona um novo projeto - Variavel Local

            var config = ConfigHelper.Instance.GetConfig();
            var directoryPath = $"{config.DefaultPathForProjects}/{name}";

            if(Directory.Exists(directoryPath)){
                var project = new Project{
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    DirectoryPath = directoryPath,
                    Files = new List<FileModel>() // usa o filemodel aqui 
                };

                Projects.Add(project);
                Console.WriteLine($"Projeto '{name}' adicionado com sucesso");
            }else{
                Console.WriteLine($"Diretório '{directoryPath}' não encontrado");
            }
        }

        public void ListProjects(){ // Lista todos os projetos - Variavel Local
            if(Projects.Count == 0){
                Console.WriteLine("Nenhum projeto encontrado");
                return;
            }

            foreach (var project in Projects){
                Console.WriteLine($"ID: {project.Id}, Nome: {project.Name}, Diretório: {project.DirectoryPath}");
            }
        }

        public void MapFiles(string Name){ // Mapeia arquivos de um projeto - Variavel Local
            var project = Projects.Find(p => p.Name == Name);

            if(project != null){
                project.Files.Clear(); // limpa a lista de arquivos antigos
                var files = Directory.GetFiles(project.DirectoryPath); // Obtém arquivos da pasta
                foreach (var filePath in files){
                    var fileInfo = new FileInfo(filePath);

                    project.Files.Add(new FileModel{
                        FileName = fileInfo.Name,
                        FilePath = filePath,
                        FileSize = fileInfo.Length,
                        LastModified = fileInfo.LastWriteTime,
                        Status = 0 // Status 0 => Padrão 
                    });
                }

                Console.WriteLine($"Os arquivos do projeto '{project.Name}' foram mapeados com sucesso");
            }else{
                Console.WriteLine($"Projeto '{Name}' não encontrado.");
            }
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
