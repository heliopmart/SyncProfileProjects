using Box.Sdk.Gen;
using WindowsApp.Helpers;
using System.Net;

namespace WindowsAppSync.Services.API
{
    public class Authenticator
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
            Console.WriteLine($"Acesse esta URL para autorizar: {authorizeUrl}");

            // Inicia o servidor HTTP para capturar o código de autorização
            string code = await StartHttpListenerAsync();

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
            string responseString = "<html><body>Autorização concluída. Você pode fechar esta janela.</body></html>";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

            listener.Stop();
            return code;
        }
    }
}
