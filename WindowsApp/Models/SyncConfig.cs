namespace WindowsApp.Models
{
    public class SyncConfig
    {
        public string LocalDirectory { get; set; }  // Diretório local para sincronização (ex.: "C:\\SyncFolder")
        public string ApiBaseUrl { get; set; }      // URL da API (ex.: "https://api.example.com")
        public int SyncInterval { get; set; }       // Intervalo de sincronização em segundos (ex.: 300)
        public bool AutoStart { get; set; }         // Aplicativo inicia automaticamente? (true/false)
    }
}