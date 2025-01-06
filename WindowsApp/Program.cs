using System;
using WindowsApp.Managers;
using WindowsApp.Test.Class;

namespace WindowsApp{
    class Program
    {
        static void Main(string[] args)
        {
            // Instancia o ProjectManager
            var projectManager = new ProjectManager();
    
            while (true)
            {
                // Exibe um menu simples
                Console.WriteLine("\nSelecione uma opção:");
                Console.WriteLine("1. Adicionar Projeto");
                Console.WriteLine("2. Listar Projetos");
                Console.WriteLine("3. Mapear Arquivos do Projeto");
                Console.WriteLine("4. Listar Arquivos Mapeados");
                Console.WriteLine("5. Sair");
                Console.WriteLine("Test. Testar Listagem");
                Console.Write("Opção: ");
                
                var opcao = Console.ReadLine();
    
                switch (opcao)
                {
                    case "1":
                        // Adicionar um novo projeto
                        Console.Write("Digite o nome do projeto: ");
                        var name = Console.ReadLine();
                        // Console.Write("Digite o caminho do diretório do projeto: ");
                        // var directoryPath = Console.ReadLine();
    
                        projectManager.AddProject(name);
                        break;
    
                    case "2":
                        // Listar todos os projetos
                        projectManager.ListProjects();
                        break;
    
                    case "3":
                        // Mapear arquivos de um projeto
                        Console.Write("Digite o Nome do projeto: ");
                        var Name = Console.ReadLine();
    
                        projectManager.MapFiles(Name);
                        break;
                
                    case "4":
                        // Listar arquivos mapeados
                        Console.Write("Digite o Nome do projeto: ");
                        var NameProject = Console.ReadLine();
    
                        projectManager.ListFiles(NameProject);
                        break;
    
                    case "5":
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
