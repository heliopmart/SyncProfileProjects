using System;
using WindowsApp.Managers;
using WindowsApp.Models; // temp
using WindowsApp.Test.Class;
using WindowsApp.Models.Class;
using WindowsAppSync.Services.API;

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
                Console.WriteLine("6. Abrir Projeto");
                Console.WriteLine("7. Fechar Projeto");
                Console.WriteLine("8. Testar Conexão com Box.com");
                Console.WriteLine("S. Sair");
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
                        await new ProjectManager().ListProjects();
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
                        break;
                    
                    case "6": 
                        // remover projeto
                        Console.WriteLine("\nAdicione o Nome do projeto: ");
                        NameProject = Console.ReadLine();

                        if(NameProject != null){
                            projectManager.OpenProjectForMonitory(NameProject);
                        }
                        break;

                    case "7": 
                        // remover projeto
                        Console.WriteLine("\nAdicione o Nome do projeto: ");
                        NameProject = Console.ReadLine();

                        if(NameProject != null){
                            projectManager.OpenProjectForMonitory(NameProject);
                        }
                        break;

                    case "8": 
                        await new Authenticator().Auth();
                        break;

                    case "S":
                        // Sair do programa
                        Console.WriteLine("Saindo...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }
            }
        }
    }

}
