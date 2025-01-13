#pragma warning disable IDE0130 // O namespace não corresponde à estrutura da pasta
namespace WindowsApp.Models
#pragma warning restore IDE0130 // O namespace não corresponde à estrutura da pasta
{
    public class SyncConfig
    {
        public required string LocalDirectory { get; set; }  // Diretório local para sincronização (ex.: "C:\\SyncFolder")
        public required string ApiBaseUrl { get; set; }      // URL da API (ex.: "https://api.example.com")
        public int SyncInterval { get; set; }       // Intervalo de sincronização em segundos (ex.: 300)
        public bool AutoStart { get; set; }         // Aplicativo inicia automaticamente? (true/false)
    }
}