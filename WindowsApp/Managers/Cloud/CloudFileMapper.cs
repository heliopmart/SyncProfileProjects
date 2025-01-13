
using Box.Sdk.Gen;
using WindowsApp.Models;
using System.Text.Json;
using WindowsAppSync.Services.API;

namespace WindowsApp.Managers.Cloud{
    public static class CloudFileMapper
    {
        public static async Task<List<CloudFileItem>> MapCloudFilesAsync(string rootFolderId = "0")
        {
            var fileList = new List<CloudFileItem>();

            async Task TraverseFolderAsync(string folderId, string parentPath){
                BoxClient client = Authenticator.Auth();
                var items = (await client.Folders.GetFolderItemsAsync(folderId)).Entries;

                if(items != null){
                    foreach (var item in items)
                    {
                        // Serializa o item como JSON
                        string jsonItem = JsonSerializer.Serialize(item);

                        // Converte o JSON para um JsonDocument
                        using var jsonDoc = JsonDocument.Parse(jsonItem);
                        var root = jsonDoc.RootElement;

                        if (root.TryGetProperty("type", out var typeProperty))
                        {
                            if (typeProperty.GetString() == "folder")
                            {
                                var id = root.GetProperty("id").GetString();
                                var name = root.GetProperty("name").GetString();

                                if(id != null){
                                    fileList.Add(new CloudFileItem
                                    {
                                        Id = id,
                                        Path = Path.Combine(parentPath, name),
                                        LastModified = null,
                                        IsFolder = true
                                    });

                                    // Recursivamente mapear subpastas
                                    await TraverseFolderAsync(id, Path.Combine(parentPath, name));
                                }else{
                                    throw new Exception("CloudFileMapper : TraverseFolderAsync(), error: Folder Id not exist");
                                }

                            }
                            else if (typeProperty.GetString() == "file")
                            {
                                var id = root.GetProperty("id").GetString();
                                var name = root.GetProperty("name").GetString();

                                if(id != null && parentPath != null){
                                    DateTime? lastModified = root.TryGetProperty("modified_at", out var modifiedAtProperty) &&
                                                             modifiedAtProperty.ValueKind != JsonValueKind.Null
                                        ? modifiedAtProperty.GetDateTime()
                                        : (DateTime?)null;


                                    fileList.Add(new CloudFileItem
                                    {
                                        Id = id,
                                        Path = Path.Combine(parentPath, name),
                                        LastModified = lastModified,
                                        IsFolder = false
                                    });
                                }else{
                                    throw new Exception("CloudFileMapper : TraverseFolderAsync(), error: File Id or parentPath not exist");
                                }
                            }else{
                                throw new Exception("CloudFileMapper : TraverseFolderAsync(), error: Not folder or files");
                            }
                        }
                    }
                }

            }

            await TraverseFolderAsync(rootFolderId, "");
            return fileList;
        }
    }

}