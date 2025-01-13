using Box.Sdk.Gen;
using WindowsApp.Helpers;

namespace WindowsAppSync.Services.API{

    public class Authenticator{
        public static BoxClient Auth(){
            var _config = ConfigHelper.Instance.GetConfig();

            var auth = new BoxDeveloperTokenAuth(token:  _config.APIConfigs.ClientId);
            var client = new BoxClient(auth: auth);
            return client;            
        }
    }

    /*
        public static BoxClient Authenticate()
    {
        var jwtConfig = new JwtConfig(
            clientId: "YOUR_CLIENT_ID",
            clientSecret: "YOUR_CLIENT_SECRET",
            jwtKeyId: "YOUR_JWT_KEY_ID",
            privateKey: "YOUR_PRIVATE_KEY",
            privateKeyPassphrase: "YOUR_PASSPHRASE"
        ) { EnterpriseId = "YOUR_ENTERPRISE_ID" };

        var jwtAuth = new BoxJwtAuth(jwtConfig);
        return new BoxClient(jwtAuth);
    }
    */

}
