using System;
using WindowsApp.Managers;
using WindowsApp.Models; // temp
using WindowsApp.Test.Class;
using WindowsApp.Models.Class;

namespace WindowsApp{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // Instancia o ProjectManager
            var projectManager = new ProjectManager();
    
            while (true)
            {
                // Exibe um menu simples
                Console.WriteLine("\nSelecione uma opção:");
                Console.WriteLine("1. Adicionar Projeto");
                Console.WriteLine("2. Listar Projetos");
                Console.WriteLine("3. Remover Projeto");
                Console.WriteLine("4. Listar Projeto por Nome");
                Console.WriteLine("5. Atualizar Status do projeto");
                Console.WriteLine("S. Sair");
                Console.WriteLine("Test. Testar Listagem");
                Console.Write("Opção: ");
                
                var opcao = Console.ReadLine();
    
                switch (opcao)
                {
                    case "1": 
                        // Adicionar projeto
                        Console.WriteLine("\n Adicione o Nome do projeto: ");
                        string NameProject = Console.ReadLine();

                        if(NameProject != null){
                            var addingProject = await projectManager.AddProject(NameProject);
                            if(addingProject){
                                Console.WriteLine("Projeto Adicionado com sucesso!");
                            }else{
                                Console.WriteLine("Algum erro deve ter acontecido!");
                            }
                        }
                        break;

                    case "2": 
                        // Listar todos os projeto
                        var dataProjects = await new getLogs().GetProjectsLogFile();
                        if(dataProjects != null){
                            foreach (var project in dataProjects.LocalProjects)
                            {
                                Console.WriteLine($"Nome: {project.Value.Name}, Data: {project.Value.DateTime}, Dispositivo: {project.Value.Device}, Status: {project.Value.Status}");
                            }
                        }else{
                            Console.WriteLine("Nenhum projeto encontrado.");
                        }
                        break;
                    case "3": 
                        // remover projeto
                        Console.WriteLine("\n Adicione o Nome do projeto: ");
                        NameProject = Console.ReadLine();

                        if(NameProject != null){
                            if(await projectManager.DeleteProject(NameProject)){
                                Console.WriteLine("Projeto Removido com sucesso!");
                            }else{
                                Console.WriteLine("Algum erro deve ter acontecido!");
                            }
                        }

                        Console.WriteLine(await new UpdateMetaData().DeleteMetaDataByName("Teste"));
                        break;
                    
                    case "4": 
                        // Listar projeto por nome
                        var ProjectData = await new getLogs().GetProjectsByName("Teste");
                        if(ProjectData != null){
                            Console.WriteLine($"Nome: {ProjectData.Name}, Data: {ProjectData.DateTime}, Dispositivo: {ProjectData.Device}, Status: {ProjectData.Status}");
                        }else{
                            Console.WriteLine("Nenhum projeto encontrado.");
                        }
                        break;

                    case "5": 
                        // remover projeto
                        Console.WriteLine("\nAdicione o Nome do projeto: ");
                        NameProject = Console.ReadLine();

                        if(NameProject != null){
                            if(await projectManager.ChangeProjectData(NameProject, "Status", 2)){
                                Console.WriteLine("Projeto Alterado com sucesso!");
                            }else{
                                Console.WriteLine("Algum erro deve ter acontecido!");
                            }
                        }

                        Console.WriteLine(await new UpdateMetaData().DeleteMetaDataByName("Teste"));
                        break;

                    case "S":
                        // Sair do programa
                        Console.WriteLine("Saindo...");
                        return;

                    case "Test":
                        Console.WriteLine("Teste de Listagem de arquivo");
                        var test = new WindowsApp.Test.Class.ListFilesTest();
                        test.RunTest();
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }
            }
        }
    }

}
