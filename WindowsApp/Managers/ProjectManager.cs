using Box.Sdk.Gen;
using WindowsApp.Models.Class; // Importa FileModel e Project
using WindowsApp.Helpers;
using WindowsApp.Utils;
using WindowsApp.Helpers.Watchers;

namespace WindowsApp.Managers{
    public class ProjectManager{
        
        private readonly BoxClient _auth;
        private readonly ManagerProject managerProject;

        public ProjectManager(BoxClient auth){
            _auth = auth;
            managerProject = new ManagerProject(_auth);
        }

        public async Task<bool> AddProject(BoxClient auth, string NameProject){ // Adiciona um novo projeto - Variavel Local
            
            var DataProject = new Project {
                Id = Guid.NewGuid().ToString(),
                Name = NameProject,
                DateTime = DateTime.Now,
                Device = Environment.MachineName.ToString(),
                Status = 0
            };
            
            if(await managerProject.CreateProject(auth, DataProject)){
                return true;
            }else{
                return false;
            }
        }

        public async Task<bool> DeleteProject(string NameProject){
            if(await managerProject.DeleteProject(NameProject)){
                return true;
            }else{
                return false;
            }
        }

        public async Task<bool> ChangeProjectData(BoxClient auth, string NameProject, string KeyForChange, string ValueForChange){
            if(await ManagerProject.ChangeProjectData(NameProject, KeyForChange, ValueForChange)){
                return true;
            }else{
                return false;
            }
        }

        public async Task<bool> ListProjects(BoxClient auth){ // Lista todos os projetos - Variavel Local
           if(await ManagerProject.ListProjects()){
                return true;
           }else{
            return false;
           }
        }

        public async Task<ProjectData> GetProject(string NameProject){ // Pega um projeto especifico - Variavel Local
            var project = await managerProject.GetProject(NameProject);
            if (project == null)
            {
                throw new Exception("Project not found");
            }
            return project;
        }


        // Verificação de mudanças de arquivo em determinado projeto

        public async Task OpenProjectForMonitory(BoxClient auth, string NameProject){
            var _config = ConfigHelper.Instance.GetConfig();
            string ProjectPath = $"{_config.DefaultPathForProjects}/{StringUtils.SanitizeString(NameProject)}";

            var projectData = await GetProject(NameProject);
            string IdFolderProject = projectData.FolderId;

            CentralCache.Instance.AddToCache("NameProject", NameProject); // adiciona dados importante em cache
            CentralCache.Instance.AddToCache("ProjectPath", ProjectPath);
            CentralCache.Instance.AddToCache("FolderId", IdFolderProject);
            
            // Inicia monitoração
            InitProjectFolderMonitory(auth, ProjectPath);

            // Inicie o SyncProcessor
            await Task.Run(() => SyncProcessor.StartSync(auth, ProjectPath, IdFolderProject, _config.SyncInterval));
        }

        public static void InitProjectFolderMonitory(BoxClient auth, string Path){
            var queueManager = new QueueManager();
            var syncManager = new SyncManager(auth, queueManager);

            var fileWatcher = new FileWatcher(Path, queueManager, syncManager);
            fileWatcher.StartWatching();
        }
    }
}
