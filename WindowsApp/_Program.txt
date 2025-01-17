using Box.Sdk.Gen;
using WindowsApp.Managers;
using WindowsApp.Models; // temp
using WindowsAppSync.Services.API;
using WindowsApp.Models.Class;

namespace WindowsApp{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var auth = await Authenticator.Auth();
            var projectManager = new ProjectManager(auth);


            while (true)
            {
                Console.WriteLine("\nSelecione uma opção:");
                Console.WriteLine("1. Listar Projetos");
                Console.WriteLine("2. Abrir Projeto");
                Console.WriteLine("3. Adicionar Projeto");
                Console.WriteLine("4. Remover Projeto");
                Console.WriteLine("5. Litar informações do projeto");
                Console.WriteLine("exit. Sair");
                Console.Write("Opção: ");
                
                var opcao = Console.ReadLine();
    
                switch (opcao)
                {
                    case "1": 
                        await ListProjects();
                        break;  
                    case "2": 
                        await OpenProject(auth, projectManager);
                        break;  
                    case "3": 
                        await AddProject(auth, projectManager);
                        break;
                    case "4": 
                        await DeleteProject(auth, projectManager);
                        break; 
                    case "5": 
                        await ListDataOfProject();
                        break;  
                    case "exit":
                        // Sair do programa
                        Console.WriteLine("Saindo...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }
            }

        }

        private static async Task AddProject(BoxClient auth, ProjectManager projectManager){
            Console.WriteLine("\n Adicione o Nome do projeto: ");
            string? NameProject = Console.ReadLine();
            if(NameProject != null){
                var addingProject = await projectManager.AddProject(auth, NameProject);
                if(addingProject){
                    Console.WriteLine("Projeto Adicionado com sucesso!");
                }else{
                    Console.WriteLine("Algum erro deve ter acontecido!");
                }
            }
        }

        private static async Task ListProjects(){
            var ProjectsData = await GetLogs.GetProjectsLogFile();
            if(ProjectsData?.LocalProjects != null ){
                foreach (var project in ProjectsData.LocalProjects.Values){
                    Console.WriteLine($"Nome: {project.Name}, \n Data: {project.DateTime}, \n Dispositivo: {project.Device}, \n Status: {project.Status}");
                    Console.WriteLine($"----------------------------------------------------");
                }                      
            }else{  
                throw new Exception($"MAIN : ListProjects(), error: Metadata or Project Name not invalid.");
            }  
        }

        private static async Task DeleteProject(BoxClient auth, ProjectManager projectManager){
            // remover projeto
            Console.WriteLine("\n Adicione o Nome do projeto: ");
            string? NameProject = Console.ReadLine();
            
            if(NameProject != null){
                if(await projectManager.DeleteProject(NameProject)){
                    Console.WriteLine("Projeto Removido com sucesso!");
                }else{
                    Console.WriteLine("Algum erro deve ter acontecido!");
                }
            }
        }
    
        private static async Task ListDataOfProject(){
            Console.WriteLine("\nAdicione o Nome do projeto: ");
            string? NameProject = Console.ReadLine();

            var ProjectData = await GetLogs.GetProjectsByName(NameProject);
            
            if(ProjectData != null){
                Console.WriteLine($"Nome: {ProjectData.Name}, \n Data: {ProjectData.DateTime}, \n Dispositivo: {ProjectData.Device}, \n Status: {ProjectData.Status}");
            }else{
                Console.WriteLine("Nenhum projeto encontrado.");
            }
        }

        private static async Task OpenProject(BoxClient auth, ProjectManager projectManager){
            Console.WriteLine("\nAdicione o Nome do projeto: ");
            string? NameProject = Console.ReadLine();

            if(NameProject != null){
                await projectManager.OpenProjectForMonitory(auth, NameProject);
            }
        }

    }

}
