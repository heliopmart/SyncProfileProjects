using Box.Sdk.Gen;
using WindowsApp.Models.Class;
using WindowsApp.Services;
using WindowsApp.Utils;
using  WindowsApp.Managers.Uploaders;

namespace WindowsApp.Managers{
    public class SyncPeddingProjects{
        public static async Task<bool> Sincronization(BoxClient client, string rooPath){
            if(await ConnectionChecker.CheckConnectionAsync()){
                var MetaDataProjects = await ManagerProject.ListProjects();
                if(MetaDataProjects != null && MetaDataProjects.LocalProjects != null){
                    ProjectData[] projects = MetaDataProjects.LocalProjects.Values
                                            .Where(project => project.Status == 2)
                                            .ToArray();

                    foreach(var project in projects){
                        string ProjectPath = $"{rooPath}/{StringUtils.SanitizeString(project.Name)}";

                        var projectData = await new ManagerProject(client).GetProject(project.Name);
                        if(projectData != null){
                            string IdFolderProject = projectData.FolderId;
                            try{
                                Console.WriteLine($"LOG: PeddingFunction : {ProjectPath}");
                                await SyncProcessor.Sync(client, ProjectPath, IdFolderProject);
                                return await BoxUploader.UpdateMetaDataProject(project.Name);
                            }catch(Exception ex){
                                Console.WriteLine($"LOG: SyncPeddingProjects : Sincronization(), Error Sincronização de projetos pendentes. ({ex})");
                                return false;
                            }
                        }else{
                            return false;
                        }
                    }
                }
            }

            return false;
        }
    }
}