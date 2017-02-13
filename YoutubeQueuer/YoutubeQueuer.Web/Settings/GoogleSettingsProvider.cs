using System.Configuration;
using System.IO;
using System.Web;

namespace YoutubeQueuer.Web.Settings
{
    public class GoogleSettingsProvider
    {
        private static readonly string SettingsFileName = ConfigurationManager.AppSettings["GoogleAccountFileName"];

        public Stream GetSerializedSettings()
        {
            var path = HttpContext.Current.Server.MapPath("~/" + SettingsFileName);
            var settingsStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return settingsStream;
        }

        private static readonly string SecretFileName = ConfigurationManager.AppSettings["SecretsFileName"];

        public string GetSerializedSecret()
        {
            var path = HttpContext.Current.Server.MapPath("~/" + SecretFileName);
            using (var settingsStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(settingsStream))
            {
                return reader.ReadToEnd();
            }
        }

        public Stream GetSecretsStream()
        {
            var path = HttpContext.Current.Server.MapPath("~/" + SecretFileName);
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public Stream GetSettingsStream()
        {
            var path = HttpContext.Current.Server.MapPath("~/" + SettingsFileName);
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}