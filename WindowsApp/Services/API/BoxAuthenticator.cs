using Box.Sdk.Gen;
using WindowsApp.Helpers;
using System.Net;

namespace WindowsAppSync.Services.API
{
    public class Authenticator : Form
    {
        public static async Task<BoxClient> Auth()
        {
            var _config = ConfigHelper.Instance.GetConfig();

            if(_config.Development){
                var authDev = new BoxDeveloperTokenAuth(token:  _config.APIConfigs.Token);
                var clientDev = new BoxClient(auth: authDev);

                return clientDev;     
            }

            var config = new OAuthConfig(clientId: _config.APIConfigs.ClientId, clientSecret: _config.APIConfigs.ClientSecret);
            var auth = new BoxOAuth(config);

            
            // Gera a URL de autorização
            string RedirectUri = "http://localhost:5000/callback";
            var authorizeUrl = $"https://account.box.com/api/oauth2/authorize?client_id={_config.APIConfigs.ClientId}&redirect_uri={RedirectUri}&response_type=code";
            
            OpenAuthorizationLinkInBrowser(authorizeUrl);

            // Inicia o servidor HTTP para capturar o código de autorização
            string? code = await Task.Run(() => WaitForCodeWithTimeout());

            // Troca o código pelo token de acesso
            var token = await auth.GetTokensAuthorizationCodeGrantAsync(code);
            CentralCache.Instance.AddToCache("token", token);

            /*
                Token: {
                  "access_token": "UJU4Qf0KlKs1VeexYpiaVFWxIhcppNmN",
                  "expires_in": 4104,
                  "token_type": "bearer",
                  "restricted_to": [],
                  "refresh_token": "PIO93XR1xpPFnzFRqbZTPi47ynE5XccoO66CHFU48GyP91gv8P49raREyvXoYnc4",
                  "issued_token_type": null
                }
            */
            
            var client = new BoxClient(auth:auth);

            return client;
        }

        private static async Task<string> StartHttpListenerAsync()
        {
            using var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/callback/");
            listener.Start();

            Console.WriteLine("Aguardando redirecionamento...");
            var context = await listener.GetContextAsync();

            // Extrai o código da URL de redirecionamento
            var code = context.Request.QueryString["code"];
            if (string.IsNullOrEmpty(code))
            {
                throw new Exception("Código de autorização não encontrado no redirecionamento.");
            }

            // Retorna uma resposta ao navegador
            using var response = context.Response;

            // Lê o conteúdo do arquivo HTML
            string responseString;
            try
            {
                responseString = File.ReadAllText("./Resources/ResponsePageBoxAuth.html");
            }
            catch
            {
                responseString = "<html><body><h1>Erro ao carregar a página de resposta.</h1></body></html>";
            }
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

            listener.Stop();
            return code;
        }

        private static async Task<string?> WaitForCodeWithTimeout()
        {
            var timeout = TimeSpan.FromMinutes(2); // 2 minutos de timeout
            var cts = new CancellationTokenSource(timeout); // Define o tempo limite
            var token = cts.Token;

            // Tarefa que espera o código de autorização
            var waitForCodeTask = Task.Run(() => StartHttpListenerAsync(), token);

            // Espera até que a tarefa seja concluída ou o tempo acabe
            var completedTask = await Task.WhenAny(waitForCodeTask, Task.Delay(timeout, token));

            if (completedTask == waitForCodeTask)
            {
                // Código recebido dentro do tempo limite
                return await waitForCodeTask;
            }
            else
            {
                // Tempo limite alcançado, reinicia o processo de autenticação
                Console.WriteLine("Tempo limite atingido. Tentando autenticação novamente...");
                await Auth(); // Chama a função novamente para reiniciar o processo
                return null;
            }
        }

        // Função para abrir o navegador com o link de autorização
        private static void OpenAuthorizationLinkInBrowser(string authorizeUrl)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = authorizeUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir o navegador: {ex.Message}");
            }
        }
    }
}
