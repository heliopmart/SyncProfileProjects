using Box.Sdk.Gen;
using WindowsApp.Managers;
using WindowsApp.Services;

namespace WindowsApp.Managers{
    public class SyncPeddingProjects{
        
        public static async Task<bool> Sincronization(BoxClient client){
            
            if(await ConnectionChecker.CheckConnectionAsync()){
                
                
            }

            return false;
        }

    }
}