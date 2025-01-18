using YamlDotNet.Core.Tokens;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WindowsApp.Helpers
{
    public sealed class ConfigHelper
    {
        private static readonly Lazy<ConfigHelper> _instance = new(() => new ConfigHelper());
        private readonly APPConfig _config;

        private ConfigHelper()
        {
            var yaml = File.ReadAllText("appsettings.yaml");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(NullNamingConvention.Instance) // CamelCase se usado no YAML
                .Build();

            _config = deserializer.Deserialize<APPConfig>(yaml);
        }

        public static ConfigHelper Instance => _instance.Value;

        public APPConfig GetConfig() => _config;

        public T GetValue<T>(Func<APPConfig, T> selector) => selector(_config);

        public void SaveConfig()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(NullNamingConvention.Instance) // Para manter o estilo de nomes no YAML
                .Build();

            var yaml = serializer.Serialize(_config);

            // Salva o YAML modificado de volta no arquivo
            File.WriteAllText("appsettings.yaml", yaml);
        }
    }

    public class ModifyAppSetting
    {
        public static async Task<bool> ChangeAppSettings(ChangeSettings settings)
        {
            var configHelper = ConfigHelper.Instance;
            var _config = configHelper.GetConfig();

            // Atualiza os valores conforme as configura��es fornecidas
            if(settings.DefaultPathForProjects != null && settings.DefaultPathForProjects != "")
            {
                _config.DefaultPathForProjects = settings.DefaultPathForProjects;
            }
            _config.Development = settings.Development;
            _config.SyncInterval = settings.SyncInterval;

            // Atualiza o Token de API
            if(settings.Token != null && settings.Token != "")
            {
                _config.APIConfigs.Token = settings.Token;
            }

            // Salva as mudan�as no arquivo YAML
            configHelper.SaveConfig();

            return await Task.FromResult(true); // Retorna true para indicar sucesso
        }
    }

    public class APPConfig
    {
        public required string DefaultPathForProjects { get; set; }
        public required string MetaDataPath { get; set; }
        public required bool Development { get; set; }
        public int SyncInterval { get; set; }
        public required APIConfigs APIConfigs { get; set; }
        public required string FirebaseAppID {get; set;}
    }
    
    public class APIConfigs
    {
        public required string Token {get; set;}
        public required string ClientId {get; set;}
        public required string ClientSecret {get; set;}
        public required string EnterpriseId {get; set;}
        public required string JwtPrivateKey { get; set; }
        public required string JwtPrivateKeyPassword {get; set;}
        public required string JwtPublicKeyId {get; set;}
        public required string UserID {get; set; }
    }

    public class ChangeSettings 
    { 
        public required string Token { get; set; }
        public required string DefaultPathForProjects { get; set; }
        public required bool Development { get; set; }
        public int SyncInterval { get; set; }
    }
}
