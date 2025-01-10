using System;
using System.IO;
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
    }

    public class APPConfig
    {
        public string DefaultPathForProjects { get; set; }
        public string MetaDataPath { get; set; }
        public int SyncInterval { get; set; }
        public APIConfigs APIConfigs { get; set; }
    }
    
    public class APIConfigs
    {
        public string ClientId {get; set;}
        public string ClientSecret {get; set;}
        public string EnterpriseId {get; set;}
        public string JwtPrivateKey { get; set; }
        public string JwtPrivateKeyPassword {get; set;}
        public string JwtPublicKeyId {get; set;}

    }
}
