// using System;
// using System.Collections.Generic;
// using System.IO;
using WindowsApp.Models.Class; // Importa FileModel e Project
using WindowsApp.Helpers;
using WindowsApp.Utils;
using WindowsApp.Helpers.Watchers;
using WindowsApp.Managers;

namespace WindowsApp.Managers{
    public class ProjectManager{
        private List<Project> Projects { get; set; } = new List<Project>();

        // Gerenciamento e criaçao de projetos Pastas e MetaData

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

        public async Task<bool> ChangeProjectData(string NameProject, string KeyForChange, string ValueForChange){
            if(await new ManagerProject().ChangeProjectData(NameProject, KeyForChange, ValueForChange)){
                return true;
            }else{
                return false;
            }
        }

        public async Task<bool> ListProjects(){ // Lista todos os projetos - Variavel Local
           if(await new ManagerProject().ListProjects()){
                return true;
           }else{
            return false;
           }
        }

        public async Task<ProjectData> GetProject(string NameProject){ // Pega um projeto especifico - Variavel Local
            return await new ManagerProject().GetProject(NameProject);
        }


        // Verificação de mudanças de arquivo em determinado projeto

        public async void OpenProjectForMonitory(string NameProject){
            var _config = ConfigHelper.Instance.GetConfig().DefaultPathForProjects;
            string ProjectPath = $"{_config}/{StringUtils.SanitizeString(NameProject)}";

            var projectData = await GetProject(NameProject);
            string IdFolderProject = projectData.FolderId;

            CentralCache.Instance.AddToCache("NameProject", NameProject); // adiciona dados importante em cache
            CentralCache.Instance.AddToCache("ProjectPath", ProjectPath);
            CentralCache.Instance.AddToCache("FolderId", IdFolderProject);
            
            InitProjectFolderMonitory(ProjectPath);
        }

        public void InitProjectFolderMonitory(string Path){
            // Inicialize os componentes
            var queueManager = new QueueManager();
            var syncManager = new SyncManager(queueManager);
            var fileWatcher = new FileWatcher(Path, queueManager, syncManager);
        
            // Inicie o monitoramento
            fileWatcher.StartWatching();
        }
    }
}
