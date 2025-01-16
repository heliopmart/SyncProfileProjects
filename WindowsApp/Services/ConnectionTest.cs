using System.Text.Json;
using System.Text.Json.Serialization;

namespace WindowsApp.Services
{
    public static class ConnectionChecker
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static bool? LastInternetCheck = null;
        private static bool? LastBoxApiCheck = null;
        private static DateTime LastCheckTime = DateTime.MinValue;

        private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(2);

        /// <summary>
        /// Verifica a conexão com a internet e a API do Box.
        /// </summary>
        /// <returns>True se a conexão for bem-sucedida; False caso contrário.</returns>
        public static async Task<bool> CheckConnectionAsync()
        {
            // Verifica se o intervalo de 2 horas já passou
            if (DateTime.Now - LastCheckTime < CheckInterval)
            {
                Console.WriteLine("Usando resultados de verificação anteriores.");

                return LastInternetCheck.GetValueOrDefault(false) && LastBoxApiCheck.GetValueOrDefault(false);
            }

            // Atualiza o horário da última verificação
            LastCheckTime = DateTime.Now;

            // Verifica conexão com a internet
            LastInternetCheck = await IsInternetAvailableAsync();
            if (!LastInternetCheck.Value)
            {
                Console.WriteLine("Conexão com a internet indisponível.");
                return false;
            }

            // Verifica conexão com a API Box
            LastBoxApiCheck = await IsBoxApiAvailableAsync();
            if (!LastBoxApiCheck.Value)
            {
                Console.WriteLine("Conexão com a API Box falhou.");
                return false;
            }

            Console.WriteLine("Conexão com a internet e a API Box disponíveis.");
            return true;
        }

        /// <summary>
        /// Verifica a conexão com a internet tentando acessar o Google.
        /// </summary>
        /// <returns>True se a conexão com a internet estiver disponível; False caso contrário.</returns>
        private static async Task<bool> IsInternetAvailableAsync()
        {
            try
            {
                using var response = await HttpClient.GetAsync("https://www.google.com");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se a API da Box está acessível.
        /// </summary>
        /// <returns>True se a API estiver disponível; False caso contrário.</returns>
        private static async Task<bool> IsBoxApiAvailableAsync()
        {
            try
            {
                var response = await HttpClient.GetAsync("https://status.box.com/api/v2/summary.json");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var status = JsonSerializer.Deserialize<BoxStatus>(content);

                if (status?.Components != null)
                {
                    var apiComponent = status.Components.Find(c => c.Name?.Equals("Content API", StringComparison.OrdinalIgnoreCase) == true);
                    if (apiComponent != null && apiComponent.Status?.Equals("operational", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        Console.WriteLine("A API do Box está operacional.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("A API do Box está enfrentando problemas.");
                        return false;
                    }
                }

                Console.WriteLine("Não foi possível obter o status da API do Box.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar o status da API do Box: {ex.Message}");
                return false;
            }
        }

        public class BoxStatus
        {
            [JsonPropertyName("components")]
            public List<Component>? Components { get; set; }
        }

        public class Component
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("status")]
            public string? Status { get; set; }
        }
    }
}
