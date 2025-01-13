using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

#pragma warning disable IDE0130 // O namespace não corresponde à estrutura da pasta
namespace WindowsApp.Helpers
#pragma warning restore IDE0130 // O namespace não corresponde à estrutura da pasta
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
    }

    public class APPConfig
    {
        public required string DefaultPathForProjects { get; set; }
        public required string MetaDataPath { get; set; }
        public int SyncInterval { get; set; }
        public required APIConfigs APIConfigs { get; set; }
    }
    
    public class APIConfigs
    {
        public required string ClientId {get; set;}
        public required string ClientSecret {get; set;}
        public required string EnterpriseId {get; set;}
        public required string JwtPrivateKey { get; set; }
        public required string JwtPrivateKeyPassword {get; set;}
        public required string JwtPublicKeyId {get; set;}

    }
}
