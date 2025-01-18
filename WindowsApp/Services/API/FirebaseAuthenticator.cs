using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using WindowsApp.Helpers;

/*
    Funciona apenas com: 
    $env:GOOGLE_APPLICATION_CREDENTIALS="D:\SyncProfileProjects\Projects\Sync_SyncProfileProjects\WindowsApp\Keys\serviceAccountKey.json"  
*/

namespace WindowsAppSync.Services.API{
   public class FirebaseAuthenticator
    {
        private static FirebaseApp? _firebaseApp;
        public static void AuthenticateWithOAuthAsync()
        {
          try
            {
                // Caminho para o arquivo de chave JSON que você baixou do Console Firebase
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory; // Diretório atual do aplicativo
                var pathToServiceAccount = Path.Combine(baseDirectory, "Keys", "serviceAccountKey.json");

                // Inicializar o Firebase Admin SDK com as credenciais da chave de serviço
                _firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(pathToServiceAccount)
                });

                Console.WriteLine("Firebase Admin SDK inicializado com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar Firebase Admin SDK: {ex.Message}");
            }
        }

         // Método para obter uma instância do FirestoreDB
        public static FirestoreDb GetFirestoreDb()
        {
            var _config = ConfigHelper.Instance.GetConfig();
            if (_firebaseApp == null)
            {
                throw new Exception("Erro: FirebaseAuth not defined");
            }
            // Retorna a instância do FirestoreDb associada à aplicação
            return FirestoreDb.Create(_config.FirebaseAppID);
        }

        public static void SourceFirebaseKeys(){
             // Obter o diretório atual da aplicação (onde o executável está localizado)
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Construir o caminho dinâmico para o arquivo de credenciais
            string credentialsPath = System.IO.Path.Combine(appDirectory, "Keys", "serviceAccountKey.json");

            // Verificar se o arquivo existe antes de definir a variável de ambiente
            if (File.Exists(credentialsPath))
            {
                // Definir a variável de ambiente com o caminho absoluto do arquivo de credenciais
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

                // Aqui você pode continuar o fluxo de autenticação do Firebase
                Console.WriteLine("Variável de ambiente GOOGLE_APPLICATION_CREDENTIALS definida com sucesso.");
            }
            else
            {
                Console.WriteLine("Arquivo de credenciais não encontrado.");
            }
        }
    }

}