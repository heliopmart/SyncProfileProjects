using Google.Cloud.Firestore;
using WindowsAppSync.Services.API;
using WindowsApp.Models.Class;

namespace WindowsApp.Managers.Firebase
{
    public class FirebaseManager
    {
        private static FirestoreDb? firestoreDb;

        // Inicializa o FirestoreDb com o ID do seu projeto Firebase
        static FirebaseManager()
        {
            try{
                FirestoreDb db = FirebaseAuthenticator.GetFirestoreDb();
                firestoreDb = db;
            }catch(Exception ex){
                Console.WriteLine(ex);
            }
        }

        // Função pública para criar um documento
        public static async Task<bool> CreateDocumentAsync(string collection, object data)
        {
            return await CreateDocumentInternal(collection, data);
        }

        // Função pública para obter todos os documentos de uma coleção
        public static async Task<List<FirestoreDocument>> GetAllDocumentsAsync(string collection)
        {
            return await GetAllDocumentsInternal(collection);
        }

        // Função pública para atualizar um documento
        public static async Task<bool> UpdateDocumentAsync(string collection, string documentId, object data)
        {
            return await UpdateDocumentInternal(collection, documentId, data);
        }

        #region Funções Privadas (Operações com Firestore)

        // Função privada para criar um documento no Firestore
        private static async Task<bool> CreateDocumentInternal(string collection, object data)
        {
            try{
                if (firestoreDb == null)
                {
                    throw new Exception("DB Auth is Null");
                }
                var documentReference = await firestoreDb.Collection(collection).AddAsync(data);
                return true;
            }catch(Exception ex){
                Console.WriteLine(ex);
                throw new Exception($"FirebaseManager : CreateDocumentInternal(), error: {ex}");
            }
        }

        // Função privada para obter todos os documentos de uma coleção
        private static async Task<List<FirestoreDocument>> GetAllDocumentsInternal(string collection)
        {
            if(firestoreDb == null){
                throw new Exception("DB Auth is Null");
            }

            try{
                var snapshot = await firestoreDb.Collection(collection).GetSnapshotAsync();
                var documents = new List<FirestoreDocument>();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    var documentData = document.ToDictionary();

                    var firestoreDocument = new FirestoreDocument
                    {
                        Name = documentData["Name"] as string ?? string.Empty,
                        DateTime = documentData["DateTime"] as string ?? string.Empty,
                        Device = documentData["Device"] as string ?? string.Empty,
                        AsyncTime = documentData["AsyncTime"] as string ?? string.Empty,
                        FolderId = documentData["FolderId"] as string ?? string.Empty,
                        Status = Convert.ToInt32(documentData["Status"]),
                        Id = document.Id
                    };

                    documents.Add(firestoreDocument);
                }

                return documents;
            }catch(Exception ex){
                throw new Exception($"GetAllDocumentsInternal(), Erro: {ex}");
            }
        }

        // Função privada para atualizar um documento no Firestore
        private static async Task<bool> UpdateDocumentInternal(string collection, string documentId, object data)
        {
            try{
                if (firestoreDb == null)
                {
                    throw new Exception("DB Auth is Null");
                }
                var documentReference = firestoreDb.Collection(collection).Document(documentId);
                await documentReference.SetAsync(data, SetOptions.MergeAll);  // Atualiza o documento
                return true;
            }catch(Exception ex){
                Console.WriteLine(ex);
                throw new Exception($"FirebaseManager : UpdateDocumentInternal(), error: {ex}");
            }
        }

        #endregion
    }
}
