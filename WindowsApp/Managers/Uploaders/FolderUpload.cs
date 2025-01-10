using Box.Sdk.Gen;
using Box.Sdk.Gen.Managers;
using System.Threading.Tasks;
using WindowsAppSync.Services.API;

namespace WindowsAppSync.Managers.Uploaders{
    public class FolderSync
    {
        private static readonly Dictionary<string, string> FolderMapping = new Dictionary<string, string>(); // Simula um banco de dados

        public static async Task<string> SyncFolderToCloud(string localFolderPath, string parentFolderId)
        {
            BoxClient client = await new Authenticator().Auth();

            // Verifique se a pasta já foi sincronizada
            if (FolderMapping.TryGetValue(localFolderPath, out var cloudFolderId))
            {
                Console.WriteLine($"Folder already synced. Local: {localFolderPath}, Cloud ID: {cloudFolderId}");
                return "Error: Folder already synced";
            }

            // Obtenha o nome da pasta e o ID do pai (pasta raiz ou outro mapeado)
            string folderName = Path.GetFileName(localFolderPath);

            // Crie a pasta na nuvem
            var createFolderRequest = new CreateFolderRequestBody(
                name: folderName,
                parent: new CreateFolderRequestBodyParentField(parentFolderId)
            );

            var folder = await client.Folders.CreateFolderAsync(createFolderRequest);

            // Salve o mapeamento local ↔ nuvem
            FolderMapping[localFolderPath] = folder.Id;
            Console.WriteLine($"Folder synced. Local: {localFolderPath}, Cloud ID: {folder.Id}");
            return folder.Id;
        }
    }

}