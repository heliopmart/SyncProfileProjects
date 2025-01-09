using Box.Sdk.Gen;
using WindowsApp.Helpers;

namespace WindowsAppSync.Services.API{

    public class Authenticator{
        public async Task Auth(){
            var _config = ConfigHelper.Instance.GetConfig();

            var auth = new BoxDeveloperTokenAuth(token:  _config.APIConfigs.ClientId);
            var client = new BoxClient(auth: auth);

            var items = await client.Folders.GetFolderItemsAsync(folderId: "0");
            if (items.Entries != null)
            {
                foreach (var item in items.Entries)
                {
                    if (item.FileFull != null)
                    {
                        Console.WriteLine(item.FileFull.Name);
                    }
                    else if (item.FolderMini != null)
                    {
                        Console.WriteLine(item.FolderMini.Name);
                    }
                    else if (item.WebLink != null)
                    {
                        Console.WriteLine(item.WebLink.Name);
                    }
                }
            }
        }

    }

}
