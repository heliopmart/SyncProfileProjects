using WindowsApp.Models.Class; // Importa FileModel e Project
using WindowsApp.Helpers;
using WindowsApp.Utils;
using WindowsApp.Helpers.Watchers;

namespace WindowsApp.Managers{
    public class ProjectManager{

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
            if(await ManagerProject.ChangeProjectData(NameProject, KeyForChange, ValueForChange)){
                return true;
            }else{
                return false;
            }
        }

        public async Task<bool> ListProjects(){ // Lista todos os projetos - Variavel Local
           if(await ManagerProject.ListProjects()){
                return true;
           }else{
            return false;
           }
        }

        public static async Task<ProjectData> GetProject(string NameProject){ // Pega um projeto especifico - Variavel Local
            var project = await new ManagerProject().GetProject(NameProject);
            if (project == null)
            {
                throw new Exception("Project not found");
            }
            return project;
        }


        // Verificação de mudanças de arquivo em determinado projeto

        public async Task OpenProjectForMonitory(string NameProject){
            var _config = ConfigHelper.Instance.GetConfig();
            string ProjectPath = $"{_config.DefaultPathForProjects}/{StringUtils.SanitizeString(NameProject)}";

            var projectData = await GetProject(NameProject);
            string IdFolderProject = projectData.FolderId;

            CentralCache.Instance.AddToCache("NameProject", NameProject); // adiciona dados importante em cache
            CentralCache.Instance.AddToCache("ProjectPath", ProjectPath);
            CentralCache.Instance.AddToCache("FolderId", IdFolderProject);
            
            InitProjectFolderMonitory(ProjectPath);

            SyncProcessor.StartSync(ProjectPath, IdFolderProject, _config.SyncInterval);
            //  _ = Task.Run(() => SyncProcessor.StartSync(ProjectPath, IdFolderProject, _config.SyncInterval));
        }

        public static void InitProjectFolderMonitory(string Path){
            // Inicialize os componentes
            var queueManager = new QueueManager();
            var syncManager = new SyncManager(queueManager);
            var fileWatcher = new FileWatcher(Path, queueManager, syncManager);
        
            fileWatcher.StartWatching();
        }
    }
}
