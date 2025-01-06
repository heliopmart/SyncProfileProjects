using System;
using WindowsApp.Managers;

namespace WindowsApp.Test.Class{
     public class ListFilesTest
    {
        public void RunTest()
        {
            var projectManager = new ProjectManager();

            // Adicionar um projeto
            string projectName = "Projeto_Teste_1";
           // string projectPath = @"D:\SyncProfileProjects\Projects\Projeto_Teste_1"; // Usando string literal
            
            Console.WriteLine($"Criando o Projeto com o Nome - {projectName}");
            projectManager.AddProject(projectName);

            // Substitua pelo ID correto do projeto criado (use o ID exibido no console)
            var projectId = "18t2781t28162817212";
            
            // Mapear arquivos no projeto
            Console.WriteLine("Mapeando os Arquivos da Pasta do projeto");
            projectManager.MapFiles(projectName);

            // Listar arquivos do projeto
            Console.WriteLine("Listando os arquivos mapeados");
            projectManager.ListFiles(projectName);
        }
    }

}