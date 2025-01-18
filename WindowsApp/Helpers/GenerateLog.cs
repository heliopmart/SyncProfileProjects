using YamlDotNet.Serialization;
using WindowsApp.Models.Class;
using WindowsApp.Managers.Firebase;
using WindowsApp.Utils;

namespace WindowsApp.Helpers
{
    public class GenerateLog
    {
        public async Task<bool> GenerateMetaDataLog()
        {
            var config = ConfigHelper.Instance.GetConfig();
            string filePath = $"{config.MetaDataPath}/metadata.yaml";
            string conteudo = "LocalProjects:";

            try
            {
                // Criar o arquivo e escrever o conteúdo
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    await writer.WriteAsync(conteudo);
                }
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: Unauthorized access to the file during upgrade. {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: I/O problem trying change metadata {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: chanding metadata error. {ex.Message}");
            }
        }
    }

    public class UpdateMetaData
    {
        public static async Task<bool> UpdateMetaDataLog(string ProjectName, ProjectData DataProject)
        {
            var metaDataProject = await GetLogs.GetProjectsLogFile();

            if ((metaDataProject?.LocalProjects != null) && ProjectName != null)
            {
                metaDataProject.LocalProjects[ProjectName] = new ProjectData
                {
                    Name = DataProject.Name,
                    DateTime = DataProject.DateTime,
                    Device = DataProject.Device,
                    Status = DataProject.Status,
                    AsyncTime = DateTime.Now,
                    FolderId = DataProject.FolderId,
                    Id = DataProject.Id ?? null
                };

                if(await ChangeMetaData(metaDataProject)){
                    if(metaDataProject.LocalProjects[ProjectName].Id != null){
                        return await SyncronizationMetaData.UpdateCloudMetaData(ProjectDataConverter.ConvertToFirestoreDocument(metaDataProject.LocalProjects[ProjectName]));
                    }else{
                        return await SyncronizationMetaData.CreateCloudMetaData(ProjectDataConverter.ConvertToFirestoreDocument(metaDataProject.LocalProjects[ProjectName]));
                    }
                }
                return false;
            }
            else
            {
                Console.WriteLine("Erro, código: |2242| - Metadados ou nome do projeto inválido.");
                return false;
            }
        }

        public static async Task<bool> DeleteMetaDataByName(string ProjectName){
            var metaDataProject = await GetLogs.GetProjectsLogFile();
            
            if(metaDataProject?.LocalProjects != null && ProjectName != null){
                metaDataProject.LocalProjects.Remove(ProjectName); // remove pelo nome do projeto (index key)

                return await ChangeMetaData(metaDataProject);
            }else
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: Metadata or Project Name not invalid.");
            }
        }

        private static async Task<bool> ChangeMetaData(Metadata metaDataProject){
            var config = ConfigHelper.Instance.GetConfig();
            try
            {
                var serializer = new SerializerBuilder().Build();
                using (StreamWriter writer = new StreamWriter($"{config.MetaDataPath}/metadata.yaml", false))
                {
                    await writer.WriteAsync(serializer.Serialize(metaDataProject));
                }
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: Unauthorized access to the file during upgrade. {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: I/O problem trying change metadata {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"GetLogs : DeleteMetaDataByName(), error: chanding metadata error. {ex.Message}");
            }
        }

    }

    public class GetLogs
    {
        public static async Task<Metadata> GetProjectsLogFile()
        {
            var config = ConfigHelper.Instance.GetConfig();
            string caminhoCompleto = $"{config.MetaDataPath}/metadata.yaml";

            if (File.Exists(caminhoCompleto))
            {
                try
                {
                    var serializer = new DeserializerBuilder().Build();
                    using (StreamReader reader = new StreamReader(caminhoCompleto))
                    {
                        return serializer.Deserialize<Metadata>(await reader.ReadToEndAsync());
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new Exception($"GetLogs : GetProjectsLogFile(), error: {ex.Message}");
                }
                catch (IOException ex)
                {
                   throw new Exception($"GetLogs : GetProjectsLogFile(), error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"GetLogs : GetProjectsLogFile(), error: {ex.Message}");
                }
            }
            else
            {
                var generateLog = new GenerateLog();
                var created = await generateLog.GenerateMetaDataLog();
                if (!created)
                {
                    throw new Exception($"GetLogs : GetProjectsLogFile(), error:  Falha ao criar arquivo metadata.yaml.");
                }
                return await GetProjectsLogFile();
            }
        }

        public static async Task<ProjectData> GetProjectsByName(string ProjectName){
            var metaDataProject = await GetProjectsLogFile();
            if(metaDataProject?.LocalProjects != null && ProjectName != null){
                if(metaDataProject.LocalProjects.ContainsKey(ProjectName)){
                    return metaDataProject.LocalProjects[ProjectName];
                }else{
                    throw new Exception($"GetLogs : GetProjectsByName(), error: Project not found.");
                }            
            }else{  
                throw new Exception($"GetLogs : GetProjectsByName(), error: Metadata or Project Name not invalid.");
            }            
        }

    }

    // Função para comparar os dados locais (LocalProjects) com os dados do Firestore
    public class SyncronizationMetaData{
        public static async Task<bool> SyncMetaData(){
            List<FirestoreDocument> _firebaseMetaData = await FirebaseManager.GetAllDocumentsAsync("metadata");
            Metadata _localMetadaData = await GetLogs.GetProjectsLogFile();
            try{
                List<FirestoreDocument> _Divergents = CompareLocalWithFirestore(_localMetadaData.LocalProjects, _firebaseMetaData);
                if(await CreateOrUpdateMetaDataBD(_Divergents, _firebaseMetaData)){
                    return true;
                }

                return false;
            }catch(Exception ex){
                Console.WriteLine($"Erro: ({ex})");
                return false;
            }
            
        }

        public static async Task<bool> CreateCloudMetaData(FirestoreDocument DocMetaData){
            return await FirebaseManager.CreateDocumentAsync("metadata", DocMetaData);
        }
        public static async Task<bool> UpdateCloudMetaData(FirestoreDocument DocMetaData){
            return await FirebaseManager.UpdateDocumentAsync("metadata", DocMetaData.Id ,DocMetaData);
        }
        private static List<FirestoreDocument> CompareLocalWithFirestore(
            Dictionary<string, ProjectData> localProjects,
            List<FirestoreDocument> firestoreDocuments)
        {
            var divergentDocuments = new List<FirestoreDocument>();

            // Comparar os documentos do Firestore com os dados locais
            foreach (var localProject in localProjects)
            {
                // Encontrar o documento correspondente nos dados do Firestore
                var firestoreDoc = firestoreDocuments.FirstOrDefault(doc => doc.Name == localProject.Key);

                // Se o documento não existir no Firestore, ele é divergente
                if (firestoreDoc == null)
                {
                
                    divergentDocuments.Add(new FirestoreDocument
                    {
                        Name = localProject.Key,
                        DateTime = localProject.Value.DateTime.ToString(),
                        AsyncTime = localProject.Value.AsyncTime.ToString(),
                        Device = localProject.Value.Device,
                        FolderId = localProject.Value.FolderId,
                        Status = localProject.Value.Status,
                        Id = null
                    });
                }
                else
                {
                    // Se o documento existir no Firestore, comparar os dados
                    bool isOutOfDate = false;

                    // Comparar DateTime e AsyncTime (conversão de string para DateTime)
                    DateTime firestoreDateTime = DateTime.Parse(firestoreDoc.DateTime);
                    DateTime firestoreAsyncTime = DateTime.Parse(firestoreDoc.AsyncTime);

                    if (localProject.Value.DateTime != firestoreDateTime || localProject.Value.AsyncTime != firestoreAsyncTime)
                    {
                        isOutOfDate = true;
                    }

                    // Comparar os outros campos diretamente
                    if (localProject.Value.Device != firestoreDoc.Device || localProject.Value.FolderId != firestoreDoc.FolderId || localProject.Value.Status != firestoreDoc.Status)
                    {
                        isOutOfDate = true;
                    }

                    // Se houver divergência, adiciona à lista
                    if (isOutOfDate)
                    {
                        divergentDocuments.Add(firestoreDoc);
                    }
                }
            }

            return divergentDocuments;
        }
    
        private static async Task<bool> CreateOrUpdateMetaDataBD(List<FirestoreDocument> _Divergents, List<FirestoreDocument>  _firebaseMetaData){
            try{
                foreach (var divergentDoc in _Divergents)
                {
                    var existingDoc = _firebaseMetaData.FirstOrDefault(doc => doc.Name == divergentDoc.Name);

                    if (existingDoc != null)
                    {
                        var documentId = existingDoc.Id;
                        await FirebaseManager.UpdateDocumentAsync("metadata", documentId, divergentDoc);
                    }
                    else
                    {
                        await FirebaseManager.CreateDocumentAsync("metadata", divergentDoc);
                    }
                }

                foreach (var metadataCloud in _firebaseMetaData){
                    var Doc = _Divergents.FirstOrDefault(doc => doc.Name == metadataCloud.Name);
                    if(Doc != null){
                        await UpdateLocalMetaData(metadataCloud.Name, metadataCloud);
                    }else{
                        await CreateLocalMetaData(metadataCloud.Name, metadataCloud);
                    }

                }

                return true;
            }catch(Exception ex){
                Console.WriteLine($"UpdateMetaData : CreateOrUpdateMetaDataBD(), Erro: ({ex})");
                return false;
            }
        }

        private static async Task<bool> UpdateLocalMetaData(string NameProject, FirestoreDocument DataProject){
            DataProject.Status = 2;
            var data = ProjectDataConverter.ConvertToProjectData(DataProject);
            return await UpdateMetaData.UpdateMetaDataLog(NameProject, data);
        }
        private static async Task<bool> CreateLocalMetaData(string NameProject, FirestoreDocument DataProject){
            var data = ProjectDataConverter.ConvertToProjectData(DataProject);
            if(await UpdateMetaData.UpdateMetaDataLog(NameProject, data)){
                var _config = ConfigHelper.Instance.GetConfig();
                var DefaultPathForProjects = _config.DefaultPathForProjects;
                string folderPath = $"{DefaultPathForProjects}/{StringUtils.SanitizeString(DataProject.Name)}";
                try{
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }catch(Exception ex){
                    throw new Exception($"SyncronizationMetaData : CreateLocalMetaData(), Erro: {ex}");
                }
            }

            return false;
        }
    }
}
